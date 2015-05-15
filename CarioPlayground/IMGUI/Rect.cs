using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;

namespace IMGUI
{
    class Rect
    {
        public static void gradient(Context g, Style aStyle, int aX, int aY, int aWidth, int aHeight)
        {
            g.FillRectangle(aStyle, aX, aY, aWidth, aHeight);
        }

	    public static bool inside(Point aPoint, int anOriginX, int anOriginY, int aX, int aY, int aWidth, int aHeight)
        {
	        int CPX = aPoint.X - anOriginX, CPY = aPoint.Y - anOriginY;
	        return CPX > aX && CPY > aY && CPX < (aX + aWidth) && CPY < (aY + aHeight);
        }

	    public void set(bool aDown, Style aStyle, int aX, int aY, int aWidth, int aHeight)
        {
	        myInside = false;
	        myDown = aDown;
	        myStyle = aStyle;
	        myX.set(aX);
	        myY.set(aY);
	        myWidth.set(aWidth);
	        myHeight.set(aHeight);
        }

	    public void render(Context g, int anOriginX, int anOriginY)
        {
	        myX.lerp();
	        myY.lerp();
	        myWidth.lerp();
	        myHeight.lerp();

	        int X = myX.get();
            int Y = myY.get();
			int WIDTH = myWidth.get();
			int HEIGHT = myHeight.get();

	        if(myInside)
	        {
		        if(myDown)
		        {
			        g.FillRectangle(myStyle, X + anOriginX, Y + anOriginY, WIDTH - 1, HEIGHT - 1);
		        }
		        else
		        {
			        g.FillRectangle(myStyle, X + anOriginX, Y + anOriginY, WIDTH - 1, HEIGHT - 1);
		        }
	        }
	        else
	        {
		        g.FillRectangle(myStyle, X + anOriginX, Y + anOriginY, WIDTH - 1, HEIGHT - 1);
	        }
	        g.StrokeRectangle(myStyle.Normal, X + anOriginX, Y + anOriginY, WIDTH - 1, HEIGHT - 1);
        }

	    public bool inside(Point aPoint, int anOriginX, int anOriginY, bool aSet)
        {
            if (inside(aPoint, anOriginX, anOriginY, myX.get(), myY.get(), myWidth.get(), myHeight.get()))
            {
                myInside = aSet;
                return true;
            }
            else
            {
                return false;
            }
        }

	    bool myInside = false, myDown;
        Style myStyle = Style.Default;
	    Scalar myX = new Scalar(), myY = new Scalar(), myWidth = new Scalar(), myHeight = new Scalar();
	    Color myColor;
    }
}
