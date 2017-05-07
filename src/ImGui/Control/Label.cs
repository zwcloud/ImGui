namespace ImGui
{
    internal class Label
    {
        internal static void DoControl(Rect rect, string text)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;

            d.DrawBoxModel(rect, text, GUISkin.Instance[GUIControlName.Label]);
        }
    }
}