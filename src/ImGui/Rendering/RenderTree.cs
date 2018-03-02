using System;
using System.Collections.Generic;
using ImGui.Layout;
using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class RenderTree
    {
        List<Node> Children = new List<Node>();

        public RenderTree(int rootId, Size size)
        {
            var rootGroup = CreateRootGroup(rootId, size);
            this.stack.Push(rootGroup);
        }

        #region Layout

        private readonly Stack<LayoutGroup> stack = new Stack<LayoutGroup>();


        private LayoutGroup CreateRootGroup(int rootId, Size size)
        {
            var rootGroup = new LayoutGroup();
            rootGroup.Init(rootId, true, null);
            rootGroup.ContentWidth = size.Width;
            rootGroup.ContentHeight = size.Height;
            rootGroup.HorizontalStretchFactor = 1;
            //rootGroup.VerticalStretchFactor = 1;
            return rootGroup;
        }
        #endregion
    }
}
