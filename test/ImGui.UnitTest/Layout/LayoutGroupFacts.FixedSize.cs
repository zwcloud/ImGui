using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public partial class LayoutGroupFacts
    {
        public class FixedSize
        {
            [Fact]
            public void TheSizeOfAnEntryIsCorrectlyCalculated()
            {
                Node node = new Node(1); node.AttachLayoutEntry(); node.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));

                node.CalcWidth();
                node.CalcHeight();

                Assert.Equal(50, node.Rect.Width);
                Assert.Equal(50, node.Rect.Height);
            }

            [Fact]
            public void TheSizeOfAEmptyVerticalGroupIsCorrectlyCalculated()
            {
                Node group = new Node(1); group.AttachLayoutGroup(true); group.RuleSet.ApplyOptions(GUILayout.Width(100).Height(200));

                group.CalcWidth();
                group.CalcHeight();

                Assert.Equal(100, group.Rect.Width);
                Assert.Equal(200, group.Rect.Height);
            }

            [Fact]
            public void TheSizeOfAVerticalGroupThatContainsASingleEntryIsCorrectlyCalculated()
            {
                Node group = new Node(1); group.AttachLayoutGroup(true);
                Node item = new Node(2); item.AttachLayoutEntry(); item.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
                group.AppendChild(item);

                group.CalcWidth();
                group.CalcHeight();

                Assert.Equal(item.Rect.Width + group.RuleSet.PaddingHorizontal + group.RuleSet.BorderHorizontal, group.Rect.Width);
                Assert.Equal(item.Rect.Height + group.RuleSet.PaddingVertical + group.RuleSet.BorderVertical, group.Rect.Height);
            }

            [Fact]
            public void TheSizeOfAVerticalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
            {
                Node group = new Node(0); group.AttachLayoutGroup(true); group.RuleSet.ApplyOptions(GUILayout.Width(100).Height(200));
                var minWidth = group.RuleSet.BorderHorizontal + group.RuleSet.PaddingHorizontal;
                var minHeight = group.RuleSet.BorderVertical + group.RuleSet.PaddingVertical;

                Node item1 = new Node(1); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.Width(minWidth + 10).Height(minHeight + 20));
                Node item2 = new Node(2); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.Width(minWidth + 20).Height(minHeight + 30));
                Node item3 = new Node(3); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.Width(minWidth + 30).Height(minHeight + 40));
                Node item4 = new Node(4); item4.AttachLayoutEntry(); item4.RuleSet.ApplyOptions(GUILayout.Width(minWidth + 40).Height(minHeight + 50));
                Node item5 = new Node(5); item5.AttachLayoutEntry(); item5.RuleSet.ApplyOptions(GUILayout.Width(minWidth + 50).Height(minHeight + 60));

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

                Assert.Equal(100, group.Rect.Width);
                Assert.Equal(200, group.Rect.Height);
            }

            [Fact]
            public void TheSizeOfAHorizontalGroupThatContainsASingleEntryIsCorrectlyCalculated()
            {
                Node group = new Node(0); group.AttachLayoutGroup(false);
                Node item = new Node(2); item.AttachLayoutEntry(); item.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
                group.AppendChild(item);

                group.CalcWidth();
                group.CalcHeight();

                Assert.Equal(item.Rect.Width + group.RuleSet.PaddingHorizontal + group.RuleSet.BorderHorizontal, group.Rect.Width);
                Assert.Equal(item.Rect.Height + group.RuleSet.PaddingVertical + group.RuleSet.BorderVertical, group.Rect.Height);
            }

            [Fact]
            public void TheSizeOfAHorizontalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
            {
                Node group = new Node(0); group.AttachLayoutGroup(false); group.RuleSet.ApplyOptions(GUILayout.Width(100).Height(200));

                Node item1 = new Node(1); item1.AttachLayoutEntry();
                Node item2 = new Node(2); item2.AttachLayoutEntry();
                Node item3 = new Node(3); item3.AttachLayoutEntry();
                Node item4 = new Node(4); item4.AttachLayoutEntry();
                Node item5 = new Node(5); item5.AttachLayoutEntry();

                var items = new[]
                {
                    item1,
                    item2,
                    item3,
                    item4,
                    item5
                };

                for (var i = 0; i < items.Length; i++)
                {
                    var item = items[i];
                    item.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
                    item.ContentWidth += i * 5;
                    item.ContentHeight += i * 8;
                    group.AppendChild(item);
                }

                group.CalcWidth();
                group.CalcHeight();

                Assert.Equal(100, group.Rect.Width);
                Assert.Equal(200, group.Rect.Height);
            }
        }
    }
}