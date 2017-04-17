//MIT, 2016-2017, WinterDev

using System.Collections.Generic;
using ImGui;
using Typography.OpenFont;
using Typography.TextLayout;

namespace Typography.Rendering
{
    /// <summary>
    /// text printer
    /// </summary>
    class TextPrinter
    {
        private readonly GlyphLayout glyphLayout = new GlyphLayout();

        private readonly List<GlyphPlan> outputGlyphPlans = new List<GlyphPlan>();
        private GlyphTranslatorToPath pathTranslator;
        private string currentFontFile;
        private GlyphPathBuilder currentGlyphPathBuilder;

        public TextPrinter()
        {
            FontSizeInPoints = 14;
            ScriptLang = ScriptLangs.Latin;
        }        

        /// <summary>
        /// Font file path
        /// </summary>
        public string FontFilename
        {
            get { return currentFontFile; }
            set
            {
                if (currentFontFile != value) 
                {
                    currentFontFile = value;
                    
                    using (var fs = Utility.ReadFile(currentFontFile))
                    {
                        var reader = new OpenFontReader();
                        Profile.Start("OpenFontReader.Read");
                        CurrentTypeFace = reader.Read(fs);
                        Profile.End();
                    }

                    //2. glyph builder
                    currentGlyphPathBuilder = new GlyphPathBuilder(CurrentTypeFace);
                    currentGlyphPathBuilder.UseTrueTypeInstructions = false; //reset
                    currentGlyphPathBuilder.UseVerticalHinting = false; //reset
                    switch (this.HintTechnique)
                    {
                        case HintTechnique.TrueTypeInstruction:
                            currentGlyphPathBuilder.UseTrueTypeInstructions = true;
                            break;
                        case HintTechnique.TrueTypeInstruction_VerticalOnly:
                            currentGlyphPathBuilder.UseTrueTypeInstructions = true;
                            currentGlyphPathBuilder.UseVerticalHinting = true;
                            break;
                        case HintTechnique.CustomAutoFit:
                            //custom agg autofit 
                            break;
                    }

                    //3. glyph translater
                    pathTranslator = new GlyphTranslatorToPath();

                    //4. Update GlyphLayout
                    glyphLayout.ScriptLang = this.ScriptLang;
                    glyphLayout.PositionTechnique = this.PositionTechnique;
                    glyphLayout.EnableLigature = this.EnableLigature;
                }
            }
        }

        public HintTechnique HintTechnique { get; set; }
        public float FontSizeInPoints { get; set; }
        public ScriptLang ScriptLang { get; set; }
        public PositionTechnique PositionTechnique { get; set; }
        public bool EnableLigature { get; set; }
        public Typeface CurrentTypeFace { get; private set; }

        /// <summary>
        /// draw glyph as paths
        /// </summary>
        /// <param name="g">the path builder used to draw the path</param>
        /// <param name="textBuffer">characters</param>
        /// <param name="x">offset x</param>
        /// <param name="y">offset y</param>
        public void Draw(ITextPathBuilder g, char[] textBuffer, float x, float y)
        {
            // layout glyphs with selected layout technique
            var sizeInPoints = this.FontSizeInPoints;
            outputGlyphPlans.Clear();
            //glyphLayout.Layout(CurrentTypeFace, textBuffer, 0, textBuffer.Length, outputGlyphPlans);
            glyphLayout.Typeface = this.CurrentTypeFace;
            glyphLayout.GenerateGlyphPlans(textBuffer, 0, textBuffer.Length, outputGlyphPlans, null);

            // render each glyph
            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(sizeInPoints);
            pathTranslator.PathBuilder = g;
            for (var i = 0; i < outputGlyphPlans.Count; ++i)
            {
                pathTranslator.Reset();
                var glyphPlan = outputGlyphPlans[i];
                currentGlyphPathBuilder.BuildFromGlyphIndex(glyphPlan.glyphIndex, sizeInPoints);
                currentGlyphPathBuilder.ReadShapes(pathTranslator, sizeInPoints, x + glyphPlan.x * scale, y + glyphPlan.y * scale);
            }
        }

        public Size Measure(char[] textBuffer, int startAt, int len)
        {
            glyphLayout.Typeface = this.CurrentTypeFace;
            var scale = CurrentTypeFace.CalculateToPixelScaleFromPointSize(this.FontSizeInPoints);
            MeasuredStringBox strBox;
            glyphLayout.MeasureString(textBuffer, startAt, len, out strBox, scale);
            return new Size(strBox.width, strBox.CalculateLineHeight());
        }

    }
}