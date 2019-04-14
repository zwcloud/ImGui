using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public class LayoutEntryFacts
    {
        public class InitFacts
        {
            [Fact]
            public void DefaultSizedEntryProperlyInitialized()
            {
                var entry = new Node(1);
                entry.AttachLayoutEntry(new Size(100, 200));

                Assert.Equal(100, entry.ContentWidth);
                Assert.Equal(1, entry.RuleSet.MinWidth);
                Assert.Equal(9999, entry.RuleSet.MaxWidth);
                Assert.False(entry.RuleSet.IsFixedWidth);
                Assert.False(entry.RuleSet.HorizontallyStretched);
                Assert.Equal(0, entry.RuleSet.HorizontalStretchFactor);

                Assert.Equal(200, entry.ContentHeight);
                Assert.Equal(1, entry.RuleSet.MinHeight);
                Assert.Equal(9999, entry.RuleSet.MaxHeight);
                Assert.False(entry.RuleSet.IsFixedHeight);
                Assert.False(entry.RuleSet.VerticallyStretched);
                Assert.Equal(0, entry.RuleSet.VerticalStretchFactor);
            }

            [Fact]
            public void FixSizedEntryProperlyInitialized()
            {
                var options = GUILayout.Width(100).Height(200);
                var entry = new Node(1);
                entry.AttachLayoutEntry();
                entry.RuleSet.ApplyOptions(options);

                Assert.Equal(0, entry.ContentWidth);
                Assert.Equal(100, entry.RuleSet.MinWidth);
                Assert.Equal(100, entry.RuleSet.MaxWidth);
                Assert.True(entry.RuleSet.IsFixedWidth);
                Assert.False(entry.RuleSet.HorizontallyStretched);
                Assert.Equal(0, entry.RuleSet.HorizontalStretchFactor);

                Assert.Equal(0, entry.ContentHeight);
                Assert.Equal(200, entry.RuleSet.MinHeight);
                Assert.Equal(200, entry.RuleSet.MaxHeight);
                Assert.True(entry.RuleSet.IsFixedHeight);
                Assert.False(entry.RuleSet.VerticallyStretched);
                Assert.Equal(0, entry.RuleSet.VerticalStretchFactor);
            }

            [Fact]
            public void FlexSizedEntryProperlyInitialized()
            {
                var options = GUILayout.StretchWidth(1).StretchHeight(2);
                var entry = new Node(1);
                entry.AttachLayoutEntry();
                entry.RuleSet.ApplyOptions(options);

                Assert.Equal(0, entry.ContentWidth);
                Assert.Equal(1, entry.RuleSet.MinWidth);
                Assert.Equal(9999, entry.RuleSet.MaxWidth);
                Assert.False(entry.RuleSet.IsFixedWidth);
                Assert.True(entry.RuleSet.HorizontallyStretched);
                Assert.Equal(1, entry.RuleSet.HorizontalStretchFactor);

                Assert.Equal(0, entry.ContentHeight);
                Assert.Equal(1, entry.RuleSet.MinHeight);
                Assert.Equal(9999, entry.RuleSet.MaxHeight);
                Assert.False(entry.RuleSet.IsFixedHeight);
                Assert.True(entry.RuleSet.VerticallyStretched);
                Assert.Equal(2, entry.RuleSet.VerticalStretchFactor);
            }
        }
    }
}
