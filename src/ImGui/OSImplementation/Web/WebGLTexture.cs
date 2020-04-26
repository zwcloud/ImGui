using System;
using System.IO;
using GL = ImGui.OSImplementation.Web.WebGL;
using ImGui.OSAbstraction.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using WebAssembly;

namespace ImGui.OSImplementation.Web
{
    internal class WebGLTexture : ITexture
    {
        private Image<Rgba32> image;
        private Rgba32[] textureData;
        private JSObject textureObject;

        public void LoadImage(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void LoadImage(Rgba32[] data,int width, int height)
        {
            this.textureData = data;
            this.Width = width;
            this.Height = height;

            // create opengl texture object
            GL.ActiveTexture(GL.GL_TEXTURE0);
            this.textureObject = GL.CreateTexture();
            GL.BindTexture(GL.GL_TEXTURE_2D, this.textureObject);
            //GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, this.Width, this.Height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, this.textureData);
            //sampler settings
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
            Utility.CheckWebGLError();
        }

        /// <summary>
        /// Load an image from a file.
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadImage(string filePath)
        {
            // check file header, save texture data to buffer
            using (Stream stream = Utility.ReadFile(filePath))
            {
                this.image = Image.Load<Rgba32>(stream);
                this.textureData = this.image.GetPixelSpan().ToArray();
                this.Width = this.image.Width;
                this.Height = this.image.Height;
            }

            // create opengl texture object
            GL.ActiveTexture(GL.GL_TEXTURE0);
            this.textureObject = GL.CreateTexture();
            GL.BindTexture(GL.GL_TEXTURE_2D, this.textureObject);
            //GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, this.Width, this.Height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, this.textureData);
            //sampler settings
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
            Utility.CheckWebGLError();
        }

        /// <summary>
        /// Width of the texture in pixels. (Read Only)
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the texture in pixels. (Read Only)
        /// </summary>
        public int Height { get; private set; }

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
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieve an graphics-API-specific id of the texture resource.
        /// </summary>
        /// <returns>
        /// The id of the OpenGL texture object.
        /// </returns>
        public int GetNativeTextureId()
        {
            throw new NotSupportedException();
        }

        public object GetNativeTextureObject()
        {
            return this.textureObject;
        }

        public bool Valid => GetNativeTextureId() > 0;

#region Implementation of IDisposable

        public void Dispose()
        {
            GL.DeleteTexture(this.textureObject);
            Utility.CheckWebGLError();
        }

#endregion
    }
}