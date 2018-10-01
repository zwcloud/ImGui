using System;
using ImGui.Common.Primitive;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public partial class NodeFacts
    {
        public class TheLayoutMethod
        {
            //misc old unit test

            [Fact]
            public void ShowANodeWithTwoChildren() // Add rect; Add rect then remove rect
            {
                Node a = new Node(1);
                a.Rect = new Rect(0, 0, 300, 400);
                a.AttachLayoutGroup(true);

                Node b = new Node(2);
                b.Rect = new Rect(0, 0, 100, 100);
                b.AttachLayoutEntry(new Size(100, 100));

                Node c = new Node(3);
                c.Rect = new Rect(0, 0, 100, 200);
                c.AttachLayoutEntry(new Size(100, 200));

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                Util.DrawNode(a);
            }

            [Fact]
            public void ShowAHorizontalGroupOf3ItemsWithDifferentStretchFactors()
            {
                Node group = new Node(0); group.AttachLayoutGroup(false, GUILayout.Width(600));
                Node item1 = new Node(1); item1.AttachLayoutEntry(new Size(20, 10), GUILayout.StretchWidth(1).Height(50));
                Node item2 = new Node(2); item2.AttachLayoutEntry(new Size(20, 10), GUILayout.StretchWidth(2).Height(60));
                Node item3 = new Node(3); item3.AttachLayoutEntry(new Size(20, 10), GUILayout.StretchWidth(1).Height(30));
                group.AppendChild(item1);
                group.AppendChild(item2);
                group.AppendChild(item3);

                group.Layout();

                Util.DrawNode(group);
            }

            [Fact]
            public void ShowAThreeLayerGroup()
            {
                // layer 1
                Node group1 = new Node(1); group1.AttachLayoutGroup(true, GUILayout.Width(400).Height(400));

                // layer 2
                Node group2 = new Node(2); group2.AttachLayoutGroup(false, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group3 = new Node(3); group3.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group4 = new Node(4); group4.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));

                // layer3
                Node group5 =  new Node(5); group5.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group6 =  new Node(6); group6.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group7 =  new Node(7); group7.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group8 =  new Node(8); group8.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group9 =  new Node(9); group9.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group10 = new Node(10); group10.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group11 = new Node(11); group11.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group12 = new Node(12); group12.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group13 = new Node(13); group13.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));

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

                Util.DrawNode(group1);
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

                Util.DrawNode(a);
            }

            [Fact]
            public void LayoutDefaultSizedNodeInFixedSizedGroup()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true, GUILayout.Width(200).Height(200));
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

                Util.DrawNode(a);
            }

            [Fact]
            public void LayoutDefaultSizedNodeInStretchSizedGroup_ShouldThrowException()
            {
                Action action = () =>
                {
                    Node a = new Node(1);
                    a.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
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
                b.AttachLayoutEntry(new Size(10, 10), GUILayout.Width(100).Height(200));
                Assert.True(b.RuleSet.IsFixedWidth);
                Assert.True(b.RuleSet.IsFixedHeight);

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(10, 10), GUILayout.Width(200).Height(100));
                Assert.True(c.RuleSet.IsFixedWidth);
                Assert.True(c.RuleSet.IsFixedHeight);

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                Util.DrawNode(a);
            }

            [Fact]
            public void LayoutFixedSizedNodeInFixedSizedGroup()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true, GUILayout.Width(500).Height(500));
                Assert.True(a.RuleSet.IsFixedWidth);
                Assert.True(a.RuleSet.IsFixedHeight);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(10, 10), GUILayout.Width(100).Height(200));
                Assert.True(b.RuleSet.IsFixedWidth);
                Assert.True(b.RuleSet.IsFixedHeight);

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(10, 10), GUILayout.Width(200).Height(100));
                Assert.True(c.RuleSet.IsFixedWidth);
                Assert.True(c.RuleSet.IsFixedHeight);

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                Util.DrawNode(a);
            }

            [Fact]
            public void LayoutFixedSizedNodeInStretchedSizedGroup_ShouldThrowException()
            {
                Action action = () =>
                {
                    Node a = new Node(1);
                    a.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(a.RuleSet.IsStretchedWidth);
                    Assert.True(a.RuleSet.IsStretchedHeight);

                    Node b = new Node(2);
                    b.AttachLayoutEntry(new Size(10, 10), GUILayout.Width(100).Height(200));
                    Assert.True(b.RuleSet.IsFixedWidth);
                    Assert.True(b.RuleSet.IsFixedHeight);

                    Node c = new Node(3);
                    c.AttachLayoutEntry(new Size(10, 10), GUILayout.Width(200).Height(100));
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
                    b.AttachLayoutEntry(new Size(10, 10), GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(b.RuleSet.IsStretchedWidth);
                    Assert.True(b.RuleSet.IsStretchedHeight);

                    Node c = new Node(3);
                    c.AttachLayoutEntry(new Size(10, 10), GUILayout.ExpandWidth(true).ExpandHeight(true));
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
                a.AttachLayoutGroup(true, GUILayout.Width(500).Height(500));
                Assert.True(a.RuleSet.IsFixedWidth);
                Assert.True(a.RuleSet.IsFixedHeight);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(10, 10), GUILayout.ExpandWidth(true).ExpandHeight(true));
                Assert.True(b.RuleSet.IsStretchedWidth);
                Assert.True(b.RuleSet.IsStretchedHeight);

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(10, 10), GUILayout.ExpandWidth(true).ExpandHeight(true));
                Assert.True(c.RuleSet.IsStretchedWidth);
                Assert.True(c.RuleSet.IsStretchedHeight);

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                Util.DrawNode(a);
            }

            [Fact]
            public void LayoutStretchedSizedNodeInStretchedSizedGroup_ShouldThrowException()
            {
                Action action = () =>
                {
                    Node a = new Node(1);
                    a.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(a.RuleSet.IsStretchedWidth);
                    Assert.True(a.RuleSet.IsStretchedHeight);

                    Node b = new Node(2);
                    b.AttachLayoutEntry(new Size(10, 10), GUILayout.ExpandWidth(true).ExpandHeight(true));
                    Assert.True(b.RuleSet.IsStretchedWidth);
                    Assert.True(b.RuleSet.IsStretchedHeight);

                    Node c = new Node(3);
                    c.AttachLayoutEntry(new Size(10, 10), GUILayout.ExpandWidth(true).ExpandHeight(true));
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
                a.AttachLayoutGroup(true, GUILayout.Width(500).Height(500));
                Assert.True(a.RuleSet.IsFixedWidth);
                Assert.True(a.RuleSet.IsFixedHeight);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(10, 10), GUILayout.ExpandWidth(true).ExpandHeight(true));
                Assert.True(b.RuleSet.IsStretchedWidth);
                Assert.True(b.RuleSet.IsStretchedHeight);
                b.ActiveSelf = false;

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(10, 10), GUILayout.ExpandWidth(true).ExpandHeight(true));
                Assert.True(c.RuleSet.IsStretchedWidth);
                Assert.True(c.RuleSet.IsStretchedHeight);

                a.AppendChild(b);
                a.AppendChild(c);

                a.Layout();

                Util.DrawNode(a);
            }
        }
    }
}
