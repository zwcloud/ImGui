using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Common.Primitive;

namespace ImGui
{
    internal partial class DrawList
    {
        /// <summary>
        /// Append a text mesh to this drawlist
        /// </summary>
        public void AddText(Rect rect, string text, GUIStyle style, GUIState state)
        {
            if (GetCurrentClipRect().IsEmpty)
            {
                return;
            }

            AddTextDrawCommand();

            var textMesh = this.TextMesh;
            var oldIndexBufferCount = textMesh.IndexBuffer.Count;

            string fontFamily = style.FontFamily;
            double fontSize = style.FontSize;
            Color fontColor = style.FontColor;

            // get offset and scale from text layout
            var scale = OSImplentation.TypographyTextContext.GetScale(fontFamily, fontSize);
            var textContext = TextMeshUtil.GetTextContext(text, rect.Size, style, state) as OSImplentation.TypographyTextContext;
            var glyphOffsets = textContext.GlyphOffsets;

            int index = -1;

            // get glyph data from typeface
            FontStyle fontStyle = style.FontStyle;
            FontWeight fontWeight = style.FontWeight;
            foreach (var character in text)
            {
                index++;
                if (char.IsWhiteSpace(character))
                {
                    continue;
                }
                var glyphData = GlyphCache.Default.GetGlyph(character, fontFamily, fontStyle, fontWeight);
                if (glyphData == null)
                {
                    Typography.OpenFont.Glyph glyph = OSImplentation.TypographyTextContext.LookUpGlyph(fontFamily, character);
                    var polygons = new List<List<Point>>();
                    var bezierSegments = new List<(Point, Point, Point)>();
                    Typography.OpenFont.GlyphLoader.Read(glyph, out polygons, out bezierSegments);
                    GlyphCache.Default.AddGlyph(character, fontFamily, fontStyle, fontWeight, polygons, bezierSegments);
                    glyphData = GlyphCache.Default.GetGlyph(character, fontFamily, fontStyle, fontWeight);
                    Debug.Assert(glyphData != null);
                }

                // append to drawlist
                Vector glyphOffset = glyphOffsets[index];
                var positionOffset = (Vector)rect.TopLeft;
                this.TextMesh.Append(positionOffset, glyphData, glyphOffset, scale, fontColor, false);
            }

            var newIndexBufferCount = textMesh.IndexBuffer.Count;

            // Update command
            var command = textMesh.Commands[textMesh.Commands.Count - 1];
            command.ElemCount += newIndexBufferCount - oldIndexBufferCount;
            textMesh.Commands[textMesh.Commands.Count - 1] = command;

            // TODO refactor this
        }
    }
}
