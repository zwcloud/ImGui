using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Input;
using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    /// <summary>
    /// Encapsulates a ImGui application.
    /// </summary>
    /// <remarks>
    /// Application is a class that encapsulates application-specific functionality.
    /// An application can be started in two ways:
    /// 1. If you have access to the main entry point, use
    /// <code>
    /// Application.Init();
    /// Application.Run(mainForm);
    /// </code>
    /// 2. If you can only provide an callback in the main loop, use
    /// <code>
    /// //first init
    /// Application.Init();
    /// //then in the loop callback, call
    /// Application.RunLoop(mainForm);
    /// </code>
    /// </remarks>
    public static partial class Application
    {
        internal static Form MainForm
        {
            get => ImGuiContext.WindowManager.MainForm;
            private set => ImGuiContext.WindowManager.MainForm = value;
        }

        internal static OSAbstraction.PlatformContext PlatformContext;

        internal static GUIConfiguration IO { get; } = new GUIConfiguration();

        internal static GUIContext ImGuiContext { get; } = new GUIContext();

        internal static void InitSysDependencies()
        {
            Time.Init();

            // load logger
            if (IsRunningInUnitTest)
            {
                try
                {
                    Logger = new EchoLogger();
                    EchoLogger.Show();
                    ImGui.Log.Enabled = true;
                }
                catch (System.Net.Sockets.SocketException)
                {
                    Debug.WriteLine("Failed to connect to EchoLogger. The program will continue without logging.");
                    ImGui.Log.Enabled = false;
                }
            }
            else
            {
                Logger = new ConsoleLogger();
                ImGui.Log.Enabled = true;
            }
            ImGui.Log.Init(Logger);

            // load platform context:
            //     platform-dependent implementation
            if (CurrentOS.IsAndroid)
            {
                PlatformContext = OSImplementation.Android.AndroidContext.MapFactory();
            }
            else if (CurrentOS.IsWindows)
            {
                PlatformContext = OSImplementation.Windows.WindowsContext.MapFactory();
            }
            else if (CurrentOS.IsLinux)
            {
                PlatformContext = OSImplementation.Linux.LinuxContext.MapFactory();
            }
        }

        internal static bool RequestQuit;

        // HACK for Android
        public static Func<string, System.IO.Stream> OpenAndroidAssets;

        public static bool Initialized { get; private set; }

        public static void Init()
        {
            InitSysDependencies();


            Initialized = true;
        }

        public static void InitForLooper(Form mainForm)
        {
            InitSysDependencies();

            MainForm = mainForm;
            mainForm.ID = IMGUI_VIEWPORT_DEFAULT_ID;
            mainForm.InitializeForm();
            Profile.Start("Create Renderer");
            renderer = Application.PlatformContext.CreateRenderer(mainForm.NativeWindow);
            mainForm.InitializeRenderer();
            renderer.SetRenderingWindow(mainForm.NativeWindow);
            Profile.End();

            mainForm.Show();
            Form.current = mainForm;
            AddFrom(mainForm);

            Initialized = true;
        }

        /// <summary>
        /// Begins running a standard application on the current thread and makes the specified form visible.
        /// </summary>
        /// <param name="mainForm">A <see cref="Form"/> that represents the form to make visible.</param>
        /// <param name="onGUI"></param>
        public static void Run(Form mainForm, Action onGUI = null)
        {
            MainForm = mainForm;
            mainForm.ID = IMGUI_VIEWPORT_DEFAULT_ID;
            mainForm.InitializeForm();
            Profile.Start("Create Renderer");
            renderer = Application.PlatformContext.CreateRenderer(mainForm.NativeWindow);
            mainForm.InitializeRenderer();
            renderer.SetRenderingWindow(mainForm.NativeWindow);
            Profile.End();

            mainForm.Show();
            Form.current = mainForm;
            AddFrom(mainForm);
            while (!mainForm.Closed)
            {
                Time.OnFrameBegin();
                Keyboard.Instance.OnFrameBegin();

                mainForm.MainLoop(() =>
                {
                    NewFrame();

                    onGUI?.Invoke();

                    EndFrame();
                    
                    //handle additional forms creation and update (the loop)
                    UpdateForms();

                    Render();

                    Log();
                    
                    if (mainForm.LastRendererSize != mainForm.ClientSize)
                    {
                        mainForm.Renderer_SetWindowSize(mainForm.ClientSize);
                        mainForm.LastRendererSize = mainForm.ClientSize;
                    }
                });

                
                if (RequestQuit)
                {
                    break;
                }

                Keyboard.Instance.OnFrameEnd();
                Time.OnFrameEnd();
            }

            //TODO clean up and shutdown renderer and windows
        }

        /// <summary>
        /// Update extra forms:
        /// 1. native window creation initiated inside OnGUI
        /// 2. synchronize native window user interaction (drag moving, resizing, etc) to ImGui context
        /// 3. run main loop of native window
        /// </summary>
        private static void UpdateForms()
        {
            var g = ImGuiContext;
            Debug.Assert(g.FrameCountEnded == g.FrameCount, "Forgot to call Render() or EndFrame() before UpdatePlatformWindows()?");
            Debug.Assert(g.FrameCountPlatformEnded < g.FrameCount);
            g.FrameCountPlatformEnded = g.FrameCount;
            if (!Utility.HasAllFlags(g.ConfigFlagsCurrFrame,ImGuiConfigFlags.ViewportsEnable))
                return;

            var w = g.WindowManager;
            var forms = w.Viewports;

            // Create/resize/destroy native windows to match each active form.
            // Skip the main form at index 0, which is always fully handled by the application.
            for (var i = 1; i < forms.Count; i++)
            {
                var viewport = forms[i];
                // Destroy platform window if the viewport if it is hosting a hidden window
                // (the implicit/fallback Debug##Default window will be registering its viewport then be disabled, causing a dummy DestroyPlatformWindow to be made each frame)
                bool destroy_platform_window = false;
                //destroy_platform_window |= (viewport.LastFrameActive < g.FrameCount - 1);
                destroy_platform_window |= (viewport.Window?.ActiveAndVisible == false);
                if (destroy_platform_window)
                {
                    viewport.Close();
                    continue;
                }

                // Create window
                bool is_new_platform_window = (viewport.PlatformWindowCreated == false);
                if (is_new_platform_window)
                {
                    ImGui.Log.Msg(string.Format("Create Platform Window 0x{0:X} ({1})\n", viewport.ID,
                        viewport.Window != null ? viewport.Window.Name : "n/a"));
                    viewport.InitializeForm();
                    viewport.InitializeRenderer();
                    viewport.LastPlatformPos = new Point(float.MaxValue, float.MaxValue);
                    viewport.LastRendererSize = new Size(float.MaxValue, float.MaxValue);
                    viewport.LastRendererSize = viewport.Size;
                }

                // Apply Position and Size (from ImGui to Platform/Renderer backends)
                if (viewport.LastPos != viewport.Pos && !viewport.PlatformRequestMove)
                    viewport.PlatformPosition = viewport.Pos;
                if (viewport.LastSize != viewport.Size && !viewport.PlatformRequestResize)
                    viewport.PlatformSize = viewport.Size;
                if (viewport.LastRendererSize != viewport.Size)
                    viewport.Renderer_SetWindowSize(viewport.Size);
                viewport.LastPos = viewport.Pos;
                viewport.LastSize = viewport.LastRendererSize = viewport.Size;

                // Update title bar (if it changed)
                if (viewport.Window != null && viewport.LastName != viewport.Window.Name)
                {
                    viewport.Platform_SetWindowTitle(viewport.Window.Name);
                    viewport.LastName = viewport.Window.Name;
                }

                // Update alpha (if it changed)
                if (viewport.LastAlpha != viewport.Alpha)
                    viewport.Platform_SetWindowAlpha(viewport.Alpha);
                viewport.LastAlpha = viewport.Alpha;

                if (is_new_platform_window)
                {
                    // On startup ensure new platform window don't steal focus (give it a few frames, as nested contents may lead to viewport being created a few frames late)
                    if (g.FrameCount < 3)
                        viewport.Flags |= ImGuiViewportFlags.NoFocusOnAppearing;

                    // Show window
                    viewport.Show();

                    // Even without focus, we assume the window becomes front-most.
                    // This is useful for our platform z-order heuristic when io.MouseHoveredViewport is not available.
                    if (viewport.LastFrontMostStampCount != g.ViewportFrontMostStampCount)
                        viewport.LastFrontMostStampCount = ++g.ViewportFrontMostStampCount;
                }

                // Clear request flags
                viewport.ClearRequestFlags();

                viewport.MainLoop(null);
            }
        }

        public static void RunLoop(Form form, Action onGUI = null)
        {
            if (!Initialized)
            {
                return;
            }

            Time.OnFrameBegin();
            Keyboard.Instance.OnFrameBegin();

            MainForm.MainLoop(() =>
            {
                NewFrame();

                onGUI?.Invoke();

                EndFrame();

                //handle additional forms creation and update (the loop)
                UpdateForms();

                Render();

                Log();

                if (MainForm.LastRendererSize != MainForm.ClientSize)
                {
                    MainForm.Renderer_SetWindowSize(MainForm.ClientSize);
                    MainForm.LastRendererSize = MainForm.ClientSize;
                }
            });

            Keyboard.Instance.OnFrameEnd();
            Time.OnFrameEnd();
        }

        /// <summary>
        /// Closes all application windows and quit the application.
        /// </summary>
        public static void Quit()
        {
            if (IsRunningInUnitTest)
            {
                if (ImGui.Log.Enabled)
                {
                    EchoLogger.Close();
                }
            }
            RequestQuit = true;
        }

        /// <summary>
        /// Helper property for the unit-test.
        /// </summary>
        internal static bool IsRunningInUnitTest { get; set; } = false;

        internal static bool EnableMSAA { get; set; } = true;

        internal static Rect InitialDebugWindowRect { get; set; } = new Rect(80, 80, 400, 250);

        internal static ILogger Logger;

        public static void AddFrom(Form form)
        {
            ImGuiContext.WindowManager.Viewports.Add(form);
        }

        public static void RemoveForm(Form form)
        {
            ImGuiContext.WindowManager.Viewports.Remove(form);
        }

        internal static IRenderer renderer;

        private const int IMGUI_VIEWPORT_DEFAULT_ID = 0x11111111;
    }
}