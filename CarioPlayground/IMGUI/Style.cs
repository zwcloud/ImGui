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
        public Style(uint anOut, uint anIn, uint anEdge)
	    {
            myOut = CarioExtension.FromArgb(anOut);
            myIn = CarioExtension.FromArgb(anIn);
            myEdge = CarioExtension.FromArgb(anEdge);
	    }

	    public Color myOut, myIn, myEdge;

        public static Style Default = new Style(0xFFEEEEEE, 0xFFCCCCCC, 0xFFFFFFFF);
    }
}
