using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Input;

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
            else if(CurrentOS.IsWindows)
            {
                PlatformContext = OSImplementation.Windows.WindowsContext.MapFactory();
            }
            else if(CurrentOS.IsLinux)
            {
                PlatformContext = OSImplementation.Linux.LinuxContext.MapFactory();
            }
        }

        internal static bool RequestQuit;

        // HACK for Android
        public static Func<string, System.IO.Stream> OpenAndroidAssets;

        public static void Init()
        {
            InitSysDependencies();
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
            mainForm.InitializeRenderer();
            mainForm.Show();
            Form.current = mainForm;
            AddFrom(mainForm);
            while (!mainForm.Closed)
            {
                Time.OnFrameBegin();
                Keyboard.Instance.OnFrameBegin();

                mainForm.MainLoop(() =>
                {
                    //Since renderer is possibly used by all methods below, bind here.
                    mainForm.renderer.Bind();

                    NewFrame();

                    onGUI?.Invoke();

                    Render();

                    Log();
                    
                    if (mainForm.LastRendererSize != mainForm.ClientSize)
                    {
                        mainForm.Renderer_SetWindowSize(mainForm.ClientSize);
                        mainForm.LastRendererSize = mainForm.ClientSize;
                    }
                    mainForm.renderer.Unbind();
                });


                //handle additional forms
                UpdateForms();
                RenderForms();
                
                if (RequestQuit)
                {
                    break;
                }

                Keyboard.Instance.OnFrameEnd();
                Time.OnFrameEnd();
            }

            //TODO clean up and shutdown renderer and windows
        }

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
                // Destroy platform window if the viewport hasn't been submitted or if it is hosting a hidden window
                // (the implicit/fallback Debug##Default window will be registering its viewport then be disabled, causing a dummy DestroyPlatformWindow to be made each frame)
                bool destroy_platform_window = false;
                destroy_platform_window |= (viewport.LastFrameActive < g.FrameCount - 1);
                destroy_platform_window |= (viewport.Window?.ActiveAndVisible == false);
                if (destroy_platform_window)
                {
                    viewport.Close();
                    continue;
                }

                // New windows that appears directly in a new viewport won't always have a size on their first frame
                if (viewport.LastFrameActive < g.FrameCount || viewport.Size.Width <= 0
                    || viewport.Size.Height <= 0)
                {
                    continue;
                }

                // Create window
                bool is_new_platform_window = (viewport.PlatformWindowCreated == false);
                if (is_new_platform_window)
                {
                    ImGui.Log.Msg("Create Platform Window %08X (%s)\n", viewport.ID,
                        viewport.Window != null ? viewport.Window.Name : "n/a");
                    viewport.InitializeForm();
                    viewport.InitializeRenderer();
                    viewport.LastPlatformPos = new Point(float.MaxValue, float.MaxValue);
                    viewport.LastRendererSize = new Size(float.MaxValue, float.MaxValue);
                    viewport.LastRendererSize = viewport.Size;
                    viewport.PlatformWindowCreated = true;
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

                // Optional, general purpose call to allow the backend to perform general book-keeping even if things haven't changed.
                viewport.Platform_UpdateWindow();

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
            }

            // Update our implicit z-order knowledge of platform windows, which is used when the backend cannot provide io.MouseHoveredViewport.
            // When setting Platform_GetWindowFocus, it is expected that the platform backend can handle calls without crashing if it doesn't have data stored.
            // FIXME-VIEWPORT: We should use this information to also set dear imgui-side focus, allowing us to handle os-level alt+tab.
            {
                Form focused_viewport = null;
                for (int n = 0; n < forms.Count && focused_viewport == null; n++)
                {
                    Form viewport = forms[n];
                    if (viewport.PlatformWindowCreated)
                        if (viewport.Platform_GetWindowFocus())
                            focused_viewport = viewport;
                }

                // Store a tag so we can infer z-order easily from all our windows
                if (focused_viewport != null
                    && focused_viewport.LastFrontMostStampCount != g.ViewportFrontMostStampCount)
                    focused_viewport.LastFrontMostStampCount = ++g.ViewportFrontMostStampCount;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform_render_arg"></param>
        /// <param name="renderer_render_arg"></param>
        /// <remarks>
        /// This is a default/basic function for performing the rendering/swap of multiple Platform Windows.
        /// Custom renderers may prefer to not call this function at all, and instead iterate the publicly exposed platform data and handle rendering/sync themselves.
        /// The Render/Swap functions stored in ImGuiPlatformIO are merely here to allow for this helper to exist, but you can do it yourself:
        /// </remarks>
        //<code>
        //   ImGuiPlatformIO& platform_io = ImGui::GetPlatformIO();
        //   for (int i = 1; i < platform_io.Viewports.Size; i++)
        //       if ((platform_io.Viewports[i]->Flags & ImGuiViewportFlags_Minimized) == 0)
        //           MyRenderFunction(platform_io.Viewports[i], my_args);
        //   for (int i = 1; i < platform_io.Viewports.Size; i++)
        //       if ((platform_io.Viewports[i]->Flags & ImGuiViewportFlags_Minimized) == 0)
        //           MySwapBufferFunction(platform_io.Viewports[i], my_args);
        //</code>
        private static void RenderForms(object platform_render_arg = null, object renderer_render_arg = null)
        {
            var w = ImGuiContext.WindowManager;
            var forms = w.Viewports;
            for (int i = 0; i < forms.Count; i++)
            {
                var viewport = forms[i];
                if (Utility.HasAllFlags(viewport.Flags, ImGuiViewportFlags.Minimized))
                    continue;
                viewport.Platform_RenderWindow(platform_render_arg);
                viewport.Renderer_RenderWindow(renderer_render_arg);
            }
            for (int i = 0; i < forms.Count; i++)
            {
                var viewport = forms[i];
                if (Utility.HasAllFlags(viewport.Flags, ImGuiViewportFlags.Minimized))
                {
                    continue;
                }
                viewport.Platform_SwapBuffers(platform_render_arg);
                viewport.Renderer_SwapBuffers(renderer_render_arg);
            }
        }

        public static void RunLoop(Form form, Action onGUI = null)
        {
            MainForm = form;

            form.InitializeForm();
            form.InitializeRenderer();
            form.Show();
            
            Time.OnFrameBegin();
            Keyboard.Instance.OnFrameBegin();

            form.MainLoop(() =>
            {
                //Since renderer is possibly used by all methods below, bind here.
                form.renderer.Bind();

                NewFrame();

                onGUI?.Invoke();

                Render();

                Log();

                form.renderer.Unbind();
            });

            //handle additional forms
            UpdateForms();
            RenderForms();
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

        private const int IMGUI_VIEWPORT_DEFAULT_ID = 0x11111111;
    }
}