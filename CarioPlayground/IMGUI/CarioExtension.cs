using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;

namespace IMGUI
{
    static class CarioExtension
    {
        /// <summary>
        /// Create a Color from a 32 bit integar
        /// </summary>
        /// <param name="color"></param>
        /// <param name="colorValue"></param>
        public static Color FromArgb(uint colorValue)
        {
            var result = new Color();
            result.A = ((colorValue >> 24) & 0xff) / 255.0f;
            result.R = ((colorValue >> 16) & 0xff) / 255.0f;
            result.G = ((colorValue >> 8) & 0xff) / 255.0f;
            result.B = (colorValue & 0xff) / 255.0f;
            return result;
        }


#region Basic element rendering
        #region rectangle
        /// <summary>
        /// Fill a rectangle
        /// </summary>
        /// <param name="style">Style of the rectangle</param>
        /// <param name="aX">x position</param>
        /// <param name="aY">y position</param>
        /// <param name="aWidth">width</param>
        /// <param name="aHeight">height</param>
        public static void FillRectangle(this Context g, Style style, int aX, int aY, int aWidth, int aHeight)
        {
            PointD topLeft = new PointD(aX, aY);
            PointD topRight = new PointD(aX + aWidth, aY);
            PointD bottomRight = new PointD(aX + aWidth, aY + aHeight);
            PointD bottomLeft = new PointD(aX, aY + aHeight);

            g.SetSourceColor(style.Normal);
            if (Rect.inside(Input.MousePos, aX, aY, 0, 0, aWidth, aHeight))
            {
                g.SetSourceColor(style.Hover);
            }
            if (Input.LeftButtonState == InputState.Down)
            {
                g.SetSourceColor(style.Active);
            }
            g.MoveTo(topLeft);
            g.LineTo(topRight);
            g.LineTo(bottomRight);
            g.LineTo(bottomLeft);
            g.LineTo(topLeft);
            g.ClosePath();
            g.Fill();
        }

        /// <summary>
        /// Stroke a rectangle
        /// </summary>
        /// <param name="style">Style of the rectangle</param>
        /// <param name="aX">x position</param>
        /// <param name="aY">y position</param>
        /// <param name="aWidth">width</param>
        /// <param name="aHeight">height</param>
        public static void StrokeRectangle(this Context g, Style style, int aX, int aY, int aWidth, int aHeight)
        {
            PointD topLeft = new PointD(aX, aY);
            PointD topRight = new PointD(aX + aWidth, aY);
            PointD bottomRight = new PointD(aX + aWidth, aY + aHeight);
            PointD bottomLeft = new PointD(aX, aY + aHeight);

            g.SetSourceColor(style.Normal);
            g.MoveTo(topLeft);
            g.LineTo(topRight);
            g.LineTo(bottomRight);
            g.LineTo(bottomLeft);
            g.LineTo(topLeft);
            g.ClosePath();
            g.Stroke();
        }

        #endregion
        public static void DrawText(this Content g, Style style, int x, int y)
        {

        }
        #region text


        #endregion

#endregion


    }
}
