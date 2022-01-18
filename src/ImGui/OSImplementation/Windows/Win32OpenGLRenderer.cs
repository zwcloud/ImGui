#define Enable_Jitter
using CSharpGL;
using ImGui.OSAbstraction.Graphics;
using System;
using System.Runtime.InteropServices;
using ImGui.OSAbstraction.Window;
using ImGui.OSImplementation.Shared;

namespace ImGui.OSImplementation.Windows
{
    internal partial class Win32OpenGLRenderer : IRenderer
    {
        private readonly OpenGLMeshDrawer meshDrawer = new OpenGLMeshDrawer();

        public Win32OpenGLRenderer(IWindow window)
        {
            Init(window.Pointer, window.ClientSize);
        }

        public void Init(IntPtr windowHandle, Size size)
        {
            CreateOpenGLContext(windowHandle);
            meshDrawer.Init(size);
        }

        public void SetRenderingWindow(IWindow window)
        {
            if (hglrc == IntPtr.Zero)
            {
                throw new InvalidOperationException("OpenGL context hasn't been created.");
            }

            var dc = GetDC(window.Pointer);
            hDC = dc;
            
            //For OpenGL, we need to set pixel format for each win32 window before make it current.

            if (GetPixelFormat(dc) == 0)//Set a valid pixel format if haven't.
            {
                var pixelformatdescriptor = new PIXELFORMATDESCRIPTOR();
                pixelformatdescriptor.Init();

                if (!Application.EnableMSAA)
                {
                    int pixelFormat = ChoosePixelFormat(dc, ref pixelformatdescriptor);
                    SetPixelFormat(dc, pixelFormat, ref pixelformatdescriptor);
                }
                else
                {
                    int[] iPixAttribs =
                    {
                        (int) WGL.WGL_SUPPORT_OPENGL_ARB, (int) GL.GL_TRUE,
                        (int) WGL.WGL_DRAW_TO_WINDOW_ARB, (int) GL.GL_TRUE,
                        (int) WGL.WGL_DOUBLE_BUFFER_ARB, (int) GL.GL_TRUE,
                        (int) WGL.WGL_PIXEL_TYPE_ARB, (int) WGL.WGL_TYPE_RGBA_ARB,
                        (int) WGL.WGL_ACCELERATION_ARB, (int) WGL.WGL_FULL_ACCELERATION_ARB,
                        (int) WGL.WGL_COLOR_BITS_ARB, 24,
                        (int) WGL.WGL_ALPHA_BITS_ARB, 8,
                        (int) WGL.WGL_DEPTH_BITS_ARB, 24,
                        (int) WGL.WGL_STENCIL_BITS_ARB, 8,
                        (int) WGL.WGL_SWAP_METHOD_ARB, (int) WGL.WGL_SWAP_EXCHANGE_ARB,
                        (int) WGL.WGL_SAMPLE_BUFFERS_ARB, (int) GL.GL_TRUE, //Enable MSAA
                        (int) WGL.WGL_SAMPLES_ARB, 16,
                        0
                    };

                    int pixelFormat;
                    uint numFormats;
                    var result1 = Wgl.ChoosePixelFormatARB(dc, iPixAttribs, null, 1, out pixelFormat,
                        out numFormats);
                    if (result1 == false || numFormats == 0)
                    {
                        throw new Exception(
                            $"wglChoosePixelFormatARB failed: error {Marshal.GetLastWin32Error()}");
                    }

                    if (!DescribePixelFormat(dc, pixelFormat, (uint) Marshal.SizeOf<PIXELFORMATDESCRIPTOR>(),
                        ref pixelformatdescriptor))
                    {
                        throw new Exception(
                            $"DescribePixelFormat failed: error {Marshal.GetLastWin32Error()}");
                    }

                    if (!SetPixelFormat(dc, pixelFormat, ref pixelformatdescriptor))
                    {
                        throw new Exception(
                            $"SetPixelFormat failed: error {Marshal.GetLastWin32Error()}");
                    }
                }
            }

            var result = Wgl.MakeCurrent(dc, hglrc);
            if (!result)
            {
                var lastError = Native.GetLastErrorString();
                PrintPixelFormat(dc);
                throw new InvalidOperationException($"Wgl.MakeCurrent failed, error: {lastError}");
            }

            var viewportSize = window.ClientSize;
            GL.Viewport(0, 0, (int)viewportSize.Width, (int)viewportSize.Height);
            GL.Scissor(0, 0, (int)viewportSize.Width, (int)viewportSize.Height);
        }

        public IWindow GetRenderingWindow()
        {
            if (hglrc == IntPtr.Zero)
            {
                throw new InvalidOperationException("OpenGL context hasn't been created.");
            }

            var currentGLContext = Wgl.GetCurrentContext();
            if(hglrc != currentGLContext)
            {
                throw new InvalidOperationException("Cached OpenGL context doesn't match actual one.");
            }

            var currentDC = Wgl.GetCurrentDC();
            var currentHWND = WindowFromDC(currentDC);
            var forms = Application.ImGuiContext.WindowManager.Viewports;
            foreach (var form in forms)
            {
                if (form.NativeWindow.Pointer == currentHWND)
                {
                    return form.NativeWindow;
                }
            }

            return null;
        }

        public void Clear(Color clearColor)
        {
            GL.ClearColor((float)clearColor.R, (float)clearColor.G, (float)clearColor.B, (float)clearColor.A);
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
        }

        public void DrawMeshes(int width, int height, (Mesh shapeMesh, Mesh imageMesh, TextMesh textMesh) meshes)
        {
            meshDrawer.DrawMeshes(width, height, meshes);
        }

        public void OnSizeChanged(Size size)
        {
            meshDrawer.RebuildTextureRenderResources(size);
        }

        public void ShutDown()
        {
            meshDrawer.ShutDown();
        }

        public byte[] GetRawBackBuffer(out int width, out int height)
        {
            int[] IntBuffer = { 0, 0, 0, 0 };
            GL.GetIntegerv(GL.GL_VIEWPORT, IntBuffer);
            int viewportX = IntBuffer[0];
            int viewportY = IntBuffer[1];
            int viewportWidth = width = IntBuffer[2];
            int viewportHeight = height = IntBuffer[3];
            var pixels = new byte[viewportWidth * viewportHeight * 4];
            GL.ReadPixels(viewportX, viewportY, viewportWidth, viewportHeight, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, pixels);
            return pixels;
        }
    }
}
