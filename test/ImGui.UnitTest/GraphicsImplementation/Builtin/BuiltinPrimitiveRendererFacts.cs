using ImGui.GraphicsImplementation;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class BuiltinPrimitiveRendererFacts
    {
        public class DrawPath
        {
            internal static void CheckExpectedImage(PathGeometry geometry, int width, int height, string expectedImageFilePath)
            {
                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();
                    var mesh = new Mesh();
                    geometryRenderer.DrawPathPrimitive(mesh, geometry, Vector.Zero);

                    context.Clear();
                    context.DrawShapeMesh(mesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            internal static void DrawGeometry(PathGeometry geometry, Brush brush, Pen pen, int width, int height, string expectedImageFilePath)
            {
                Application.EnableMSAA = false;

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();
                BuiltinGeometryRenderer renderer = new BuiltinGeometryRenderer();
                byte[] imageRawBytes;

                using (var context = new RenderContextForTest(width, height))
                {
                    var shapeMesh = MeshPool.ShapeMeshPool.Get();
                    shapeMesh.Clear();
                    shapeMesh.CommandBuffer.Add(DrawCommand.Default);
                    var textMesh = MeshPool.TextMeshPool.Get();
                    textMesh.Clear();
                    var imageMesh = MeshPool.ImageMeshPool.Get();
                    imageMesh.Clear();

                    renderer.SetShapeMesh(shapeMesh);
                    renderer.SetTextMesh(textMesh);
                    renderer.SetImageMesh(imageMesh);
                    renderer.DrawGeometry(brush, pen, geometry);//This must be called after the RenderContextForTest is created, for uploading textures to GPU via OpenGL.
                    renderer.SetShapeMesh(null);
                    renderer.SetTextMesh(null);
                    renderer.SetImageMesh(null);

                    meshList.AddOrUpdateShapeMesh(shapeMesh);
                    meshList.AddOrUpdateTextMesh(textMesh);
                    meshList.AddOrUpdateImageMesh(imageMesh);

                    //rebuild mesh buffer
                    meshBuffer.Clear();
                    meshBuffer.Init();
                    meshBuffer.Build(meshList);

                    //draw mesh buffer to screen
                    context.Clear();
                    context.DrawMeshes(meshBuffer);

                    imageRawBytes = context.GetRenderedRawBytes();
                }
                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }


            [Fact]
            public void StrokeAPath()
            {
                var geometry = new PathGeometry();
                var figure = new PathFigure();
                geometry.Figures.Add(figure);

                figure.StartPoint = new Point(10, 10);

                figure.Segments.Add(new LineSegment(new Point(10, 10), true));
                figure.Segments.Add(new LineSegment(new Point(10, 80), true));
                figure.Segments.Add(new LineSegment(new Point(80, 80), true));
                figure.Segments.Add(new LineSegment(new Point(80, 10), true));
                figure.Segments.Add(new LineSegment(new Point(10, 10), true));

                Pen pen = new Pen(Color.Red, 2);

                DrawGeometry(geometry, null, pen, 100, 100,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts\DrawPath\StrokeAPath.png");
            }

            [Fact]
            public void FillAPath()
            {
                var primitive = new PathGeometry();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 80));
                primitive.PathLineTo(new Point(80, 80));
                primitive.PathClose();
                primitive.PathFill(Color.Red);

                CheckExpectedImage(primitive, 100, 100,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.FillAPath.png");
            }

            [Fact]
            public void FillARect()
            {
                var primitive = new PathGeometry();
                primitive.PathRect(new Rect(10, 10, 80, 60));
                primitive.PathFill(Color.Red);

                CheckExpectedImage(primitive, 100, 100,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.FillARect.png");
            }
        }

        public class DrawText
        {
            internal static void CheckExpectedImage(TextGeometry geometry, int width, int height, Rect contentRect, string expectedImageFilePath)
            {
                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();
                    var textMesh = new TextMesh();
                    geometryRenderer.DrawTextPrimitive(textMesh, geometry, contentRect, new StyleRuleSet(), Vector.Zero);

                    context.Clear();
                    context.DrawTextMesh(textMesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }
                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Theory]
            [InlineData("Hello你好こんにちは")]
            [InlineData("textwithoutspace")]
            [InlineData("text with space")]
            public void DrawOnelineText(string text)
            {
                TextGeometry geometry = new TextGeometry(text);

                CheckExpectedImage(geometry, 200, 50, new Rect(10, 10, 200, 40),
                    $@"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawText.DrawOnelineText_{text}.png");
            }
        }

        public class DrawImage
        {
            internal static void CheckExpectedImage(ImageGeometry geometry, int width, int height, Rect contentRect, StyleRuleSet style, string expectedImageFilePath)
            {
                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();
                    var mesh = new Mesh();
                    geometryRenderer.DrawImagePrimitive(mesh, geometry, contentRect, style, Vector.Zero);

                    context.Clear();
                    context.DrawImageMesh(mesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void DrawOriginalImage()
            {
                var primitive = new ImageGeometry(@"assets\images\logo.png");
                var styleRuleSet = new StyleRuleSet {BackgroundColor = Color.White};

                CheckExpectedImage(primitive, 300, 400,
                    new Rect(10, 10, primitive.Image.Width, primitive.Image.Height), styleRuleSet,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawImage.DrawOriginalImage.png");
            }
        }

        public class DrawSlicedImage
        {
            internal static void CheckExpectedImage(ImageGeometry geometry, int width, int height, Rect rect, StyleRuleSet style, string expectedImageFilePath)
            {
                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();
                    var mesh = new Mesh();
                    geometryRenderer.DrawSlicedImagePrimitive(mesh, geometry, rect, style, Vector.Zero);

                    context.Clear();
                    context.DrawImageMesh(mesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void DrawOneImage()
            {
                var primitive = new ImageGeometry(@"assets\images\button.png");
                var styleRuleSet = new StyleRuleSet {BorderImageSlice = (83, 54, 54, 54)};

                CheckExpectedImage(primitive, 300, 400,
                    new Rect(2, 2, primitive.Image.Width + 50, primitive.Image.Height + 100), styleRuleSet,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawSlicedImage.DrawOneImage.png");
            }
        }

        public class DrawBoxModel
        {
            [Fact]
            public void DrawEmptyBoxModel()
            {
                var styleRuleSet = new StyleRuleSet();
                var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
                styleRuleSetBuilder
                    .BackgroundColor(Color.White)
                    .Border((1, 3, 1, 3))
                    .BorderColor(Color.Black)
                    .Padding((10, 5, 10, 5));
                var rect = new Rect(10, 10, 300, 60);
                const string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawBoxModel.DrawEmptyBoxModel.png";
                const int width = 400, height = 100;

                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();
                    var mesh = new Mesh();
                    mesh.CommandBuffer.Add(DrawCommand.Default);
                    geometryRenderer.SetShapeMesh(mesh);
                    var textMesh = new TextMesh();
                    geometryRenderer.SetTextMesh(textMesh);
                    var imageMesh = new Mesh();
                    geometryRenderer.SetImageMesh(imageMesh);
                    geometryRenderer.DrawBoxModel(rect, styleRuleSet);

                    context.Clear();
                    context.DrawShapeMesh(mesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void DrawBoxModelWithTextContent()
            {
                TextGeometry textGeometry = new TextGeometry("Hello你好こんにちは");
                var styleRuleSet = new StyleRuleSet();
                var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
                styleRuleSetBuilder
                    .BackgroundColor(Color.White)
                    .Border((1, 3, 1, 3))
                    .BorderColor(Color.Black)
                    .Padding((10, 5, 10, 5))
                    .FontSize(24)
                    .FontColor(Color.Black);
                var rect = new Rect(10, 10, 350, 60);

                const string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawBoxModel.DrawBoxModelWithTextContent.png";
                const int width = 400, height = 100;

                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();
                    var mesh = new Mesh();
                    mesh.CommandBuffer.Add(DrawCommand.Default);
                    geometryRenderer.SetShapeMesh(mesh);
                    var textMesh = new TextMesh();
                    geometryRenderer.SetTextMesh(textMesh);
                    var imageMesh = new Mesh();
                    geometryRenderer.SetImageMesh(imageMesh);
                    geometryRenderer.DrawBoxModel(textGeometry, rect, styleRuleSet);

                    context.Clear();
                    context.DrawShapeMesh(mesh);
                    context.DrawTextMesh(textMesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void DrawBoxModelWithImageContent()
            {
                var primitive = new ImageGeometry(@"assets\images\logo.png");

                var ruleSet = new StyleRuleSet();
                var styleSetBuilder = new StyleRuleSetBuilder(ruleSet);
                styleSetBuilder
                    .BackgroundColor(Color.White)
                    .Border((top: 1, right: 3, bottom: 1, left: 3))
                    .BorderColor(Color.LightBlue)
                    .Padding((10, 5, 10, 5));
                var rect = new Rect(10, 10, 300, 400);

                const string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawBoxModel.DrawBoxModelWithImageContent.png";
                const int width = 500, height = 500;

                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();
                    var mesh = new Mesh();
                    mesh.CommandBuffer.Add(DrawCommand.Default);
                    geometryRenderer.SetShapeMesh(mesh);
                    var imageMesh = new Mesh();
                    geometryRenderer.SetImageMesh(imageMesh);
                    geometryRenderer.DrawBoxModel(primitive, rect, ruleSet);

                    context.Clear();
                    context.DrawShapeMesh(mesh);
                    context.DrawImageMesh(imageMesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }
        }
    }
}
