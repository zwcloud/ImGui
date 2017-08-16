using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Text;
using Typography.OpenFont;
using Typography.Rendering;
using Typography.TextLayout;

namespace ImGui.OSImplentation
{
    /// <summary>
    /// Text context based on Typography
    /// </summary>
    /// <remarks>TypographyTextContext is an pure C# implementation of <see cref="ITextContext"/>.</remarks>
    class TypographyTextContext : ITextContext
    {
        private static Typeface GetTypeFace(string fontFamily)
        {
            Typeface typeFace;
            if (!typefaceCache.TryGetValue(fontFamily, out typeFace))
            {
                using (var fs = Utility.ReadFile(fontFamily))
                {
                    var reader = new OpenFontReader();
                    Profile.Start("OpenFontReader.Read");
                    typeFace = reader.Read(fs);
                    Profile.End();
                }
                typefaceCache.Add(fontFamily, typeFace);
            }
            return typeFace;
        }

        public static Glyph LookUpGlyph(string fontFamily, char character)
        {
            Typeface typeFace = GetTypeFace(fontFamily);
            var glyph = typeFace.Lookup(character);
            return glyph;
        }

        public static Glyph LookUpGlyph(string fontFamily, int glyphIndex)
        {
            Typeface typeFace = GetTypeFace(fontFamily);
            var glyph = typeFace.GetGlyphByIndex(glyphIndex);
            return glyph;
        }

        public static double GetScale(string fontFamily, double fontSize)
        {
            Typeface typeFace = GetTypeFace(fontFamily);
            var scale = typeFace.CalculateToPixelScaleFromPointSize((float)fontSize);
            return scale;
        }

        public readonly List<GlyphPlan> glyphPlans = new List<GlyphPlan>();
        private readonly GlyphLayout glyphLayout = new GlyphLayout();

        private char[] textCharacters;
        private string text;
        private string fontFamily;
        private HintTechnique HintTechnique { get; set; }
        private PositionTechnique PositionTechnique { get; set; }
        private bool EnableLigature { get; set; }
        private Typeface CurrentTypeFace { get; set; }

        private static readonly Dictionary<string, Typeface> typefaceCache = new Dictionary<string, Typeface>();

        public TypographyTextContext(string text, string fontFamily, float fontSize,
            FontStretch stretch, FontStyle style, FontWeight weight,
            int maxWidth, int maxHeight,
            TextAlignment alignment)
        {
            this.Text = text;
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
            this.Alignment = alignment;
        }

        #region Implementation of ITextContext

        //TODO Implement those properties when Typography is ready.

        /// <summary>
        /// Font file path
        /// </summary>
        public string FontFamily
        {
            get { return this.fontFamily; }
            set
            {
                if (this.fontFamily != value)
                {
                    this.fontFamily = value;

                    Typeface typeFace;
                    if(!typefaceCache.TryGetValue(this.fontFamily, out typeFace))
                    {
                        using (var fs = Utility.ReadFile(this.fontFamily))
                        {
                            var reader = new OpenFontReader();
                            Profile.Start("OpenFontReader.Read");
                            typeFace = reader.Read(fs);
                            Profile.End();
                        }
                        typefaceCache.Add(this.fontFamily, typeFace);
                    }
                    this.CurrentTypeFace = typeFace;

                    //2. Update GlyphLayout
                    this.glyphLayout.ScriptLang = ScriptLangs.Latin;
                    this.glyphLayout.PositionTechnique = this.PositionTechnique;
                    this.glyphLayout.EnableLigature = this.EnableLigature;
                }
            }
        }

        public float FontSize { get; }

        public TextAlignment Alignment { get; set; }

        public Point Position { get; private set; }

        public Size Size { get; private set; }

        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                this.textCharacters = this.text.ToCharArray();
            }
        }

        #region line data

        public int lineCount = 0;
        public float lineHeight;
        float lineBreakWidth;
        List<float> LineWidthList = new List<float>();
        List<uint> LineCharacterCountList = new List<uint>();

        #endregion

        public List<Vector> GlyphOffsets = new List<Vector>();

        public Size Measure()
        {
            //Profile.Start("TypographyTextContext.Measure");
            this.Position = Point.Zero;
            this.glyphLayout.Typeface = this.CurrentTypeFace;
            var scale = this.CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Size = Size.Zero;
            }
            else
            {
                if (this.glyphPlans.Count == 0)
                {
                    this.glyphLayout.Typeface = this.CurrentTypeFace;
                    this.glyphLayout.GenerateGlyphPlans(this.textCharacters, 0, this.textCharacters.Length, this.glyphPlans, null);
                }

                int j = this.glyphPlans.Count;
                Typeface currentTypeface = this.CurrentTypeFace;
                MeasuredStringBox strBox;
                if (j == 0)
                {
                    strBox = new MeasuredStringBox(0,
                        currentTypeface.Ascender * scale,
                        currentTypeface.Descender * scale,
                        currentTypeface.LineGap * scale);
                }
                else
                {
                    GlyphPlan lastOne = this.glyphPlans[j - 1];
                    strBox = new MeasuredStringBox((lastOne.x + lastOne.advX) * scale,
                        currentTypeface.Ascender * scale,
                        currentTypeface.Descender * scale,
                        currentTypeface.LineGap * scale);
                }
                this.lineHeight = strBox.CalculateLineHeight();

                // get line count
                {
                    this.lineCount = 1;
                    int i;
                    for (i = 0; i < this.glyphPlans.Count; ++i)
                    {
                        var glyphPlan = this.glyphPlans[i];
                        if (glyphPlan.glyphIndex == 0)
                        {
                            this.lineCount++;
                            continue;
                        }
                    }
                    if (this.glyphPlans.Count > 0)
                    {
                        var lastGlyph = this.glyphPlans[this.glyphPlans.Count - 1];
                        if (lastGlyph.glyphIndex == 0)//last glyph is '\n', add an additional empty line
                        {
                            this.lineCount++;
                        }
                    }
                }

                this.Size = new Size(strBox.width, this.lineCount * this.lineHeight);
            }
            //Profile.End();

            return this.Size;
        }

        public void Build(Point offset)
        {
            //Profile.Start("TypographyTextContext.Build");
            this.GlyphOffsets.Clear();
            // layout glyphs with selected layout technique
            this.Position = offset;
            this.glyphPlans.Clear();
            this.glyphLayout.Typeface = this.CurrentTypeFace;
            this.glyphLayout.GenerateGlyphPlans(this.textCharacters, 0, this.textCharacters.Length, this.glyphPlans, null);

            int j = this.glyphPlans.Count;
            this.lineHeight = this.CurrentTypeFace.Ascender - this.CurrentTypeFace.Descender + this.CurrentTypeFace.LineGap;

            {
                // render each glyph
                this.lineCount = 1;
                float back = 0;
                for (int i = 0; i < this.glyphPlans.Count; ++i)
                {
                    var glyphPlan = this.glyphPlans[i];
                    Glyph glyph = this.CurrentTypeFace.GetGlyphByIndex(glyphPlan.glyphIndex);

                    //1. start with original points/contours from glyph
                    if (glyphPlan.glyphIndex == 0)
                    {
                        this.lineCount++;
                        back = glyphPlan.x + glyphPlan.advX;
                    }

                    var offsetX = glyphPlan.x - back;
                    var offsetY = glyphPlan.y + this.lineCount * this.lineHeight;

                    this.GlyphOffsets.Add(new Vector(offsetX, offsetY));
                }
            }

            // recording line data
            var scale = this.CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);
            {
                this.lineCount = 1;
                float back = 0;
                int backCharCount = 0;
                int i;
                for (i = 0; i < this.glyphPlans.Count; ++i)
                {
                    var glyphPlan = this.glyphPlans[i];
                    if (glyphPlan.glyphIndex == 0)
                    {
                        this.lineCount++;
                        this.LineWidthList.Add((glyphPlan.x + glyphPlan.advX) * scale - back);
                        this.LineCharacterCountList.Add((uint)(i + 1 - backCharCount));// count in line break ('\n')
                        backCharCount = i + 1;
                        back = (glyphPlan.x + glyphPlan.advX) * scale;
                        continue;
                    }
                }
                if(this.glyphPlans.Count >0)
                {
                    var lastGlyph = this.glyphPlans[this.glyphPlans.Count - 1];
                    this.LineWidthList.Add((lastGlyph.x + lastGlyph.advX) * scale - back);
                    this.LineCharacterCountList.Add((uint)(i - backCharCount));
                    if(lastGlyph.glyphIndex == 0)//last glyph is '\n', add an additional empty line
                    {
                        this.LineWidthList.Add(0);
                        this.LineCharacterCountList.Add(0);
                        this.lineCount++;
                    }
                }
                if(this.LineWidthList.Count == 0)
                {
                    this.LineWidthList.Add(0);
                    this.LineCharacterCountList.Add(0);
                }
            }
            
            {
                this.lineBreakWidth = this.CurrentTypeFace.GetHAdvanceWidthFromGlyphIndex(0) * scale;
            }

            //Profile.End();
        }

        int LineIndex;

        public uint XyToIndex(float pointX, float pointY, out bool isInside)
        {
            var position = this.Position;
            isInside = false;
            int i = 0;

            this.LineIndex = (int)Math.Ceiling(pointY / this.lineHeight) - 1;//line index start from 0
            if(this.LineIndex < 0)
            {
                this.LineIndex = 0;
            }
            if(this.LineIndex > this.lineCount - 1)
            {
                this.LineIndex = this.lineCount-1;
            }

            System.Diagnostics.Debug.Assert(this.lineCount == this.LineWidthList.Count);

            uint result = 0;
            float back = 0;
            for (i = 0; i < this.LineIndex; i++)
            {
                result += this.LineCharacterCountList[i];
                back += this.LineWidthList[i];
            }

            // ↓↓↓
            //   ^CONTENT_OF_THIS_LINE$
            if (pointX < position.X)//first index of this line
            {
                return result;
            }

            //                      ↓↓↓
            // ^CONTENT_OF_THIS_LINE$
            float currentLineWidth = this.LineWidthList[this.LineIndex];
            uint currentLineCharacterCount = this.LineCharacterCountList[this.LineIndex];
            if (pointX > position.X + currentLineWidth)//last index of this line
            {
                result += currentLineCharacterCount;
                if (result > 0 && currentLineCharacterCount != 0 && this.glyphPlans[(int)result - 1].glyphIndex == 0)
                {
                    result -= 1;
                }
                return result;
            }

            //  ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
            // ^CONTENT_OF_THIS_LINE\n$
            var scale = this.CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);//TODO cache scale
            uint characterCountBeforeThisLine = 0;
            for (i = 0; i < this.LineIndex; i++)
            {
                characterCountBeforeThisLine += this.LineCharacterCountList[i];
            }
            var firstGlyphIndex = (int)characterCountBeforeThisLine;
            float offsetX = (float)position.X;
            for (i = firstGlyphIndex; i < this.glyphPlans.Count; i++)
            {
                var glyph = this.glyphPlans[i];
                var minX = offsetX;
                var glyphWidth = glyph.advX * scale;
                var maxX = minX + glyphWidth;
                offsetX += glyphWidth;
                if (minX <= pointX && pointX < maxX)
                {
                    isInside = true;
                    return (uint)i;
                }
            }
            return (uint)i;
        }

        public void IndexToXY(uint caretIndex, bool isTrailing, out float pointX, out float pointY, out float height)
        {
            height = this.lineHeight;
            if (this.glyphPlans.Count == 0)
            {
                pointX = (float)this.Position.X;
                pointY = (float)this.Position.Y;
                return;
            }

            int previousCharIndex = -1;
            if(caretIndex > 0)
            {
                previousCharIndex = (int)(caretIndex - 1);
            }

            int newLinesBeforeThisCaretPosition = 0;
            for (int i = 0; i < caretIndex; i++)
            {
                var g = this.glyphPlans[i];
                if (g.glyphIndex == 0)
                {
                    newLinesBeforeThisCaretPosition++;
                }
            }

            var scale = this.CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);
            bool previousCharIsLineBreak = false;
            GlyphPlan previousGlyph = new GlyphPlan();
            if (previousCharIndex!=-1)
            {
                previousGlyph = this.glyphPlans[previousCharIndex];
                if (previousGlyph.glyphIndex == 0)// \n
                {
                    previousCharIsLineBreak = true;
                }
            }

            pointX = (float)this.Position.X;
            pointY = (float)this.Position.Y;
            if(previousCharIndex!=-1)
            {
                pointX += (previousGlyph.x + previousGlyph.advX) * scale;
            }

            if(previousCharIsLineBreak)
            {
                pointX = (float)this.Position.X;
                for (int i = 0; i < newLinesBeforeThisCaretPosition; i++)
                {
                    pointY += this.lineHeight;
                }
            }
            else
            {
                for (int i = 0; i < newLinesBeforeThisCaretPosition; i++)
                {
                    pointX -= this.LineWidthList[i];
                    pointY += this.lineHeight;
                }
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            // No native resource is used.
        }

        #endregion
    }

}
