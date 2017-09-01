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

            GUIContext g = GetCurrentContext();
            var s = g.StyleStack;
            var style = s.Style;

            // rect
            var boxSize = new Size(50, 50);

            int rId;
            int gId;
            int bId;
            int aId;
            int colorId;

            Rect rectR, rectG, rectB, rectA, rectColor;

            BeginHorizontal("#RGBA&Color", GUILayout.Width((int)GUISkin.Instance.FieldWidth).Height(100));
            {
                BeginVertical(label + "#RGBA", GUILayout.ExpandWidth(true));
                {
                    BeginHorizontal("#RGB");
                    {
                        PushHStretchFactor(1);
                        rId = window.GetID("#R");
                        gId = window.GetID("#G");
                        bId = window.GetID("#B");
                        rectR = window.GetRect(rId, (0, 50));
                        rectG = window.GetRect(gId, (0, 50));
                        rectB = window.GetRect(bId, (0, 50));
                        PopStyleVar(1);
                    }
                    EndHorizontal();

                    aId = window.GetID("#A");
                    rectA = window.GetRect(aId, (0, 10), GUILayout.ExpandWidth(true));
                }
                EndVertical();
            }
            colorId = window.GetID("#Color");
            rectColor = window.GetRect(colorId, (20, 20));
            EndHorizontal();

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

            return value;
        }

        private static void DrawColorDragButton(DrawList drawList, Rect rect, int id, char colorChar, double value, GUIState state)
        {
            GUIContext g = Form.current.uiContext;
            Window window = g.WindowManager.CurrentWindow;

            var s = g.StyleStack;
            var style = s.Style;
            var d = window.DrawList;

            s.PushBgColor(new Color(0.80f, 0.80f, 0.80f, 0.30f));//+1
            d.AddRectFilled(rect, style.BackgroundColor);
            s.PopStyle();//-1

            string text;
            text = string.Format("{0}:{1,3}", colorChar, (int)(value * 255));
            var fullTextSize = style.CalcSize(text, state);
            var contentRect = style.GetContentRect(rect, state);
            if(fullTextSize.Width > contentRect.Width)
            {
                text = ((int)(value * 255)).ToString();
            }

            d.DrawBoxModel(rect, text, style, state);
            
        }
    }

}
