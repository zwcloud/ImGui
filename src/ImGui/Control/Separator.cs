using ImGui.Rendering;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout horizontal separating line.
        /// </summary>
        public static void Separator(string str_id)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            //get or create the root node
            var id = window.GetID(str_id);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            if (node == null)
            {
                node = new Node(id, $"Separator<{str_id}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Separator]);
                node.RuleSet.HorizontalStretchFactor = 1;
            }
            container.AppendChild(node);

            node.ActiveSelf = true;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // rect
            node.Rect = window.GetRect(id);

            // render
            var offset = new Vector(node.Rect.Width / 2, 0);
            var start = node.Rect.Center - offset;
            var end = node.Rect.Center + offset;
            using (var dc = node.RenderOpen())
            {
                dc.DrawLine(new Pen(node.RuleSet.StrokeColor, node.RuleSet.StrokeWidth), start, end);
            }
        }
    }

    internal partial class GUISkin
    {
        private void InitSeparatorStyle(StyleRuleSet ruleSet)
        {
            var builder = new StyleRuleSetBuilder(ruleSet);
            builder.Padding(2.0)
                .StrokeWidth(1, GUIState.Normal)
                .StrokeWidth(1, GUIState.Hover)
                .StrokeWidth(1, GUIState.Active)
                .StrokeColor(new Color(0.50f, 0.50f, 0.50f, 0.60f), GUIState.Normal)
                .StrokeColor(new Color(0.60f, 0.60f, 0.70f, 1.00f), GUIState.Hover)
                .StrokeColor(new Color(0.70f, 0.70f, 0.90f, 1.00f), GUIState.Active);
        }
    }
}
