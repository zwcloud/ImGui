namespace ImGui
{
    internal class Label
    {
        internal static void DoControl(Rect rect, Content content, string name)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;

            d.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Label]);
        }
    }
}