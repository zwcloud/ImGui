using ImGui.OSAbstraction.Text;
using ImGui.OSImplementation;
using Typography.OpenFont;
using Xunit;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit.Abstractions;

namespace ImGui.UnitTest
{
    public class OpenFontReaderFacts
    {
        public class TheReadMethod
        {
            private readonly ITestOutputHelper o;
            public TheReadMethod(ITestOutputHelper output)
            {
                o = output;
            }

            [Theory]
            [InlineData("msjh.ttf", 'D', "3rd/Typography/Typography.OpenFont/images/GlyphReaderFacts.TheReadMethod.Read_msjh.ttf_D.png")]
            [InlineData("DroidSans.ttf", 'o', "3rd/Typography/Typography.OpenFont/images/GlyphReaderFacts.TheReadMethod.Read_DroidSans.ttf_o.png")]
            [InlineData("msjh.ttf", '乐', "3rd/Typography/Typography.OpenFont/images/GlyphReaderFacts.TheReadMethod.Read_msjh.ttf_乐.png")]
            public void Read(string fontFileName, char character, string expectedImageFilePath)
            {
                Typeface typeFace;
                using (var fs = Utility.ReadFile(Utility.FontDir + fontFileName))
                {
                    var reader = new OpenFontReader();
                    typeFace = reader.Read(fs);
                }

                Typography.OpenFont.Glyph glyph = typeFace.Lookup(character);

                // read polygons and bezier segments
                GlyphLoader.Read(glyph, out var polygons, out var bezierSegments);

                Rect aabb = new Rect(polygons[0][0], polygons[0][1]);

                //print to test output
                //calcualte the aabb
                for (int i = 0; i < polygons.Count; i++)
                {
                    o.WriteLine("Polygon " + i);
                    var polygon = polygons[i];
                    foreach (var p in polygon)
                    {
                        aabb.Union(p);
                        o.WriteLine("{0}, {1}", (int)p.X, (int)p.Y);
                    }
                    o.WriteLine("");
                }

                foreach (var segment in bezierSegments)
                {
                    aabb.Union(segment.Item1);
                    aabb.Union(segment.Item2);
                    aabb.Union(segment.Item3);
                    o.WriteLine("<{0}, {1}> <{2}, {3}> <{4}, {5}>",
                        (int)segment.Item1.X, (int)segment.Item1.Y,
                        (int)segment.Item2.X, (int)segment.Item2.Y,
                        (int)segment.Item3.X, (int)segment.Item3.Y);
                }

                o.WriteLine("");

                // draw to an image
                using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, MathEx.RoundToInt(aabb.Width), MathEx.RoundToInt(aabb.Height)))
                using (Cairo.Context g = new Cairo.Context(surface))
                {
                    g.Translate(-aabb.Min.X, -aabb.Min.Y);
                    //essential: set surface back ground to white (1,1,1,1)
                    g.SetSourceColor(CairoEx.ColorWhite);
                    g.Paint();

                    for (var i = 0; i < polygons.Count; i++)
                    {
                        var polygon = polygons[i];
                        g.MoveTo(polygon[0].X, polygon[0].Y);
                        foreach (var point in polygon)
                        {
                            g.LineTo(point.X, point.Y);
                        }
                        g.ClosePath();
                    }
                    g.SetSourceColor(new Cairo.Color(0, 0, 0));
                    g.LineWidth = 4;
                    g.StrokePreserve();
                    g.SetSourceColor(new Cairo.Color(0.8, 0, 0, 0.6));
                    g.Fill();

                    foreach(var segment in bezierSegments)
                    {
                        var p0 = segment.Item1;
                        var c = segment.Item2;
                        var p1 = segment.Item3;
                        g.MoveTo(p0.X, p0.Y);
                        g.QuadraticTo(c.X, c.Y, p1.X, p1.Y);
                    }

                    g.LineWidth = 4;
                    g.SetSourceColor(CairoEx.ColorRgb(0, 122, 204));
                    g.Stroke();

                    //used to generate expected image
                    //surface.WriteToPng($"{Util.OutputPath}\\GlyphReaderFacts.TheReadMethod.Read_{fontFileName}_{character}.png");

                    var image = Image.LoadPixelData<Bgra32>(Configuration.Default, surface.Data, surface.Width, surface.Height);
                    var expectedImage = Image.Load(expectedImageFilePath);
                    Assert.True(Util.CompareImage(expectedImage, image));
                }
            }
        }
    }
}
