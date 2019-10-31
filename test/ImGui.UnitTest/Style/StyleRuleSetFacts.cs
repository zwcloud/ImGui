using Xunit;

namespace ImGui.UnitTest.Style
{
    public class StyleRuleSetFacts
    {
        public class GetRule
        {
            [Fact]
            public void String()
            {
                var styleRuleSet = new StyleRuleSet();
                styleRuleSet.Set<string>(StylePropertyName.BorderImageSource, @"assets\images\button.png", GUIState.Normal);
                var rule = styleRuleSet.GetRule<string>(StylePropertyName.BorderImageSource);
                Assert.Equal(@"assets\images\button.png", rule.Value);
            }

            [Fact]
            public void Number()
            {
                var styleRuleSet = new StyleRuleSet();
                styleRuleSet.Set<double>(StylePropertyName.BorderImageSliceLeft, 12.5, GUIState.Normal);
                var rule = styleRuleSet.GetRule<double>(StylePropertyName.BorderImageSliceLeft);
                Assert.Equal(12.5, rule.Value);
            }

            [Fact]
            public void Enum()
            {
                var styleRuleSet = new StyleRuleSet();
                styleRuleSet.Set<int>(StylePropertyName.BorderImageSliceLeft, (int)Alignment.Center, GUIState.Normal);
                var rule = styleRuleSet.GetRule<int>(StylePropertyName.BorderImageSliceLeft);
                Assert.Equal(Alignment.Center,  (Alignment)rule.Value);
            }
        }

    }
}