using ImGui.Common;
using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        public static double ProgressBar(string str_id, double percent, Size size, string overlayText = null)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return percent;

            GUIContext g = GetCurrentContext();
            int id = window.GetID(str_id);

            // style
            var style = GUIStyle.Basic;

            // rect
            var rect = window.GetRect(id);

            percent = MathEx.Clamp01(percent);

            // render
            DrawList d = window.DrawList;
            GUIAppearance.DrawProgressBar(rect, percent);
            if(overlayText != null)
            {
                style.PushTextAlignment(TextAlignment.Center);
                d.DrawBoxModel(rect, overlayText, style);
                style.PopStyle();
            }

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
            GUIStyle style = GUIStyle.Basic;
            DrawList d = window.DrawList;

            style.PushBgColor(new Color(0.80f, 0.80f, 0.80f, 0.30f));//+1
            d.AddRectFilled(rect, style.BackgroundColor);
            style.PopStyle();//-1
            var fillWidth = rect.Width * percent;
            var fillRect = new Rect(rect.X, rect.Y, fillWidth, rect.Height);
            style.PushFillColor(new Color(0.90f, 0.70f, 0.00f, 1.00f));//+1
            d.AddRectFilled(fillRect, style.FillColor);
            style.PopStyle();//-1
        }
    }
}
