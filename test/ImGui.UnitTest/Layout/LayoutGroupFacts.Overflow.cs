using ImGui.Common.Primitive;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public partial class LayoutGroupFacts
    {
        public class Overflow : IClassFixture<NodeRenderingFixture>
        {
            private static void CheckExpectedImage(Node node, string expectedImageFilePath)
            {
                int width = (int)node.Rect.Width;
                int height = (int)node.Rect.Height;
                Util.DrawNodeTreeToImage(out var imageRawBytes, node, width, height);
                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void HorizontallyOverflow1()
            {
                Node group = new Node(0);
                group.AttachLayoutGroup(false);
                group.RuleSet.ApplyOptions(GUILayout.Width(100));

                Node item1 = new Node(1); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.Width(50).Height(20));
                Node item2 = new Node(2); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.Width(60).Height(20));
                Node item3 = new Node(3); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.Width(30).Height(20));
                group.AppendChild(item1);
                group.AppendChild(item2);
                group.AppendChild(item3);

                group.Layout();

                CheckExpectedImage(group, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(Overflow)}.{nameof(HorizontallyOverflow1)}.png");
            }

            [Fact]
            public void HorizontallyOverflow2()
            {
                Node group = new Node(0);
                group.AttachLayoutGroup(false);
                group.RuleSet.ApplyOptions(GUILayout.Width(100));
                group.RuleSet.AlignmentHorizontal = Alignment.Center;

                Node item1 = new Node(1); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.Width(50).Height(20));
                Node item2 = new Node(2); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.Width(60).Height(20));
                Node item3 = new Node(3); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.Width(30).Height(20));
                group.AppendChild(item1);
                group.AppendChild(item2);
                group.AppendChild(item3);

                group.Layout();

                CheckExpectedImage(group, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(Overflow)}.{nameof(HorizontallyOverflow2)}.png");
            }

            [Fact]
            public void HorizontallyOverflow3()
            {
                Node group = new Node(0);
                group.AttachLayoutGroup(false);
                group.RuleSet.ApplyOptions(GUILayout.Width(100));
                group.RuleSet.AlignmentHorizontal = Alignment.SpaceAround;

                Node item1 = new Node(1); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.Width(50).Height(20));
                Node item2 = new Node(2); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.Width(60).Height(20));
                Node item3 = new Node(3); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.Width(30).Height(20));
                group.AppendChild(item1);
                group.AppendChild(item2);
                group.AppendChild(item3);

                group.Layout();

                CheckExpectedImage(group, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(Overflow)}.{nameof(HorizontallyOverflow3)}.png");
            }

            [Fact]
            public void HorizontallyOverflow4()
            {
                Node group = new Node(0);
                group.AttachLayoutGroup(false);
                group.RuleSet.ApplyOptions(GUILayout.Width(100));
                group.RuleSet.AlignmentHorizontal = Alignment.SpaceAround;

                Node item1 = new Node(1); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.Width(50).Height(20));
                Node item2 = new Node(2); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).Height(20));
                Node item3 = new Node(3); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.Width(60).Height(20));
                group.AppendChild(item1);
                group.AppendChild(item2);
                group.AppendChild(item3);

                group.Layout();

                item1.RuleSet.BorderColor = (Color.Red, Color.Red, Color.Red, Color.Red);
                item2.RuleSet.BorderColor = (Color.Green, Color.Green, Color.Green, Color.Green);//item2 should be hidden: no green rect displayed
                item3.RuleSet.BorderColor = (Color.Blue, Color.Blue, Color.Blue, Color.Blue);

                CheckExpectedImage(group, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(Overflow)}.{nameof(HorizontallyOverflow4)}.png");
            }

        }
    }
}