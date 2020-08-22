using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.OSAbstraction.Text
{
    public class FormattedText
    {
        public Point OriginPoint { get; private set; }//TODO we should define bounding rect of FormattedText
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
        internal IList<GlyphData> GlyphDataList { get; private set; } = new List<GlyphData>();
        internal IList<Vector> GlyphOffsets { get; private set; } = new List<Vector>();

        public bool TextChanged { get; set; } = false;

        public FormattedText(Point origin, string text, string fontFamily, double fontSize)
        {
            //pre-compute hash code
            unchecked
            {
                int hash = 17;
                if(text != null) hash = hash * 23 + text.GetHashCode();
                hash = hash * 23 + fontFamily.GetHashCode();
                hash = hash * 23 + fontSize.GetHashCode();
                this.hashCode = hash;
            }

            Initialize(origin, text, fontFamily, fontSize);
        }

        public override bool Equals(object obj)
        {
            FormattedText other = obj as FormattedText;
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
            //TEMP use TypographyTextContext
            var textContext = (OSImplementation.TypographyTextContext)TextContextCache.Default.GetOrAdd(s, fontFamily, fontSize, TextAlignment.Leading);

            var glyphDataList = new List<GlyphData>(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                char character = s[i];
                if (char.IsControl(character))
                {
                    continue;
                }
                GlyphData glyphData;
                glyphData = GlyphCache.Default.GetGlyph(character, fontFamily);
                if (glyphData == null)
                {
                    Typography.OpenFont.Glyph glyph = OSImplementation.TypographyTextContext.LookUpGlyph(fontFamily, character);
                    GlyphLoader.Read(glyph, out var polygons, out var bezierSegments);
                    glyphData = GlyphCache.Default.AddGlyph(character, fontFamily, polygons, bezierSegments);
                }
                Debug.Assert(glyphData != null);
                this.GlyphDataList.Add(glyphData);
                var glyphOffset = textContext.GlyphOffsets[i];
                this.GlyphOffsets.Add(glyphOffset);
            }

            this.text = s;
            this.OriginPoint = origin;
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
        }

        private string text;
        private readonly int hashCode;
    }
}