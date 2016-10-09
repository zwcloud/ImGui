using Cairo;
using System;

namespace ImGui
{
    internal class RenderContext : IDisposable
    {
        public RenderContext(int clientWidth, int clientHeight)
        {
            Build(clientWidth, clientHeight);
        }

        public Context FrontContext { get; set; }
        public ImageSurface FrontSurface { get; set; }

        public Context BackContext { get; set; }
        public ImageSurface BackSurface { get; set; }

        public Context DebugContext { get; set; }
        public ImageSurface DebugSurface { get; set; }

        /// <summary>
        /// Build the render contect
        /// </summary>
        /// <param name="clientWidth"></param>
        /// <param name="clientHeight"></param>
        public void Build(int clientWidth, int clientHeight)
        {
            this.BackSurface = CairoEx.BuildSurface(clientWidth, clientHeight,
                CairoEx.ColorWhite, Format.Argb32);
            this.BackContext = new Context(this.BackSurface);
            this.FrontSurface = CairoEx.BuildSurface(clientWidth, clientHeight,
                CairoEx.ColorWhite, Format.Argb32);
            this.FrontContext = new Context(this.FrontSurface);

            this.DebugSurface = CairoEx.BuildSurface(clientWidth, clientHeight,
                CairoEx.ColorClear, Format.Argb32);
            this.DebugContext = new Context(this.DebugSurface);
        }

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