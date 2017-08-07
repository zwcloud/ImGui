using System;
using ImGui.Common.Primitive;
using ImGui.Layout;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public class LayoutEntryFacts
    {
        public class TheCtor
        {
            [Fact]
            public void CreatedLayoutEntryIsProperlySet()
            {
                var style = new GUIStyle();
                var options = new[] {GUILayout.Width(100), GUILayout.Height(200)};
                var entry = new LayoutEntry(style, options);

                //Assert.Equal(100, entry.ContentWidth);
                Assert.Equal(100, entry.MinWidth);
                Assert.Equal(100, entry.MaxWidth);
                Assert.True(entry.IsFixedWidth);
                Assert.False(entry.HorizontallyStretched);
                Assert.Equal(0, entry.HorizontalStretchFactor);

                //Assert.Equal(200, entry.ContentHeight);
                Assert.Equal(200, entry.MinHeight);
                Assert.Equal(200, entry.MaxHeight);
                Assert.True(entry.IsFixedHeight);
                Assert.False(entry.VerticallyStretched);
                Assert.Equal(0, entry.VerticalStretchFactor);
            }
        }
    }
}
