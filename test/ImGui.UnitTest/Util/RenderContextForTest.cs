using ImGui.OSImplentation.Windows;
using System;
using CSharpGL;
using ImGui.Rendering;

namespace ImGui.UnitTest
{
    internal class RenderContextForTest : IDisposable
    {
        //TODO de-cuple with Windows platform

        public Win32Window Window { get; private set; }
        public Win32OpenGLRenderer Renderer { get; private set; }

        private readonly int viewportWidth;
        private readonly int viewportHeight;

        public RenderContextForTest(int viewportWidth, int viewportHeight)
        {
            this.viewportWidth = viewportWidth;
            this.viewportHeight = viewportHeight;

            Application.Init();

            this.Window = new Win32Window();
            this.Window.Init(Point.Zero, new Size(1000, 1000)/*This size should be enough for unit testing.*/, WindowTypes.Regular);

            this.Renderer = new Win32OpenGLRenderer();
            this.Renderer.Init(this.Window.Pointer, this.Window.ClientSize);
        }

        public void Clear()
        {
            this.Renderer.Clear(Color.White);
        }

        public void DrawShapeMesh(Mesh shapeMesh)
        {
            Win32OpenGLRenderer.DrawMesh(this.Renderer.shapeMaterial, shapeMesh,
                viewportWidth, viewportHeight);
        }

        public void DrawImageMesh(Mesh imageMesh)
        {
            Win32OpenGLRenderer.DrawMesh(this.Renderer.imageMaterial, imageMesh,
                viewportWidth, viewportHeight);
        }

        public void DrawTextMesh(TextMesh textMesh)
        {
            Win32OpenGLRenderer.DrawTextMesh(this.Renderer.glyphMaterial, textMesh,
                viewportWidth, viewportHeight);
        }

        public void DrawMeshes(MeshBuffer meshBuffer)
        {
            this.DrawShapeMesh(meshBuffer.ShapeMesh);
            this.DrawImageMesh(meshBuffer.ImageMesh);
            this.DrawTextMesh(meshBuffer.TextMesh);
        }

        public byte[] GetRenderedRawBytes()
        {
            GL.Viewport(0,0, this.viewportWidth, this.viewportHeight);//fix the viewport TODO make this cleaner
            return this.Renderer.GetRawBackBuffer(out _, out _);
        }

        public void Dispose()
        {
            this.Renderer.ShutDown();
            this.Window.Close();
        }
    }
}