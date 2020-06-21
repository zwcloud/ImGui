using System;

namespace ImGui.Style
{
    internal static class StyleRuleSetEx
    {
        public static Size CalcSize(this StyleRuleSet ruleSet, string text, GUIState state)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            //Note text height shouldn't be set to zero but to the line height,
            //since it determines the height of inline controls.

            // apply font and text styles
            var measureContext = TextMeshUtil.GetTextContext(text, new Size(4096, 4096), ruleSet, state);
            var actualSize = measureContext.Measure();
            double width = actualSize.Width;
            double height = actualSize.Height;
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
