using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.OSAbstraction.Text
{
    /// <summary>
    /// Represents a sequence of glyphs from a single face of a single font at a single size, and with a single rendering style, and without line break.
    /// </summary>
    internal class GlyphRun
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

        public GlyphRun(Point origin, string text, string fontFamily, double fontSize)
        {
            this.Text = text;
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;

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

        private void Initialize(Point origin, string _text, string fontFamily, double fontSize)
        {
            var textContext = new OSImplentation.TypographyTextContext(_text, fontFamily, fontSize);
            textContext.Build(origin);

            var glyphDataList = new List<GlyphData>(_text.Length);
            foreach (var character in _text)
            {
                Typography.OpenFont.Glyph glyph = OSImplentation.TypographyTextContext.LookUpGlyph(fontFamily, character);
                GlyphLoader.Read(glyph, out var polygons, out var bezierSegments);
                var glyphData = GlyphCache.Default.GetGlyph(character, fontFamily) ??
                                GlyphCache.Default.AddGlyph(character, fontFamily, polygons, bezierSegments);
                Debug.Assert(glyphData != null);
                glyphDataList.Add(glyphData);
            }

            this.text = _text;
            this.OriginPoint = origin;
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
            this.GlyphDataList = glyphDataList;
            this.GlyphOffsets = textContext.GlyphOffsets;
        }

        private string text;
        private readonly int hashCode;
    }
}