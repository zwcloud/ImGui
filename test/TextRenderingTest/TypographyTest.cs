using ImGui;
using System;
using System.Diagnostics;
using Xunit;
using System.Collections.Generic;
using System.Text;
using Typography.Rendering;
using System.IO;
using Typography.OpenFont;

namespace TextRenderingTest
{
    public class TypographyTest
    {
        static readonly string FontFile = Utility.FontDir + "DroidSans.ttf";
        private Typeface typeFace;

        public TypographyTest()
        {
            using (var fs = new FileStream(FontFile, FileMode.Open))
            {
                var fontReader = new OpenFontReader();
                typeFace = fontReader.Read(fs);
            }
        }

        [Fact]
        public void ShouldGetCorrectContourFromTypeFace()
        {
            var glyph = typeFace.GetGlyphByIndex(36);
            var points = glyph.GlyphPoints;
        }
    }
}