using System;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal class TextGeometry : Geometry
    {
        public string Text
        {
            get => this.text;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
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

        public TextGeometry(string text)
        {
            this.Text = text;
        }

        internal override PathGeometryData GetPathGeometryData()
        {
            throw new System.NotImplementedException();
        }
    }
}
