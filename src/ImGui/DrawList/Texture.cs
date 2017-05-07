using System;
using System.Collections.Generic;

namespace ImGui
{
    class TextureUtil
    {
        static Dictionary<int, ITexture> TextureCache = new Dictionary<int, ITexture>();

        static int GetTextureHash(string path)
        {
            int hash = 17;
            hash = hash * 23 + path.GetHashCode();
            return hash;
        }

        internal static ITexture GetTexture(string filePath)
        {
            int id = GetTextureHash(filePath);

            ITexture texture;
            if(!TextureCache.TryGetValue(id, out texture))
            {
                texture = Application.platformContext.CreateTexture();
                texture.LoadImage(filePath);
            }

            return texture;
        }
    }
}
