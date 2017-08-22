using System;
using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool TreeNode(string label, ref bool open)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            PushHStretchFactor(1);
            BeginVertical(label + "_Tree");
            PopStyleVar(1);
                do
                {
                    var id = window.GetID(label);

                    // style apply
                    var s = g.StyleStack;
                    var style = s.Style;
                    s.PushStretchFactor(false, 1);//+1, always expand width

                    // rect
                    var lineHeight = style.GetLineHeight();
                    Rect rect = window.GetRect(id, new Size(0, lineHeight));
                    if (rect == Layout.StackLayout.DummyRect)//TODO how shold dummy rect be correctly handled in every control?
                    {
                        s.PopStyle();//-1
                        break;
                    }

                    // interact
                    bool hovered, held;
                    bool pressed = GUIBehavior.ButtonBehavior(rect, id, out hovered, out held, ButtonFlags.PressedOnClick);
                    if (pressed)
                    {
                        open = !open;
                    }

                    // render
                    {
                        DrawList d = window.DrawList;
                        var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
                        if(hovered || held)
                        {
                            s.PushBgColor(new Color(0.40f, 0.40f, 0.90f, 0.45f), GUIState.Normal);//+1 TODO It's stupid to sprcifiy style like this. There should be a better way to do this.
                            s.PushBgColor(new Color(0.45f, 0.45f, 0.90f, 0.80f), GUIState.Hover);//+1
                            s.PushBgColor(new Color(0.53f, 0.53f, 0.87f, 0.80f), GUIState.Active);//+1
                            var color = style.Get<Color>(GUIStyleName.BackgroundColor, state);
                            d.RenderFrame(rect.Min, rect.Max, color, false, 0);
                            s.PopStyle(3);//-3
                        }
                        d.RenderCollapseTriangle(rect.Min + new Vector(0+style.PaddingTop, lineHeight * 0.15f), open, lineHeight, Color.White, 0.7);
                        rect.X += rect.Height;
                        var delta = rect.Width - rect.Height;
                        if (delta > 0)
                        {
                            rect.Width = delta;
                        }
                        d.DrawText(rect, label, style, state);
                    }

                    // style restore
                    s.PopStyle();//-1
                }while(false);
                BeginHorizontal("#Content");
                    Space("Space", 20);
                    PushHStretchFactor(1);
                    BeginVertical("#Items");
                    PopStyleVar(1);
            return open;
        }

        public static void TreePop()
        {
                    EndVertical();
                EndHorizontal();
            EndVertical();
        }
    }
}
