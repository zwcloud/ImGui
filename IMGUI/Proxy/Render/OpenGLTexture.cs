using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CSharpGL;

namespace ImGui
{
    public class OpenGLTexture : ITexture
    {
        public static readonly byte[] PngHeaderEightBytes =
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
        };

        private ImageSharp.Image image;
        private readonly uint[] textureIdBuffer = {0};
        private ImageSharp.Color[] textureData;
        
        public bool LoadImage(byte[] data)
        {
            throw new NotImplementedException();
        }

        public bool LoadImage(string filePath)
        {
            // check file header, save texture data to buffer
            using (var fileStream = File.Open(filePath, FileMode.Open))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                var headEightBytes = binaryReader.ReadBytes(8);
                var isPngFile = headEightBytes.SequenceEqual(PngHeaderEightBytes);
                if (!isPngFile)
                {
                    throw new NotSupportedException("Only PNG format is supported.");
                }
                image = new ImageSharp.Image(fileStream);
                textureData = image.Pixels;
            }

            // create opengl texture object
            GL.ActiveTexture(GL.GL_TEXTURE0);
            GL.GenTextures(1, textureIdBuffer);
            var textureHandle = textureIdBuffer[0];
            GL.BindTexture(GL.GL_TEXTURE_2D, textureHandle);
            var textureDataPtr = Marshal.UnsafeAddrOfPinnedArrayElement(textureData, 0);
            GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, Width, Height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, textureDataPtr);
            //sampler settings
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
            Utility.CheckGLError();

            return true;
        }

        /// <summary>
        /// Width of the texture in pixels. (Read Only)
        /// </summary>
        public int Width
        {
            get
            {
                return image.Width;
            }
        }

        /// <summary>
        /// Height of the texture in pixels. (Read Only)
        /// </summary>
        public int Height
        {
            get
            {
                return image.Height;
            }
        }

        /// <summary>
        /// Filtering mode of the texture.
        /// </summary>
        public FilterMode FilterMode
        {
            get;
            set;
        }

        /// <summary>
        /// Wrap mode (Repeat or Clamp) of the texture.
        /// </summary>
        public TextureWrapMode WrapMode
        {
            get;
            set;
        }

        /// <summary>
        /// Retrieve a native (underlying graphics API) pointer to the texture resource.
        /// </summary>
        /// <returns>
        /// The id of the OpenGL texture object, converted to an `IntPtr`.
        /// </returns>
        public IntPtr GetNativeTexturePtr()
        {
            IntPtr result = new IntPtr(textureIdBuffer[0]);
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
            return (int)textureIdBuffer[0];
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            GL.DeleteTextures(1, textureIdBuffer);
            Utility.CheckGLError();
        }

        #endregion
    }
}