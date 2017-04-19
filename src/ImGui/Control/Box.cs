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
        public static void DoControl(Rect rect, Content content, string name)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Box]);
        }
    }
}