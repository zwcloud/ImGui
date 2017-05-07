using System.Diagnostics;

namespace ImGui
{
    /// <summary>
    /// Box
    /// </summary>
    /// <remarks>
    /// The box is a simple control containing an optinal Content.
    /// </remarks>
    internal class Box
    {
        public static void DoControl(Rect rect, string text)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, text, GUISkin.Instance[GUIControlName.Box]);
        }
    }
}