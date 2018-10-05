using ImGui.Common.Primitive;

namespace ImGui
{
    internal class StyleRuleSetBuilder
    {
        private StyleRuleSet s;

        public StyleRuleSetBuilder(IStyleRuleSet ruleSet)
        {
            this.s = ruleSet.RuleSet;
        }

        public StyleRuleSetBuilder(StyleRuleSet ruleSet)
        {
            this.s = ruleSet;
        }

        public StyleRuleSetBuilder Border(double value)
        {
            s.Border = (value, value, value, value);
            return this;
        }

        public StyleRuleSetBuilder Border((double top, double right, double bottom, double left) value)
        {
            s.Border = value;
            return this;
        }

        public StyleRuleSetBuilder BorderColor(Color value, GUIState state = GUIState.Normal)
        {
            s.Set(GUIStyleName.BorderTopColor, value, state);
            s.Set(GUIStyleName.BorderRightColor, value, state);
            s.Set(GUIStyleName.BorderBottomColor, value, state);
            s.Set(GUIStyleName.BorderLeftColor, value, state);
            return this;
        }

        public StyleRuleSetBuilder Padding(double value)
        {
            s.Padding = (value, value, value, value);
            return this;
        }

        public StyleRuleSetBuilder Padding((double top, double right, double bottom, double left) value)
        {
            s.Padding = value;
            return this;
        }

        public StyleRuleSetBuilder AlignmentVertical(Alignment value, GUIState state = GUIState.Normal)
        {
            s.Set(GUIStyleName.AlignmentVertical, value, state);
            return this;
        }

        public StyleRuleSetBuilder AlignmentHorizontal(Alignment value, GUIState state = GUIState.Normal)
        {
            s.Set(GUIStyleName.AlignmentHorizontal, value, state);
            return this;
        }

        public StyleRuleSetBuilder BackgroundColor(Color value, GUIState state = GUIState.Normal)
        {
            s.Set(GUIStyleName.BackgroundColor, value, state);
            return this;
        }

        public StyleRuleSetBuilder BackgroundGradient(Gradient value, GUIState state = GUIState.Normal)
        {
            s.Set(GUIStyleName.BackgroundGradient, value, state);
            return this;
        }

        public StyleRuleSetBuilder GradientTopDownColor(Color value1, Color value2, GUIState state = GUIState.Normal)
        {
            s.Set(GUIStyleName.GradientTopColor, value1, state);
            s.Set(GUIStyleName.GradientBottomColor, value2, state);
            return this;
        }

        public StyleRuleSetBuilder GradientLeftRightColor(Color value1, Color value2, GUIState state = GUIState.Normal)
        {
            s.Set(GUIStyleName.GradientLeftColor, value1, state);
            s.Set(GUIStyleName.GradientRightColor, value2, state);
            return this;
        }

        public StyleRuleSetBuilder FontSize(double value)
        {
            s.FontSize = value;
            return this;
        }

        public StyleRuleSetBuilder FontColor(Color value)
        {
            s.FontColor = value;
            return this;
        }
    }

}