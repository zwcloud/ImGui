using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;

namespace IMGUI
{
    public struct Style
    {
        public Style(uint fontColor, uint normal, uint hover, uint active)
	    {
            FontColor = CarioExtension.FromArgb(fontColor);
            Normal = CarioExtension.FromArgb(normal);
            Hover = CarioExtension.FromArgb(hover);
            Active = CarioExtension.FromArgb(active);
	    }

        public Color FontColor;
	    public Color Normal, Hover, Active;

        public static Style Default = new Style(0xFF000000, 0xFFCCCCCC, 0xFF0000FF, 0xFF00FF00);
    }
}
