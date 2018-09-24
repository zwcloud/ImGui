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

        public StyleRuleSetBuilder BorderColor(Color value)
        {
            s.BorderColor = (value, value, value, value);
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

        public StyleRuleSetBuilder BackgroundColor(Color value)
        {
            s.BackgroundColor = value;
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