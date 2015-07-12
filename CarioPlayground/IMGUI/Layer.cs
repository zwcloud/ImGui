using System.Collections.Generic;
using Cairo;

namespace IMGUI
{
    internal class Layer
    {
        internal static List<Layer> Layers { get; set; }

        static Layer()
        {
            Layers = new List<Layer>(16);
            Layers.AddRange(new Layer[16]);
        }

        internal int Name { get; set; }
        // ReSharper disable once InconsistentNaming
        internal Context g { get; set; }
        internal Surface FrontSurface { get; set; }
    }

    internal class DualSurfaceLayer : Layer
    {
        internal Surface BackSurface { get; set; }
    }
}