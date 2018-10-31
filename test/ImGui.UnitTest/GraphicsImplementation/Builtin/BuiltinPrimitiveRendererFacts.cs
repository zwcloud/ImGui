//#define GenerateExpectedImages

using ImGui.Common.Primitive;
using ImGui.GraphicsImplementation;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public partial class BuiltinPrimitiveRendererFacts
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

                var primitiveRenderer = new BuiltinPrimitiveRenderer();

                var mesh = new Mesh();
                mesh.CommandBuffer.Add(DrawCommand.Default);
                primitiveRenderer.SetShapeMesh(mesh);

                primitiveRenderer.DrawPath(primitive);

                var image = Util.RenderShapeMeshToImage(primitiveRenderer.ShapeMesh, new Size(100, 100));
                const string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.StrokeAPath.png";
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
                primitive.PathLineTo(new Point(10, 80));
                primitive.PathLineTo(new Point(80, 80));
                primitive.PathLineTo(new Point(80, 10));
                primitive.PathClose();
                primitive.PathFill(Color.Red);

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                var mesh = new Mesh();
                mesh.CommandBuffer.Add(DrawCommand.Default);
                primitiveRenderer.SetShapeMesh(mesh);
                primitiveRenderer.DrawPath(primitive);

                var image = Util.RenderShapeMeshToImage(primitiveRenderer.ShapeMesh, new Size(100, 100));
                const string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.FillAPath.png";
                #if GenerateExpectedImages
                Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
                #else
                var expectedImage = Util.LoadImage(expectedImageFilePath);
                Assert.True(Util.CompareImage(expectedImage, image));
                #endif
            }

            [Fact]
            public void FillARect()
            {
                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                var mesh = new Mesh();
                mesh.CommandBuffer.Add(DrawCommand.Default);
                primitiveRenderer.SetShapeMesh(mesh);
                primitiveRenderer.PathRect(new Rect(10, 10, 100, 40));
                primitiveRenderer.PathFill(Color.Red);

                var image = Util.RenderShapeMeshToImage(primitiveRenderer.ShapeMesh, new Size(100, 100));
                const string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.FillARect.png";
#if GenerateExpectedImages
                Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
#else
                var expectedImage = Util.LoadImage(expectedImageFilePath);
                Assert.True(Util.CompareImage(expectedImage, image));
                #endif
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

                BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                var textMesh = new TextMesh();
                primitiveRenderer.SetTextMesh(textMesh);
                primitiveRenderer.DrawText(primitive, new Rect(10, 10, 200, 40), new StyleRuleSet());

                var image = Util.RenderTextMeshToImage(primitiveRenderer.TextMesh, new Size(200, 50));
                string expectedImageFilePath =
                    $@"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawText.DrawOnelineText_{text}.png";
                #if GenerateExpectedImages
                Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
                #else
                var expectedImage = Util.LoadImage(expectedImageFilePath);
                Assert.True(Util.CompareImage(expectedImage, image));
                #endif
            }
        }

        public class DrawImage
        {
            [Fact]
            public void DrawOriginalImage()
            {
                byte[] imageRawBytes;
                int width, height;
                using (var context = new RenderContextForTest(300, 400))
                {
                    var primitive = new ImagePrimitive(@"assets\images\logo.png");
                    primitive.Offset = new Vector(10, 10);

                    var styleRuleSet = new StyleRuleSet();
                    var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
                    styleRuleSetBuilder.BackgroundColor(Color.White);

                    var primitiveRenderer = new BuiltinPrimitiveRenderer();

                    var mesh = new Mesh();
                    mesh.CommandBuffer.Add(DrawCommand.Default);
                    primitiveRenderer.SetImageMesh(mesh);
                    primitiveRenderer.DrawImage(primitive, new Rect(10, 10, primitive.Image.Width, primitive.Image.Height), styleRuleSet);

                    context.Clear();
                    context.DrawImageMesh(mesh);

                    imageRawBytes = context.Renderer.GetRawBackBuffer(out width, out height);
                }

                var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
                string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawImage.DrawOriginalImage.png";
                #if GenerateExpectedImages
                Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
                #else
                var expectedImage = Util.LoadImage(expectedImageFilePath);
                Assert.True(Util.CompareImage(expectedImage, image));
                #endif
            }
        }

        public class DrawSlicedImage
        {
            [Fact]
            public void DrawOneImage()
            {
                byte[] imageRawBytes;
                int width, height;
                using (var context = new RenderContextForTest(300, 400))
                {
                    var styleRuleSet = new StyleRuleSet();
                    var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
                    styleRuleSetBuilder
                        .BorderImageSource(@"assets\images\button.png")
                        .BorderImageSlice((83, 54, 54, 54));

                    var primitiveRenderer = new BuiltinPrimitiveRenderer();

                    //build image and get the image primitive
                    var rule = styleRuleSet.GetRule<string>(GUIStyleName.BorderImageSource);
                    if (rule.primitive == null)
                    {
                        rule.primitive = new ImagePrimitive(rule.Value);
                    }
                    Assert.True(rule.primitive is ImagePrimitive);
                    var primitive = (ImagePrimitive) rule.primitive;

                    var mesh = new Mesh();
                    mesh.CommandBuffer.Add(DrawCommand.Default);
                    primitiveRenderer.SetImageMesh(mesh);
                    primitiveRenderer.DrawSlicedImage(primitive, new Rect(2, 2, primitive.Image.Width+50, primitive.Image.Height+100), styleRuleSet);

                    context.Clear();
                    context.DrawImageMesh(mesh);

                    imageRawBytes = context.Renderer.GetRawBackBuffer(out width, out height);
                }

                var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
                string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawSlicedImage.DrawOneImage.png";
#if GenerateExpectedImages
                Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
                #else
                var expectedImage = Util.LoadImage(expectedImageFilePath);
                Assert.True(Util.CompareImage(expectedImage, image));
#endif
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
