using ImGui.Rendering;

namespace ImGui.Development
{
    public class GUIDebug
    {
        public static void SetWindowPosition(string windowName, Point position)
        {
            var possibleWindow = Application.ImGuiContext.WindowManager.Windows.Find(window => window.Name == windowName);
            if (possibleWindow != null)
            {
                possibleWindow.Position = position;
            }
        }
    }
}

namespace ImGui
{
    public partial class GUI
    {
        public static void DebugBox(int id, Rect rect, Color color)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            //get or create the root node
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            if (node == null)
            {
                //create node
                node = new Node(id, $"DebugBox<{id}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Box]);
                node.RuleSet.BackgroundColor = color;
                container.Add(node);
            }
            node.ActiveSelf = true;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // rect
            node.Rect = window.GetRect(rect);

            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(node.RuleSet, node.Rect);
            }
        }
    }
}