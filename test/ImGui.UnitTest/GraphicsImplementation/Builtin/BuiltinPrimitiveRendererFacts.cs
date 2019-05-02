using System;
using System.Runtime.CompilerServices;
using ImGui.GraphicsImplementation;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class BuiltinPrimitiveRendererFacts
    {
        private const string RootDir = @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts\";

        public class DrawLine
        {
            internal static void CheckLine(Pen pen, Point point0, Point point1,
                int width, int height,
                [CallerMemberName] string methodName = "unknown")
            {
                Application.EnableMSAA = false;

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();
                BuiltinGeometryRenderer renderer = new BuiltinGeometryRenderer();
                byte[] bytes;

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

                    renderer.DrawLine(pen, point0, point1);//This must be called after the RenderContextForTest is created, for uploading textures to GPU via OpenGL.

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

                    bytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(bytes, width, height, $"{RootDir}{nameof(DrawLine)}\\{methodName}.png");
            }

            [Fact]
            public void DrawALine()
            {
                Pen pen = new Pen(Color.Black, 1);
                Point p0 = new Point(10, 10);
                Point p1 = new Point(90, 10);

                CheckLine(pen, p0, p1, 100, 100);
            }

            [Fact]
            public void DrawAThickLine()
            {
                Pen pen = new Pen(Color.Black, 5);
                Point p0 = new Point(10, 10);
                Point p1 = new Point(90, 10);

                CheckLine(pen, p0, p1, 100, 100);
            }
        }

        public class DrawRectangle
        {
            internal static void CheckRectangle(Brush brush, Pen pen, Rect rectangle,
                int width, int height,
                [CallerMemberName] string methodName = "unknown")
            {
                Application.EnableMSAA = false;

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();
                BuiltinGeometryRenderer renderer = new BuiltinGeometryRenderer();
                byte[] bytes;

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

                    renderer.DrawRectangle(brush, pen, rectangle);//This must be called after the RenderContextForTest is created, for uploading textures to GPU via OpenGL.

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

                    bytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(bytes, width, height, $"{RootDir}{nameof(DrawRectangle)}\\{methodName}.png");
            }

            [Fact]
            public void DrawARectangle()
            {
                Brush brush = new Brush(Color.Aqua);
                Pen pen = new Pen(Color.Black, 4);
                Rect rectangle = new Rect(new Point(20, 20), new Point(80, 80));

                CheckRectangle(brush, pen, rectangle, 100, 100);
            }
        }

        public class DrawRoundedRectangle
        {
            internal static void Check(Rect rectangle, double radiusX, double radiusY, Brush brush, Pen pen, int width, int height,
                [CallerMemberName] string methodName = "unknown")
            {
                Application.EnableMSAA = false;

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();
                BuiltinGeometryRenderer renderer = new BuiltinGeometryRenderer();
                byte[] bytes;

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
                    renderer.DrawRoundedRectangle(brush, pen, rectangle, radiusX, radiusY);//This must be called after the RenderContextForTest is created, for uploading textures to GPU via OpenGL.
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

                    bytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(bytes, width, height, $"{RootDir}{nameof(DrawRoundedRectangle)}\\{methodName}.png");
            }

            [Fact]
            public void DrawARectangle()
            {
                Brush brush = new Brush(Color.Aqua);
                Pen pen = new Pen(Color.Black, 1);
                Rect rectangle = new Rect(new Point(20, 20), new Point(160, 160));

                Check(rectangle, 20, 40, brush, pen, 200, 200);
            }
        }

        public class DrawGeometry
        {
            internal static void CheckGeometry(Geometry geometry, Brush brush, Pen pen, int width, int height,
                [CallerMemberName] string methodName = "unknown")
            {
                Application.EnableMSAA = false;

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();
                BuiltinGeometryRenderer renderer = new BuiltinGeometryRenderer();
                byte[] bytes;

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

                    bytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(bytes, width, height, $"{RootDir}{nameof(DrawGeometry)}\\{methodName}.png");
            }

            [Fact]
            public void StrokeAPathGeometry()
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
                figure.IsClosed = true;
                Pen pen = new Pen(Color.Red, 2);
                CheckGeometry(geometry, null, pen, 100, 100);
            }

            [Fact]
            public void FillAPathGeometry()
            {
                var geometry = new PathGeometry();
                var figure = new PathFigure();
                figure.IsFilled = true;
                geometry.Figures.Add(figure);
                figure.StartPoint = new Point(10, 10);
                figure.Segments.Add(new LineSegment(new Point(10, 10), false));
                figure.Segments.Add(new LineSegment(new Point(10, 80), false));
                figure.Segments.Add(new LineSegment(new Point(80, 80), false));
                figure.Segments.Add(new LineSegment(new Point(80, 10), false));
                figure.Segments.Add(new LineSegment(new Point(10, 10), false));
                figure.IsClosed = true;
                Brush brush = new Brush(Color.Red);
                CheckGeometry(geometry, brush, null, 100, 100);
            }
        }

        public class DrawGlyphRun
        {
            internal static void Check(Rect rectangle, GlyphRun glyphRun, Brush brush, int width, int height,
                [CallerMemberName] string methodName = "unknown")
            {
                Application.EnableMSAA = false;

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();
                BuiltinGeometryRenderer renderer = new BuiltinGeometryRenderer();
                byte[] bytes;

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
                    renderer.DrawGlyphRun(brush, glyphRun, rectangle);//This must be called after the RenderContextForTest is created, for uploading textures to GPU via OpenGL.
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

                    bytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(bytes, width, height, $"{RootDir}{nameof(DrawGlyphRun)}\\{methodName}.png");
            }

            [Fact]
            public void DrawOneLineText()
            {
                GlyphRun glyphRun = new GlyphRun("Hello你好こんにちは", GUIStyle.Default.FontFamily, 24, FontStyle.Normal, FontWeight.Normal);
                Brush brush = new Brush(Color.Black);

                Check(new Rect(10, 10, 400, 40), glyphRun, brush, 400, 50);
            }

            [Fact]
            public void DrawOneLineTextWithoutSpace()
            {
                GlyphRun glyphRun = new GlyphRun("textwithoutspace", GUIStyle.Default.FontFamily, 24, FontStyle.Normal, FontWeight.Normal);
                Brush brush = new Brush(Color.Black);

                Check(new Rect(10, 10, 400, 40), glyphRun, brush, 400, 50);
            }

            [Fact]
            public void DrawOneLineTextWithSpace()
            {
                GlyphRun glyphRun = new GlyphRun("text with space", GUIStyle.Default.FontFamily, 24, FontStyle.Normal, FontWeight.Normal);
                Brush brush = new Brush(Color.Black);

                Check(new Rect(10, 10, 400, 40), glyphRun, brush, 400, 50);
            }

            [Fact]
            public void DrawMultipleLineText()
            {
                throw new Exception("The result is incorrect. FIXME.");

                GlyphRun glyphRun = new GlyphRun("Hello\n你好\nこんにちは", GUIStyle.Default.FontFamily, 24, FontStyle.Normal, FontWeight.Normal);
                Brush brush = new Brush(Color.Black);

                Check(new Rect(10, 10, 400, 120), glyphRun, brush, 400, 130);
            }

        }

        public class DrawImage
        {
            internal static void Check(System.Func<ImGui.OSAbstraction.Graphics.ITexture> textureGettter, Rect rectangle, int width, int height,
                [CallerMemberName] string methodName = "unknown")
            {
                Application.EnableMSAA = false;

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();
                BuiltinGeometryRenderer renderer = new BuiltinGeometryRenderer();
                byte[] bytes;

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
                    renderer.DrawImage(textureGettter(), rectangle);//This must be called after the RenderContextForTest is created, for uploading textures to GPU via OpenGL.
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

                    bytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(bytes, width, height, $"{RootDir}{nameof(DrawImage)}\\{methodName}.png");
            }

            internal static void Check(System.Func<ImGui.OSAbstraction.Graphics.ITexture> textureGettter, Rect rectangle, (double top, double right, double bottom, double left) slice, int width, int height,
                [CallerMemberName] string methodName = "unknown")
            {
                Application.EnableMSAA = false;

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();
                BuiltinGeometryRenderer renderer = new BuiltinGeometryRenderer();
                byte[] bytes;

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
                    renderer.DrawImage(textureGettter(), rectangle, slice);//This must be called after the RenderContextForTest is created, for uploading textures to GPU via OpenGL.
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

                    bytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(bytes, width, height, $"{RootDir}{nameof(DrawImage)}\\{methodName}.png");
            }

            [Fact]
            public void DrawOriginalImage()
            {
                var image = new ImGui.GraphicsAbstraction.Image(@"assets\images\logo.png");

                Check(() =>
                    {
                        var texture = Application.PlatformContext.CreateTexture();
                        texture.LoadImage(image.Data, image.Width, image.Height);
                        return texture;
                    },
                    new Rect(10, 10, image.Width, image.Height), 300, 300);
            }

            [Fact]
            public void DrawScaledImage()
            {
                var image = new ImGui.GraphicsAbstraction.Image(@"assets\images\logo.png");

                Check(() =>
                    {
                        var texture = Application.PlatformContext.CreateTexture();
                        texture.LoadImage(image.Data, image.Width, image.Height);
                        return texture;
                    },
                    new Rect(10, 10, 200, 100), 250, 250);
            }

            [Fact]
            public void DrawSlicedImage1()
            {
                var image = new ImGui.GraphicsAbstraction.Image(@"assets\images\button.png");

                Check(() =>
                    {
                        var texture = Application.PlatformContext.CreateTexture();
                        texture.LoadImage(image.Data, image.Width, image.Height);
                        return texture;
                    },
                    new Rect(2, 2, image.Width + 50, image.Height*0.8),
                    (83, 54, 54, 54),
                    500, 500);
            }

            [Fact]
            public void DrawSlicedImage2()
            {
                var image = new ImGui.GraphicsAbstraction.Image(@"assets\images\button.png");

                Check(() =>
                    {
                        var texture = Application.PlatformContext.CreateTexture();
                        texture.LoadImage(image.Data, image.Width, image.Height);
                        return texture;
                    },
                    new Rect(2, 2, image.Width*1.5, image.Height*1.8),
                    (83, 54, 54, 54),
                    500, 500);
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
