using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.GraphicsImplementation;
using ImGui.Input;
using ImGui.OSImplentation;
using ImGui.OSImplentation.Windows;
using ImGui.Rendering;
using System.Collections.Generic;
using System.Diagnostics;
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

            var primitiveRenderer = new BuiltinPrimitiveRenderer();
            var brush = new Brush();
            brush.LineColor = Color.Red;
            
            var mesh = new Mesh();
            mesh.CommandBuffer.Add(DrawCommand.Default);
            primitiveRenderer.SetShapeMesh(mesh);

            primitiveRenderer.Stroke(primitive, brush, new StrokeStyle());

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

            var primitiveRenderer = new BuiltinPrimitiveRenderer();
            var brush = new Brush();
            brush.LineColor = Color.Red;
            var mesh = new Mesh();
            mesh.CommandBuffer.Add(DrawCommand.Default);
            primitiveRenderer.SetShapeMesh(mesh);
            primitiveRenderer.Fill(primitive, brush);

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
            TextPrimitive primitive = new TextPrimitive();
            primitive.Text = "Hello你好こんにちは";
            primitive.Rect = new Rect(10, 10, 500, 40);
            var style = GUIStyle.Default;

            BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
            var textMesh = new TextMesh();
            primitiveRenderer.SetTextMesh(textMesh);
            primitiveRenderer.DrawText(primitive, style.FontFamily, style.FontSize, style.FontColor, style.FontStyle, style.FontWeight);

            //render text

            var window = new Win32Window();
            window.Init(new Point(100, 100), new Size(500, 400), WindowTypes.Regular);

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
            var primitive = new ImagePrimitive();
            primitive.Image = image;
            primitive.Offset = new Vector(10, 10);

            var primitiveRenderer = new BuiltinPrimitiveRenderer();
            var brush = new Brush();
            brush.FillColor = Color.White;

            var mesh = new Mesh();
            mesh.CommandBuffer.Add(DrawCommand.Default);
            primitiveRenderer.SetImageMesh(mesh);
            primitiveRenderer.DrawImage(primitive, brush);


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
    }
}
