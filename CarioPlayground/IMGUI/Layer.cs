using System.Collections.Generic;
using Cairo;

namespace IMGUI
{
    internal static class Layer
    {
        internal static Context FrontContext { get; set; }
        internal static Surface FrontSurface { get; set; }

        internal static Context BackContext { get; set; }
        internal static Surface BackSurface { get; set; }

        internal static Context TopContext { get; set; }
        internal static Surface TopSurface { get; set; }
    }
}