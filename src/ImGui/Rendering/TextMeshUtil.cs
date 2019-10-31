using System;
using ImGui.OSAbstraction.Text;

namespace ImGui
{
    internal class TextMeshUtil
    {
        internal static ITextContext GetTextContext(string text, Size size, StyleRuleSet style, GUIState state)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (style == null) throw new ArgumentNullException(nameof(style));

            var fontFamily = style.Get<string>(StylePropertyName.FontFamily, state);
            var fontSize = style.Get<double>(StylePropertyName.FontSize, state);
            var textAlignment = (TextAlignment)style.Get<int>(StylePropertyName.TextAlignment, state);
            return TextContextCache.Default.GetOrAdd(text, fontFamily, fontSize, textAlignment);
        }

        //FIXME remove this method
        internal static ITextContext GetTextContext(string text, Size size, GUIStyle style, GUIState state)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (style == null) throw new ArgumentNullException(nameof(style));

            var fontFamily = style.Get<string>(StylePropertyName.FontFamily, state);
            var fontSize = style.Get<double>(StylePropertyName.FontSize, state);
            var textAlignment = (TextAlignment)style.Get<int>(StylePropertyName.TextAlignment, state);
            return TextContextCache.Default.GetOrAdd(text, fontFamily, fontSize, textAlignment);
        }
    }
}