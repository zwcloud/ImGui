using ImGui.GraphicsAbstraction;
using ImGui.OSAbstraction.Graphics;

namespace ImGui.Rendering
{
    internal class ImagePrimitive : Primitive
    {
        public Image Image { get; set; }
        public ITexture Texture { get; set; }

        public ImagePrimitive(Image image)
        {
            this.Image = image;
        }

        public ImagePrimitive(string filePath)
        {
            this.Image = new Image(filePath);
        }

        public void SendToGPU()
        {
            this.Texture = Application.PlatformContext.CreateTexture();
            this.Texture.LoadImage(this.Image.Data, this.Image.Width, this.Image.Height);
        }

        public override void UpdateContent()
        {
            throw new System.NotImplementedException();
        }
    }
}
