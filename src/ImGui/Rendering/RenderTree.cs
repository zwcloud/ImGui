using System;

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
        public void Foreach(Func<Visual, bool> func)
        {
            this.Root.Foreach(func);
        }

        public void BeginLayoutGroup(int id, bool isVertical, LayoutOptions? options = null, string str_id = null)
        {
            var group = this.CurrentContainer.GetDirectNodeById(id);
            if (group == null)
            {
                group = new Node(id, str_id ?? "group");
                group.AttachLayoutGroup(isVertical);
            }
            this.CurrentContainer.AppendChild(group);
            group.RuleSet.ApplyOptions(options);
            group.ActiveSelf = true;
            this.CurrentContainer = group;
        }

        public void EndLayoutGroup()
        {
            if (this.CurrentContainer == this.Root)
            {
                throw new InvalidOperationException("BeginLayoutGroup/EndLayoutGroup mismatch.");
            }

            var group = this.CurrentContainer;
            group.OnGUI();
            this.CurrentContainer = (Node) this.CurrentContainer.Parent;
        }
    }

}
