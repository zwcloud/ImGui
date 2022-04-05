using System;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Window;
using ImGui.OSImplementation.Shared;
using GL = CSharpGL.GL;

namespace ImGui.OSImplementation.Web
{
    internal partial class WebGLRenderer : IRenderer
    {
        private readonly OpenGLMeshDrawer meshDrawer = new OpenGLMeshDrawer();
        
        public WebGLRenderer(IWindow window)
        {
            Init(window.Pointer, window.ClientSize);
        }

        public void Init(IntPtr windowHandle, Size size)
        {
            CreateWebGLContext(size);
            meshDrawer.Init(size);
        }
        
        public void SetRenderingWindow(IWindow window)
        {
            //TODO
        }

        public IWindow GetRenderingWindow()
        {
            //TODO consider this
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
            // consider this
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
