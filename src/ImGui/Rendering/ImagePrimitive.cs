using ImGui.GraphicsAbstraction;

namespace ImGui.Rendering
{
    internal class ImagePrimitive : Primitive
    {
        public Image Image { get; set; }

        public ImagePrimitive(Image image)
        {
            this.Image = image;
        }

        public ImagePrimitive(string filePath)
        {
            this.Image = new Image(filePath);
        }
    }
}
