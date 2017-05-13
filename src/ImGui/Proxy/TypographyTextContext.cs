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

        public void Build(Point offset, ITextPathBuilder textMesh)
        {
            //Profile.Start("TypographyTextContext.Build");
            // layout glyphs with selected layout technique
            this.Position = offset;
            glyphPlans.Clear();
            glyphLayout.Typeface = this.CurrentTypeFace;
            glyphLayout.GenerateGlyphPlans(this.textCharacters, 0, this.textCharacters.Length, glyphPlans, null);

            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);

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
            var lineHeight = strBox.CalculateLineHeight();
            this.Size = new Size(strBox.width, lineHeight);

            // render each glyph
            glyphPathTranslator.PathBuilder = textMesh;
            for (var i = 0; i < glyphPlans.Count; ++i)
            {
                glyphPathTranslator.Reset();
                var glyphPlan = glyphPlans[i];
                glyphPathBuilder.BuildFromGlyphIndex(glyphPlan.glyphIndex, this.FontSize);
                glyphPathBuilder.ReadShapes(
                    glyphPathTranslator, this.FontSize,
                    (float)offset.X + glyphPlan.x * scale,
                    (float)offset.Y + glyphPlan.y * scale
                        + lineHeight//this extra  offset moves all shapes from (0, 0) to (0, line height)
                    );
            }

            //Profile.End();
        }

        public Size Measure()
        {
            //Profile.Start("TypographyTextContext.Measure");
            this.Position = Point.Zero;
            glyphLayout.Typeface = this.CurrentTypeFace;
            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);
            if(string.IsNullOrEmpty(this.Text))
            {
                this.Size = Size.Zero;
            }
            else
            {
                if(glyphPlans.Count == 0)
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
                var lineHeight = strBox.CalculateLineHeight();
                this.Size = new Size(strBox.width, lineHeight);
            }
            //Profile.End();

            return this.Size;
        }

        public uint XyToIndex(float pointX, float pointY, out bool isInside)
        {
            //TODO handle y when Typography is ready
            var position = this.Position;
            isInside = false;
            if (pointX < position.X)
            {
                return 0;
            }
            if(pointX > position.X + this.Size.Width)
            {
                return (uint)glyphPlans.Count;
            }

            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);
            int i = 0;
            for (i = 0; i < glyphPlans.Count; i++)
            {
                var glyph = glyphPlans[i];
                var minX = position.X + glyph.x * scale;
                var maxX = minX + glyph.advX * scale;
                if(minX <= pointX && pointX < maxX)
                {
                    isInside = true;
                    return (uint)i;
                }
            }
            return (uint)i;
        }

        public void IndexToXY(uint charIndex, bool isTrailing, out float pointX, out float pointY, out float height)
        {
            //TODO handle y when Typography is ready
            height = (float)(this.Size.Height);//TODO use real line height instead of layout-box height
            pointY = (float)(Position.Y);

            if (glyphPlans.Count == 0)
            {
                pointX = (float)(Position.X - height);
                return;
            }

            if(charIndex > glyphPlans.Count - 1)
            {
                charIndex = (uint)(glyphPlans.Count - 1);
                isTrailing = true;
            }
            var position = this.Position;
            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSize);
            var lastGlyph = glyphPlans[(int)charIndex];
            if (isTrailing)
            {
                pointX = (float)(Position.X + (lastGlyph.x + lastGlyph.advX) * scale);
            }
            else
            {
                pointX = (float)(Position.X + lastGlyph.x * scale);
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
