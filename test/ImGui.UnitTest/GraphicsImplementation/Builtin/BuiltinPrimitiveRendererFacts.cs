using ImGui.Common.Primitive;
using ImGui.GraphicsImplementation;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class BuiltinPrimitiveRendererFacts
    {
        public class DrawPath
        {
            [Fact]
            public void StrokeAPath()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 80));
                primitive.PathLineTo(new Point(80, 80));
                primitive.PathLineTo(new Point(80, 10));
                primitive.PathClose();
                primitive.PathStroke(2, Color.Red);

                Util.CheckExpectedImage(primitive, 100, 100,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.StrokeAPath.png");
            }

            [Fact]
            public void FillAPath()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 80));
                primitive.PathLineTo(new Point(80, 80));
                primitive.PathClose();
                primitive.PathFill(Color.Red);

                Util.CheckExpectedImage(primitive, 100, 100,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.FillAPath.png");
            }

            [Fact]
            public void FillARect()
            {
                var primitive = new PathPrimitive();
                primitive.PathRect(new Rect(10, 10, 80, 60));
                primitive.PathFill(Color.Red);

                Util.CheckExpectedImage(primitive, 100, 100,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.FillARect.png");
            }
        }

        public class DrawText
        {
            [Theory]
            [InlineData("Hello你好こんにちは")]
            [InlineData("textwithoutspace")]
            [InlineData("text with space")]
            public void DrawOnelineText(string text)
            {
                TextPrimitive primitive = new TextPrimitive(text);

                Util.CheckExpectedImage(primitive, 200, 50, new Rect(10, 10, 200, 40),
                    $@"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawText.DrawOnelineText_{text}.png");
            }
        }

        public class DrawImage
        {
            [Fact]
            public void DrawOriginalImage()
            {
                var primitive = new ImagePrimitive(@"assets\images\logo.png");
                var styleRuleSet = new StyleRuleSet {BackgroundColor = Color.White};

                Util.CheckExpectedImage(primitive, 300, 400, styleRuleSet,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawImage.DrawOriginalImage.png");
            }
        }

        public class DrawSlicedImage
        {
            [Fact]
            public void DrawOneImage()
            {
                var primitive = new ImagePrimitive(@"assets\images\button.png");
                var styleRuleSet = new StyleRuleSet {BorderImageSlice = (83, 54, 54, 54)};

                Util.CheckExpectedImageSliced(primitive, 300, 400,
                    new Rect(2, 2, primitive.Image.Width + 50, primitive.Image.Height + 100), styleRuleSet,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawSlicedImage.DrawOneImage.png");
            }
        }

        public class DrawBoxModel
        {
            [Fact]
            public void DrawEmptyBoxModel()
            {
                byte[] imageRawBytes;
                int width, height;
                using (var context = new RenderContextForTest(400, 100))
                {
                    var styleRuleSet = new StyleRuleSet();
                    var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
                    styleRuleSetBuilder
                        .BackgroundColor(Color.White)
                        .Border((1, 3, 1, 3))
                        .BorderColor(Color.Black)
                        .Padding((10, 5, 10, 5));

                    BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                    var mesh = new Mesh();
                    mesh.CommandBuffer.Add(DrawCommand.Default);
                    primitiveRenderer.SetShapeMesh(mesh);
                    primitiveRenderer.DrawBoxModel(new Rect(10, 10, 300, 60), styleRuleSet);

                    context.Clear();
                    context.DrawShapeMesh(mesh);

                    imageRawBytes = context.Renderer.GetRawBackBuffer(out width, out height);
                }

                var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
                string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawBoxModel.DrawEmptyBoxModel.png";
                #if GenerateExpectedImages
                Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
                #else
                var expectedImage = Util.LoadImage(expectedImageFilePath);
                Assert.True(Util.CompareImage(expectedImage, image));
                #endif
            }

            [Fact]
            public void DrawBoxModelWithTextContent()
            {
                byte[] imageRawBytes;
                int width, height;
                using (var context = new RenderContextForTest(400, 100))
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
                    primitiveRenderer.DrawBoxModel(textPrimitive, new Rect(10, 10, 350, 60), styleRuleSet);

                    context.Clear();
                    context.DrawShapeMesh(mesh);
                    context.DrawTextMesh(textMesh);

                    imageRawBytes = context.Renderer.GetRawBackBuffer(out width, out height);
                }

                var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
                string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawBoxModel.DrawBoxModelWithTextContent.png";
                #if GenerateExpectedImages
                Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
                #else
                var expectedImage = Util.LoadImage(expectedImageFilePath);
                Assert.True(Util.CompareImage(expectedImage, image));
                #endif
            }

            [Fact]
            public void DrawBoxModelWithImageContent()
            {
                byte[] imageRawBytes;
                int width, height;
                using (var context = new RenderContextForTest(500, 500))
                {
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

                    context.Clear();
                    context.DrawShapeMesh(mesh);
                    context.DrawImageMesh(imageMesh);

                    imageRawBytes = context.Renderer.GetRawBackBuffer(out width, out height);
                }

                var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
                string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawBoxModel.DrawBoxModelWithImageContent.png";
                #if GenerateExpectedImages
                Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
                #else
                var expectedImage = Util.LoadImage(expectedImageFilePath);
                Assert.True(Util.CompareImage(expectedImage, image));
                #endif
            }
        }
    }
}
