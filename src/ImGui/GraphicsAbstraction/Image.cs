using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImGui.GraphicsAbstraction
{
    internal class Image
    {
        private SixLabors.ImageSharp.Image<SixLabors.ImageSharp.Rgba32> image;

        public SixLabors.ImageSharp.Rgba32[] Data { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Image(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                this.image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.Rgba32>(stream);
                this.Data = new Rgba32[this.image.Width * this.image.Height];
                this.image.SavePixelData<SixLabors.ImageSharp.Rgba32>(this.Data);
                this.Width = this.image.Width;
                this.Height = this.image.Height;
            }
        }
    }
}
