namespace ImGui
{
    internal class Image
    {
        internal static void DoControl(Rect rect, string filePath, GUIStyle style)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;

            var texture = TextureUtil.GetTexture(filePath);

            d.DrawBoxModel(rect, texture, style);
        }

        internal static void DoControl(Rect rect, ITexture texture, GUIStyle style)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;

            d.DrawBoxModel(rect, texture, style);
        }
    }
}
