using ImGui.Common;
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

            var id = window.GetID(label);

            GUIContext g = GetCurrentContext();
            var s = g.StyleStack;
            var style = s.Style;

            // rect
            var textSize = style.CalcSize(label, GUIState.Normal);
            var cSize = style.CalcSize("R", GUIState.Normal).Width;
            var boxSize = new Size(cSize, textSize.Height);
            var spaceSize = new Size(0, textSize.Height);
            var colorBoxSize = new Size(200, 200);
            var spaceWidth = 5;

            GUILayout.PushFixedHeight(100);//+1
            GUILayout.PushPadding((10, 10, 10, 10));
            GUILayout.BeginHorizontal(label+"#HGroug");
                GUILayout.PopStyleVar(4);//-4
                GUILayout.PopStyleVar(1);//-1
                s.PushStretchFactor(false, 1);//+1
                s.PushStretchFactor(true, 1);//+1

                var rId = window.GetID("#R");
                var space0Id = window.GetID("#space0");
                var gId = window.GetID("#G");
                var space1Id = window.GetID("#space1");
                var bId = window.GetID("#B");
                var aId = window.GetID("#A");
                var space2Id = window.GetID("#space2");
                var colorId = window.GetID("#Color");

                GUILayout.BeginVertical("#RGBA");
                    GUILayout.BeginHorizontal("#RGB");
                        var rectR = window.GetRect(rId, boxSize);
                        GUILayout.PushFixedWidth(spaceWidth);//+2
                        window.GetRect(space0Id, spaceSize);
                        GUILayout.PopStyleVar(2);//-2
                        var rectG = window.GetRect(gId, boxSize);
                        GUILayout.PushFixedWidth(spaceWidth);//+2
                        window.GetRect(space1Id, spaceSize);
                        GUILayout.PopStyleVar(2);//-2
                        var rectB = window.GetRect(bId, boxSize);
                    GUILayout.EndHorizontal();
                    var rectA = window.GetRect(aId, boxSize);
                GUILayout.EndVertical();
                GUILayout.PushFixedWidth(spaceWidth);//+2
                window.GetRect(space2Id, spaceSize);
                GUILayout.PopStyleVar(2);//-2
                GUILayout.PushVStretchFactor(1);//+1
                var rectColor = window.GetRect(colorId, colorBoxSize);
                GUILayout.PopStyleVar(1);//-1
                s.PopStyle();//-1
                s.PopStyle();//-1
            GUILayout.EndHorizontal();

            var d = window.DrawList;

            value.R = GUIBehavior.SliderBehavior(rectR, rId, true, value.R, 0, 1.0, out bool _);
            DrawColorDragButton(d, rectR, rId, 'R', value.R);

            value.G = GUIBehavior.SliderBehavior(rectG, gId, true, value.G, 0, 1.0, out bool _);
            DrawColorDragButton(d, rectG, gId, 'G', value.G);

            value.B = GUIBehavior.SliderBehavior(rectB, bId, true, value.B, 0, 1.0, out bool _);
            DrawColorDragButton(d, rectB, bId, 'B', value.B);

            value.A = GUIBehavior.SliderBehavior(rectA, aId, true, value.A, 0, 1.0, out bool _);
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
