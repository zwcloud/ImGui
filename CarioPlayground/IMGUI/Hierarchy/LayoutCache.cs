using System.Collections.Generic;

namespace ImGui
{
    internal class LayoutCache
    {
        private Stack<LayoutGroup> groupStack = new Stack<LayoutGroup>();

        public LayoutGroup topGroup { get { return this.groupStack.Peek(); } }

        public void Push(LayoutGroup group)
        {
            this.groupStack.Push(group);
        }

        public void Pop()
        {
            this.groupStack.Pop();
        }

        public LayoutCache()
        {
        }
    }
}