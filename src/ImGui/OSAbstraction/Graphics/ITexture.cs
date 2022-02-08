using System;

namespace ImGui.OSAbstraction.Graphics
{
    /// <summary>
    /// Texture-related functions
    /// </summary>
    public interface ITexture : IDisposable
    {
        /// <summary>
        /// Load image data from in R8G8B8A8 format bytes into the texture
        /// </summary>
        void LoadImage(byte[] data, int width, int height);

        /// <summary>
        /// Load image data from a file into the texture.
        /// </summary>
        /// <returns>succeeded?true:false</returns>
        void LoadImage(string filePath);

        /// <summary>
        /// Width of the texture in pixels. (Read Only)
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of the texture in pixels. (Read Only)
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Size of the texture. (Read Only)
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Retrieve a native (underlying graphics API) pointer to the texture resource.
        /// </summary>
        /// <returns>
        /// e.g. The id of the OpenGL texture object, converted to an `IntPtr`.
        /// </returns>
        IntPtr GetNativeTexturePtr();

        /// <summary>
        /// Retrieve an graphics-API-specific id of the texture resource.
        /// </summary>
        /// <returns>
        /// e.g. The id of the OpenGL texture object.
        /// </returns>
        int GetNativeTextureId();

        /// <summary>
        /// Retrieve an graphics-API-specific object of the texture resource.
        /// </summary>
        /// <returns>
        /// e.g. The WebGLTexture object (JSObject).
        /// </returns>
        object GetNativeTextureObject();

        bool Valid { get; }
    }
}