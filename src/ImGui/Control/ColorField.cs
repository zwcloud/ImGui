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

            // rect
            var text = Utility.FindRenderedText(label);
            var textSize = GUISkin.Current[GUIControlName.ColorField].CalcContentBoxSize(text, GUIState.Normal);
            BeginHorizontal(label, GUILayout.Height(textSize.Height).ExpandWidth(true));
            {
                int rId, gId, bId, aId, colorId;
                Rect rectR, rectG, rectB, rectA, rectColor;
                using (HScope("#RGBA&Color", GUILayout.ExpandHeight(true).ExpandWidth(true)))
                {
                    using (VScope(label + "#RGBA", GUILayout.ExpandHeight(true).ExpandWidth(true)))
                    {
                        using (HScope("#RGB", GUILayout.ExpandHeight(true).ExpandWidth(true)))
                        {
                            rId = window.GetID("#R");
                            var rNode = window.RenderTree.CurrentContainer.GetNodeById(rId);
                            if(rNode == null)
                            {
                                rNode = new Node(rId, "#R");
                                rNode.AttachLayoutEntry();
                                rNode.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true).ExpandWidth(true));
                            }
                            window.RenderTree.CurrentContainer.AppendChild(rNode);
                            rNode.ActiveSelf = true;

                            gId = window.GetID("#G");
                            var gNode = window.RenderTree.CurrentContainer.GetNodeById(gId);
                            if (gNode == null)
                            {
                                gNode = new Node(gId, "#G");
                                gNode.AttachLayoutEntry();
                                gNode.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true).ExpandWidth(true));
                            }
                            window.RenderTree.CurrentContainer.AppendChild(gNode);
                            gNode.ActiveSelf = true;

                            bId = window.GetID("#B");
                            var bNode = window.RenderTree.CurrentContainer.GetNodeById(bId);
                            if (bNode == null)
                            {
                                bNode = new Node(bId, "#B");
                                bNode.AttachLayoutEntry();
                                bNode.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true).ExpandWidth(true));
                            }
                            window.RenderTree.CurrentContainer.AppendChild(bNode);
                            bNode.ActiveSelf = true;

                            rectR = window.GetRect(rId);
                            rectG = window.GetRect(gId);
                            rectB = window.GetRect(bId);
                        }

                        aId = window.GetID("#A");
                        var aNode = window.RenderTree.CurrentContainer.GetNodeById(aId);
                        if (aNode == null)
                        {
                            aNode = new Node(aId, "#A");
                            aNode.AttachLayoutEntry();
                            aNode.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true).ExpandWidth(true));
                        }
                        window.RenderTree.CurrentContainer.AppendChild(aNode);
                        aNode.ActiveSelf = true;
                        rectA = window.GetRect(aId);
                    }
                    colorId = window.GetID("#Color");
                    var colorNode = window.RenderTree.CurrentContainer.GetNodeById(colorId);
                    if (colorNode == null)
                    {
                        colorNode = new Node(colorId, "#Color");
                        colorNode.AttachLayoutEntry();
                        colorNode.RuleSet.ApplyOptions(GUILayout.ExpandHeight(true).ExpandWidth(true));
                    }
                    window.RenderTree.CurrentContainer.AppendChild(colorNode);
                    colorNode.ActiveSelf = true;
                    rectColor = window.GetRect(colorId);
                }

                // interact
                value.R = GUIBehavior.SliderBehavior(rectR, rId, true, value.R, 0, 1.0, out bool R_hovered, out bool R_held);
                value.G = GUIBehavior.SliderBehavior(rectG, gId, true, value.G, 0, 1.0, out bool G_hovered, out bool G_held);
                value.B = GUIBehavior.SliderBehavior(rectB, bId, true, value.B, 0, 1.0, out bool B_hovered, out bool B_held);
                value.A = GUIBehavior.SliderBehavior(rectA, aId, true, value.A, 0, 1.0, out bool A_hovered, out bool A_held);

                // render
                var node = window.RenderTree.CurrentContainer;
                using (var d = node.RenderOpen())
                {
                    DrawColorDragButton(d, node.RuleSet, rectR, rId, 'R', value.R, (R_hovered && R_held) ? GUIState.Active : R_hovered ? GUIState.Hover : GUIState.Normal);
                    DrawColorDragButton(d, node.RuleSet, rectG, gId, 'G', value.G, (G_hovered && G_held) ? GUIState.Active : G_hovered ? GUIState.Hover : GUIState.Normal);
                    DrawColorDragButton(d, node.RuleSet, rectB, bId, 'B', value.B, (B_hovered && B_held) ? GUIState.Active : B_hovered ? GUIState.Hover : GUIState.Normal);

                    var fillWidth = rectA.Width * value.A;
                    var fillRect = new Rect(rectA.X, rectA.Y, fillWidth, rectA.Height);
                    //d.DrawRectangle(new Brush(new Color(0.80f, 0.80f, 0.80f, 0.30f)), null, node.Rect);
                    d.DrawRectangle(new Brush(new Color(0.90f, 0.70f, 0.00f, 1.00f)), null, fillRect);
                    d.DrawRectangle(new Brush(value), null, rectColor);
                }

                Space("FieldSpacing", node.RuleSet.Get<double>("ControlLabelSpacing"));
                Label(label, GUILayout.Width((int)node.RuleSet.Get<double>("LabelWidth")));
            }
            EndHorizontal();

            return value;
        }

        private static void DrawColorDragButton(DrawingContext dc, StyleRuleSet ruleSet, Rect buttonRect, int id, char colorChar, double value, GUIState state)
        {
            dc.DrawRectangle(new Brush(new Color(0.80f, 0.80f, 0.80f, 0.30f)), null, buttonRect);

            string text = string.Format("{0}:{1,3}", colorChar, (int)(value * 255));
            var fullTextSize = ruleSet.CalcContentBoxSize(text, state);
            if (fullTextSize.Width > buttonRect.Width)
            {
                text = ((int)(value * 255)).ToString();
            }

            dc.DrawBoxModel(text, ruleSet, buttonRect);
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
