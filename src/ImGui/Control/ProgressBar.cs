using ImGui.OSAbstraction.Text;
using ImGui.Rendering;

namespace ImGui
{
    public partial class GUILayout
    {
        public static double ProgressBar(string str_id, double percent, Size size, string overlayText = null)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return 0;

            //get or create the root node
            var id = window.GetID(str_id);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            if (node == null)
            {
                //create node
                node = new Node(id, $"ProgressBar<{str_id}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.ProgressBar]);
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);
            
            // last item state
            window.TempData.LastItemState = node.State;

            // render
            percent = MathEx.Clamp01(percent);
            var rect = node.Rect;
            var fillWidth = rect.Width * percent;
            var fillRect = new Rect(rect.X, rect.Y, fillWidth, rect.Height);
            using (var dc = node.RenderOpen())
            {
                dc.DrawRectangle(new Brush(new Color(0.80f, 0.80f, 0.80f, 0.30f)), null, rect);
                dc.DrawRectangle(new Brush(new Color(0.90f, 0.70f, 0.00f, 1.00f)), null, fillRect);
                if(overlayText != null)
                {
                    dc.DrawGlyphRun(node.RuleSet, overlayText, node.Rect.Location);
                }
            }

            return percent;
        }
    }

    internal partial class GUISkin
    {
        private void InitProgressBarStyles(StyleRuleSet ruleSet)
        {
            ruleSet.TextAlignment = TextAlignment.Center;
        }
    }
}
