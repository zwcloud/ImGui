using Cairo;
using System;

namespace ImGui
{
    internal class RenderContext : IDisposable
    {
        public Context FrontContext { get; set; }
        public ImageSurface FrontSurface { get; set; }

        public Context BackContext { get; set; }
        public ImageSurface BackSurface { get; set; }

        public Context DebugContext { get; set; }
        public ImageSurface DebugSurface { get; set; }

        /// <summary>
        /// Draw the back surface to the front surface
        /// </summary>
        /// <remarks>
        /// The front surface's data will be used as a texture in OpenGL.
        /// </remarks>
        public void SwapSurface()
        {
            this.BackSurface.Flush();
            this.FrontContext.SetSourceSurface(this.BackSurface, 0, 0);
            this.FrontContext.Paint();
        }

        public void Dispose()
        {
            FrontContext.Dispose();
            FrontSurface.Dispose();
            BackContext.Dispose();
            BackSurface.Dispose();
            DebugSurface.Dispose();
        }
    }
}