using System;
using System.Collections.Generic;
using Typography;
using Typography.OpenFont;
using Typography.TextLayout;

namespace ImGui.OSImplementation
{
    public static class TypographyTypefaceExtension
    {
        //For character that cannot be represented as one single char
        public static Glyph Lookup(this Typeface typeface, char[] str)
        {
            int codepoint = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                char ch = str[i];
                codepoint = ch;
                if (ch >= 0xd800 && ch <= 0xdbff && i + 1 < str.Length)
                {
                    char nextCh = str[i + 1];
                    if (nextCh >= 0xdc00 && nextCh <= 0xdfff)
                    {
                        //please note: 
                        //num of codepoint may be less than  original user input char 
                        ++i;
                        codepoint = char.ConvertToUtf32(ch, nextCh);
                    }
                }
            }

            var glyphIndex = typeface.GetGlyphIndex(codepoint);
            return typeface.GetGlyph(glyphIndex);
        }

        public static Glyph Lookup(this Typeface typeface, char character)
        {
            int codepoint = character;
            var glyphIndex = typeface.GetGlyphIndex(codepoint);
            return typeface.GetGlyph(glyphIndex);
        }
    }

    public static class TypographyGlyphLayoutExtension
    {
        public static void GenerateGlyphPlans(this GlyphLayout glyphLayout,
            char[] textBuffer,
            int startAt,
            int len,
            UnscaledGlyphPlanList list)
        {
            //generate glyph plan based on its current setting
            glyphLayout.Layout(textBuffer, startAt, len);
            ReadOutput(glyphLayout, list);
        }
        
        /// <summary>
        /// read GlyphPlan latest layout output
        /// </summary>
        private static void ReadOutput(GlyphLayout glyphLayout,
            UnscaledGlyphPlanList outputGlyphPlanList)
        {
            Typeface typeface = glyphLayout.Typeface;
            var glyphPositions = glyphLayout._glyphPositions;
            //3.read back
            int finalGlyphCount = glyphPositions.Count;
            int cx = 0;
            short cy = 0;

            PositionTechnique posTech = glyphLayout.PositionTechnique;
            ushort prev_index = 0;
            for (int i = 0; i < finalGlyphCount; ++i)
            {

                GlyphPos glyphPos = glyphPositions[i];
                switch (posTech)
                {
                    default: throw new NotSupportedException();
                    case PositionTechnique.None:
                        outputGlyphPlanList.Append(new UnscaledGlyphPlan(
                            0,
                            glyphPos.glyphIndex, glyphPos.advanceW, cx, cy));
                        break;
                    case PositionTechnique.OpenFont:
                        outputGlyphPlanList.Append(new UnscaledGlyphPlan(
                            0,
                            glyphPos.glyphIndex,
                            glyphPos.advanceW,
                            cx + glyphPos.xoffset,
                            (short)(cy + glyphPos.yoffset)));
                        break;
                    case PositionTechnique.Kerning:

                        if (i > 0)
                        {
                            cx += typeface.GetKernDistance(prev_index, glyphPos.glyphIndex);
                        }
                        outputGlyphPlanList.Append(new UnscaledGlyphPlan(
                            0,
                            prev_index = glyphPos.glyphIndex,
                            glyphPos.advanceW,
                            cx,
                            cy));

                        break;
                }
                cx += glyphPos.advanceW;
            }
        }
    }
}