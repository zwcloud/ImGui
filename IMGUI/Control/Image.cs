namespace ImGui
{
    internal class Image
    {
        static internal void DoControl(Rect rect, Texture texture, string id)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, Content.Cached(texture, id), Skin.current.Image["Normal"]);
            }
        }
        
    }
}
