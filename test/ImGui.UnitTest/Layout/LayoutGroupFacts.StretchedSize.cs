using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public partial class LayoutGroupFacts
    {
        public class StretchedSize : IClassFixture<NodeRenderingFixture>
        {
            private static void CheckExpectedImage(Node node, string expectedImageFilePath)
            {
                int width = (int)node.Rect.Width;
                int height = (int)node.Rect.Height;
                Util.DrawNodeTreeToImage(out var imageRawBytes, node, width, height);
                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void ShowAHorizontalGroupOf1Item()
            {
                Node group = new Node(1); group.AttachLayoutGroup(false); group.RuleSet.ApplyOptions(GUILayout.Width(600));
                Node item = new Node(2); item.AttachLayoutEntry(); item.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).Height(50));
                group.AppendChild(item);

                group.Layout();

                CheckExpectedImage(group, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowAHorizontalGroupOf1Item)}.png");
            }

            [Fact]
            public void ShowAHorizontalGroupOf3Items()
            {
                Node group = new Node(0); group.AttachLayoutGroup(false); group.RuleSet.ApplyOptions(GUILayout.Width(600));
                Node item1 = new Node(1); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).Height(50));
                Node item2 = new Node(2); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).Height(60));
                Node item3 = new Node(3); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).Height(30));
                group.AppendChild(item1);
                group.AppendChild(item2);
                group.AppendChild(item3);

                group.Layout();

                CheckExpectedImage(group, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowAHorizontalGroupOf3Items)}.png");
            }

            [Fact]
            public void ShowAHorizontalGroupOf3ItemsWithDifferentStretchFactors()
            {
                Node group = new Node(0); group.AttachLayoutGroup(false); group.RuleSet.ApplyOptions(GUILayout.Width(600));
                Node item1 = new Node(1); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.StretchWidth(1).Height(50));
                Node item2 = new Node(2); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.StretchWidth(2).Height(60));
                Node item3 = new Node(3); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.StretchWidth(1).Height(30));
                group.AppendChild(item1);
                group.AppendChild(item2);
                group.AppendChild(item3);

                group.Layout();

                CheckExpectedImage(group, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowAHorizontalGroupOf3ItemsWithDifferentStretchFactors)}.png");
            }

            [Fact]
            public void ShowAVerticalGroupOf1Item()
            {
                Node group = new Node(0); group.AttachLayoutGroup(true); group.RuleSet.ApplyOptions(GUILayout.Height(600));
                Node item = new Node(1); item.AttachLayoutEntry(); item.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true).Width(50));
                group.AppendChild(item);

                group.Layout();

                CheckExpectedImage(group, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowAVerticalGroupOf1Item)}.png");
            }

            [Fact]
            public void ShowAVerticalGroupOf3Items()
            {
                Node group = new Node(0); group.AttachLayoutGroup(true); group.RuleSet.ApplyOptions(GUILayout.Height(600));
                Node item1 = new Node(1); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true).Width(50));
                Node item2 = new Node(2); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true).Width(60));
                Node item3 = new Node(3); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true).Width(30));
                group.AppendChild(item1);
                group.AppendChild(item2);
                group.AppendChild(item3);

                group.Layout();

                CheckExpectedImage(group, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowAVerticalGroupOf3Items)}.png");
            }

            [Fact]
            public void ShowAVerticalGroupOf3ItemsWithDifferentStretchFactors()
            {
                Node group = new Node(0); group.AttachLayoutGroup(true); group.RuleSet.ApplyOptions(GUILayout.Height(600));
                Node item1 = new Node(1); item1.AttachLayoutEntry(); item1.RuleSet.ApplyOptions(GUILayout.StretchHeight(1).Width(50));
                Node item2 = new Node(2); item2.AttachLayoutEntry(); item2.RuleSet.ApplyOptions(GUILayout.StretchHeight(2).Width(60));
                Node item3 = new Node(3); item3.AttachLayoutEntry(); item3.RuleSet.ApplyOptions(GUILayout.StretchHeight(1).Width(30));
                group.AppendChild(item1);
                group.AppendChild(item2);
                group.AppendChild(item3);

                group.Layout();

                CheckExpectedImage(group, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowAVerticalGroupOf3ItemsWithDifferentStretchFactors)}.png");
            }

            [Fact]
            public void ShowATwoLayerGroup1()
            {
                Node group1 = new Node(0); group1.AttachLayoutGroup(true); group1.RuleSet.ApplyOptions(GUILayout.Width(400).Height(400));
                Node group2 = new Node(1); group2.AttachLayoutGroup(false); group2.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true));
                group1.AppendChild(group2);

                group1.Layout();

                CheckExpectedImage(group1, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowATwoLayerGroup1)}.png");
            }

            [Fact]
            public void ShowATwoLayerGroup2()
            {
                Node group1 = new Node(0); group1.AttachLayoutGroup(true); group1.RuleSet.ApplyOptions(GUILayout.Width(400).Height(400));
                Node group2 = new Node(1); group2.AttachLayoutGroup(false); group2.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true));
                group1.AppendChild(group2);

                group1.Layout();

                CheckExpectedImage(group1, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowATwoLayerGroup2)}.png");
            }

            [Fact]
            public void ShowATwoLayerGroup3()
            {
                Node group1 = new Node(0); group1.AttachLayoutGroup(true); group1.RuleSet.ApplyOptions(GUILayout.Width(400).Height(400));
                Node group2 = new Node(1); group2.AttachLayoutGroup(false); group2.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                group1.AppendChild(group2);

                group1.Layout();

                CheckExpectedImage(group1, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowATwoLayerGroup3)}.png");
            }

            [Fact]
            public void ShowATwoLayerGroup4()
            {
                Node group1 = new Node(0); group1.AttachLayoutGroup(true); group1.RuleSet.ApplyOptions(GUILayout.Width(400).Height(400));
                Node group2 = new Node(1); group2.AttachLayoutGroup(false); group2.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true));
                Node group3 = new Node(2); group3.AttachLayoutGroup(false); group3.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true));
                group1.AppendChild(group2);
                group1.AppendChild(group3);

                group1.Layout();

                CheckExpectedImage(group1, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowATwoLayerGroup4)}.png");
            }

            [Fact]
            public void ShowATwoLayerGroup5()
            {
                Node group1 = new Node(0); group1.AttachLayoutGroup(true); group1.RuleSet.ApplyOptions(GUILayout.Width(400).Height(400));
                Node group2 = new Node(1); group2.AttachLayoutGroup(false); group2.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true));
                Node group3 = new Node(2); group3.AttachLayoutGroup(false); group3.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true));
                group1.AppendChild(group2);
                group1.AppendChild(group3);

                group1.Layout();

                CheckExpectedImage(group1, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowATwoLayerGroup5)}.png");
            }

            [Fact]
            public void ShowATwoLayerGroup6()
            {
                Node group1 = new Node(0); group1.AttachLayoutGroup(true); group1.RuleSet.ApplyOptions(GUILayout.Width(400).Height(400));
                Node group2 = new Node(1); group2.AttachLayoutGroup(false); group2.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group3 = new Node(2); group3.AttachLayoutGroup(false); group3.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                group1.AppendChild(group2);
                group1.AppendChild(group3);

                group1.Layout();

                CheckExpectedImage(group1, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowATwoLayerGroup6)}.png");
            }


            [Fact]
            public void ShowAThreeLayerGroup()
            {
                // layer 1
                Node group1 = new Node(1); group1.AttachLayoutGroup(true); group1.RuleSet.ApplyOptions(GUILayout.Width(400).Height(400));

                // layer 2
                Node group2 = new Node(2); group2.AttachLayoutGroup(false); group2.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group3 = new Node(3); group3.AttachLayoutGroup(true);  group3.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group4 = new Node(4); group4.AttachLayoutGroup(false); group4.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));

                // layer3
                Node group5  = new Node(5 ); group5 .AttachLayoutGroup(false); group5 .RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group6  = new Node(6 ); group6 .AttachLayoutGroup(false); group6 .RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group7  = new Node(7 ); group7 .AttachLayoutGroup(false); group7 .RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group8  = new Node(8 ); group8 .AttachLayoutGroup(false); group8 .RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group9  = new Node(9 ); group9 .AttachLayoutGroup(false); group9 .RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group10 = new Node(10); group10.AttachLayoutGroup(false); group10.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group11 = new Node(11); group11.AttachLayoutGroup(false); group11.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group12 = new Node(12); group12.AttachLayoutGroup(false); group12.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group13 = new Node(13); group13.AttachLayoutGroup(false); group13.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));

                group1.AppendChild(group2);
                group1.AppendChild(group3);
                group1.AppendChild(group4);

                group2.AppendChild(group5);
                group2.AppendChild(group6);
                group2.AppendChild(group7);
                group3.AppendChild(group8);
                group3.AppendChild(group9);
                group3.AppendChild(group10);
                group4.AppendChild(group11);
                group4.AppendChild(group12);
                group4.AppendChild(group13);

                group1.Layout();

                CheckExpectedImage(group1, $@"Layout\images\{nameof(LayoutGroupFacts)}.{nameof(StretchedSize)}.{nameof(ShowAThreeLayerGroup)}.png");
            }
        }
    }
}