using BigGustave;

namespace ImGui.GraphicsAbstraction
{
    internal class Image
    {
        public byte[] Data { get; }

        public int Width { get; }
        public int Height { get; }

        public Image(string filePath)
        {
            using var stream = Utility.ReadFile(filePath);
            var png = Png.Open(stream);
            this.Data = new byte[png.Width * png.Height * 4];
            int byteOffset = 0;
            for (int y = 0; y < png.Height; y++)
            {
                for (int x = 0; x < png.Width; x++)
                {
                    var pixel = png.GetPixel(x, y);

                    this.Data[byteOffset++] = pixel.R;
                    this.Data[byteOffset++] = pixel.G;
                    this.Data[byteOffset++] = pixel.B;
                    this.Data[byteOffset++] = pixel.A;
                }
            }
            this.Width = png.Width;
            this.Height = png.Height;
        }
    }
}
