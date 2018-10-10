using System;
using System.Collections.Generic;
using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    internal class TextureUtil
    {
        private static readonly Dictionary<int, ITexture> TextureCache = new Dictionary<int, ITexture>();

        private static int GetTextureHash(string path)
        {
            int hash = 17;
            hash = hash * 23 + path.GetHashCode();
            return hash;
        }

        public static ITexture GetTexture(string filePath)
        {
            int id = GetTextureHash(filePath);

            ITexture texture;
            if(!TextureCache.TryGetValue(id, out texture))
            {
                texture = Application.PlatformContext.CreateTexture();
                texture.LoadImage(filePath);
                TextureCache.Add(id, texture);
            }

            return texture;
        }
    }
}
