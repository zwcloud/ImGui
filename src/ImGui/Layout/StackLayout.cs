using ImGui.Rendering;

namespace ImGui.Layout
{
    internal static class StackLayout
    {
        public static void BeginLayoutGroup(this RenderTree renderTree, int id, bool isVertical, LayoutOptions? options = null, string str_id = null)
        {
            var group = new Node(id, str_id ?? "group");
            group.RuleSet.ApplyOptions(options);
            group.AttachLayoutGroup(isVertical);
            renderTree.CurrentContainer.AppendChild(group);
        }

        public static void EndLayoutGroup(this RenderTree renderTree)
        {
            renderTree.CurrentContainer = renderTree.CurrentContainer.Parent;
        }
    }
}
