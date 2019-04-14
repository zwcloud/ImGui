using ImGui.Rendering;

namespace ImGui.Layout
{
    internal static class StackLayout
    {
        public static void Layout(Node node, Point position)
        {
            node.CalcWidth(node.ContentWidth);
            node.CalcHeight(node.ContentHeight);
            node.SetX(position.X);
            node.SetY(position.Y);
        }

    }
}
