using System;
using ImGui.Common.Primitive;
using ImGui.Layout;
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
                var entry = new LayoutEntry();
                entry.Init(123, (100, 200), null);

                Assert.Equal(100, entry.ContentWidth);
                Assert.Equal(1, entry.MinWidth);
                Assert.Equal(9999, entry.MaxWidth);
                Assert.False(entry.IsFixedWidth);
                Assert.False(entry.HorizontallyStretched);
                Assert.Equal(0, entry.HorizontalStretchFactor);

                Assert.Equal(200, entry.ContentHeight);
                Assert.Equal(1, entry.MinHeight);
                Assert.Equal(9999, entry.MaxHeight);
                Assert.False(entry.IsFixedHeight);
                Assert.False(entry.VerticallyStretched);
                Assert.Equal(0, entry.VerticalStretchFactor);
            }

            [Fact]
            public void FixSizedEntryProperlyInitialized()
            {
                var options = GUILayout.Width(100).Height(200);
                var entry = new LayoutEntry();
                entry.Init(123, Size.Zero, options);

                Assert.Equal(0, entry.ContentWidth);
                Assert.Equal(100, entry.MinWidth);
                Assert.Equal(100, entry.MaxWidth);
                Assert.True(entry.IsFixedWidth);
                Assert.False(entry.HorizontallyStretched);
                Assert.Equal(0, entry.HorizontalStretchFactor);

                Assert.Equal(0, entry.ContentHeight);
                Assert.Equal(200, entry.MinHeight);
                Assert.Equal(200, entry.MaxHeight);
                Assert.True(entry.IsFixedHeight);
                Assert.False(entry.VerticallyStretched);
                Assert.Equal(0, entry.VerticalStretchFactor);
            }

            [Fact]
            public void FlexSizedEntryProperlyInitialized()
            {
                var options = GUILayout.StretchWidth(1).StretchHeight(2);
                var entry = new LayoutEntry();
                entry.Init(123, Size.Zero, options);

                Assert.Equal(0, entry.ContentWidth);
                Assert.Equal(1, entry.MinWidth);
                Assert.Equal(9999, entry.MaxWidth);
                Assert.False(entry.IsFixedWidth);
                Assert.True(entry.HorizontallyStretched);
                Assert.Equal(1, entry.HorizontalStretchFactor);

                Assert.Equal(0, entry.ContentHeight);
                Assert.Equal(1, entry.MinHeight);
                Assert.Equal(9999, entry.MaxHeight);
                Assert.False(entry.IsFixedHeight);
                Assert.True(entry.VerticallyStretched);
                Assert.Equal(2, entry.VerticalStretchFactor);
            }
        }
    }
}
