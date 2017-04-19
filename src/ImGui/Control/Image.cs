namespace ImGui
{
    internal class Image
    {
        internal static void DoControl(Rect rect, Content content, GUIStyle style, string id)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;

            d.DrawBoxModel(rect, content, style);
        }
        
    }
}
