using System;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool RadioButton(string text, bool active)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(text);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create node
                node = new Node(id, $"RadioButton<{text}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Button]);
                var size = node.RuleSet.CalcContentBoxSize(text, GUIState.Normal);
                size.Width += size.Height + node.RuleSet.PaddingLeft;
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            bool pressed = GUIBehavior.ButtonBehavior(node.Rect, id, out var hovered, out var held);
            node.State = (held && hovered) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            
            // last item state
            window.TempData.LastItemState = node.State;

            using (var d = node.RenderOpen())
            {
                Color backgroundCircleColor;
                switch (node.State)
                {
                    case GUIState.Active:
                        backgroundCircleColor = Color.DarkBlue;
                        break;
                    case GUIState.Hover:
                        backgroundCircleColor = Color.Argb(200, 173, 216, 230);
                        break;
                    default:
                        backgroundCircleColor = Color.LightBlue;
                        break;
                }


                Point center = new Point(node.Rect.X + node.Height/2, node.Rect.Y + node.Height / 2);
                center.x = (int)center.x + 0.3f;
                center.y = (int)center.y + 0.3f;
                var radius = node.Rect.Height * 0.3f;

                d.DrawEllipse(new Brush(backgroundCircleColor), null, center, radius, radius);
                if (active)
                {
                    var pad = Math.Max(1.0f, (int)(node.Height * 0.15));
                    d.DrawEllipse(new Brush(Color.Black), null, center, radius - pad, radius - pad);
                }

                if (!string.IsNullOrWhiteSpace(text))
                {
                    d.DrawGlyphRun(node.RuleSet, text, node.Rect.Location + new Vector(node.Rect.Height + node.RuleSet.PaddingLeft, 0));
                }
            }

            return pressed;
        }

        public static bool RadioButton(string label, ref int v, int v_button)
        {
            bool pressed = RadioButton(label, v == v_button);
            if (pressed)
            {
                v = v_button;
            }
            return pressed;
        }
    }
}
