using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.OSAbstraction.Text
{
    /// <summary>
    /// Represents a sequence of glyphs from a single face of a single font at a single size, and with a single rendering style, and without line break.
    /// </summary>
    public class GlyphRun
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
        public string FontFamily { get; private set; }
        public double FontSize { get; private set; }
        internal IList<GlyphData> GlyphDataList { get; private set; } = new List<GlyphData>();
        internal IList<Vector> GlyphOffsets { get; private set; } = new List<Vector>();

        /// <summary>
        /// The ink bounding box size of the glyph run.
        /// </summary>
        public Size InkBoundingBoxSize { get; private set; }

        public bool TextChanged { get; set; } = false;

        public GlyphRun(Point origin, string text, string fontFamily, double fontSize) : this(text, fontFamily, fontSize)
        {
            this.origin = origin;
        }

        public GlyphRun(string text, string fontFamily, double fontSize)
        {
            this.Text = text;
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;

            //pre-compute hash code
            unchecked
            {
                int hash = 17;
                if (text != null) hash = hash * 23 + text.GetHashCode();
                hash = hash * 23 + fontFamily.GetHashCode();
                hash = hash * 23 + fontSize.GetHashCode();
                this.hashCode = hash;
            }

            Initialize(text, fontFamily, fontSize);
        }

        public void SetOffset(Point origin)
        {
            this.origin = origin;
        }

        public Point Origin => this.origin;

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

        private void Initialize(string _text, string fontFamily, double fontSize)
        {
            var textContext = (OSImplementation.TypographyTextContext)TextContextCache.Default.GetOrAdd(_text, fontFamily, fontSize, TextAlignment.Leading);
            textContext.Build(Point.Zero);

            var glyphDataList = new List<GlyphData>(_text.Length);
            foreach (var character in _text)
            {
                Typography.OpenFont.Glyph glyph = OSImplementation.TypographyTextContext.LookUpGlyph(fontFamily, character);
                GlyphLoader.Read(glyph, out var polygons, out var bezierSegments);
                var glyphData = GlyphCache.Default.GetGlyph(character, fontFamily) ??
                                GlyphCache.Default.AddGlyph(character, fontFamily, polygons, bezierSegments);
                Debug.Assert(glyphData != null);
                glyphDataList.Add(glyphData);
            }

            this.text = _text;
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
            this.GlyphDataList = glyphDataList;
            this.GlyphOffsets = textContext.GlyphOffsets;

            this.InkBoundingBoxSize = textContext.Measure();
        }

        private string text;
        private Point origin;
        private readonly int hashCode;
    }
}