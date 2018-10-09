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

        public StyleRuleSetBuilder Border(double value, GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = (value, value, value, value);
            this.s.Set<double>(GUIStyleName.BorderTop, top, state);
            this.s.Set<double>(GUIStyleName.BorderRight, right, state);
            this.s.Set<double>(GUIStyleName.BorderBottom, bottom, state);
            this.s.Set<double>(GUIStyleName.BorderLeft, left, state);
            return this;
        }

        public StyleRuleSetBuilder Border((double top, double right, double bottom, double left) value,
            GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = value;
            this.s.Set<double>(GUIStyleName.BorderTop, top, state);
            this.s.Set<double>(GUIStyleName.BorderRight, right, state);
            this.s.Set<double>(GUIStyleName.BorderBottom, bottom, state);
            this.s.Set<double>(GUIStyleName.BorderLeft, left, state);
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

        public StyleRuleSetBuilder Padding(double value, GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = (value, value, value, value);
            this.s.Set<double>(GUIStyleName.PaddingTop, top, state);
            this.s.Set<double>(GUIStyleName.PaddingRight, right, state);
            this.s.Set<double>(GUIStyleName.PaddingBottom, bottom, state);
            this.s.Set<double>(GUIStyleName.PaddingLeft, left, state);
            return this;
        }

        public StyleRuleSetBuilder Padding((double top, double right, double bottom, double left) value, GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = value;
            this.s.Set<double>(GUIStyleName.PaddingTop, top, state);
            this.s.Set<double>(GUIStyleName.PaddingRight, right, state);
            this.s.Set<double>(GUIStyleName.PaddingBottom, bottom, state);
            this.s.Set<double>(GUIStyleName.PaddingLeft, left, state);
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