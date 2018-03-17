using ImGui.Common.Primitive;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal class TextPrimitive : Primitive
    {
        //TODO no need to save acutual text here
        public string Text { get; set; }

        public List<Vector> Offsets { get; set; } = new List<Vector>();
        public List<GlyphData> Glyphs { get; set; } = new List<GlyphData>();
    }
}
