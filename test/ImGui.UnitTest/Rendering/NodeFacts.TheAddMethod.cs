using System;
using ImGui.Common.Primitive;
using ImGui.Rendering;
using Xunit;


namespace ImGui.UnitTest.Rendering
{
    public partial class NodeFacts
    {
        public class TheAddMethod
        {
            [Fact]
            public void AddAPlainNodeToAPlainNode()
            {
                Node node1 = new Node(1);
                Node node2 = new Node(2);

                node1.Add(node2);

                Assert.Equal(node1, node2.Parent);
                Assert.True(node1.Children.Contains(node2));
            }

            [Fact]
            public void AddAPlainNodeToALayoutEntryNode()
            {
                Node node1 = new Node(1);
                Node node2 = new Node(2);
                node2.AttachLayoutEntry(new Size(100,100));

                node1.Add(node2);

                Assert.Equal(node1, node2.Parent);
                Assert.True(node1.Children.Contains(node2));
            }

            [Fact]
            public void AddAPlainNodeToALayoutGroupNode()
            {
                Node node1 = new Node(1);
                Node node2 = new Node(2);
                node2.AttachLayoutGroup(true);

                node1.Add(node2);

                Assert.Equal(node1, node2.Parent);
                Assert.True(node1.Children.Contains(node2));
            }


            [Fact]
            public void AddALayoutEntryNodeToAPlainNode()
            {
                Node node1 = new Node(1);
                node1.AttachLayoutEntry(new Size(100, 100));
                Node node2 = new Node(2);

                node1.Add(node2);

                Assert.Equal(node1, node2.Parent);
                Assert.True(node1.Children.Contains(node2));
            }

            [Fact]
            public void AddALayoutEntryNodeToALayoutEntryNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node node1 = new Node(1);
                    node1.AttachLayoutEntry(new Size(100, 100));
                    Node node2 = new Node(2);
                    node2.AttachLayoutEntry(new Size(100, 100));

                    node1.Add(node2);

                    Assert.Equal(node1, node2.Parent);
                    Assert.True(node1.Children.Contains(node2));
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AddALayoutEntryNodeToALayoutGroupNode()
            {
                Node node1 = new Node(1);
                node1.AttachLayoutEntry(new Size(100, 100));
                Node node2 = new Node(2);
                node2.AttachLayoutGroup(true);

                node1.Add(node2);

                Assert.Equal(node1, node2.Parent);
                Assert.True(node1.Children.Contains(node2));
            }

            [Fact]
            public void AddALayoutGroupNodeToAPlainNode()
            {
                Node node1 = new Node(1);
                node1.AttachLayoutGroup(true);
                Node node2 = new Node(2);

                node1.Add(node2);

                Assert.Equal(node1, node2.Parent);
                Assert.True(node1.Children.Contains(node2));
            }

            [Fact]
            public void AddALayoutGroupNodeToALayoutEntryNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node node1 = new Node(1);
                    node1.AttachLayoutGroup(true);
                    Node node2 = new Node(2);
                    node2.AttachLayoutEntry(new Size(100, 100));

                    node1.Add(node2);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AddALayoutGroupNodeToALayoutGroupNode()
            {
                Node node1 = new Node(1);
                node1.AttachLayoutGroup(true);
                Node node2 = new Node(2);
                node2.AttachLayoutGroup(true);

                node1.Add(node2);

                Assert.Equal(node1, node2.Parent);
                Assert.True(node1.Children.Contains(node2));
            }
        }
    }
}