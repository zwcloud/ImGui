namespace ImGui
{
    internal class StyleRuleSetBuilder
    {
        private StyleRuleSet s;

        private void Set<T>(StylePropertyName styleName, T value)
        {
            this.s.Set<T>(styleName, value);
        }

        private void Set<T>(StylePropertyName styleName, T value, GUIState state)
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
            this.Set<double>(StylePropertyName.BorderTop, top, state);
            this.Set<double>(StylePropertyName.BorderRight, right, state);
            this.Set<double>(StylePropertyName.BorderBottom, bottom, state);
            this.Set<double>(StylePropertyName.BorderLeft, left, state);
            return this;
        }

        public StyleRuleSetBuilder Border((double top, double right, double bottom, double left) value,
            GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = value;
            this.Set<double>(StylePropertyName.BorderTop, top, state);
            this.Set<double>(StylePropertyName.BorderRight, right, state);
            this.Set<double>(StylePropertyName.BorderBottom, bottom, state);
            this.Set<double>(StylePropertyName.BorderLeft, left, state);
            return this;
        }

        public StyleRuleSetBuilder BorderColor(Color value, GUIState state = GUIState.Normal)
        {
            this.s.Set<Color>(StylePropertyName.BorderTopColor, value, state);
            this.s.Set<Color>(StylePropertyName.BorderRightColor, value, state);
            this.s.Set<Color>(StylePropertyName.BorderBottomColor, value, state);
            this.s.Set<Color>(StylePropertyName.BorderLeftColor, value, state);
            return this;
        }

        public StyleRuleSetBuilder BorderImageSource(string value, GUIState state = GUIState.Normal)
        {
            this.Set<string>(StylePropertyName.BorderImageSource, value, state);
            return this;
        }

        public StyleRuleSetBuilder BorderImageSlice((double top, double right, double bottom, double left) value, GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = value;
            this.Set<double>(StylePropertyName.BorderImageSliceTop, top);
            this.Set<double>(StylePropertyName.BorderImageSliceRight, right);
            this.Set<double>(StylePropertyName.BorderImageSliceBottom, bottom);
            this.Set<double>(StylePropertyName.BorderImageSliceLeft, left);
            return this;
        }

        public StyleRuleSetBuilder Padding(double value, GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = (value, value, value, value);
            this.Set<double>(StylePropertyName.PaddingTop, top, state);
            this.Set<double>(StylePropertyName.PaddingRight, right, state);
            this.Set<double>(StylePropertyName.PaddingBottom, bottom, state);
            this.Set<double>(StylePropertyName.PaddingLeft, left, state);
            return this;
        }

        public StyleRuleSetBuilder Padding((double top, double right, double bottom, double left) value, GUIState state = GUIState.Normal)
        {
            var (top, right, bottom, left) = value;
            this.Set<double>(StylePropertyName.PaddingTop, top, state);
            this.Set<double>(StylePropertyName.PaddingRight, right, state);
            this.Set<double>(StylePropertyName.PaddingBottom, bottom, state);
            this.Set<double>(StylePropertyName.PaddingLeft, left, state);
            return this;
        }

        public StyleRuleSetBuilder AlignmentVertical(Alignment value, GUIState state = GUIState.Normal)
        {
            this.Set(StylePropertyName.AlignmentVertical, value, state);
            return this;
        }

        public StyleRuleSetBuilder AlignmentHorizontal(Alignment value, GUIState state = GUIState.Normal)
        {
            this.Set(StylePropertyName.AlignmentHorizontal, value, state);
            return this;
        }

        public StyleRuleSetBuilder BackgroundColor(Color value, GUIState state = GUIState.Normal)
        {
            this.Set(StylePropertyName.BackgroundColor, value, state);
            return this;
        }

        public StyleRuleSetBuilder BackgroundGradient(Gradient value, GUIState state = GUIState.Normal)
        {
            this.Set(StylePropertyName.BackgroundGradient, value, state);
            return this;
        }

        public StyleRuleSetBuilder GradientTopDownColor(Color value1, Color value2, GUIState state = GUIState.Normal)
        {
            this.Set(StylePropertyName.GradientTopColor, value1, state);
            this.Set(StylePropertyName.GradientBottomColor, value2, state);
            return this;
        }

        public StyleRuleSetBuilder GradientLeftRightColor(Color value1, Color value2, GUIState state = GUIState.Normal)
        {
            this.Set(StylePropertyName.GradientLeftColor, value1, state);
            this.Set(StylePropertyName.GradientRightColor, value2, state);
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

        public StyleRuleSetBuilder StrokeColor(Color value, GUIState state)
        {
            this.Set(StylePropertyName.StrokeColor, value, state);
            return this;
        }

        public StyleRuleSetBuilder StrokeWidth(double value, GUIState state)
        {
            this.Set(StylePropertyName.StrokeWidth, value, state);
            return this;
        }
    }

}