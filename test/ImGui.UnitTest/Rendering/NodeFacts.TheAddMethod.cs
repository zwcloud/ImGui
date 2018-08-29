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
                Node plainNode1 = new Node(1);
                Node plainNode2 = new Node(2);

                plainNode1.Add(plainNode2);

                Assert.Equal(plainNode1, plainNode2.Parent);
                Assert.Contains(plainNode2, plainNode1.Children);
            }

            [Fact]
            public void AddAPlainNodeToALayoutEntryNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node plainNode = new Node(1);
                    Node entryNode = new Node(2);
                    entryNode.AttachLayoutEntry(new Size(100, 100));

                    entryNode.Add(plainNode);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AddAPlainNodeToALayoutGroupNode()
            {
                Node plainNode = new Node(1);
                Node groupNode = new Node(2);
                groupNode.AttachLayoutGroup(true);

                groupNode.Add(plainNode);

                Assert.Equal(groupNode, plainNode.Parent);
                Assert.Contains(plainNode, groupNode.Children);
            }


            [Fact]
            public void AddALayoutEntryNodeToAPlainNode()
            {
                Node entryNode = new Node(1);
                entryNode.AttachLayoutEntry(new Size(100, 100));
                Node plainNode = new Node(2);

                plainNode.Add(entryNode);

                Assert.Equal(plainNode, entryNode.Parent);
                Assert.Contains(entryNode, plainNode.Children);
            }

            [Fact]
            public void AddALayoutEntryNodeToALayoutEntryNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node entryNode1 = new Node(1);
                    entryNode1.AttachLayoutEntry(new Size(100, 100));
                    Node entryNode2 = new Node(2);
                    entryNode2.AttachLayoutEntry(new Size(100, 100));

                    entryNode1.Add(entryNode2);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AddALayoutEntryNodeToALayoutGroupNode()
            {
                Node entryNode = new Node(1);
                entryNode.AttachLayoutEntry(new Size(100, 100));
                Node groupNode = new Node(2);
                groupNode.AttachLayoutGroup(true);

                groupNode.Add(entryNode);

                Assert.Equal(groupNode, entryNode.Parent);
                Assert.Contains(entryNode, groupNode.Children);
            }

            [Fact]
            public void AddALayoutGroupNodeToAPlainNode()
            {
                Node groupNode = new Node(1);
                groupNode.AttachLayoutGroup(true);
                Node plainNode = new Node(2);

                plainNode.Add(groupNode);

                Assert.Equal(plainNode, groupNode.Parent);
                Assert.Contains(groupNode, plainNode.Children);
            }

            [Fact]
            public void AddALayoutGroupNodeToALayoutEntryNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node groupNode = new Node(1);
                    groupNode.AttachLayoutGroup(true);
                    Node entryNode = new Node(2);
                    entryNode.AttachLayoutEntry(new Size(100, 100));

                    entryNode.Add(groupNode);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AddALayoutGroupNodeToALayoutGroupNode()
            {
                Node groupNode1 = new Node(1);
                groupNode1.AttachLayoutGroup(true);
                Node groupNode2 = new Node(2);
                groupNode2.AttachLayoutGroup(true);

                groupNode1.Add(groupNode2);

                Assert.Equal(groupNode1, groupNode2.Parent);
                Assert.Contains(groupNode2, groupNode1.Children);
            }
        }
    }
}