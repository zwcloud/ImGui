using ImGui;
using ImGui.Rendering;
using ImGui.Style;
using ImGui.OSImplementation;
using ImGui.OSAbstraction.Text;
using ImGui.Development;
using System;
using System.Diagnostics;
using Xunit;
using System.IO;

namespace TextRenderingTest
{
    public class TypographyTextContextTest
    {
        static readonly string FontFile = Utility.FontDir + "DroidSans.ttf";
        private const string ModelViewerPath = @"C:\Program Files\Autodesk\FBX Review\fbxreview.exe";

        public TypographyTextContextTest()
        {
            Application.InitSysDependencies();
        }
        
        static readonly string PathImagePath =
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +  "typography_path_image.png";
        static readonly string PathTextPath =
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "typography_path_text.txt";
        Cairo.ImageSurface surface;
        Cairo.Context g;

        [Fact]
        public void ShouldGetCorrectOutline()
        {
            ITextContext textContext = new TypographyTextContext(
                "0123456", FontFile, 36,
                TextAlignment.Leading);

            // prepare debug contexts
            {
                surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 2000, 2000);
                g = new Cairo.Context(surface);
                g.SetSourceRGBA(1, 1, 1, 1);
                g.Paint();
                g.SetSourceRGBA(0, 0, 0, 1);
                g.LineWidth = 1;
            }

            // build path
            CairoPathBuilder cairoPathBuilder;
            {
                cairoPathBuilder = new CairoPathBuilder(g, 0, 0, 1);
                //TODO Implement CairoRenderer as a ImGui.Rendering.Composition.GeometryRenderer
                //textContext.Build(Point.Zero, cairoPathBuilder);
            }

            // show debug results
            // image
            {
                surface.WriteToPng(PathImagePath);
                g.Dispose();
            
                Assert.False(PathImagePath.Contains(" "));
                // open the image in Windows Photo Viewer
                Process.Start(@"C:\WINDOWS\System32\rundll32.exe", @"""C:\Program Files\Windows Photo Viewer\PhotoViewer.dll"", ImageView_Fullscreen " + PathImagePath);
            }
            //text
            {
                var text = cairoPathBuilder.Result;
                File.WriteAllText(PathTextPath, text);
                Process.Start(@"notepad", PathTextPath);
            }

            // Now, check if the output text and image shows a right outline of the text.
        }

        [Fact]
        public void ShouldGetCorrectOffsetOfAGlyph()
        {
            var text = "A";
            var fontFamily = Utility.FontDir + "msjh.ttf";
            var fontSize = 36;
            GlyphRun glyphRun = new GlyphRun(new Point(50, 100), text, fontFamily, fontSize);

            var geometryRenderer = new ImGui.GraphicsImplementation.BuiltinGeometryRenderer();
            var textMesh = new TextMesh();
            geometryRenderer.SetTextMesh(textMesh);
            geometryRenderer.DrawGlyphRun(new Brush(Color.Black), glyphRun);
            geometryRenderer.SetTextMesh(null);

            var objFilePath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                + "/ShouldGetARightMeshFromTypography.obj";
            ImGui.Development.Graphics.SaveTextMeshToObjFile(objFilePath, textMesh);
            Process.Start(ModelViewerPath, objFilePath);
        }

        [Fact]
        public void ShouldGetARightMeshFromTypography()
        {
            var text = "8";
            var fontFamily = Utility.FontDir + "msjh.ttf";
            var fontSize = 8;
            GlyphRun glyphRun = new GlyphRun(new Point(50, 100), text, fontFamily, fontSize);

            var geometryRenderer = new ImGui.GraphicsImplementation.BuiltinGeometryRenderer();
            var textMesh = new TextMesh();
            geometryRenderer.SetTextMesh(textMesh);
            geometryRenderer.DrawGlyphRun(new Brush(Color.Black), glyphRun);
            geometryRenderer.SetTextMesh(null);

            //PathUtil.SaveToPng(paths, @"D:\TypographyTextPath.png");

            var objFilePath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                + "/ShouldGetARightMeshFromTypography.obj";
            ImGui.Development.Graphics.SaveTextMeshToObjFile(objFilePath, textMesh);
            Process.Start(ModelViewerPath, objFilePath);
        }


        [Fact]
        public void XyToIndexShouldWorkCorrectly()
        {
            ITextContext context =
                new TypographyTextContext("0123456", FontFile, 36, TextAlignment.Leading);

            // prepare debug contexts
            {
                surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 2000, 2000);
                g = new Cairo.Context(surface);
                g.SetSourceRGBA(1, 1, 1, 1);
                g.Paint();
                g.SetSourceRGBA(0, 0, 0, 1);
                g.LineWidth = 1;
            }

            // build path
            CairoPathBuilder cairoPathBuilder;
            cairoPathBuilder = new CairoPathBuilder(g, 0, 0, 1);
            //FIXME fix this when CairoRenderer is ready
            //context.Build(Point.Zero, cairoPathBuilder);

            bool isInside = false;
            uint charIndex = context.XyToIndex(-1, 0, out isInside);

            Assert.False(isInside);
            Assert.Equal(0u, charIndex);

            charIndex = context.XyToIndex(13, 0, out isInside);
            Assert.True(isInside);
            Assert.Equal(0u, charIndex);

            charIndex = context.XyToIndex(37, 0, out isInside);
            Assert.True(isInside);
            Assert.Equal(1u, charIndex);

            charIndex = context.XyToIndex(64, 0, out isInside);
            Assert.True(isInside);
            Assert.Equal(2u, charIndex);

            charIndex = context.XyToIndex(89, 0, out isInside);
            Assert.True(isInside);
            Assert.Equal(3u, charIndex);

        }

        [Fact]
        public void IndexToXyShouldWorkCorrectly()
        {
            ITextContext context = 
                new TypographyTextContext("0123456", FontFile, 36, TextAlignment.Leading);

            // prepare debug contexts
            {
                surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 2000, 2000);
                g = new Cairo.Context(surface);
                g.SetSourceRGBA(1, 1, 1, 1);
                g.Paint();
                g.SetSourceRGBA(0, 0, 0, 1);
                g.LineWidth = 1;
            }

            // build path
            CairoPathBuilder cairoPathBuilder;
            cairoPathBuilder = new CairoPathBuilder(g, 0, 0, 1);
            //FIXME fix this when CairoRenderer is ready
            //context.Build(Point.Zero, cairoPathBuilder);

            float x, y, height;
            context.IndexToXY(0, false, out x, out y, out height);
            context.IndexToXY(1, false, out x, out y, out height);
            context.IndexToXY(2, false, out x, out y, out height);
            context.IndexToXY(3, false, out x, out y, out height);
            context.IndexToXY(4, false, out x, out y, out height);
            context.IndexToXY(5, false, out x, out y, out height);

        }
    }
}