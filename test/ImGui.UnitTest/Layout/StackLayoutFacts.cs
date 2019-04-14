using ImGui.Layout;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public partial class NodeFacts
    {
        public partial class Layout
        {
            [Fact]
            public void CreatedNodeIsProperlySet()
            {
                var group = new Node(1);
                group.AttachLayoutGroup(true);
                var options = GUILayout.Width(200);
                group.RuleSet.ApplyOptions(options);

                Assert.True(group.IsVertical);
                Assert.True(group.RuleSet.IsFixedWidth);
            }

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

            [Fact]
            public void GetRectWithPadding()
            {
                var group = new Node(1);
                group.AttachLayoutGroup(true);
                group.RuleSet.ApplyOptions(GUILayout.Width(800).Height(600));

                var node = new Node(2);
                var contentSize = new Size(200, 300);
                node.AttachLayoutEntry(contentSize);
                var builder = new StyleRuleSetBuilder(node.RuleSet);
                builder.Padding((1, 2, 3, 4));
                group.AppendChild(node);

                group.Layout();

                var rect = node.Rect;

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(200 + 2 + 4, rect.Width);
                Assert.Equal(300 + 1 + 3, rect.Height);
            }

            [Fact]
            public void GetRectWithBorder()
            {
                var group = new Node(1);
                group.AttachLayoutGroup(true);
                group.RuleSet.ApplyOptions(GUILayout.Width(800).Height(600));

                var node = new Node(2);
                var contentSize = new Size(200, 300);
                node.AttachLayoutEntry(contentSize);
                var builder = new StyleRuleSetBuilder(node.RuleSet);
                builder.Border((1, 2, 3, 4));
                group.AppendChild(node);

                group.Layout();

                var rect = node.Rect;
                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(200 + 2 + 4, rect.Width);
                Assert.Equal(300 + 1 + 3, rect.Height);
            }

            [Fact]
            public void GetRectWithPaddingAndBorder()
            {
                var group = new Node(1);
                group.AttachLayoutGroup(true);
                group.RuleSet.ApplyOptions(GUILayout.Width(800).Height(600));

                var node = new Node(2);
                var contentSize = new Size(200, 300);
                node.AttachLayoutEntry(contentSize);
                var builder = new StyleRuleSetBuilder(node.RuleSet);
                builder.Border((1, 2, 3, 4))
                    .Padding((5, 6, 7, 8));
                group.AppendChild(node);

                group.Layout();
                var rect = node.Rect;

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(200 + 2 + 4 + 6 + 8, rect.Width);
                Assert.Equal(300 + 1 + 3 + 5 + 7, rect.Height);
            }

            [Fact]
            public void GetRectWithFixedWidthAndHeight()
            {
                var group = new Node(1);
                group.AttachLayoutGroup(true);
                group.RuleSet.ApplyOptions(GUILayout.Width(800).Height(600));

                const int fixedWidth = 100;
                const int fixedHeight = 200;
                var options = GUILayout.Width(fixedWidth).Height(fixedHeight);
                var node = new Node(2);
                var contentSize = new Size(200, 300);
                node.AttachLayoutEntry(contentSize);
                node.RuleSet.ApplyOptions(options);
                group.AppendChild(node);

                group.Layout();
                var rect = node.Rect;

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(100, rect.Width);
                Assert.Equal(200, rect.Height);
            }

            [Theory]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(true, true)]
            public void GetRectWithExpandWidth(bool expandWidth, bool expandHeight)
            {
                var group = new Node(1);
                group.AttachLayoutGroup(true);
                group.RuleSet.ApplyOptions(GUILayout.Width(800).Height(600));

                var options = GUILayout.ExpandWidth(expandWidth).ExpandHeight(expandHeight);
                var node = new Node(2);
                var contentSize = new Size(200, 300);
                node.AttachLayoutEntry(contentSize);
                node.RuleSet.ApplyOptions(options);
                group.AppendChild(node);

                group.Layout();
                var rect = node.Rect;

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(expandWidth ? 800 : 200, rect.Width);
                Assert.Equal(expandHeight ? 600 : 300, rect.Height);
            }

            [Fact]
            public void GetRectInsideGroup()
            {
                GUIStyle.Default.CellSpacing = (0, 0);//reset the cell-spacing of any group to 0

                var renderTree = new RenderTree(0, Point.Zero, new Size(800, 600));
                var size = new Size(200, 300);

                renderTree.BeginLayoutGroup(1, true);
                var node = new Node(2);
                node.AttachLayoutEntry(size);
                renderTree.CurrentContainer.AppendChild(node);
                renderTree.EndLayoutGroup();

                renderTree.Root.Layout();

                var rect = node.Rect;

                Assert.Equal(0, rect.X);
                Assert.Equal(0, rect.Y);
                Assert.Equal(200, rect.Width);
                Assert.Equal(300, rect.Height);
            }
        }
    }
}
