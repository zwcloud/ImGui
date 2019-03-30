using System;
using ImGui.Rendering;

namespace ImGui.Layout
{
    internal static class StackLayout
    {
        public static void BeginLayoutGroup(this RenderTree renderTree, int id, bool isVertical, LayoutOptions? options = null, string str_id = null)
        {
            var group = (Node)renderTree.CurrentContainer.Children.Find(n => n.Id == id);
            if (group == null)
            {
                group = new Node(id, str_id ?? "group");
                group.AttachLayoutGroup(isVertical);
                renderTree.CurrentContainer.AppendChild(group);
            }
            group.RuleSet.ApplyOptions(options);
            renderTree.CurrentContainer = group;
        }

        public static void EndLayoutGroup(this RenderTree renderTree)
        {
            if (renderTree.CurrentContainer == renderTree.Root)
            {
                throw new InvalidOperationException("BeginLayoutGroup/EndLayoutGroup mismatch.");
            }
            renderTree.CurrentContainer = (Node)renderTree.CurrentContainer.Parent;
        }
    }
}
