using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUI
    {
        public static Color ColorField(string label, Rect boxRect, Rect labelRect, Color value)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return value;

            // style apply
            var s = g.StyleStack;
            var colorFieldModifiers = GUISkin.Instance[GUIControlName.ColorField];
            s.PushRange(colorFieldModifiers);

            //rect

            // interact

            // render
            var d = window.DrawList;
            var style = s.Style;
            d.AddRectFilled(boxRect, value);
            d.DrawText(labelRect, label, style, GUIState.Normal);

            // style restore
            s.PopStyle(colorFieldModifiers.Length);

            return value;
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
            var textSize = style.CalcSize(label, GUIState.Normal);
            var boxSize = new Size(0, textSize.Height);

            int rId;
            int gId;
            int bId;
            int aId;
            int colorId;

            Rect rectR, rectG, rectB, rectA, rectColor;

            PushFixedHeight(100);//+2
            PushPadding((10, 10, 10, 10));//+4
            BeginHorizontal(label+"#HGroup");
            PopStyleVar(4); //-4
            PopStyleVar(2); //-2
            {
                PushHStretchFactor(1); //+1
                PushVStretchFactor(1); //+1
                BeginVertical("#RGBA");
                {
                    BeginHorizontal("#RGB");
                    {
                        rId = window.GetID("#R");
                        gId = window.GetID("#G");
                        bId = window.GetID("#B");
                        rectR = window.GetRect(rId, boxSize);
                        PushHStretchFactor(0); //+1
                        Space("#space0", 5);
                        PopStyleVar(); //-1
                        rectG = window.GetRect(gId, boxSize);
                        PushHStretchFactor(0); //+1
                        Space("#space1", 5);
                        PopStyleVar(); //-1
                        rectB = window.GetRect(bId, boxSize);
                    }
                    EndHorizontal();
                    aId = window.GetID("#A");
                    rectA = window.GetRect(aId, Size.Zero);
                }
                EndVertical();
                colorId = window.GetID("#Color");
                PushHStretchFactor(0); //+1
                Space("#space2", 10);
                PopStyleVar(); //-1
                PushVStretchFactor(1); //+1
                rectColor = window.GetRect(colorId, Size.Zero);
                PopStyleVar(); //-1
                PopStyleVar(); //-1
                PopStyleVar(); //-1
            }
            EndHorizontal();

            // interact
            value.R = GUIBehavior.SliderBehavior(rectR, rId, true, value.R, 0, 1.0, out bool _);
            value.G = GUIBehavior.SliderBehavior(rectG, gId, true, value.G, 0, 1.0, out bool _);
            value.B = GUIBehavior.SliderBehavior(rectB, bId, true, value.B, 0, 1.0, out bool _);
            value.A = GUIBehavior.SliderBehavior(rectA, aId, true, value.A, 0, 1.0, out bool _);

            // render
            var d = window.DrawList;
            DrawColorDragButton(d, rectR, rId, 'R', value.R);
            DrawColorDragButton(d, rectG, gId, 'G', value.G);
            DrawColorDragButton(d, rectB, bId, 'B', value.B);
            DrawColorDragButton(d, rectA, aId, 'A', value.A);
            d.AddRectFilled(rectColor, value);

            return value;
        }

        private static void DrawColorDragButton(DrawList drawList, Rect rect, int id, char colorChar, double value)
        {
            GUIContext g = Form.current.uiContext;
            Window window = g.WindowManager.CurrentWindow;

            var s = g.StyleStack;
            var style = s.Style;
            var d = window.DrawList;

            s.PushBorder((1, 1, 1, 1));//+4
            s.PushBorderColor(Color.White);//+4
            d.DrawBoxModel(rect, string.Format("{0}:{1,3}", colorChar,(int)(value * 255)), style);
            s.PopStyle(4);//-4
            s.PopStyle(4);//-4
        }

    }

    internal partial class GUISkin
    {
        private void InitColorFieldStyles()
        {
            var colorFieldStyles = new StyleModifier[] { };
            this.styles.Add(GUIControlName.ColorField, colorFieldStyles);
        }
    }
}
