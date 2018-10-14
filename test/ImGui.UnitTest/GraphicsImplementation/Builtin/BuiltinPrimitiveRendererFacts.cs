//#define GenerateExpectedImages
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
            primitive.PathMoveTo(new Point(10, 10)  );
            primitive.PathLineTo(new Point(10, 100) );
            primitive.PathLineTo(new Point(100, 100));
            primitive.PathLineTo(new Point(100, 10) );
            primitive.PathClose();
            primitive.PathStroke(2, Color.Red);

            var primitiveRenderer = new BuiltinPrimitiveRenderer();

            var mesh = new Mesh();
            mesh.CommandBuffer.Add(DrawCommand.Default);
            primitiveRenderer.SetShapeMesh(mesh);

            primitiveRenderer.DrawPath(primitive);

            var image = Util.RenderShapeMeshToImage(primitiveRenderer.ShapeMesh, new Size(300, 400));
            const string expectedImageFilePath =
                @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.StrokeAPath.png";
            #if GenerateExpectedImages
            Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
            #else
            var expectedImage = Util.LoadImage(expectedImageFilePath);
            Assert.True(Util.CompareImage(expectedImage, image));
            #endif
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

            var image = Util.RenderShapeMeshToImage(primitiveRenderer.ShapeMesh, new Size(300, 400));
            const string expectedImageFilePath =
                @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.FillAPath.png";
            #if GenerateExpectedImages
            Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
            #else
            var expectedImage = Util.LoadImage(expectedImageFilePath);
            Assert.True(Util.CompareImage(expectedImage, image));
            #endif
        }

        [Theory]
        [InlineData("Hello你好こんにちは")]
        [InlineData("textwithoutspace")]
        [InlineData("text with space")]
        public void DrawText(string text)
        {
            TextPrimitive primitive = new TextPrimitive(text);

            BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
            var textMesh = new TextMesh();
            primitiveRenderer.SetTextMesh(textMesh);
            primitiveRenderer.DrawText(primitive, new Rect(100, 100, 500, 40), new StyleRuleSet());
            
            var image = Util.RenderTextMeshToImage(primitiveRenderer.TextMesh, new Size(500, 500));
            string expectedImageFilePath =
                $@"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawText_{text}.png";
#if GenerateExpectedImages
            Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
            #else
            var expectedImage = Util.LoadImage(expectedImageFilePath);
            Assert.True(Util.CompareImage(expectedImage, image));
#endif
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

            var styleRuleSet = new StyleRuleSet();
            var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
            styleRuleSetBuilder.BackgroundColor(Color.White);

            var primitiveRenderer = new BuiltinPrimitiveRenderer();

            var mesh = new Mesh();
            mesh.CommandBuffer.Add(DrawCommand.Default);
            primitiveRenderer.SetImageMesh(mesh);
            primitiveRenderer.DrawImage(primitive, new Rect(10, 10, image.Width, image.Height), styleRuleSet);

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
                var styleRuleSet = new StyleRuleSet();
                var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
                styleRuleSetBuilder
                    .BackgroundColor(Color.White)
                    .Border((1, 3, 1, 3))
                    .BorderColor(Color.Black)
                    .Padding((10, 5, 10, 5))
                    .FontSize(24)
                    .FontColor(Color.Black);

                BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                var mesh = new Mesh();
                mesh.CommandBuffer.Add(DrawCommand.Default);
                primitiveRenderer.SetShapeMesh(mesh);
                var textMesh = new TextMesh();
                primitiveRenderer.SetTextMesh(textMesh);
                primitiveRenderer.DrawBoxModel(textPrimitive, new Rect(10, 10, 500, 60), styleRuleSet);

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

                var ruleSet = new StyleRuleSet();
                var styleSetBuilder = new StyleRuleSetBuilder(ruleSet);
                styleSetBuilder
                    .BackgroundColor(Color.White)
                    .Border((top: 1, right: 3, bottom: 1, left: 3))
                    .BorderColor(Color.LightBlue)
                    .Padding((10, 5, 10, 5));

                BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                var mesh = new Mesh();
                mesh.CommandBuffer.Add(DrawCommand.Default);
                primitiveRenderer.SetShapeMesh(mesh);
                var imageMesh = new Mesh();
                imageMesh.CommandBuffer.Add(DrawCommand.Default);
                primitiveRenderer.SetImageMesh(imageMesh);
                primitiveRenderer.DrawBoxModel(primitive, new Rect(10, 10, 300, 400), ruleSet);

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
