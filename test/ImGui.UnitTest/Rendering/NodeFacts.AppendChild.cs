using System;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public partial class NodeFacts
    {
        public class AppendChild
        {
            [Fact]
            public void AppendALayoutEntryNodeToALayoutEntryNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node entryNode1 = new Node(1);
                    entryNode1.AttachLayoutEntry(new Size(100, 100));
                    Node entryNode2 = new Node(2);
                    entryNode2.AttachLayoutEntry(new Size(100, 100));

                    entryNode1.AppendChild(entryNode2);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AppendALayoutEntryNodeToALayoutGroupNode()
            {
                Node entryNode = new Node(1);
                entryNode.AttachLayoutEntry(new Size(100, 100));
                Node groupNode = new Node(2);
                groupNode.AttachLayoutGroup(true);

                groupNode.AppendChild(entryNode);

                Assert.Same(groupNode, entryNode.Parent);
                Assert.Contains(entryNode, groupNode);
            }

            [Fact]
            public void AppendALayoutGroupNodeToALayoutEntryNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node groupNode = new Node(1);
                    groupNode.AttachLayoutGroup(true);
                    Node entryNode = new Node(2);
                    entryNode.AttachLayoutEntry(new Size(100, 100));

                    entryNode.AppendChild(groupNode);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AppendALayoutGroupNodeToALayoutGroupNode()
            {
                Node groupNode1 = new Node(1);
                groupNode1.AttachLayoutGroup(true);
                Node groupNode2 = new Node(2);
                groupNode2.AttachLayoutGroup(true);

                groupNode1.AppendChild(groupNode2);

                Assert.Equal(groupNode1, groupNode2.Parent);
                Assert.Contains(groupNode2, groupNode1);
            }
        }
    }
}