using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Text;

namespace ImGui
{
    class TextMeshUtil
    {
        static readonly Dictionary<int, ITextContext> TextContextCache = new Dictionary<int, ITextContext>(255);

        static int GetTextId(string text, GUIState state)
        {
            int hash = 17;
            hash = hash * 23 + text.GetHashCode();
            hash = hash * 23 + state.GetHashCode();
            return hash;
        }

        internal static ITextContext GetTextContext(string text, Size size, StyleRuleSet style, GUIState state)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (style == null) throw new ArgumentNullException(nameof(style));

            int textMeshId = GetTextId(text, state);

            ITextContext textContext;
            if (TextContextCache.TryGetValue(textMeshId, out textContext))
            {
                return textContext;
            }
            else
            {
                // create a TextContent for the text
                var fontFamily = style.Get<string>(GUIStyleName.FontFamily, state);
                var fontSize = style.Get<double>(GUIStyleName.FontSize, state);
                var fontStretch = (FontStretch)style.Get<int>(GUIStyleName.FontStretch, state);
                var fontStyle = (FontStyle)style.Get<int>(GUIStyleName.FontStyle, state);
                var fontWeight = (FontWeight)style.Get<int>(GUIStyleName.FontWeight, state);
                var textAlignment = (TextAlignment)style.Get<int>(GUIStyleName.TextAlignment, state);
                textContext = Application.PlatformContext.CreateTextContext(
                    text,
                    fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                    (int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height),
                    textAlignment);
                textContext.Build(Point.Zero);
                TextContextCache.Add(textMeshId, textContext);
            }
            return textContext;
        }

        //FIXME remove this method
        internal static ITextContext GetTextContext(string text, Size size, GUIStyle style, GUIState state)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (style == null) throw new ArgumentNullException(nameof(style));

            int textMeshId = GetTextId(text, state);

            ITextContext textContext;
            if (TextContextCache.TryGetValue(textMeshId, out textContext))
            {
                return textContext;
            }
            else
            {
                // create a TextContent for the text
                var fontFamily = style.Get<string>(GUIStyleName.FontFamily, state);
                var fontSize = style.Get<double>(GUIStyleName.FontSize, state);
                var fontStretch = (FontStretch)style.Get<int>(GUIStyleName.FontStretch, state);
                var fontStyle = (FontStyle)style.Get<int>(GUIStyleName.FontStyle, state);
                var fontWeight = (FontWeight)style.Get<int>(GUIStyleName.FontWeight, state);
                var textAlignment = (TextAlignment)style.Get<int>(GUIStyleName.TextAlignment, state);
                textContext = Application.PlatformContext.CreateTextContext(
                    text,
                    fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                    (int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height),
                    textAlignment);
                textContext.Build(Point.Zero);
                TextContextCache.Add(textMeshId, textContext);
            }
            return textContext;
        }
    }
}