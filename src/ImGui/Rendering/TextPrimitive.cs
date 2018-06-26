using ImGui.Common.Primitive;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal class TextPrimitive : Primitive
    {
        public string Text { get; set; }
        public string FontFamily { get; set; }
        public double FontSize;
        public FontStyle FontStyle;
        public FontWeight FontWeight;

        public Rect Rect { get; set; }
        public List<Vector> Offsets { get; set; } = new List<Vector>();
        public List<GlyphData> Glyphs { get; set; } = new List<GlyphData>();
    }
}
