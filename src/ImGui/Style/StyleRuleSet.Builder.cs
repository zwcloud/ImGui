namespace ImGui
{
    internal class StyleRuleSetBuilder
    {
        private StyleRuleSet s;

        private void Set<T>(GUIStyleName styleName, T value)
        {
            this.s.Set<T>(styleName, value);
        }

        private void Set<T>(GUIStyleName styleName, T value, GUIState state)
        {
            this.s.Set<T>(styleName, value, state);
        }

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
            this.Set<double>(GUIStyleName.BorderTop, top, state);
            this.Set<double>(GUIStyleName.BorderRight, right, state);
            this.Set<double>(GUIStyleName.BorderBottom, bottom, state);
            this.Set<double>(GUIStyleName.BorderLeft, left, state);
            return this;
        }

        public StyleRuleSetBuilder Border((double top, double right, double bottom, double left) value,
            GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = value;
            this.Set<double>(GUIStyleName.BorderTop, top, state);
            this.Set<double>(GUIStyleName.BorderRight, right, state);
            this.Set<double>(GUIStyleName.BorderBottom, bottom, state);
            this.Set<double>(GUIStyleName.BorderLeft, left, state);
            return this;
        }

        public StyleRuleSetBuilder BorderColor(Color value, GUIState state = GUIState.Normal)
        {
            this.s.Set<Color>(GUIStyleName.BorderTopColor, value, state);
            this.s.Set<Color>(GUIStyleName.BorderRightColor, value, state);
            this.s.Set<Color>(GUIStyleName.BorderBottomColor, value, state);
            this.s.Set<Color>(GUIStyleName.BorderLeftColor, value, state);
            return this;
        }

        public StyleRuleSetBuilder BorderImageSource(string value, GUIState state = GUIState.Normal)
        {
            this.Set<string>(GUIStyleName.BorderImageSource, value, state);
            return this;
        }

        public StyleRuleSetBuilder BorderImageSlice((double top, double right, double bottom, double left) value, GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = value;
            this.Set<double>(GUIStyleName.BorderImageSliceTop, top);
            this.Set<double>(GUIStyleName.BorderImageSliceRight, right);
            this.Set<double>(GUIStyleName.BorderImageSliceBottom, bottom);
            this.Set<double>(GUIStyleName.BorderImageSliceLeft, left);
            return this;
        }

        public StyleRuleSetBuilder Padding(double value, GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = (value, value, value, value);
            this.Set<double>(GUIStyleName.PaddingTop, top, state);
            this.Set<double>(GUIStyleName.PaddingRight, right, state);
            this.Set<double>(GUIStyleName.PaddingBottom, bottom, state);
            this.Set<double>(GUIStyleName.PaddingLeft, left, state);
            return this;
        }

        public StyleRuleSetBuilder Padding((double top, double right, double bottom, double left) value, GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = value;
            this.Set<double>(GUIStyleName.PaddingTop, top, state);
            this.Set<double>(GUIStyleName.PaddingRight, right, state);
            this.Set<double>(GUIStyleName.PaddingBottom, bottom, state);
            this.Set<double>(GUIStyleName.PaddingLeft, left, state);
            return this;
        }

        public StyleRuleSetBuilder AlignmentVertical(Alignment value, GUIState state = GUIState.Normal)
        {
            this.Set(GUIStyleName.AlignmentVertical, value, state);
            return this;
        }

        public StyleRuleSetBuilder AlignmentHorizontal(Alignment value, GUIState state = GUIState.Normal)
        {
            this.Set(GUIStyleName.AlignmentHorizontal, value, state);
            return this;
        }

        public StyleRuleSetBuilder BackgroundColor(Color value, GUIState state = GUIState.Normal)
        {
            this.Set(GUIStyleName.BackgroundColor, value, state);
            return this;
        }

        public StyleRuleSetBuilder BackgroundGradient(Gradient value, GUIState state = GUIState.Normal)
        {
            this.Set(GUIStyleName.BackgroundGradient, value, state);
            return this;
        }

        public StyleRuleSetBuilder GradientTopDownColor(Color value1, Color value2, GUIState state = GUIState.Normal)
        {
            this.Set(GUIStyleName.GradientTopColor, value1, state);
            this.Set(GUIStyleName.GradientBottomColor, value2, state);
            return this;
        }

        public StyleRuleSetBuilder GradientLeftRightColor(Color value1, Color value2, GUIState state = GUIState.Normal)
        {
            this.Set(GUIStyleName.GradientLeftColor, value1, state);
            this.Set(GUIStyleName.GradientRightColor, value2, state);
            return this;
        }

        public StyleRuleSetBuilder FontSize(double value)
        {
            this.s.FontSize = value;
            return this;
        }

        public StyleRuleSetBuilder FontColor(Color value)
        {
            this.s.FontColor = value;
            return this;
        }

        public StyleRuleSetBuilder FillColor(Color value)
        {
            this.s.FillColor = value;
            return this;
        }
    }

}