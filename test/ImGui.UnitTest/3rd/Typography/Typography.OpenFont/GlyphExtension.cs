using System.Text;
using Typography.OpenFont;
using Xunit;

namespace ImGui.UnitTest
{
    public static class GlyphExtension
    {
        public static string Dump(this Glyph glyph)
        {
            StringBuilder o = new StringBuilder();
            o.AppendLine("[GlyphPoints]");
            var glyphPoints = glyph.GlyphPoints;
            foreach(var p in glyphPoints)
            {
                o.AppendFormat("{0}, {1}\n", p.X, p.Y);
            }

            o.AppendLine("[EndPoints]");
            var endPoints = glyph.EndPoints;
            foreach (var end in endPoints)
            {
                o.AppendLine(end.ToString());
            }

            return o.ToString();
        }
    }
}