using System;
using System.Diagnostics;
using CSharpGL;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Window;
using ImGui.OSImplementation.Shared;

namespace ImGui.OSImplementation.Linux
{
    internal partial class LinuxOpenGLRenderer : IRenderer
    {
        private readonly OpenGLMeshDrawer meshDrawer = new OpenGLMeshDrawer();
        
        public LinuxOpenGLRenderer(IWindow window)
        {
            Init(window.Pointer, window.ClientSize);
        }

        public void Init(IntPtr windowHandle, Size size)
        {
            if (!CreateEGLContext())
            {
                Debug.WriteLine("Failed to create EGL context.");
                return;
            }

            var nativeWindow = windowHandle;
            if (!CreateEGLSurface(nativeWindow))
            {
                Debug.WriteLine("Failed to create EGL surface.");
                return;
            }

            meshDrawer.Init(size);
        }
        
        public void SetRenderingWindow(IWindow window)
        {
            //TODO eglMakeCurrent
            //TODO consider how to handle egl display and surface
        }

        public IWindow GetRenderingWindow()
        {
            //TODO get the window associated with this renderer via eglGetCurrentContext();
            return Application.ImGuiContext.WindowManager.MainForm.NativeWindow;
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
        }

        public void SwapBuffers()
        {
            eglSwapBuffers(this.display, this.surface);
        }

        public void ShutDown()
        {
            meshDrawer.ShutDown();
            //BUG we should not destory EGL context here,
            //because multiple window's renderer will share one single EGL context
            DestroyEGL();
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
