using ImGui.Common.Primitive;
using System;

namespace ImGui.Style
{
    internal static class StyleRuleSetEx
    {
        public static Size CalcSize(this StyleRuleSet ruleSet, string text, GUIState state)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text == "") { return Size.Zero; }

            var width = -1d;
            var height = -1d;

            // apply font and text styles
            {
                var measureContext = TextMeshUtil.GetTextContext(text, new Size(4096, 4096), ruleSet, state);
                var actualSize = measureContext.Measure();
                width = actualSize.Width;
                height = actualSize.Height;
            }
            if (width < 0)
            {
                width = 0;
            }
            if (height < 0)
            {
                height = 0;
            }

            var size = new Size(Math.Ceiling(width), Math.Ceiling(height));
            return size;
        }
    }
}
