﻿using CSharpGL;
using GLM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// 5. Time
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

        internal static Tree<BaseForm> Forms;

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
        
        public static void Run(SFMLForm form)
        {
            if(form == null)
            {
                throw new ArgumentNullException("form");
            }

            InitIocContainer();

            Forms = new Tree<BaseForm>(form);

            var window = form.internalForm;

            // Make it the active window for OpenGL calls
            window.SetActive();

            // Setup event handlers
            window.Closed += OnClosed;
            window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPressed);
            window.Resized += new EventHandler<SFML.Window.SizeEventArgs>(OnResized);
            window.TextEntered += new EventHandler<SFML.Window.TextEventArgs>(OnTextEntered);
            
            MyClass o = new MyClass();
            o.OnLoad();

            // Start game loop
            while (window.IsOpen)
            {
                // Process events
                window.DispatchEvents();
                
                var isRepainted = form.GUILoop();

#if true
                if (isRepainted)
                {
                    var surface = (form.FrontSurface as Cairo.ImageSurface);
                    if(surface != null)
                    {
                        //TODO update entire surface is too slow(When it happens, a CPU peak occurs.), this can be improved by only updating re-rendered section or using cairo-gl
                        o.OnUpdateTexture(surface.Width, surface.Height, surface.DataPtr);
                        o.OnRenderFrame();
                    }
                }
#endif

                // Display the rendered frame on screen, this method must be called on Windows or the dwm.exe will totally consume the CPU.
                window.Display();

            }

            stopwatch.Stop();
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


    class MyClass
    {
        private SFML.System.Vector2i ClientSize = new SFML.System.Vector2i(800, 600);

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

        private float[] vertexData = new float[]
            {
		        -1, -1, 0,	0,0,
		        -1, 1, 0,	0,1,
		        1, 1, 0,	1,1,
		        1, -1, 0,	1,0
            };

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
            using (System.Drawing.Bitmap image = new System.Drawing.Bitmap("resources\\CheckerMap.png"))
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
                var imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, image.Width, image.Height, 0, GL.GL_BGRA,
                    GL.GL_UNSIGNED_BYTE, imageData.Scan0);
                image.UnlockBits(imageData);
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