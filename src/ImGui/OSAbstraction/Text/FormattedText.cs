using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.OSAbstraction.Text
{
    internal class FormattedText
    {
        public Point OriginPoint { get; private set; }
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
        public string FontFamily { get; private set; }
        public double FontSize { get; private set; }
        public IList<GlyphData> GlyphDataList { get; private set; } = new List<GlyphData>();
        public IList<Vector> GlyphOffsets { get; private set; } = new List<Vector>();

        public bool TextChanged { get; set; } = false;

        public FormattedText(Point origin, string text, string fontFamily, double fontSize)
        {
            //pre-compute hash code
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + text.GetHashCode();
                hash = hash * 23 + fontFamily.GetHashCode();
                hash = hash * 23 + fontSize.GetHashCode();
                this.hashCode = hash;
            }

            Initialize(origin, text, fontFamily, fontSize);
        }

        public override bool Equals(object obj)
        {
            GlyphRun other = obj as GlyphRun;
            if (other == null)
            {
                return false;
            }

            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        private void Initialize(Point origin, string s, string fontFamily, double fontSize)
        {
            var textContext = new OSImplentation.TypographyTextContext(s, fontFamily, fontSize, TextAlignment.Leading);
            textContext.Build(origin);

            var glyphDataList = new List<GlyphData>(s.Length);
            foreach (var character in s)
            {
                Typography.OpenFont.Glyph glyph = OSImplentation.TypographyTextContext.LookUpGlyph(fontFamily, character);
                GlyphLoader.Read(glyph, out var polygons, out var bezierSegments);
                var glyphData = GlyphCache.Default.GetGlyph(character, fontFamily) ??
                                GlyphCache.Default.AddGlyph(character, fontFamily, polygons, bezierSegments);
                Debug.Assert(glyphData != null);
                glyphDataList.Add(glyphData);
            }

            this.text = s;
            this.OriginPoint = origin;
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
            this.GlyphDataList = glyphDataList;
            this.GlyphOffsets = GlyphOffsets;
        }

        private string text;
        private readonly int hashCode;
    }
}