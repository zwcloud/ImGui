namespace ImGui
{
    internal class Image
    {
        internal static void DoControl(Rect rect, Content content, string id)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Image]);
            }
        }
        
    }
}
