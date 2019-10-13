using ImGui.Rendering;
using ImGui.Style;
using System;

namespace ImGui
{
    public partial class GUILayout
    {
        public static Color ColorField(string label, Color value)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            //get or create the root node
            var id = window.GetID(label);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            if (node == null)
            {
                //create node
                node = new Node(id, $"ColorField<{label}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.ColorField]);
                node.AttachLayoutGroup(false);
                container.AppendChild(node);
            }
            node.ActiveSelf = true;

            // rect

            int rId;
            int gId;
            int bId;
            int aId;
            int colorId;

            Rect rectR, rectG, rectB, rectA, rectColor;

            BeginHorizontal("FieldGroup~" + id);
            {
                using (HScope("#RGBA&Color", GUILayout.ExpandWidth(true)))
                {
                    using (VScope(label + "#RGBA"))
                    {
                        using (HScope("#RGB", GUILayout.ExpandWidth(true)))
                        {
                            rId = window.GetID("#R");
                            gId = window.GetID("#G");
                            bId = window.GetID("#B");
                            rectR = window.GetRect(rId);
                            rectG = window.GetRect(gId);
                            rectB = window.GetRect(bId);
                        }

                        aId = window.GetID("#A");
                        rectA = window.GetRect(aId);
                    }
                    colorId = window.GetID("#Color");
                    rectColor = window.GetRect(colorId);
                }

                // interact
                value.R = GUIBehavior.SliderBehavior(rectR, rId, true, value.R, 0, 1.0, out bool R_hovered, out bool R_held);
                value.G = GUIBehavior.SliderBehavior(rectG, gId, true, value.G, 0, 1.0, out bool G_hovered, out bool G_held);
                value.B = GUIBehavior.SliderBehavior(rectB, bId, true, value.B, 0, 1.0, out bool B_hovered, out bool B_held);
                value.A = GUIBehavior.SliderBehavior(rectA, aId, true, value.A, 0, 1.0, out bool A_hovered, out bool A_held);

                // render
                using (var d = node.RenderOpen())
                {

                    DrawColorDragButton(d, node.RuleSet, rectR, rId, 'R', value.R, (R_hovered && R_held) ? GUIState.Active : R_hovered ? GUIState.Hover : GUIState.Normal);
                    DrawColorDragButton(d, node.RuleSet, rectG, gId, 'G', value.G, (G_hovered && G_held) ? GUIState.Active : G_hovered ? GUIState.Hover : GUIState.Normal);
                    DrawColorDragButton(d, node.RuleSet, rectB, bId, 'B', value.B, (B_hovered && B_held) ? GUIState.Active : B_hovered ? GUIState.Hover : GUIState.Normal);

                    var fillWidth = node.Rect.Width * value.A;
                    var fillRect = new Rect(rectA.X, rectA.Y, fillWidth, node.Rect.Height);
                    d.DrawRectangle(new Brush(new Color(0.80f, 0.80f, 0.80f, 0.30f)), null, node.Rect);
                    d.DrawRectangle(new Brush(new Color(0.90f, 0.70f, 0.00f, 1.00f)), null, fillRect);
                    d.DrawRectangle(new Brush(value), null, rectColor);
                }

                Space("FieldSpacing", GUISkin.Current.FieldSpacing);
                Label(label, GUILayout.Width((int)GUISkin.Current.LabelWidth));
            }
            EndHorizontal();

            return value;
        }

        private static void DrawColorDragButton(DrawingContext dc, StyleRuleSet ruleSet, Rect buttonRect, int id, char colorChar, double value, GUIState state)
        {
            dc.DrawRectangle(new Brush(new Color(0.80f, 0.80f, 0.80f, 0.30f)), null, buttonRect);

            string text = string.Format("{0}:{1,3}", colorChar, (int)(value * 255));
            var fullTextSize = ruleSet.CalcSize(text, state);
            if (fullTextSize.Width > buttonRect.Width)
            {
                text = ((int)(value * 255)).ToString();
            }

            dc.DrawGlyphRun(ruleSet, text, buttonRect.TopLeft);
        }
    }

    internal partial class GUISkin
    {
        private void InitColorFieldStyles(StyleRuleSet ruleSet)
        {
            //TODO
        }
    }

}
