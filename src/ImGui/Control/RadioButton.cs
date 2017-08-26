using ImGui.Common.Primitive;
using System;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool RadioButton(string label, bool active)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            GUIContext g = GetCurrentContext();
            int id = window.GetID(label);

            // style
            var s = g.StyleStack;
            var style = s.Style;

            Size label_size = style.CalcSize(label, GUIState.Normal);

            Rect check_bb = window.GetRect(window.GetID(label + "_check"), new Size(label_size.Height, label_size.Height));
            Rect text_bb = window.GetRect(id, label_size);

            Rect total_bb = Rect.Union(check_bb, text_bb);

            Point center = check_bb.Center;
            center.x = (int)center.x + 0.5f;
            center.y = (int)center.y + 0.5f;
            var radius = check_bb.Height * 0.5f;

            bool hovered, held;
            bool pressed = GUIBehavior.ButtonBehavior(total_bb, id, out hovered, out held);

            var d = window.DrawList;
            var state = ((held && hovered) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal);
            d.AddCircleFilled(center, (float)radius, ((held && hovered) ? Color.FrameBgActive : hovered ? Color.FrameBgHovered : Color.FrameBg), 16);
            if (active)
            {
                var check_sz = Math.Min(check_bb.Width, check_bb.Height);
                var pad = Math.Max(1.0f, (int)(check_sz / 6.0f));
                d.AddCircleFilled(center, radius-pad, Color.CheckMark, 16);
            }

            if (label_size.Width > 0.0f)
                d.AddText(text_bb, label, style, state);

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
