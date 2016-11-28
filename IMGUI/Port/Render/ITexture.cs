using System;

namespace ImGui
{
    public interface ITexture : IDisposable
    {
        bool LoadImage(byte[] data);
        bool LoadImage(string filePath);

        /// <summary>
        /// Width of the texture in pixels. (Read Only)
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of the texture in pixels. (Read Only)
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Filtering mode of the texture.
        /// </summary>
        FilterMode FilterMode { get; set; }

        /// <summary>
        /// Wrap mode (Repeat or Clamp) of the texture.
        /// </summary>
        TextureWrapMode WrapMode { get; set; }

        /// <summary>
        /// Retrieve a native (underlying graphics API) pointer to the texture resource.
        /// </summary>
        /// <returns>
        /// The id of the OpenGL texture object, converted to an `IntPtr`.
        /// </returns>
        IntPtr GetNativeTexturePtr();

        /// <summary>
        /// Retrieve an graphics-API-specific id of the texture resource.
        /// </summary>
        /// <returns>
        /// The id of the OpenGL texture object.
        /// </returns>
        int GetNativeTextureID();
    }
}