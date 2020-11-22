using System;
using ImGui.GraphicsAbstraction;
using Microsoft.Extensions.Caching.Memory;

namespace ImGui.OSAbstraction.Graphics
{
    internal class TextureCache
    {
        public static TextureCache Default { get; } = new TextureCache();

        private MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        public ITexture GetOrAdd(Image image, IRenderer renderer)
        {
            int key = CalcKey(image, renderer);

            var texture = cache.Get<ITexture>(key);
            if (texture != null)
            {
                return texture;
            }

            texture = Application.PlatformContext.CreateTexture();
            texture.LoadImage(image.Data, image.Width, image.Height);
            cache.Set(key, texture);

            return texture;
        }

        public ITexture GetOrAdd(string path, IRenderer renderer)
        {
            int key = CalcKey(path, renderer);

            var texture = cache.Get<ITexture>(key);
            if (texture != null)
            {
                return texture;
            }

            var image = new Image(path);
            texture = Application.PlatformContext.CreateTexture();
            texture.LoadImage(image.Data, image.Width, image.Height);
            cache.Set(key, texture);

            return texture;
        }

        private int CalcKey(string path, IRenderer renderer)
        {
            return HashCode.Combine(path.GetHashCode(), renderer.GetHashCode());
        }

        private int CalcKey(Image image, IRenderer renderer)
        {
            return HashCode.Combine(image.GetHashCode(), renderer.GetHashCode());
        }
    }
}