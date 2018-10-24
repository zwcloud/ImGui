using ImGui.Common.Primitive;
using ImGui.OSImplentation.Windows;
using System;

namespace ImGui.UnitTest
{
    internal class RenderContextForTest : IDisposable
    {
        //TODO de-cuple with Windows platform

        public Win32Window Window { get; private set; }
        public Win32OpenGLRenderer Renderer { get; private set; }

        public RenderContextForTest(Size size)
        {
            Application.Init();

            this.Window = new Win32Window();
            this.Window.Init(Point.Zero, size, WindowTypes.Regular);

            this.Renderer = new Win32OpenGLRenderer();
            this.Renderer.Init(this.Window.Pointer, this.Window.ClientSize);
        }

        public void Clear()
        {
            this.Renderer.Clear(Color.FrameBg);
        }

        public void DrawShapeMesh(Mesh shapeMesh)
        {
            Win32OpenGLRenderer.DrawMesh(this.Renderer.shapeMaterial, shapeMesh,
                (int)this.Window.ClientSize.Width, (int)this.Window.ClientSize.Height);
        }

        public void DrawImageMesh(Mesh imageMesh)
        {
            Win32OpenGLRenderer.DrawMesh(this.Renderer.imageMaterial, imageMesh,
                (int)this.Window.ClientSize.Width, (int)this.Window.ClientSize.Height);
        }

        public void DrawTextMesh(TextMesh textMesh)
        {
            Win32OpenGLRenderer.DrawTextMesh(this.Renderer.glyphMaterial, textMesh,
                (int)this.Window.ClientSize.Width, (int)this.Window.ClientSize.Height);
        }

        public void Dispose()
        {
            this.Renderer.ShutDown();
            this.Window.Close();
        }
    }
}