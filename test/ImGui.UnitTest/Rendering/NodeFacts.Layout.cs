using System;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public partial class NodeFacts
    {
        public class Layout : IClassFixture<NodeRenderingFixture>
        {
            internal static void CheckExpectedImage(Node node, string expectedImageFilePath)
            {
                int width = (int) node.Rect.Width;
                int height = (int) node.Rect.Height;
                Util.DrawNodeTreeToImage(out var imageRawBytes, node, width, height);
                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            //misc old unit test

            [Fact]
            public void ShowANodeWithTwoChildren()
            {
                Node a = new Node(1);
                a.Rect = new Rect(0, 0, 100, 200);
                a.AttachLayoutGroup(true);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(100, 100));

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(100, 50));

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                CheckExpectedImage(a, @"Rendering\images\NodeFacts.Layout.ShowANodeWithTwoChildren.png");
            }

            [Fact]
            public void ShowAHorizontalGroupOf3ItemsWithDifferentStretchFactors()
            {
                Node group = new Node(0); group.AttachLayoutGroup(false); group.RuleSet.ApplyOptions(GUILayout.Width(600));
                Node item1 = new Node(1); item1.AttachLayoutEntry(new Size(20, 10)); item1.RuleSet.ApplyOptions(GUILayout.StretchWidth(1).Height(50));
                Node item2 = new Node(2); item2.AttachLayoutEntry(new Size(20, 10)); item2.RuleSet.ApplyOptions(GUILayout.StretchWidth(2).Height(60));
                Node item3 = new Node(3); item3.AttachLayoutEntry(new Size(20, 10)); item3.RuleSet.ApplyOptions(GUILayout.StretchWidth(1).Height(30));
                group.AppendChild(item1);
                group.AppendChild(item2);
                group.AppendChild(item3);

                group.Layout();

                CheckExpectedImage(group, @"Rendering\images\NodeFacts.Layout.ShowAHorizontalGroupOf3ItemsWithDifferentStretchFactors.png");
            }

            [Fact]
            public void ShowAThreeLayerGroup()
            {
                // layer 1
                Node group1 = new Node(1); group1.AttachLayoutGroup(true); group1.RuleSet.ApplyOptions(GUILayout.Width(400).Height(400));

                // layer 2
                Node group2 = new Node(2); group2.AttachLayoutGroup(false); group2.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group3 = new Node(3); group3.AttachLayoutGroup(true); group3.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group4 = new Node(4); group4.AttachLayoutGroup(true); group4.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));

                // layer3
                Node group5 = new Node(5); group5.AttachLayoutGroup(true); group5.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group6 = new Node(6); group6.AttachLayoutGroup(true); group6.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group7 = new Node(7); group7.AttachLayoutGroup(true); group7.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group8 = new Node(8); group8.AttachLayoutGroup(true); group8.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group9 = new Node(9); group9.AttachLayoutGroup(true); group9.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group10 = new Node(10); group10.AttachLayoutGroup(true); group10.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group11 = new Node(11); group11.AttachLayoutGroup(true); group11.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group12 = new Node(12); group12.AttachLayoutGroup(true); group12.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group13 = new Node(13); group13.AttachLayoutGroup(true); group13.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));

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

                CheckExpectedImage(group1, @"Rendering\images\NodeFacts.Layout.ShowAThreeLayerGroup.png");
            }

            //default sized children

            [Fact]
            public void LayoutDefaultSizedNodeInDefaultSizedGroup()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true);
                Assert.True(a.RuleSet.IsDefaultWidth);
                Assert.True(a.RuleSet.IsDefaultHeight);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(100, 100));
                Assert.True(b.RuleSet.IsDefaultWidth);
                Assert.True(b.RuleSet.IsDefaultHeight);

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(100, 200));
                Assert.True(c.RuleSet.IsDefaultWidth);
                Assert.True(c.RuleSet.IsDefaultHeight);

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                CheckExpectedImage(a, @"Rendering\images\NodeFacts.Layout.LayoutDefaultSizedNodeInDefaultSizedGroup.png");
            }

            [Fact]
            public void LayoutDefaultSizedNodeInFixedSizedGroup()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true); a.RuleSet.ApplyOptions(GUILayout.Width(200).Height(200));
                Assert.True(a.RuleSet.IsFixedWidth);
                Assert.True(a.RuleSet.IsFixedHeight);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(100, 100));
                Assert.True(b.RuleSet.IsDefaultWidth);
                Assert.True(b.RuleSet.IsDefaultHeight);

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(100, 200));
                Assert.True(c.RuleSet.IsDefaultWidth);
                Assert.True(c.RuleSet.IsDefaultHeight);

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                CheckExpectedImage(a, @"Rendering\images\NodeFacts.Layout.LayoutDefaultSizedNodeInFixedSizedGroup.png");
            }

            [Fact]
            public void LayoutDefaultSizedNodeInStretchSizedGroup_ShouldThrowException()
            {
                Action action = () =>
                {
                    Node a = new Node(1);
                    a.AttachLayoutGroup(true); a.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(a.RuleSet.IsStretchedWidth);
                    Assert.True(a.RuleSet.IsStretchedHeight);

                    Node b = new Node(2);
                    b.AttachLayoutEntry(new Size(100, 100));
                    Assert.True(b.RuleSet.IsDefaultWidth);
                    Assert.True(b.RuleSet.IsDefaultHeight);

                    Node c = new Node(3);
                    c.AttachLayoutEntry(new Size(100, 200));
                    Assert.True(c.RuleSet.IsDefaultWidth);
                    Assert.True(c.RuleSet.IsDefaultHeight);

                    a.AppendChild(b);
                    a.AppendChild(c);

                    a.Layout();
                };

                Assert.Throws<LayoutException>(action);
            }

            //fixed sized children

            [Fact]
            public void LayoutFixedSizedNodeInDefaultSizedGroup()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true);
                Assert.True(a.RuleSet.IsDefaultWidth);
                Assert.True(a.RuleSet.IsDefaultHeight);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(10, 10));
                b.RuleSet.ApplyOptions(GUILayout.Width(100).Height(200));
                Assert.True(b.RuleSet.IsFixedWidth);
                Assert.True(b.RuleSet.IsFixedHeight);

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(10, 10));
                c.RuleSet.ApplyOptions(GUILayout.Width(200).Height(100));
                Assert.True(c.RuleSet.IsFixedWidth);
                Assert.True(c.RuleSet.IsFixedHeight);

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                CheckExpectedImage(a, @"Rendering\images\NodeFacts.Layout.LayoutFixedSizedNodeInDefaultSizedGroup.png");
            }

            [Fact]
            public void LayoutFixedSizedNodeInFixedSizedGroup()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true); a.RuleSet.ApplyOptions(GUILayout.Width(500).Height(500));
                Assert.True(a.RuleSet.IsFixedWidth);
                Assert.True(a.RuleSet.IsFixedHeight);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(10, 10));
                b.RuleSet.ApplyOptions(GUILayout.Width(100).Height(200));
                Assert.True(b.RuleSet.IsFixedWidth);
                Assert.True(b.RuleSet.IsFixedHeight);

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(10, 10));
                c.RuleSet.ApplyOptions(GUILayout.Width(200).Height(100));
                Assert.True(c.RuleSet.IsFixedWidth);
                Assert.True(c.RuleSet.IsFixedHeight);

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                CheckExpectedImage(a, @"Rendering\images\NodeFacts.Layout.LayoutFixedSizedNodeInFixedSizedGroup.png");
            }

            [Fact]
            public void LayoutFixedSizedNodeInStretchedSizedGroup_ShouldThrowException()
            {
                Action action = () =>
                {
                    Node a = new Node(1);
                    a.AttachLayoutGroup(true); a.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(a.RuleSet.IsStretchedWidth);
                    Assert.True(a.RuleSet.IsStretchedHeight);

                    Node b = new Node(2);
                    b.AttachLayoutEntry(new Size(10, 10)); b.RuleSet.ApplyOptions(GUILayout.Width(100).Height(200));
                    Assert.True(b.RuleSet.IsFixedWidth);
                    Assert.True(b.RuleSet.IsFixedHeight);

                    Node c = new Node(3);
                    c.AttachLayoutEntry(new Size(10, 10)); c.RuleSet.ApplyOptions(GUILayout.Width(200).Height(100));
                    Assert.True(c.RuleSet.IsFixedWidth);
                    Assert.True(c.RuleSet.IsFixedHeight);

                    a.AppendChild(b);
                    a.AppendChild(c);

                    a.Layout();
                };

                Assert.Throws<LayoutException>(action);
            }

            //stretched sized children

            [Fact]
            public void LayoutStretchedSizedNodeInDefaultSizedGroup_ShouldThrowException()
            {
                Action action = () =>
                {
                    Node a = new Node(1);
                    a.AttachLayoutGroup(true);
                    Assert.True(a.RuleSet.IsDefaultWidth);
                    Assert.True(a.RuleSet.IsDefaultHeight);

                    Node b = new Node(2);
                    b.AttachLayoutEntry(new Size(10, 10));
                    b.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(b.RuleSet.IsStretchedWidth);
                    Assert.True(b.RuleSet.IsStretchedHeight);

                    Node c = new Node(3);
                    c.AttachLayoutEntry(new Size(10, 10));
                    c.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(c.RuleSet.IsStretchedWidth);
                    Assert.True(c.RuleSet.IsStretchedHeight);

                    a.AppendChild(b);
                    a.AppendChild(c);

                    a.Layout();
                };

                Assert.Throws<LayoutException>(action);
                //this should throw LayoutException, because a default-sized group is not allowed to have stretched-sized children
            }

            [Fact]
            public void LayoutStretchedSizedNodeInFixedSizedGroup()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true); a.RuleSet.ApplyOptions(GUILayout.Width(500).Height(500));
                Assert.True(a.RuleSet.IsFixedWidth);
                Assert.True(a.RuleSet.IsFixedHeight);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(10, 10));
                b.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Assert.True(b.RuleSet.IsStretchedWidth);
                Assert.True(b.RuleSet.IsStretchedHeight);

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(10, 10));
                c.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Assert.True(c.RuleSet.IsStretchedWidth);
                Assert.True(c.RuleSet.IsStretchedHeight);

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                CheckExpectedImage(a, @"Rendering\images\NodeFacts.Layout.LayoutStretchedSizedNodeInFixedSizedGroup.png");
            }

            [Fact]
            public void LayoutStretchedSizedNodeInStretchedSizedGroup_ShouldThrowException()
            {
                Action action = () =>
                {
                    Node a = new Node(1);
                    a.AttachLayoutGroup(true); a.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(a.RuleSet.IsStretchedWidth);
                    Assert.True(a.RuleSet.IsStretchedHeight);

                    Node b = new Node(2);
                    b.AttachLayoutEntry(new Size(10, 10));
                    b.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(b.RuleSet.IsStretchedWidth);
                    Assert.True(b.RuleSet.IsStretchedHeight);

                    Node c = new Node(3);
                    c.AttachLayoutEntry(new Size(10, 10));
                    c.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(c.RuleSet.IsStretchedWidth);
                    Assert.True(c.RuleSet.IsStretchedHeight);

                    a.AppendChild(b);
                    a.AppendChild(c);

                    a.Layout();
                };

                Assert.Throws<LayoutException>(action);
                //This should throw LayoutException, because a stretched-sized group cannot have stretched-sized children
            }


            [Fact]
            public void LayoutNodeWithInactiveChildren()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true); a.RuleSet.ApplyOptions(GUILayout.Width(500).Height(500));
                Assert.True(a.RuleSet.IsFixedWidth);
                Assert.True(a.RuleSet.IsFixedHeight);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(10, 10));
                b.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Assert.True(b.RuleSet.IsStretchedWidth);
                Assert.True(b.RuleSet.IsStretchedHeight);
                b.ActiveSelf = false;

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(10, 10));
                c.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                Assert.True(c.RuleSet.IsStretchedWidth);
                Assert.True(c.RuleSet.IsStretchedHeight);

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                CheckExpectedImage(a, @"Rendering\images\NodeFacts.Layout.LayoutNodeWithInactiveChildren.png");
            }

            [Fact]
            public void LayoutDynamicSizedNode()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true); a.RuleSet.ApplyOptions(GUILayout.Width(500).Height(400));
                Assert.True(a.RuleSet.IsFixedWidth);
                Assert.True(a.RuleSet.IsFixedHeight);
                a.Layout();
                Assert.Equal(500, a.Rect.Width);
                Assert.Equal(400, a.Rect.Height);
                a.RuleSet.ApplyOptions(GUILayout.Width(200));
                a.Layout();
                Assert.Equal(200, a.Rect.Width);
                a.RuleSet.ApplyOptions(GUILayout.Height(200));
                a.Layout();
                Assert.Equal(200, a.Rect.Height);
            }
        }
    }
}
