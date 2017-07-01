using System;
using System.Collections.Generic;
using Typography.OpenFont;
using Typography.Rendering;
using Typography.TextLayout;

namespace ImGui
{
    /// <summary>
    /// Text context based on Typography
    /// </summary>
    /// <remarks>TypographyTextContext is an pure C# implementation of <see cref="ITextContext"/>.</remarks>
    class TypographyTextContext : ITextContext
    {
        private readonly List<GlyphPlan> glyphPlans = new List<GlyphPlan>();
        private readonly GlyphLayout glyphLayout = new GlyphLayout();
        private GlyphTranslatorToPath glyphPathTranslator;
        private GlyphPathBuilder glyphPathBuilder;

        private char[] textCharacters;
        private string text;
        private string fontFamily;
        private HintTechnique HintTechnique { get; set; }
        private PositionTechnique PositionTechnique { get; set; }
        private bool EnableLigature { get; set; }
        private Typeface CurrentTypeFace { get; set; }

        private static Dictionary<string, Typeface> typefaceCache = new Dictionary<string, Typeface>();

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
            get { return fontFamily; }
            set
            {
                if (fontFamily != value)
                {
                    fontFamily = value;

                    Typeface typeFace;
                    if(!typefaceCache.TryGetValue(fontFamily, out typeFace))
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
                    this.CurrentTypeFace = typeFace;

                    //2. glyph builder
                    glyphPathBuilder = new GlyphPathBuilder(CurrentTypeFace);
                    glyphPathBuilder.UseTrueTypeInstructions = false; //reset
                    glyphPathBuilder.UseVerticalHinting = false; //reset
                    switch (this.HintTechnique)
                    {
                        case HintTechnique.TrueTypeInstruction:
                            glyphPathBuilder.UseTrueTypeInstructions = true;
                            break;
                        case HintTechnique.TrueTypeInstruction_VerticalOnly:
                            glyphPathBuilder.UseTrueTypeInstructions = true;
                            glyphPathBuilder.UseVerticalHinting = true;
                            break;
                        case HintTechnique.CustomAutoFit:
                            //custom agg autofit 
                            break;
                    }

                    //3. glyph translater
                    glyphPathTranslator = new GlyphTranslatorToPath();

                    //4. Update GlyphLayout
                    glyphLayout.ScriptLang = ScriptLangs.Latin;
                    glyphLayout.PositionTechnique = this.PositionTechnique;
                    glyphLayout.EnableLigature = this.EnableLigature;
                }
            }
        }

        public float FontSize { get; }

        public TextAlignment Alignment { get; set; }

        public Point Position { get; private set; }

        public Size Size { get; private set; }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                textCharacters = text.ToCharArray();
            }
        }

        #region line data

        int lineCount = 0;
        float lineHeight;
        float lineBreakWidth;
        List<float> LineWidthList = new List<float>();
        List<uint> LineCharacterCountList = new List<uint>();

        #endregion

        public Size Measure()
        {
            //Profile.Start("TypographyTextContext.Measure");
            this.Position = Point.Zero;
            glyphLayout.Typeface = this.CurrentTypeFace;
            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Size = Size.Zero;
            }
            else
            {
                if (glyphPlans.Count == 0)
                {
                    glyphLayout.Typeface = this.CurrentTypeFace;
                    glyphLayout.GenerateGlyphPlans(this.textCharacters, 0, this.textCharacters.Length, glyphPlans, null);
                }

                int j = glyphPlans.Count;
                Typeface currentTypeface = glyphLayout.Typeface;
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
                    GlyphPlan lastOne = glyphPlans[j - 1];
                    strBox = new MeasuredStringBox((lastOne.x + lastOne.advX) * scale,
                        currentTypeface.Ascender * scale,
                        currentTypeface.Descender * scale,
                        currentTypeface.LineGap * scale);
                }
                lineHeight = strBox.CalculateLineHeight();
                this.Size = new Size(strBox.width, lineHeight);//FIXME incorrect, line-height * line-count not calculated.
            }
            //Profile.End();

            return this.Size;
        }

        public void Build(Point offset, ITextPathBuilder pathBuilder)
        {
            //Profile.Start("TypographyTextContext.Build");
            // layout glyphs with selected layout technique
            this.Position = offset;
            glyphPlans.Clear();
            glyphLayout.Typeface = this.CurrentTypeFace;
            glyphLayout.GenerateGlyphPlans(this.textCharacters, 0, this.textCharacters.Length, glyphPlans, null);

            int j = glyphPlans.Count;
            Typeface currentTypeface = glyphLayout.Typeface;
            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);
            lineHeight = (currentTypeface.Ascender - currentTypeface.Descender + currentTypeface.LineGap)*scale;

            if (pathBuilder != null)
            {
                // render each glyph
                glyphPathTranslator.PathBuilder = pathBuilder;
                lineCount = 1;
                float back = 0;
                for (int i = 0; i < glyphPlans.Count; ++i)
                {
                    glyphPathTranslator.Reset();
                    var glyphPlan = glyphPlans[i];
                    glyphPathBuilder.BuildFromGlyphIndex(glyphPlan.glyphIndex, this.FontSize);
                    if (glyphPlan.glyphIndex == 0)
                    {
                        lineCount++;
                        back = (glyphPlan.x + glyphPlan.advX) * scale;
                        continue;
                    }
                    glyphPathBuilder.ReadShapes(
                        glyphPathTranslator, this.FontSize,
                        (float)glyphPlan.x * scale - back,//minus total width of previous lines
                        (float)glyphPlan.y * scale
                            + lineCount * lineHeight//this extra  offset moves all shapes from (0, 0) to (0, line height)
                        );
                }
            }

            // recording line data
            {
                lineCount = 1;
                float back = 0;
                int backCharCount = 0;
                int i;
                for (i = 0; i < glyphPlans.Count; ++i)
                {
                    var glyphPlan = glyphPlans[i];
                    if (glyphPlan.glyphIndex == 0)
                    {
                        lineCount++;
                        LineWidthList.Add((glyphPlan.x + glyphPlan.advX) * scale - back);
                        LineCharacterCountList.Add((uint)(i + 1 - backCharCount));// count in line break ('\n')
                        backCharCount = i + 1;
                        back = (glyphPlan.x + glyphPlan.advX) * scale;
                        continue;
                    }
                }
                if(glyphPlans.Count >0)
                {
                    var lastGlyph = glyphPlans[glyphPlans.Count - 1];
                    LineWidthList.Add((lastGlyph.x + lastGlyph.advX) * scale - back);
                    LineCharacterCountList.Add((uint)(i - backCharCount));
                }
                if(LineWidthList.Count == 0)
                {
                    LineWidthList.Add(0);
                    LineCharacterCountList.Add(0);
                }
            }
            
            {
                lineBreakWidth = this.CurrentTypeFace.GetHAdvanceWidthFromGlyphIndex(0) * scale;
            }

            //Profile.End();
        }

        int LineIndex;

        public uint XyToIndex(float pointX, float pointY, out bool isInside)
        {
            var position = this.Position;
            isInside = false;
            int i = 0;

            LineIndex = (int)Math.Ceiling(pointY / lineHeight) - 1;//line index start from 0
            if(LineIndex < 0)
            {
                LineIndex = 0;
            }
            if(LineIndex > lineCount - 1)
            {
                LineIndex = lineCount-1;
            }

            System.Diagnostics.Debug.Assert(lineCount == LineWidthList.Count);

            uint result = 0;
            float back = 0;
            for (i = 0; i < LineIndex; i++)
            {
                result += LineCharacterCountList[i];
                back += LineWidthList[i];
            }

            // ↓↓↓
            //   ^CONTENT_OF_THIS_LINE$
            if (pointX < position.X)//first index of this line
            {
                return result;
            }

            //                      ↓↓↓
            // ^CONTENT_OF_THIS_LINE$
            float currentLineWidth = LineWidthList[LineIndex];
            float currentLineWidthWithoutLineBreak = currentLineWidth - lineBreakWidth;
            uint currentLineCharacterCount = LineCharacterCountList[LineIndex];
            uint currentLineCharacterCountWithoutLineBreak = currentLineCharacterCount;
            if (currentLineCharacterCountWithoutLineBreak > 0)
            {
                currentLineCharacterCountWithoutLineBreak--;
            }
            if (pointX > position.X + currentLineWidthWithoutLineBreak)//last index of this line
            {
                result += currentLineCharacterCountWithoutLineBreak;
                return result;
            }

            //  ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
            // ^CONTENT_OF_THIS_LINE$
            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);//TODO cache scale
            uint characterCountBeforeThisLine = 0;
            for (i = 0; i < LineIndex; i++)
            {
                characterCountBeforeThisLine += LineCharacterCountList[i];
            }
            var firstGlyphIndex = (int)characterCountBeforeThisLine;
            float offsetX = (float)position.X;
            for (i = firstGlyphIndex; i < glyphPlans.Count; i++)
            {
                var glyph = glyphPlans[i];
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
            height = lineHeight;
            if (glyphPlans.Count == 0)
            {
                pointX = (float)Position.X;
                pointY = (float)Position.Y;
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
                var g = glyphPlans[i];
                if (g.glyphIndex == 0)
                {
                    newLinesBeforeThisCaretPosition++;
                }
            }

            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);
            bool previousCharIsLineBreak = false;
            GlyphPlan previousGlyph = new GlyphPlan();
            if (previousCharIndex!=-1)
            {
                previousGlyph = glyphPlans[previousCharIndex];
                if (previousGlyph.glyphIndex == 0)// \n
                {
                    previousCharIsLineBreak = true;
                }
            }

            pointX = (float)Position.X;
            pointY = (float)Position.Y;
            if(previousCharIndex!=-1)
            {
                pointX += (previousGlyph.x + previousGlyph.advX) * scale;
            }

            if(previousCharIsLineBreak)
            {
                pointX = (float)Position.X;
                for (int i = 0; i < newLinesBeforeThisCaretPosition; i++)
                {
                    pointY += lineHeight;
                }
            }
            else
            {
                for (int i = 0; i < newLinesBeforeThisCaretPosition; i++)
                {
                    pointX -= LineWidthList[i];
                    pointY += lineHeight;
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
