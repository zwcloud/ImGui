using ImGui.GraphicsAbstraction;
using ImGui.OSAbstraction.Graphics;

namespace ImGui.Rendering
{
    internal class ImageGeometry : Geometry
    {
        public Image Image { get; set; }
        public ITexture Texture { get; set; }

        public ImageGeometry(Image image)
        {
            this.Image = image;
        }

        public ImageGeometry(string filePath)
        {
            this.Image = new Image(filePath);
        }

        public void SendToGPU()
        {
            this.Texture = Application.PlatformContext.CreateTexture();
            this.Texture.LoadImage(this.Image.Data, this.Image.Width, this.Image.Height);
        }

        internal override PathGeometryData GetPathGeometryData()
        {
            throw new System.NotImplementedException();
        }
    }
}
