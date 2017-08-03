using System;
using System.Collections.Generic;
using System.Text;
using Typography.OpenFont;
using Xunit;
using Xunit.Abstractions;

namespace ImGui.UnitTest
{
    public class GlyphFacts
    {
        public class GlyphPointsFacts
        {
            private readonly ITestOutputHelper o;
            public GlyphPointsFacts(ITestOutputHelper output)
            {
                o = output;
            }

            [Fact]
            public void GetGlyphPoints()
            {
                Typeface typeFace;
                using (var fs = Utility.ReadFile(Utility.FontDir + "msjh.ttf"))
                {
                    var reader = new OpenFontReader();
                    Profile.Start("OpenFontReader.Read");
                    typeFace = reader.Read(fs);
                    Profile.End();
                }

                // character 'D'
                Glyph glyph = typeFace.GetGlyphByIndex(8000);
                var glyphPoints = glyph.GlyphPoints;
                o.WriteLine("GlyphPoints of 'D':");
                foreach(var p in glyphPoints)
                {
                    o.WriteLine("{0}, {1}", p.X, p.Y);
                }

                var endPoints = glyph.EndPoints;
                o.WriteLine("endPoints of 'D':");
                foreach (var end in endPoints)
                {
                    o.WriteLine(end.ToString());
                }
            }


        }
    }
}
