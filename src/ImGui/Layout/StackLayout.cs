using ImGui.Rendering;

namespace ImGui.Layout
{
    internal static class StackLayout
    {
        public static void BeginLayoutGroup(this Window window, int id, bool isVertical, LayoutOptions? options = null, string str_id = null)
        {
            var group = new Node(id, str_id ?? "group");
            group.RuleSet.ApplyOptions(options);
            group.AttachLayoutGroup(isVertical);
            window.RenderTree.CurrentContainer.AppendChild(group);
        }

        public static void EndLayoutGroup(this Window window)
        {
            window.RenderTree.CurrentContainer = window.RenderTree.CurrentContainer.Parent;
        }
    }
}
