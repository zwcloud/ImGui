using System;
using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUI
    {
        public static Color ColorField(string label, Rect rect, Color value)
        {
            throw new NotImplementedException();
        }
    }

    public partial class GUILayout
    {
        public static Color ColorField(string label, Color value)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            var style = GUIStyle.Basic;

            var id = window.GetID(label);

            // rect

            int rId;
            int gId;
            int bId;
            int aId;
            int colorId;

            Rect rectR, rectG, rectB, rectA, rectColor;

            var labelHeight = style.CalcSize(label, GUIState.Normal).Height;

            BeginHorizontal("FieldGroup~" + id);
            {
                {
                    using (HScope("#RGBA&Color", GUILayout.ExpandWidth(true)))
                    {
                        using (VScope(label + "#RGBA"))
                        {
                            using (HScope("#RGB"))
                            {
                                PushHStretchFactor(1);
                                rId = window.GetID("#R");
                                gId = window.GetID("#G");
                                bId = window.GetID("#B");
                                rectR = window.GetRect(rId);
                                rectG = window.GetRect(gId);
                                rectB = window.GetRect(bId);
                                PopStyleVar(1);
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
                    var d = window.DrawList;
                    DrawColorDragButton(d, rectR, rId, 'R', value.R, (R_hovered && R_held) ? GUIState.Active : R_hovered ? GUIState.Hover : GUIState.Normal);
                    DrawColorDragButton(d, rectG, gId, 'G', value.G, (G_hovered && G_held) ? GUIState.Active : G_hovered ? GUIState.Hover : GUIState.Normal);
                    DrawColorDragButton(d, rectB, bId, 'B', value.B, (B_hovered && B_held) ? GUIState.Active : B_hovered ? GUIState.Hover : GUIState.Normal);

                    GUIAppearance.DrawProgressBar(rectA, value.A);

                    d.AddRectFilled(rectColor, value);
                }

                Space("FieldSpacing", GUISkin.Current.FieldSpacing);
                Label(label, GUILayout.Width((int)GUISkin.Current.LabelWidth));
            }
            EndHorizontal();

            return value;
        }

        private static void DrawColorDragButton(DrawList drawList, Rect rect, int id, char colorChar, double value, GUIState state)
        {
            GUIContext g = Form.current.uiContext;
            Window window = g.WindowManager.CurrentWindow;

            var style = GUIStyle.Basic;
            var d = window.DrawList;

            style.PushBgColor(new Color(0.80f, 0.80f, 0.80f, 0.30f));//+1
            d.AddRectFilled(rect, style.BackgroundColor);
            style.PopStyle();//-1

            string text;
            text = string.Format("{0}:{1,3}", colorChar, (int)(value * 255));
            var fullTextSize = style.CalcSize(text, state);
            var contentRect = style.GetContentRect(rect, state);
            if(fullTextSize.Width > contentRect.Width)
            {
                text = ((int)(value * 255)).ToString();
            }

            style.PushTextAlignment(TextAlignment.Center);
            d.DrawBoxModel(rect, text, style, state);
            style.PopStyle();
            
        }
    }

}
