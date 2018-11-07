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
                styleRuleSet.Set<string>(GUIStyleName.BorderImageSource, @"assets\images\button.png", GUIState.Normal);
                var rule = styleRuleSet.GetRule<string>(GUIStyleName.BorderImageSource);
                Assert.Equal(@"assets\images\button.png", rule.Value);
            }

            [Fact]
            public void Number()
            {
                var styleRuleSet = new StyleRuleSet();
                styleRuleSet.Set<double>(GUIStyleName.BorderImageSliceLeft, 12.5, GUIState.Normal);
                var rule = styleRuleSet.GetRule<double>(GUIStyleName.BorderImageSliceLeft);
                Assert.Equal(12.5, rule.Value);
            }

            [Fact]
            public void Enum()
            {
                var styleRuleSet = new StyleRuleSet();
                styleRuleSet.Set<int>(GUIStyleName.BorderImageSliceLeft, (int)Alignment.Center, GUIState.Normal);
                var rule = styleRuleSet.GetRule<int>(GUIStyleName.BorderImageSliceLeft);
                Assert.Equal(Alignment.Center,  (Alignment)rule.Value);
            }
        }

    }
}