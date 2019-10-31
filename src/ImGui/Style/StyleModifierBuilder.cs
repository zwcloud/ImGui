using System.Collections.Generic;

namespace ImGui
{
    public class StyleModifierBuilder
    {
        private List<StyleModifier> modifiers = new List<StyleModifier>();

        public int Count => modifiers.Count;

        public void Clear()
        {
            modifiers.Clear();
        }

        public StyleModifier[] ToArray()
        {
            return modifiers.ToArray();
        }

        #region border

        public void PushBorder((double, double, double, double) border, GUIState state)
        {
            var modifier1 = new StyleModifier(StylePropertyName.BorderTop, StyleType.@double, border.Item1, state);
            var modifier2 = new StyleModifier(StylePropertyName.BorderRight, StyleType.@double, border.Item2, state);
            var modifier3 = new StyleModifier(StylePropertyName.BorderBottom, StyleType.@double, border.Item3, state);
            var modifier4 = new StyleModifier(StylePropertyName.BorderLeft, StyleType.@double, border.Item4, state);
            this.modifiers.Add(modifier1);
            this.modifiers.Add(modifier2);
            this.modifiers.Add(modifier3);
            this.modifiers.Add(modifier4);
        }

        public void PushBorder(double border, GUIState state = GUIState.Normal) => PushBorder((border, border, border, border), state);

        public void PushBorder(double border) => PushBorder(border, GUIState.Normal);

        #endregion

        #region padding

        public void PushPadding((double, double, double, double) padding, GUIState state)
        {
            var modifier1 = new StyleModifier(StylePropertyName.PaddingTop, StyleType.@double, padding.Item1, state);
            var modifier2 = new StyleModifier(StylePropertyName.PaddingRight, StyleType.@double, padding.Item2, state);
            var modifier3 = new StyleModifier(StylePropertyName.PaddingBottom, StyleType.@double, padding.Item3, state);
            var modifier4 = new StyleModifier(StylePropertyName.PaddingLeft, StyleType.@double, padding.Item4, state);
            this.modifiers.Add(modifier1);
            this.modifiers.Add(modifier2);
            this.modifiers.Add(modifier3);
            this.modifiers.Add(modifier4);
        }

        public void PushPadding(double padding, GUIState state = GUIState.Normal) => PushPadding((padding, padding, padding, padding), state);

        public void PushPadding(double padding) => PushPadding(padding, GUIState.Normal);

        #endregion


        #region image, color

        public void PushBorderColor(Color color, GUIState state = GUIState.Normal)
        {
            var modifier1 = new StyleModifier(StylePropertyName.BorderTopColor, StyleType.Color, color, state);
            var modifier2 = new StyleModifier(StylePropertyName.BorderRightColor, StyleType.Color, color, state);
            var modifier3 = new StyleModifier(StylePropertyName.BorderLeftColor, StyleType.Color, color, state);
            var modifier4 = new StyleModifier(StylePropertyName.BorderBottomColor, StyleType.Color, color, state);
            this.modifiers.Add(modifier1);
            this.modifiers.Add(modifier2);
            this.modifiers.Add(modifier3);
            this.modifiers.Add(modifier4);
        }

        public void PushBgColor(Color color, GUIState state = GUIState.Normal)
        {
            var modifier = new StyleModifier(StylePropertyName.BackgroundColor, StyleType.Color, color, state);
            this.modifiers.Add(modifier);
        }

        public void PushBgGradient(Gradient gradient, GUIState state = GUIState.Normal)
        {
            var modifier = new StyleModifier(StylePropertyName.BackgroundGradient, StyleType.@int, (int)gradient, state);
            this.modifiers.Add(modifier);
        }

        public void PushGradientColor(Color topColor, Color bottomColor, GUIState state = GUIState.Normal)
        {
            var modifier1 = new StyleModifier(StylePropertyName.GradientTopColor, StyleType.Color, topColor, state);
            var modifier2 = new StyleModifier(StylePropertyName.GradientBottomColor, StyleType.Color, bottomColor, state);
            this.modifiers.Add(modifier1);
            this.modifiers.Add(modifier2);
        }

        #endregion
    }
}
