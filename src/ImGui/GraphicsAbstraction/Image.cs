using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace ImGui.GraphicsAbstraction
{
    internal class Image
    {
        private SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image;

        public Rgba32[] Data { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Image(string filePath)
        {
            using (var stream = Utility.ReadFile(filePath))
            {
                this.image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(stream);
                this.Data = new Rgba32[this.image.Width * this.image.Height];
                this.Data = image.GetPixelSpan().ToArray();
                this.Width = this.image.Width;
                this.Height = this.image.Height;
            }
        }
    }
}
