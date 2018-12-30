using System;
using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class RenderTree
    {
        public Node Root { get; }

        private Node currentContainer;
        public Node CurrentContainer
        {
            get
            {
                return this.currentContainer ?? this.Root;
            }
            set
            {
                this.currentContainer = value;
            }
        }

        public RenderTree(int rootId, Point position, Size size)
        {
            this.Root = new Node(rootId, "layout-root");
            this.Root.Rect = new Rect(position, size);
            this.Root.AttachLayoutGroup(true);
        }

        public Node GetNodeById(int id)
        {
            return this.Root.GetNodeById(id);
        }

        /// <summary>
        /// Performs the specified function on each node of the render tree.
        /// And return when the function return false.
        /// </summary>
        /// <param name="func"></param>
        public void Foreach(Func<Node, bool> func)
        {
            this.Root.Foreach(func);
        }
    }
}
