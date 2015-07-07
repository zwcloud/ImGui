using Cairo;

namespace IMGUI
{
    internal class Image : Control
    {
        static internal void DoControl(Context g, Rect rect, Texture image, string name)
        {
            g.DrawBoxModel(rect, new Content(image), Skin._current.Image["Normal"]);
        }
    }
}
