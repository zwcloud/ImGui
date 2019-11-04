using System;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public partial class LayoutGroupFacts
    {
        public class DefaultSize
        {
            [Fact]
            public void TheSizeOfAnEntryIsCorrectlyCalculated()
            {
                Node node = new Node(1);
                node.AttachLayoutEntry(new Size(50, 50));

                node.CalcWidth();
                node.CalcHeight();

                Assert.Equal(node.ContentWidth + node.RuleSet.PaddingLeft + node.RuleSet.PaddingRight + node.RuleSet.BorderLeft + node.RuleSet.BorderRight, node.Rect.Width);
                Assert.Equal(node.ContentHeight + node.RuleSet.PaddingTop + node.RuleSet.PaddingBottom + node.RuleSet.BorderTop + node.RuleSet.BorderBottom, node.Rect.Height);
            }

            [Fact]
            public void TheSizeOfAEmptyVerticalGroupIsCorrectlyCalculated()
            {
                Node group = new Node(1); group.AttachLayoutGroup(true);

                group.CalcWidth();
                group.CalcHeight();

                Assert.Equal(group.RuleSet.PaddingHorizontal + group.RuleSet.BorderHorizontal, group.Rect.Width);
                Assert.Equal(group.RuleSet.PaddingVertical + group.RuleSet.BorderVertical, group.Rect.Height);
            }

            [Fact]
            public void TheSizeOfAVerticalGroupThatContainsASingleEntryIsCorrectlyCalculated()
            {
                Node group = new Node(1); group.AttachLayoutGroup(true);
                Node item = new Node(2); item.AttachLayoutEntry(new Size(50, 50));
                group.AppendChild(item);

                group.CalcWidth();
                group.CalcHeight();

                Assert.Equal(item.Rect.Width + group.RuleSet.PaddingHorizontal + group.RuleSet.BorderHorizontal, group.Rect.Width);
                Assert.Equal(item.Rect.Height + group.RuleSet.PaddingVertical + group.RuleSet.BorderVertical, group.Rect.Height);
            }

            [Fact]
            public void TheSizeOfAVerticalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
            {
                Node group = new Node(0); group.AttachLayoutGroup(true);

                Node item1 = new Node(1); item1.AttachLayoutEntry(new Size(10, 20));
                Node item2 = new Node(2); item2.AttachLayoutEntry(new Size(20, 30));
                Node item3 = new Node(3); item3.AttachLayoutEntry(new Size(30, 40));
                Node item4 = new Node(4); item4.AttachLayoutEntry(new Size(40, 50));
                Node item5 = new Node(5); item5.AttachLayoutEntry(new Size(50, 60));

                var items = new[]
                {
                    item1,
                    item2,
                    item3,
                    item4,
                    item5
                };

                foreach (var item in items)
                {
                    group.AppendChild(item);
                }

                group.CalcWidth();
                group.CalcHeight();

                var expectedWidth = 0d;
                var expectedHeight = 0d;
                foreach (var item in items)
                {
                    expectedWidth = Math.Max(expectedWidth, item.Rect.Width);
                    expectedHeight += item.Rect.Height + group.RuleSet.CellSpacingVertical;
                }
                expectedHeight -= group.RuleSet.CellSpacingVertical;
                expectedWidth += group.RuleSet.PaddingHorizontal + group.RuleSet.BorderHorizontal;
                expectedHeight += group.RuleSet.PaddingVertical + group.RuleSet.BorderVertical;

                Assert.Equal(expectedWidth, group.Rect.Width);
                Assert.Equal(expectedHeight, group.Rect.Height);
            }

            [Fact]
            public void TheSizeOfAHorizontalGroupThatContainsASingleEntryIsCorrectlyCalculated()
            {
                Node group = new Node(0); group.AttachLayoutGroup(false);
                Node item = new Node(2); item.AttachLayoutEntry(new Size(50, 50));
                group.AppendChild(item);

                group.CalcWidth();
                group.CalcHeight();

                Assert.Equal(item.Rect.Width + group.RuleSet.PaddingHorizontal + group.RuleSet.BorderHorizontal, group.Rect.Width);
                Assert.Equal(item.Rect.Height + group.RuleSet.PaddingVertical + group.RuleSet.BorderVertical, group.Rect.Height);
            }

            [Fact]
            public void TheLocationIsCorrect()
            {
                Node group = new Node(0); group.AttachLayoutGroup(true);
                Node item1 = new Node(1); item1.AttachLayoutEntry(new Size(50, 50));
                item1.RuleSet.Border = (10, 10, 10, 10);
                item1.RuleSet.Padding = (20, 20, 20, 20);
                Node item2 = new Node(2); item2.AttachLayoutEntry(new Size(50, 50));
                group.AppendChild(item1);
                group.AppendChild(item2);

                group.CalcWidth();
                group.CalcHeight();
                group.SetX(0);
                group.SetY(0);

                Assert.Equal(0, group.X);
                Assert.Equal(0, group.Y);
                Assert.Equal(group.X + group.RuleSet.BorderLeft + group.RuleSet.PaddingLeft, item1.Rect.X);
                Assert.Equal(group.X + group.RuleSet.BorderLeft + group.RuleSet.PaddingLeft, item2.Rect.X);
                Assert.Equal(group.Y + group.RuleSet.BorderTop + group.RuleSet.PaddingTop, item1.Rect.Y);
                Assert.Equal(item1.ContentHeight + item1.BorderVertical + item1.PaddingVertical, item1.Rect.Height);
                Assert.Equal(50 + 10*2 + 20*2, item1.Rect.Height);
                Assert.Equal(group.X + group.RuleSet.BorderTop + group.RuleSet.PaddingTop + item1.Rect.Height, item2.Rect.Y);
            }

            [Fact]
            public void TheSizeOfAHorizontalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
            {
                Node group = new Node(0); group.AttachLayoutGroup(false);

                Node item1 = new Node(1); item1.AttachLayoutEntry(new Size(10, 20));
                Node item2 = new Node(2); item2.AttachLayoutEntry(new Size(20, 30));
                Node item3 = new Node(3); item3.AttachLayoutEntry(new Size(30, 40));
                Node item4 = new Node(4); item4.AttachLayoutEntry(new Size(40, 50));
                Node item5 = new Node(5); item5.AttachLayoutEntry(new Size(50, 60));

                var items = new[]
                {
                    item1,
                    item2,
                    item3,
                    item4,
                    item5
                };

                foreach (var item in items)
                {
                    group.AppendChild(item);
                }

                group.CalcWidth();
                group.CalcHeight();

                var expectedWidth = 0d;
                var expectedHeight = 0d;
                foreach (var item in items)
                {
                    expectedWidth += item.Rect.Width + group.RuleSet.CellSpacingHorizontal;
                    expectedHeight = Math.Max(expectedHeight, item.Rect.Height);
                }
                expectedWidth -= group.RuleSet.CellSpacingHorizontal;
                expectedWidth += group.RuleSet.PaddingHorizontal + group.RuleSet.BorderHorizontal;
                expectedHeight += group.RuleSet.PaddingVertical + group.RuleSet.BorderHorizontal;
                Assert.Equal(expectedWidth, group.Rect.Width);
                Assert.Equal(expectedHeight, group.Rect.Height);
            }
        }
    }
}