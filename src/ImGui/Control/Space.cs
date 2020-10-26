using ImGui.Rendering;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Create a fixed size space.
        /// </summary>
        public static void Space(string str_id, double thickness)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(str_id);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            if (node == null)
            {
                node = new Node(id, $"Space<{str_id}>");
                node.UseBoxModel = true;
                var size = window.RenderTree.CurrentContainer.IsVertical ? new Size(0, thickness) : new Size(thickness, 0);
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);
            node.ActiveSelf = true;
            node.Rect = window.GetRect(id);
            
            // last item state
            window.TempData.LastItemState = node.State;

            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(node);
            }
        }

        /// <summary>
        /// Create a flexible space.
        /// </summary>
        public static void FlexibleSpace(string str_id, int stretchFactor = 1)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(str_id);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            if (node == null)
            {
                node = new Node(id, $"Space<{str_id}>");
                node.UseBoxModel = true;
                node.AttachLayoutEntry();
                if (window.RenderTree.CurrentContainer.IsVertical)
                {
                    node.RuleSet.VerticalStretchFactor = stretchFactor;
                }
                else
                {
                    node.RuleSet.HorizontalStretchFactor = stretchFactor;
                }
            }
            container.AppendChild(node);
            node.ActiveSelf = true;
            node.Rect = window.GetRect(id);
            
            // last item state
            window.TempData.LastItemState = node.State;

            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(node);
            }
        }
    }
}
