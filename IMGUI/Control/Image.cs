namespace ImGui
{
    internal class Image
    {
        static internal void DoControl(Rect rect, Content content, string id)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, content, Skin.current.Image["Normal"]);
            }
        }
        
    }
}
