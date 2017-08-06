using System;
using System.IO;
using System.Runtime.InteropServices;
using CSharpGL;
using ImGui.Common.Primitive;

namespace ImGui
{
    internal class OpenGLTexture : ITexture
    {
        private ImageSharp.Image<ImageSharp.Rgba32> image;
        private readonly uint[] textureIdBuffer = {0};
        private ImageSharp.Rgba32[] textureData;
        
        public void LoadImage(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Load an image from a file.
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadImage(string filePath)
        {
            // check file header, save texture data to buffer
            using (FileStream stream = File.OpenRead(filePath))
            {
                this.image = ImageSharp.Image.Load<ImageSharp.Rgba32>(stream);
                this.textureData = this.image.Pixels.ToArray();
            }

            // create opengl texture object
            GL.ActiveTexture(GL.GL_TEXTURE0);
            GL.GenTextures(1, this.textureIdBuffer);
            var textureHandle = this.textureIdBuffer[0];
            GL.BindTexture(GL.GL_TEXTURE_2D, textureHandle);
            var textureDataPtr = Marshal.UnsafeAddrOfPinnedArrayElement(this.textureData, 0);
            GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, this.Width, this.Height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, textureDataPtr);
            //sampler settings
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
            Utility.CheckGLError();
        }

        /// <summary>
        /// Width of the texture in pixels. (Read Only)
        /// </summary>
        public int Width => this.image.Width;

        /// <summary>
        /// Height of the texture in pixels. (Read Only)
        /// </summary>
        public int Height => this.image.Height;

        /// <summary>
        /// Size of the texture in pixels. (Read Only)
        /// </summary>
        public Size Size => new Size(this.Width, this.Height);

        /// <summary>
        /// Retrieve a native (underlying graphics API) pointer to the texture resource.
        /// </summary>
        /// <returns>
        /// The id of the OpenGL texture object, converted to an `IntPtr`.
        /// </returns>
        public IntPtr GetNativeTexturePtr()
        {
            IntPtr result = new IntPtr(this.textureIdBuffer[0]);
            return result;
        }

        /// <summary>
        /// Retrieve an graphics-API-specific id of the texture resource.
        /// </summary>
        /// <returns>
        /// The id of the OpenGL texture object.
        /// </returns>
        public int GetNativeTextureId()
        {
            return (int) this.textureIdBuffer[0];
        }

#region Implementation of IDisposable

        public void Dispose()
        {
            GL.DeleteTextures(1, this.textureIdBuffer);
            Utility.CheckGLError();
        }

#endregion
    }
}