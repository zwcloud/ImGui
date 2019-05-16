using System;
using CSharpGL;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Window;
using ImGui.Rendering;

namespace ImGui.UnitTest
{
    internal class RenderContextForTest : IDisposable
    {
        public IWindow Window { get; private set; }
        public IRenderer Renderer { get; private set; }

        private readonly int viewportWidth;
        private readonly int viewportHeight;

        private const int WindowWidth = 1000;
        private const int WindowHeight = 1000;//This size should be enough for unit testing.

        public RenderContextForTest(int viewportWidth, int viewportHeight)
        {
            if (viewportWidth > WindowWidth || viewportHeight > WindowHeight)
            {

            }
            this.viewportWidth = viewportWidth;
            this.viewportHeight = viewportHeight;

            Application.Init();

            this.Window = Application.PlatformContext.CreateWindow(
                Point.Zero,
                new Size(WindowWidth, WindowHeight),
                WindowTypes.Regular);

            this.Renderer = Application.PlatformContext.CreateRenderer();
            this.Renderer.Init(this.Window.Pointer, this.Window.ClientSize);
        }

        public void Clear()
        {
            this.Renderer.Clear(Color.White);
        }

        public void DrawMeshes(MeshBuffer meshBuffer)
        {
            this.Renderer.DrawMeshes(this.viewportWidth, this.viewportHeight,
                (meshBuffer.ShapeMesh, meshBuffer.ImageMesh, meshBuffer.TextMesh));
        }

        public byte[] GetRenderedRawBytes()
        {
            GL.Viewport(0,0, this.viewportWidth, this.viewportHeight);
            return this.Renderer.GetRawBackBuffer(out _, out _);
        }

        public void Dispose()
        {
            this.Renderer.ShutDown();
            this.Window.Close();
        }
    }
}