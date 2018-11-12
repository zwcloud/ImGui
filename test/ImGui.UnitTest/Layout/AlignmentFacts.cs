using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public class AlignmentFacts : NodeRenderingFixture
    {
        private void CheckExpectedImage(Node node, string expectedImageFilePath)
        {
            int width = (int)node.Rect.Width;
            int height = (int)node.Rect.Height;
            Util.DrawNodeTreeToImage(out var imageRawBytes, node, width, height);
            Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
        }

        [Fact]
        public void Show3HorizontalGroupOf1ItemWithDifferentAlignment()
        {
            Node group = new Node(0); group.AttachLayoutGroup(true);

            Node group1 = new Node(1); group1.AttachLayoutGroup(false); group1.RuleSet.ApplyOptions(GUILayout.Width(200).Height(200));
            group1.RuleSet.AlignmentVertical = Alignment.Start;

            Node group2 = new Node(2); group2.AttachLayoutGroup(false); group2.RuleSet.ApplyOptions(GUILayout.Width(200).Height(200));
            group2.RuleSet.AlignmentVertical = Alignment.Center;

            Node group3 = new Node(3); group3.AttachLayoutGroup(false); group3.RuleSet.ApplyOptions(GUILayout.Width(200).Height(200));
            group3.RuleSet.AlignmentVertical = Alignment.End;

            Node item1 = new Node(4); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item2 = new Node(5); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item3 = new Node(6); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));

            group1.AppendChild(item1);
            group2.AppendChild(item2);
            group3.AppendChild(item3);
            group.AppendChild(group1);
            group.AppendChild(group2);
            group.AppendChild(group3);

            group.Layout();

            CheckExpectedImage(group, $@"Layout\images\{nameof(AlignmentFacts)}.{nameof(Show3HorizontalGroupOf1ItemWithDifferentAlignment)}.png");
        }

        [Fact]
        public void Show3VerticalGroupOf1ItemWithDifferentAlignment()
        {
            Node group = new Node(0); group.AttachLayoutGroup(false);

            Node group1 = new Node(1); group1.AttachLayoutGroup(true); group1.RuleSet.ApplyOptions(GUILayout.Width(200).Height(200));
            group1.RuleSet.AlignmentHorizontal = Alignment.Start;
            Node group2 = new Node(2); group2.AttachLayoutGroup(true); group2.RuleSet.ApplyOptions(GUILayout.Width(200).Height(200));
            group2.RuleSet.AlignmentHorizontal = Alignment.Center;
            Node group3 = new Node(3); group3.AttachLayoutGroup(true); group3.RuleSet.ApplyOptions(GUILayout.Width(200).Height(200));
            group3.RuleSet.AlignmentHorizontal = Alignment.End;

            Node item1 = new Node(4); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item2 = new Node(5); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item3 = new Node(6); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));

            group1.AppendChild(item1);
            group2.AppendChild(item2);
            group3.AppendChild(item3);
            group.AppendChild(group1);
            group.AppendChild(group2);
            group.AppendChild(group3);

            group.Layout();

            CheckExpectedImage(group, $@"Layout\images\{nameof(AlignmentFacts)}.{nameof(Show3VerticalGroupOf1ItemWithDifferentAlignment)}.png");
        }


        [Fact]
        public void Show9GroupOf1ItemWithDifferentAlignment()
        {
            Node group = new Node(0); group.AttachLayoutGroup(true);

            Node groupRow1 = new Node(1); groupRow1.AttachLayoutGroup(false);
            Node groupRow2 = new Node(2); groupRow2.AttachLayoutGroup(false);
            Node groupRow3 = new Node(3); groupRow3.AttachLayoutGroup(false);

            Node group1 = new Node(4);  group1.AttachLayoutGroup(false);
            Node group2 = new Node(5);  group2.AttachLayoutGroup(false);
            Node group3 = new Node(6);  group3.AttachLayoutGroup(false);
            Node group4 = new Node(7);  group4.AttachLayoutGroup(false);
            Node group5 = new Node(8);  group5.AttachLayoutGroup(false);
            Node group6 = new Node(9);  group6.AttachLayoutGroup(false);
            Node group7 = new Node(10); group7.AttachLayoutGroup(false);
            Node group8 = new Node(11); group8.AttachLayoutGroup(false);
            Node group9 = new Node(12); group9.AttachLayoutGroup(false);

            Node item1 = new Node(13); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item2 = new Node(14); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item3 = new Node(15); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item4 = new Node(16); item4.AttachLayoutEntry(); item4.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item5 = new Node(17); item5.AttachLayoutEntry(); item5.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item6 = new Node(18); item6.AttachLayoutEntry(); item6.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item7 = new Node(19); item7.AttachLayoutEntry(); item7.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item8 = new Node(20); item8.AttachLayoutEntry(); item8.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item9 = new Node(21); item9.AttachLayoutEntry(); item9.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));

            group1.RuleSet.AlignmentHorizontal = Alignment.Start;
            group1.RuleSet.AlignmentVertical = Alignment.Start;
            group2.RuleSet.AlignmentHorizontal = Alignment.Center;
            group2.RuleSet.AlignmentVertical = Alignment.Start;
            group3.RuleSet.AlignmentHorizontal = Alignment.End;
            group3.RuleSet.AlignmentVertical = Alignment.Start;

            group4.RuleSet.AlignmentHorizontal = Alignment.Start;
            group4.RuleSet.AlignmentVertical = Alignment.Center;
            group5.RuleSet.AlignmentHorizontal = Alignment.Center;
            group5.RuleSet.AlignmentVertical = Alignment.Center;
            group6.RuleSet.AlignmentHorizontal = Alignment.End;
            group6.RuleSet.AlignmentVertical = Alignment.Center;

            group7.RuleSet.AlignmentHorizontal = Alignment.Start;
            group7.RuleSet.AlignmentVertical = Alignment.End;
            group8.RuleSet.AlignmentHorizontal = Alignment.Center;
            group8.RuleSet.AlignmentVertical = Alignment.End;
            group9.RuleSet.AlignmentHorizontal = Alignment.End;
            group9.RuleSet.AlignmentVertical = Alignment.End;

            group1.AppendChild(item1);
            group2.AppendChild(item2);
            group3.AppendChild(item3);
            group4.AppendChild(item4);
            group5.AppendChild(item5);
            group6.AppendChild(item6);
            group7.AppendChild(item7);
            group8.AppendChild(item8);
            group9.AppendChild(item9);

            groupRow1.AppendChild(group1);
            groupRow1.AppendChild(group2);
            groupRow1.AppendChild(group3);
            groupRow2.AppendChild(group4);
            groupRow2.AppendChild(group5);
            groupRow2.AppendChild(group6);
            groupRow3.AppendChild(group7);
            groupRow3.AppendChild(group8);
            groupRow3.AppendChild(group9);

            group.AppendChild(groupRow1);
            group.AppendChild(groupRow2);
            group.AppendChild(groupRow3);

            group.Layout();

            CheckExpectedImage(group, $@"Layout\images\{nameof(AlignmentFacts)}.{nameof(Show9GroupOf1ItemWithDifferentAlignment)}.png");
        }

        [Fact]
        public void Show5HorizontalGroupWithDifferentAlignment()
        {
            Node group = new Node(0); group.AttachLayoutGroup(true);

            Node group1 = new Node(1); group1.AttachLayoutGroup(false); group1.RuleSet.ApplyOptions(GUILayout.Width(600).Height(150));
            Node group2 = new Node(2); group2.AttachLayoutGroup(false); group2.RuleSet.ApplyOptions(GUILayout.Width(600).Height(150));
            Node group3 = new Node(3); group3.AttachLayoutGroup(false); group3.RuleSet.ApplyOptions(GUILayout.Width(600).Height(150));
            Node group4 = new Node(4); group4.AttachLayoutGroup(false); group4.RuleSet.ApplyOptions(GUILayout.Width(600).Height(150));
            Node group5 = new Node(5); group5.AttachLayoutGroup(false); group5.RuleSet.ApplyOptions(GUILayout.Width(600).Height(150));

            Node item1  = new Node(6 ); item1 .AttachLayoutEntry(); item1 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item2  = new Node(2 ); item2 .AttachLayoutEntry(); item2 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item3  = new Node(3 ); item3 .AttachLayoutEntry(); item3 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item4  = new Node(4 ); item4 .AttachLayoutEntry(); item4 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item5  = new Node(5 ); item5 .AttachLayoutEntry(); item5 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item6  = new Node(6 ); item6 .AttachLayoutEntry(); item6 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item7  = new Node(7 ); item7 .AttachLayoutEntry(); item7 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item8  = new Node(8 ); item8 .AttachLayoutEntry(); item8 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item9  = new Node(9 ); item9 .AttachLayoutEntry(); item9 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item10 = new Node(10); item10.AttachLayoutEntry(); item10.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item11 = new Node(11); item11.AttachLayoutEntry(); item11.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item12 = new Node(12); item12.AttachLayoutEntry(); item12.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item13 = new Node(13); item13.AttachLayoutEntry(); item13.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item14 = new Node(14); item14.AttachLayoutEntry(); item14.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item15 = new Node(15); item15.AttachLayoutEntry(); item15.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));

            group1.RuleSet.AlignmentHorizontal = Alignment.Start;
            group2.RuleSet.AlignmentHorizontal = Alignment.Center;
            group3.RuleSet.AlignmentHorizontal = Alignment.End;
            group4.RuleSet.AlignmentHorizontal = Alignment.SpaceAround;
            group5.RuleSet.AlignmentHorizontal = Alignment.SpaceBetween;

            group1.AppendChild(item1);
            group1.AppendChild(item2);
            group1.AppendChild(item3);
            group2.AppendChild(item4);
            group2.AppendChild(item5);
            group2.AppendChild(item6);
            group3.AppendChild(item7);
            group3.AppendChild(item8);
            group3.AppendChild(item9);
            group4.AppendChild(item10);
            group4.AppendChild(item11);
            group4.AppendChild(item12);
            group5.AppendChild(item13);
            group5.AppendChild(item14);
            group5.AppendChild(item15);

            group.AppendChild(group1);
            group.AppendChild(group2);
            group.AppendChild(group3);
            group.AppendChild(group4);
            group.AppendChild(group5);

            group.Layout();

            CheckExpectedImage(group, $@"Layout\images\{nameof(AlignmentFacts)}.{nameof(Show5HorizontalGroupWithDifferentAlignment)}.png");
        }

        [Fact]
        public void Show5VerticalGroupWithDifferentAlignment()
        {
            Node group = new Node(0); group.AttachLayoutGroup(false);

            Node group1 = new Node(1); group1.AttachLayoutGroup(true); group1.RuleSet.ApplyOptions(GUILayout.Width(150).Height(600));
            Node group2 = new Node(2); group2.AttachLayoutGroup(true); group2.RuleSet.ApplyOptions(GUILayout.Width(150).Height(600));
            Node group3 = new Node(3); group3.AttachLayoutGroup(true); group3.RuleSet.ApplyOptions(GUILayout.Width(150).Height(600));
            Node group4 = new Node(4); group4.AttachLayoutGroup(true); group4.RuleSet.ApplyOptions(GUILayout.Width(150).Height(600));
            Node group5 = new Node(5); group5.AttachLayoutGroup(true); group5.RuleSet.ApplyOptions(GUILayout.Width(150).Height(600));

            Node item1  = new Node(1 ); item1 .AttachLayoutEntry(); item1 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item2  = new Node(2 ); item2 .AttachLayoutEntry(); item2 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item3  = new Node(3 ); item3 .AttachLayoutEntry(); item3 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item4  = new Node(4 ); item4 .AttachLayoutEntry(); item4 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item5  = new Node(5 ); item5 .AttachLayoutEntry(); item5 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item6  = new Node(6 ); item6 .AttachLayoutEntry(); item6 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item7  = new Node(7 ); item7 .AttachLayoutEntry(); item7 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item8  = new Node(8 ); item8 .AttachLayoutEntry(); item8 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item9  = new Node(9 ); item9 .AttachLayoutEntry(); item9 .RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item10 = new Node(10); item10.AttachLayoutEntry(); item10.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item11 = new Node(11); item11.AttachLayoutEntry(); item11.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item12 = new Node(12); item12.AttachLayoutEntry(); item12.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item13 = new Node(13); item13.AttachLayoutEntry(); item13.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item14 = new Node(14); item14.AttachLayoutEntry(); item14.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));
            Node item15 = new Node(15); item15.AttachLayoutEntry(); item15.RuleSet.ApplyOptions(GUILayout.Width(50).Height(50));

            group1.RuleSet.AlignmentVertical = Alignment.Start;
            group2.RuleSet.AlignmentVertical = Alignment.Center;
            group3.RuleSet.AlignmentVertical = Alignment.End;
            group4.RuleSet.AlignmentVertical = Alignment.SpaceAround;
            group5.RuleSet.AlignmentVertical = Alignment.SpaceBetween;

            group1.AppendChild(item1);
            group1.AppendChild(item2);
            group1.AppendChild(item3);
            group2.AppendChild(item4);
            group2.AppendChild(item5);
            group2.AppendChild(item6);
            group3.AppendChild(item7);
            group3.AppendChild(item8);
            group3.AppendChild(item9);
            group4.AppendChild(item10);
            group4.AppendChild(item11);
            group4.AppendChild(item12);
            group5.AppendChild(item13);
            group5.AppendChild(item14);
            group5.AppendChild(item15);

            group.AppendChild(group1);
            group.AppendChild(group2);
            group.AppendChild(group3);
            group.AppendChild(group4);
            group.AppendChild(group5);

            group.Layout();

            CheckExpectedImage(group, $@"Layout\images\{nameof(AlignmentFacts)}.{nameof(Show5VerticalGroupWithDifferentAlignment)}.png");
        }
    }
}