using System;
using System.Collections.Generic;
using System.Text;
using ImGui.Common;
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

            [Theory]
            [InlineData("DroidSans.ttf", 'D')]
            [InlineData("msjh.ttf", 'D')]
            [InlineData("msjh.ttf", '啊')]
            [InlineData("msjh.ttf", 'あ')]
            [InlineData("msjh.ttf", '아')]
            public void GetGlyphPoints(string fontFile, char character)
            {
                Typeface typeFace;
                using (var fs = Utility.ReadFile(Utility.FontDir + fontFile))
                {
                    var reader = new OpenFontReader();
                    typeFace = reader.Read(fs);
                }

                Glyph glyph = typeFace.Lookup(character);

                o.WriteLine("GlyphPoints of '{0}':", character);
                var glyphPoints = glyph.GlyphPoints;
                foreach(var p in glyphPoints)
                {
                    o.WriteLine("{0}, {1}", p.X, p.Y);
                }

                o.WriteLine("EndPoints of '{0}':", character);
                var endPoints = glyph.EndPoints;
                foreach (var end in endPoints)
                {
                    o.WriteLine(end.ToString());
                }
            }

        }
    }
}
