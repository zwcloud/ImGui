using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;

namespace IMGUI
{
    public class GUI
    {
        Context g;
        public GUI(Context context)
        {
            g = context;
        }

        public bool Button(Style style, int aX, int aY,
                       int aWidth, int aHeight,
                       string aText)
        {
            g.FillRectangle(style, aX, aY, aWidth, aHeight);
            g.MoveTo(aX, aY);
            g.ShowText(aText);

            return Input.LeftButtonClicked &&
                   Input.MousePos.X >= aX &&
                   Input.MousePos.Y >= aY &&
                   Input.MousePos.X < (aX + aWidth) &&
                   Input.MousePos.Y < (aY + aHeight);
        }

        public void Label(Style style, int x, int y, string text)
        {
            g.SelectFontFace("serif", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(18);
            g.SetSourceColor(style.FontColor);
            g.MoveTo(x, y);
            g.ShowText(text);
        }
    }
}
