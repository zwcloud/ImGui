using System;
using System.Diagnostics;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Window;
using ImGui.OSImplementation.Shared;
using CSharpGL;

namespace ImGui.OSImplementation.Android
{
    internal partial class OpenGLESRenderer : IRenderer
    {
        private readonly OpenGLMeshDrawer meshDrawer = new OpenGLMeshDrawer();

        public OpenGLESRenderer(IWindow window)
        {
            Init(window.Pointer, window.ClientSize);
        }

        public void Init(IntPtr windowHandle, Size size)
        {
            //OpenGLES context is created by Xamarin.Android.
            //If the backend is manually implmented via a NativeActivity provided by NDK in the future,
            //we need to create and initialize the OpenGLES context via EGL.
            meshDrawer.Init(size);
        }

        public void SetRenderingWindow(IWindow window)
        {
            //for android, only one unique native window.
            Debug.Assert(window == Application.ImGuiContext.WindowManager.MainForm.NativeWindow);
        }

        public IWindow GetRenderingWindow()
        {
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
