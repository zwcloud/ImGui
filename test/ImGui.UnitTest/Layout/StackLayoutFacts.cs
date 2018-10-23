using System;
using ImGui.Common.Primitive;
using ImGui.Layout;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public partial class StackLayoutFacts
    {
        public class Constructor
        {
            [Fact]
            public void CreatedStackLayoutIsProperlySet()
            {
                var group = new Node(1);
                group.AttachLayoutGroup(true);
                var options = GUILayout.Width(200);
                group.RuleSet.ApplyOptions(options);

                Assert.True(group.LayoutGroup.IsVertical);
                Assert.True(group.RuleSet.IsFixedWidth);
            }
        }

        public partial class TheGetRectMethod
        {
            [Fact]
            public void GetNormalRectAfterLayout()
            {
                var group = new Node(1);
                group.AttachLayoutGroup(true);
                group.RuleSet.ApplyOptions(GUILayout.Width(800).Height(600));
                var node = new Node(2);
                node.AttachLayoutEntry(new Size(100, 30));
                group.AppendChild(node);

                group.Layout();

                var rect = node.Rect;
                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(100, rect.Width);
                Assert.Equal(30, rect.Height);
            }

            [Fact]
            public void GetRectOfVerySmallSize()
            {
                Size zeroSize = Size.Zero;
                Size smallSize = new Size(0.5, 0.6);

                var group = new Node(1);
                group.AttachLayoutGroup(true);
                group.RuleSet.ApplyOptions(GUILayout.Width(800).Height(600));

                var node1 = new Node(2);
                node1.AttachLayoutEntry(zeroSize);
                group.AppendChild(node1);

                var node2 = new Node(2);
                node2.AttachLayoutEntry(smallSize);
                group.AppendChild(node2);
                
                group.Layout();

                var rect1 = node1.Rect;
                Assert.Equal(0, rect1.Width);
                Assert.Equal(0, rect1.Height);
                var rect2 = node2.Rect;
                Assert.Equal(0.5, rect2.Width);
                Assert.Equal(0.6, rect2.Height);
            }

#if false
            [Fact]
            public void GetRectWithPadding()
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);
                var style = new GUIStyle();
                style.Padding = (1, 2, 3, 4);
                //FIXME per-entry style modification

                layout.Begin();
                layout.GetRect(1, size);
                layout.Layout();
                var rect = layout.GetRect(1, size);

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
                //FIXME per-entry style modification

                layout.Begin();
                layout.GetRect(1, size);
                layout.Layout();
                var rect = layout.GetRect(1, size);

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
                //FIXME per-entry style modification

                layout.Begin();
                layout.GetRect(1, size);
                layout.Layout();
                var rect = layout.GetRect(1, size);

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
                var options = GUILayout.Width(fixedWidth).Height(fixedHeight);

                layout.Begin();
                layout.GetRect(1, size, options);
                layout.Layout();
                var rect = layout.GetRect(1, size, options);

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
                var options = GUILayout.ExpandWidth(expandWidth).ExpandHeight(expandHeight);

                layout.Begin();
                layout.GetRect(1, size, options);
                layout.Layout();
                var rect = layout.GetRect(1, size, options);

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
            #endif
        }
    }
}
