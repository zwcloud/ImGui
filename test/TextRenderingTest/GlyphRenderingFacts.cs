using ImGui;
using ImGui.Rendering;
using ImGui.Style;
using ImGui.OSAbstraction.Text;
using ImGui.Development;
using System;
using System.Diagnostics;
using Xunit;
using Typography.OpenFont;
using System.IO;
using System.Linq;

namespace TextRenderingTest
{
    public class GlyphRenderingFacts
    {
        public GlyphRenderingFacts()
        {
            Application.InitSysDependencies();
        }

        private const string ModelViewerPath = @"C:\Program Files\Autodesk\FBX Review\fbxreview.exe";
        //"E:\Program Files (green)\open3mod_1_1_standalone\open3mod.exe";

        [Fact]
        public void RenderAGlyphWithDirectionSegments()
        {
            //fetch a glyph 'e'
            string fontFile = Utility.FontDir + "DroidSans.ttf";
            Typeface typeFace;
            using (var fs = new FileStream(fontFile, FileMode.Open))
            {
                var fontReader = new OpenFontReader();
                typeFace = fontReader.Read(fs);
            }
            var glyph = typeFace.Lookup('e');

            //read polygons and curves (quadratic-bezier) from the glyph
            GlyphLoader.Read(glyph, out var polygons, out var quadraticBezierSegments);

            //calculate the baseline origin
            var scale = typeFace.CalculateToPixelScaleFromPointSize(650);
            var ascent = typeFace.Ascender * scale;
            var offset = new Vector(0, ascent);

            //create a geometry for polygons
            Geometry polygonGeometry;
            {
                var d = new PathGeometryBuilder();
                d.BeginPath();
                foreach (var polygon in polygons)
                {
                    d.MoveTo(polygon[0] * scale + offset);
                    foreach (var p in polygon.Skip(1))
                    {
                        d.LineTo(p * scale + offset);
                    }
                    d.ClosePath();
                    d.Stroke();
                }
                polygonGeometry = d.ToGeometry();
            }
            var polygonPen = new Pen(Color.Black, 1);

            Geometry quadraticGeometry;
            {
                var d = new PathGeometryBuilder();
                d.BeginPath();
                foreach (var qs in quadraticBezierSegments)
                {
                    d.MoveTo(qs.Item1 * scale + offset);
                    d.QuadraticCurveTo(qs.Item2 * scale + offset, qs.Item3 * scale + offset);
                    d.Stroke();
                }
                quadraticGeometry = d.ToGeometry();
            }
            var quadraticPen = new Pen(Color.Yellow, 1);

            //draw the geometry
            Application.Run(new Form1(() => {
                var g = Form.current.ForegroundDrawingContext;
                g.DrawGeometry(null, polygonPen, polygonGeometry);
                g.DrawGeometry(null, quadraticPen, quadraticGeometry);
            }));
        }
    }
}
