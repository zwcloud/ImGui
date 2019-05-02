using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.OSAbstraction.Text
{
    internal class GlyphRun
    {
        #region human-friendly description
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
        public FontStretch FontStretch = FontStretch.Normal;//not used

        public TextAlignment TextAlignment = TextAlignment.Leading;//TODO apply text alignment

        private string text;
        public bool TextChanged { get; set; } = false;
        #endregion

        #region glyph data
        public Rect Rectangle { get; private set; }
        public List<Vector> Offsets { get; } = new List<Vector>();
        public List<GlyphData> Glyphs { get; } = new List<GlyphData>();
        #endregion

        public GlyphRun(string text, string fontFamily, double fontSize,
            FontStyle fontStyle, FontWeight fontWeight)
        {
            this.text = text;
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
            this.FontStyle = fontStyle;
            this.FontWeight = fontWeight;
        }

        public void BuildGlyphData(Rect rect)
        {
            this.Rectangle = rect;
            this.Glyphs.Clear();
            this.Offsets.Clear();

            var fontStretch = this.FontStretch;
            var fontStyle = this.FontStyle;
            var fontWeight = this.FontWeight;
            var textAlignment = this.TextAlignment;

            var textContext = new OSImplentation.TypographyTextContext(this.Text,
                this.FontFamily,
                this.FontSize,
                this.FontStretch,
                this.FontStyle,
                this.FontWeight,
                (int)rect.Width,
                (int)rect.Height,
                textAlignment);
            textContext.Build(rect.Location);

            this.Offsets.AddRange(textContext.GlyphOffsets);

            foreach (var character in this.Text)
            {
                Typography.OpenFont.Glyph glyph = OSImplentation.TypographyTextContext.LookUpGlyph(this.FontFamily, character);
                Typography.OpenFont.GlyphLoader.Read(glyph, out var polygons, out var bezierSegments);
                var glyphData = GlyphCache.Default.GetGlyph(character, this.FontFamily, fontStyle, fontWeight) ??
                                GlyphCache.Default.AddGlyph(character, this.FontFamily, fontStyle, fontWeight, polygons, bezierSegments);
                Debug.Assert(glyphData != null);
                this.Glyphs.Add(glyphData);
            }
        }
    }
}