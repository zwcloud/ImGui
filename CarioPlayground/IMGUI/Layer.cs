using System.Collections.Generic;
using Cairo;

namespace IMGUI
{
    internal class Layer
    {
        internal Context FrontContext { get; set; }
        internal Surface FrontSurface { get; set; }

        internal Context BackContext { get; set; }
        internal Surface BackSurface { get; set; }
    }
}