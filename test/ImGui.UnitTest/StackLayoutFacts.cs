using System;
using Xunit;
using ImGui;
using ImGui.Common.Primitive;
using ImGui.Layout;

namespace ImGui.UnitTest
{
    public class StackLayoutFacts
    {
        public class TheGetRectMethod
        {
            [Fact]
            public void GetNormalRectAfterLayout()
            {
                var layout = new StackLayout(0, new Size(200,600));
                var rectFirst = layout.GetRect(1, new Size(100, 30), null, null);
                layout.Layout(new Size(200, 600));
                var rect = layout.GetRect(1, new Size(100, 30), null, null);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(100, rect.Width);
                Assert.Equal(30, rect.Height);
            }
        }

        public class TheFindLayoutEntryMethod
        {
        }
    }
}
