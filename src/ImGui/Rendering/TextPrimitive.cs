using ImGui.Common.Primitive;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal class TextPrimitive : Primitive
    {
        public string Text
        {
            get => this.text;
            set
            {
                if (value != this.text)
                {
                    this.TextChanged = true;
                    this.text = value;
                }
            }
        }

        public string FontFamily { get; set; }
        public double FontSize;
        public FontStyle FontStyle;
        public FontWeight FontWeight;
        private string text;
        public bool TextChanged { get; set; } = false;

        public List<Vector> Offsets { get; set; } = new List<Vector>();
        public List<GlyphData> Glyphs { get; set; } = new List<GlyphData>();

        public TextPrimitive(string text)
        {
            this.Text = text;
        }
    }
}
