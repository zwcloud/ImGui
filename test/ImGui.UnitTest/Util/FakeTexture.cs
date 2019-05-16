using ImGui.OSAbstraction.Graphics;

namespace ImGui.UnitTest
{
    public class FakeTexture : ITexture
    {
        public void Dispose()
        {
        }

        public void LoadImage(byte[] data)
        {
        }

        public void LoadImage(SixLabors.ImageSharp.PixelFormats.Rgba32[] data, int width, int height)
        {
        }

        public void LoadImage(string filePath)
        {
        }

        public int Width { get; }
        public int Height { get; }
        public Size Size { get; }
        public System.IntPtr GetNativeTexturePtr()
        {
            throw new System.NotImplementedException();
        }

        public int GetNativeTextureId()
        {
            throw new System.NotImplementedException();
        }

        public object GetNativeTextureObject()
        {
            throw new System.NotImplementedException();
        }

        public bool Valid => true;
    }
}