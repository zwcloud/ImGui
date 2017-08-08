using System;
using ImGui.Common.Primitive;
using ImGui.Layout;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public partial class StackLayoutFacts
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

        public partial class TheGetRectMethod
        {
            [Fact]
            public void GetNormalRectAfterLayout()
            {
                var layout = new StackLayout(0, new Size(800,600));
                layout.Begin();
                layout.GetRect(1, new Size(100, 30));
                layout.Layout();
                var rect = layout.GetRect(1, new Size(100, 30));

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(100, rect.Width);
                Assert.Equal(30, rect.Height);
            }

            [Fact]
            public void CannotGetRectOfVerySmallSize()
            {
                var layout = new StackLayout(0, new Size(800, 600));
                Size zeroSize = Size.Zero;
                Size smallSize = new Size(0.5, 0.6);
                layout.Begin();

                Assert.Throws<ArgumentOutOfRangeException>("contentSize", () =>
                {
                    layout.GetRect(1, zeroSize);
                });

                Assert.Throws<ArgumentOutOfRangeException>("contentSize", () =>
                {
                    layout.GetRect(1, smallSize);
                });

            }

            [Fact]
            public void GetRectWithPadding()
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);
                var style = new GUIStyle();
                style.Padding = (1, 2, 3, 4);

                layout.Begin();
                layout.GetRect(1, size, style);
                layout.Layout();
                var rect = layout.GetRect(1, size, style);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(size.Width + style.PaddingHorizontal, rect.Width);
                Assert.Equal(size.Height + style.PaddingVertical, rect.Height);
            }
            
            [Fact]
            public void GetRectWithBorder()
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);
                var style = new GUIStyle();
                style.Border = (1, 2, 3, 4);

                layout.Begin();
                layout.GetRect(1, size, style);
                layout.Layout();
                var rect = layout.GetRect(1, size, style);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(size.Width + style.BorderHorizontal, rect.Width);
                Assert.Equal(size.Height + style.BorderVertical, rect.Height);
            }

            [Fact]
            public void GetRectWithPaddingAndBorder()
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);
                var style = new GUIStyle();
                style.Border = (1, 2, 3, 4);
                style.Padding = (5, 6, 7, 8);

                layout.Begin();
                layout.GetRect(1, size, style);
                layout.Layout();
                var rect = layout.GetRect(1, size, style);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(size.Width + style.BorderHorizontal + style.PaddingHorizontal, rect.Width);
                Assert.Equal(size.Height + style.BorderVertical + style.PaddingVertical, rect.Height);
            }

            [Fact]
            public void GetRectWithFixedWidthAndHeight()
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);
                const int fixedWidth = 100;
                const int fixedHeight = 200;
                var options = new[] {GUILayout.Width(fixedWidth), GUILayout.Height(fixedHeight) };

                layout.Begin();
                layout.GetRect(1, size, null, options);
                layout.Layout();
                var rect = layout.GetRect(1, size, null, options);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(fixedWidth, rect.Width);
                Assert.Equal(fixedHeight, rect.Height);
            }

            [Theory]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(true, true)]
            public void GetRectWithExpandWidth(bool expandWidth, bool expandHeight)
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);
                var options = new[] { GUILayout.ExpandWidth(expandWidth),
                    GUILayout.ExpandHeight(expandHeight) };

                layout.Begin();
                layout.GetRect(1, size, null, options);
                layout.Layout();
                var rect = layout.GetRect(1, size, null, options);

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(expandWidth ? 800 : size.Width, rect.Width);
                Assert.Equal(expandHeight ? 600 : size.Height, rect.Height);
            }

            [Fact]
            public void GetRectInsideGroup()
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);

                layout.Begin();
                layout.BeginLayoutGroup(1, true);
                    layout.GetRect(2, size);
                layout.EndLayoutGroup();
                layout.Layout();

                layout.Begin();
                layout.BeginLayoutGroup(1, true);
                    var rect = layout.GetRect(2, size);
                layout.EndLayoutGroup();

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(size.Width, rect.Width);
                Assert.Equal(size.Height, rect.Height);
            }

        }
    }
}
