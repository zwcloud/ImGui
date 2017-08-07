using System;
using ImGui.Common.Primitive;
using ImGui.Layout;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public class StackLayoutFacts
    {
        public class TheCtor
        {
            [Fact]
            public void CreatedStackLayoutIsProperlySet()
            {
                var layout = new StackLayout(0, new Size(100, 100));

                Assert.True(layout.TopGroup.IsVertical);
                Assert.True(layout.TopGroup.IsFixedWidth);
            }
        }

        public class TheGetRectMethod
        {
            [Fact]
            public void GetNormalRectAfterLayout()
            {
                var layout = new StackLayout(0, new Size(200,600));
                var rectFirst = layout.GetRect(1, new Size(100, 30), null, null);
                layout.Layout();
                var rect = layout.GetRect(1, new Size(100, 30), null, null);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(100, rect.Width);
                Assert.Equal(30, rect.Height);
            }

            [Fact]
            public void CannotGetRectOfVerySmallSize()
            {
                Size size = Size.Zero;
                var layout = new StackLayout(0, size);

                Assert.Throws<ArgumentOutOfRangeException>("contentSize", () =>
                {
                    layout.GetRect(1, size, null, null);
                });
            }

            [Fact]
            public void GetRectWithPadding()
            {
                var size = new Size(200, 300);
                var style = new GUIStyle();
                style.Padding = (1, 2, 3, 4);
                var layout = new StackLayout(0, size);

                layout.GetRect(1, size, style, null);
                layout.Layout();
                var rect = layout.GetRect(1, size, style, null);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(size.Width + style.PaddingHorizontal, rect.Width);
                Assert.Equal(size.Height + style.PaddingVertical, rect.Height);
            }
            
            [Fact]
            public void GetRectWithBorder()
            {
                var size = new Size(200, 300);
                var style = new GUIStyle();
                style.Border = (1, 2, 3, 4);
                var layout = new StackLayout(0, size);

                layout.GetRect(1, size, style, null);
                layout.Layout();
                var rect = layout.GetRect(1, size, style, null);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(size.Width + style.BorderHorizontal, rect.Width);
                Assert.Equal(size.Height + style.BorderVertical, rect.Height);
            }

            [Fact]
            public void GetRectWithPaddingAndBorder()
            {
                var size = new Size(200, 300);
                var style = new GUIStyle();
                style.Border = (1, 2, 3, 4);
                style.Padding = (5, 6, 7, 8);
                var layout = new StackLayout(0, size);

                layout.GetRect(1, size, style, null);
                layout.Layout();
                var rect = layout.GetRect(1, size, style, null);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(size.Width + style.BorderHorizontal + style.PaddingHorizontal, rect.Width);
                Assert.Equal(size.Height + style.BorderVertical + style.PaddingVertical, rect.Height);
            }

            [Fact]
            public void GetRectWithFixedWidthAndHeight()
            {
                var size = new Size(200, 300);
                var layout = new StackLayout(0, size);
                const int fixedWidth = 100;
                const int fixedHeight = 200;
                var options = new[] {GUILayout.Width(fixedWidth), GUILayout.Height(fixedHeight) };

                layout.GetRect(1, size, null, options);
                layout.Layout();
                var rect = layout.GetRect(1, size, null, options);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(fixedWidth, rect.Width);
                Assert.Equal(fixedHeight, rect.Height);
            }

        }
    }
}
