using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.GraphicsImplementation;
using ImGui.Input;
using ImGui.OSImplentation.Windows;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class BuiltinPrimitiveRendererFacts
    {
        [Fact]
        public void StrokeAPath()
        {
            var primitive = new PathPrimitive();
            primitive.PathMoveTo(new Point(10, 10));
            primitive.PathLineTo(new Point(10, 100));
            primitive.PathLineTo(new Point(100, 100));
            primitive.PathLineTo(new Point(100, 10));
            primitive.PathClose();
            primitive.PathStroke(2, Color.Red);

            var primitiveRenderer = new BuiltinPrimitiveRenderer();

            var mesh = new Mesh();
            mesh.CommandBuffer.Add(DrawCommand.Default);
            primitiveRenderer.SetShapeMesh(mesh);

            primitiveRenderer.DrawPath(primitive);

            var window = new Win32Window();
            window.Init(new Point(100, 100), new Size(300, 400), WindowTypes.Regular);

            var renderer = new Win32OpenGLRenderer();
            renderer.Init(window.Pointer, window.ClientSize);

            while (true)
            {
                window.MainLoop(() =>
                {
                    renderer.Clear(Color.FrameBg);
                    Win32OpenGLRenderer.DrawMesh(renderer.shapeMaterial, primitiveRenderer.ShapeMesh,
                        (int)window.ClientSize.Width, (int)window.ClientSize.Height);
                    renderer.SwapBuffers();
                });
                if (Input.Keyboard.Instance.KeyDown(Key.Escape))
                {
                    break;
                }
            }
        }

        [Fact]
        public void FillAPath()
        {
            var primitive = new PathPrimitive();
            primitive.PathMoveTo(new Point(10, 10));
            primitive.PathLineTo(new Point(10, 100));
            primitive.PathLineTo(new Point(100, 100));
            primitive.PathLineTo(new Point(100, 10));
            primitive.PathClose();
            primitive.PathFill(Color.Red);

            var primitiveRenderer = new BuiltinPrimitiveRenderer();
            var mesh = new Mesh();
            mesh.CommandBuffer.Add(DrawCommand.Default);
            primitiveRenderer.SetShapeMesh(mesh);
            primitiveRenderer.DrawPath(primitive);

            var window = new Win32Window();
            window.Init(new Point(100, 100), new Size(300, 400), WindowTypes.Regular);

            var renderer = new Win32OpenGLRenderer();
            renderer.Init(window.Pointer, window.ClientSize);

            while (true)
            {
                window.MainLoop(() =>
                {
                    renderer.Clear(Color.FrameBg);
                    Win32OpenGLRenderer.DrawMesh(renderer.shapeMaterial, primitiveRenderer.ShapeMesh,
                        (int)window.ClientSize.Width, (int)window.ClientSize.Height);
                    renderer.SwapBuffers();
                });
                if (Input.Keyboard.Instance.KeyDown(Key.Escape))
                {
                    break;
                }
            }
        }

        [Fact]
        public void DrawText()
        {
            TextPrimitive primitive = new TextPrimitive("Hello你好こんにちは");
            var style = GUIStyle.Default;

            BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
            var textMesh = new TextMesh();
            primitiveRenderer.SetTextMesh(textMesh);
            primitiveRenderer.DrawText(primitive, new Rect(100, 100, 500, 40), style);

            //render text

            var window = new Win32Window();
            window.Init(new Point(100, 100), new Size(500, 500), WindowTypes.Regular);

            var renderer = new Win32OpenGLRenderer();
            renderer.Init(window.Pointer, window.ClientSize);

            while (true)
            {
                window.MainLoop(() =>
                {
                    renderer.Clear(Color.FrameBg);
                    Win32OpenGLRenderer.DrawTextMesh(renderer.glyphMaterial, primitiveRenderer.TextMesh,
                        (int)window.ClientSize.Width, (int)window.ClientSize.Height);
                    renderer.SwapBuffers();
                });
                if (Input.Keyboard.Instance.KeyDown(Key.Escape))
                {
                    break;
                }
            }
        }

        [Fact]
        public void DrawImage()
        {
            var window = new Win32Window();
            window.Init(new Point(100, 100), new Size(300, 400), WindowTypes.Regular);

            var renderer = new Win32OpenGLRenderer();
            renderer.Init(window.Pointer, window.ClientSize);

            var image = new Image(@"assets\images\logo.png");
            var primitive = new ImagePrimitive(image);
            primitive.Offset = new Vector(10, 10);

            var style = GUIStyle.Default;
            style.BackgroundColor = Color.White;

            var primitiveRenderer = new BuiltinPrimitiveRenderer();

            var mesh = new Mesh();
            mesh.CommandBuffer.Add(DrawCommand.Default);
            primitiveRenderer.SetImageMesh(mesh);
            primitiveRenderer.DrawImage(primitive, new Rect(10, 10, image.Width, image.Height), style);

            window.Show();

            while (true)
            {
                window.MainLoop(() =>
                {
                    renderer.Clear(Color.FrameBg);
                    Win32OpenGLRenderer.DrawMesh(renderer.imageMaterial, primitiveRenderer.ImageMesh,
                        (int)window.ClientSize.Width, (int)window.ClientSize.Height);
                    renderer.SwapBuffers();
                });
                if (Input.Keyboard.Instance.KeyDown(Key.Escape))
                {
                    break;
                }
            }
        }


        public class DrawBoxModel
        {
            [Fact]
            public void DrawBoxModelWithTextContent()
            {
                TextPrimitive textPrimitive = new TextPrimitive("Hello你好こんにちは");
                var style = GUIStyle.Default;
                style.BackgroundColor = Color.White;
                style.Border = (1, 3, 1, 3);
                style.BorderColor = Color.Black;
                style.Padding = (10, 5, 10, 5);
                style.FontSize = 24;
                style.FontColor = Color.Black;

                BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                var mesh = new Mesh();
                mesh.CommandBuffer.Add(DrawCommand.Default);
                primitiveRenderer.SetShapeMesh(mesh);
                var textMesh = new TextMesh();
                primitiveRenderer.SetTextMesh(textMesh);
                primitiveRenderer.DrawBoxModel(textPrimitive, new Rect(10, 10, 500, 60), style);

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(800, 600), WindowTypes.Regular);
                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);
                
                while (true)
                {
                    window.MainLoop(() =>
                    {
                        renderer.Clear(Color.FrameBg);
                        Win32OpenGLRenderer.DrawMesh(renderer.shapeMaterial, primitiveRenderer.ShapeMesh,
                            (int)window.ClientSize.Width, (int)window.ClientSize.Height);
                        Win32OpenGLRenderer.DrawTextMesh(renderer.glyphMaterial, primitiveRenderer.TextMesh,
                            (int)window.ClientSize.Width, (int)window.ClientSize.Height);
                        renderer.SwapBuffers();
                    });
                    if (Input.Keyboard.Instance.KeyDown(Key.Escape))
                    {
                        break;
                    }
                }
            }

            
            [Fact]
            public void DrawBoxModelWithImageContent()
            {
                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(800, 600), WindowTypes.Regular);
                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                var primitive = new ImagePrimitive(@"assets\images\logo.png");
                var style = GUIStyle.Default;
                style.BackgroundColor = Color.White;
                style.Border = (1, 3, 1, 3);
                style.BorderColor = Color.LightBlue;
                style.Padding = (10, 5, 10, 5);

                BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                var mesh = new Mesh();
                mesh.CommandBuffer.Add(DrawCommand.Default);
                primitiveRenderer.SetShapeMesh(mesh);
                var imageMesh = new Mesh();
                imageMesh.CommandBuffer.Add(DrawCommand.Default);
                primitiveRenderer.SetImageMesh(imageMesh);
                primitiveRenderer.DrawBoxModel(primitive, new Rect(10, 10, 300, 400), style);

                while (true)
                {
                    window.MainLoop(() =>
                    {
                        renderer.Clear(Color.FrameBg);
                        Win32OpenGLRenderer.DrawMesh(renderer.shapeMaterial, primitiveRenderer.ShapeMesh,
                            (int)window.ClientSize.Width, (int)window.ClientSize.Height);
                        Win32OpenGLRenderer.DrawMesh(renderer.imageMaterial, primitiveRenderer.ImageMesh,
                            (int)window.ClientSize.Width, (int)window.ClientSize.Height);
                        renderer.SwapBuffers();
                    });
                    if (Input.Keyboard.Instance.KeyDown(Key.Escape))
                    {
                        break;
                    }
                }
            }
        }
    }
}
