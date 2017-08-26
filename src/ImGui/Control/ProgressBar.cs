using ImGui.Common;
using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        public static double ProgressBar(string str_id, float percent, Size size, string overlayText)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return percent;

            GUIContext g = GetCurrentContext();
            int id = window.GetID(str_id);

            // style
            var s = g.StyleStack;
            var style = s.Style;

            // rect
            var rect = window.GetRect(id, size);

            percent = MathEx.Clamp01(percent);

            // render
            GUIAppearance.DrawProgressBar(rect, percent);

            return percent;
        }
    }

    internal partial class GUIAppearance
    {
        public static void DrawProgressBar(Rect rect, double percent, GUIState state = GUIState.Normal)
        {
            GUIContext g = Form.current.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;
            StyleStack s = g.StyleStack;
            GUIStyle style = s.Style;
            DrawList d = window.DrawList;

            d.AddRectFilled(rect, style.BackgroundColor);
            var fillWidth = rect.Width * percent;
            var fillRect = new Rect(rect.X, rect.Y, fillWidth, rect.Height);
            s.PushFillColor(new Color(0.90f, 0.70f, 0.00f, 1.00f));
            d.AddRectFilled(fillRect, style.FillColor);
            s.PopStyle();
        }
    }
}
