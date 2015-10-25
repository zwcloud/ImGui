using System.Collections.Generic;
using Cairo;

namespace IMGUI
{
    internal class RenderContext
    {
        public Context FrontContext { get; set; }
        public Surface FrontSurface { get; set; }

        public Context BackContext { get; set; }
        public Surface BackSurface { get; set; }

        public bool IsReady { get { return this.BackContext != null && this.FrontContext != null; } }
        
        /// <summary>
        /// Draw the back surface to the front surface
        /// </summary>
        /// <remarks>
        /// The front surface's data will be used as a texture in OpenGL. TODO
        /// </remarks>
        public void SwapSurface()
        {
            this.BackSurface.Flush();
            this.FrontContext.SetSourceSurface(this.BackSurface, 0, 0);
            this.FrontContext.Paint();
        }
    }
}