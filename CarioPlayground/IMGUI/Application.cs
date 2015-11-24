using CSharpGL;
using GLM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using TinyIoC;

namespace IMGUI
{
    /// <summary>
    /// Unique application class
    /// </summary>
    /// <remarks>
    /// Manage application-wide objects:
    /// 1. IME(Internal)
    /// 2. Input
    /// 3. Ioc container(Internal)
    /// 4. Windows(internal)
    /// 5. Time(not implemented, still in Utility.cs)
    /// </remarks>
    public static class Application
    {
        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> imeBuffer = new Queue<char>();

        internal static readonly TinyIoCContainer IocContainer = TinyIoCContainer.Current;

        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> ImeBuffer
        {
            get { return imeBuffer; }
            set { imeBuffer = value; }
        }

        internal static List<BaseForm> Forms;

        private static void InitIocContainer()
        {
            if(Utility.CurrentOS.IsWindows)
            {
                IocContainer.Register<DWriteTextFormatProxy>();
                IocContainer.Register<ITextFormat, DWriteTextFormatProxy>().AsMultiInstance();

                IocContainer.Register<DWriteTextLayoutProxy>();
                IocContainer.Register<ITextLayout, DWriteTextLayoutProxy>().AsMultiInstance();
            }
            else if(Utility.CurrentOS.IsLinux)
            {
                throw new NotImplementedException();
                //IocContainer.Register<PangoTextFormatProxy>();
                //IocContainer.Register<ITextFormat, PangoTextFormatProxy>();
                //
                //IocContainer.Register<PangoTextLayoutProxy>();
                //IocContainer.Register<ITextLayout, PangoTextLayoutProxy>();
            }
        }

        private static readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Time in ms since the application started.
        /// </summary>
        public static long Time
        {
            get
            {
                if(!stopwatch.IsRunning)
                {
                    throw new InvalidOperationException(
                        "The application's time cannot be obtained because it isn't running. Call Application.Run to run it first.");
                }
                return stopwatch.ElapsedMilliseconds;
            }
        }

        public static void Run(Form mainForm)
        {
            if(mainForm == null)
            {
                throw new ArgumentNullException("mainForm");
            }

            InitIocContainer();

            Forms = new List<BaseForm>();
            Forms.Add(mainForm);
            mainForm.Name = "MainForm";

            for (int i = 0; i < Forms.Count; i++)//Only one form now
            {
                var form = (SFMLForm) Forms[i];
                var window = (SFML.Window.Window) form.InternalForm;

                // Setup event handlers
                window.Closed += OnClosed;
                window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPressed);
                window.Resized += new EventHandler<SFML.Window.SizeEventArgs>(OnResized);
                window.TextEntered += new EventHandler<SFML.Window.TextEventArgs>(OnTextEntered);

                // Make it the active window for OpenGL calls
                window.SetActive();

                // Create OpenGL GUI renderer
                form.guiRenderer = new GUIRenderer(form.Size);
                form.guiRenderer.OnLoad();
            }

            mainForm.Show();

            while (mainForm.internalForm.IsOpen)
            {
                //Input
                Input.Mouse.Refresh();
                Input.Keyboard.Refresh();

                //TODO a better method for manage newly created windows
                for (int i = 0; i < Forms.Count; i++)
                {
                    var form = (SFMLForm)Forms[i];
                    var window = (SFML.Window.Window)form.InternalForm;

                    bool mouseSuspended = false;
                    if(!form.Focused)
                    {
                        Input.Mouse.Suspend();
                        mouseSuspended = true;
                    }

                    // Start window loop
                    if(window.IsOpen)
                    {
                        // Process events
                        window.DispatchEvents();
                        // Run GUI looper
                        var isRepainted = form.GUILoop();
                        if(isRepainted)
                        {
                            var surface = (form.FrontSurface as Cairo.ImageSurface);
                            if(surface != null)
                            {
                                // Make it the active window for OpenGL calls
                                window.SetActive();
                                if (form.guiRenderer == null)
                                {
                                    form.guiRenderer = new GUIRenderer(form.Size);
                                    form.guiRenderer.OnLoad();
                                }
                                //TODO update entire surface is too slow(When it happens, a CPU peak occurs.), this can be improved by only updating re-rendered section (or using cairo-gl?)
                                form.guiRenderer.OnUpdateTexture(surface.Width, surface.Height, surface.DataPtr);
                                form.guiRenderer.OnRenderFrame();
                            }
                        }
                        // Display the rendered frame on screen
                        window.Display();
                    }

                    if (mouseSuspended)
                    {
                        Input.Mouse.Resume();
                    }

                }
            }
        }

        private static void OnTextEntered(object sender, SFML.Window.TextEventArgs e)
        {
            var text = e.Unicode;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsControl(text[i]))
                    continue;
                ImeBuffer.Enqueue(text[i]);
            }
        }

        /// <summary>
        /// Function called when the window is closed
        /// </summary>
        static void OnClosed(object sender, EventArgs e)
        {
            SFML.Window.Window window = (SFML.Window.Window)sender;
            window.Close();
        }

        /// <summary>
        /// Function called when a key is pressed
        /// </summary>
        static void OnKeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            SFML.Window.Window window = (SFML.Window.Window)sender;
            if(e.Code == SFML.Window.Keyboard.Key.Escape)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Function called when the window is resized
        /// </summary>
        static void OnResized(object sender, SFML.Window.SizeEventArgs e)
        {

        }

    }

    class GUIRenderer
    {
        private Size SurfaceSize { get; set; }

        private string vertexShaderSource = @"
#version 330 core
in vec4 in_Position;
in vec2 in_TexCoord;
out vec2 TexCoord;
void main()
{
	gl_Position = in_Position;
	TexCoord = in_TexCoord;
}";

        private string fragmentShaderSource = @"
#version 330 core
uniform sampler2D mysampler;
in vec2 TexCoord;
out vec4 out_Color;
void main()
{
	vec2 st = TexCoord.st;
	out_Color = texture2D(mysampler,vec2(st.s, 1- st.t));
}";

        private uint vertexShaderHandle,
            fragmentShaderHandle,
            shaderProgramHandle,
            positionVboHandle,
            textureHandle;

        private uint attributePos, attributeTexCoord;

        private readonly float[] vertexData = new float[]
            {
		        -1, -1, 0,	0,0,
		        -1, 1, 0,	0,1,
		        1, 1, 0,	1,1,
		        1, -1, 0,	1,0
            };

        public GUIRenderer(Size surfaceSize)
        {
            SurfaceSize = surfaceSize;
        }

        public void OnLoad()
        {
            CreateShaders();
            CreateVBOs();
            CreateTexture();

            // Other state
            GL.Enable(GL.GL_DEPTH_TEST);
            GL.ClearColor(0, 0, 0.9f, 1);
        }

        private void CreateShaders()
        {
            vertexShaderHandle = GL.CreateShader(GL.GL_VERTEX_SHADER);
            fragmentShaderHandle = GL.CreateShader(GL.GL_FRAGMENT_SHADER);

            GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            //System.Diagnostics.Debug.WriteLine(GL.GetShaderInfoLog(vertexShaderHandle));
            //System.Diagnostics.Debug.WriteLine(GL.GetShaderInfoLog(fragmentShaderHandle));

            // Create program
            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.LinkProgram(shaderProgramHandle);
            //System.Diagnostics.Debug.WriteLine(GL.GetProgramInfoLog(shaderProgramHandle));
            GL.UseProgram(shaderProgramHandle);

            attributePos = (uint)GL.GetAttribLocation(shaderProgramHandle, "in_Position");
            attributeTexCoord = (uint)GL.GetAttribLocation(shaderProgramHandle, "in_TexCoord");
        }

        private void CreateVBOs()
        {
            {
                uint[] buffers = { 0 };
                GL.GenBuffers(1, buffers);
                positionVboHandle = buffers[0];
                GL.BindBuffer(BufferTarget.ArrayBuffer, positionVboHandle);

                var dataHandle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
                GL.BufferData(GL.GL_ARRAY_BUFFER, vertexData.Length * Marshal.SizeOf(typeof(vec3)), dataHandle.AddrOfPinnedObject(), GL.GL_STATIC_DRAW);
                dataHandle.Free();
            }

            GL.VertexAttribPointer(attributePos, 3, GL.GL_FLOAT, false, 5 * Marshal.SizeOf(typeof(float)), IntPtr.Zero);
            GL.VertexAttribPointer(attributeTexCoord, 2, GL.GL_FLOAT, false, 5 * Marshal.SizeOf(typeof(float)),
                new IntPtr(3 * Marshal.SizeOf(typeof(float))));

            GL.EnableVertexAttribArray(attributePos);
            GL.EnableVertexAttribArray(attributeTexCoord);
        }

        private void CreateTexture()
        {
            GL.ActiveTexture(GL.GL_TEXTURE0);
            GL.Enable(GL.GL_TEXTURE_2D);

            uint[] textures = { 0 };
            GL.GenTextures(1, textures);
            textureHandle = textures[0];
            GL.BindTexture(GL.GL_TEXTURE_2D, textureHandle);

            {
                var textureData = Enumerable.Repeat(new byte(), 4*(int) SurfaceSize.Width*(int) SurfaceSize.Height).ToArray();
                var handle = GCHandle.Alloc(textureData, GCHandleType.Pinned);
                GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, (int)SurfaceSize.Width, (int)SurfaceSize.Height, 0, GL.GL_BGRA,
                    GL.GL_UNSIGNED_BYTE, handle.AddrOfPinnedObject());
                handle.Free();
            }

            //sampler settings
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
        }

        public void OnUpdateFrame(long time)
        {
        }

        public void OnUpdateTexture(int width, int height, IntPtr data)
        {
            GL.BindTexture(GL.GL_TEXTURE_2D, textureHandle);
            GL.TexSubImage2D(GL.GL_TEXTURE_2D, 0, 0, 0, width, height, GL.GL_BGRA, GL.GL_UNSIGNED_BYTE, data);
        }

        public void OnRenderFrame()
        {
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
            
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, positionVboHandle);
            
            GL.BindTexture(GL.GL_TEXTURE_2D, textureHandle);

            GL.DrawArrays(GL.GL_TRIANGLE_FAN, 0, 6);
        }
    }
}