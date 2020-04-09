using System;
using Cairo;

namespace ImGui.UnitTest
{
    public static class CairoEx
    {
        public static void QuadraticTo(this Context g,
                     double x1, double y1,
                     double x2, double y2)
        {
            var currentPoint = g.CurrentPoint;
            var x0 = currentPoint.X;
            var y0 = currentPoint.Y;
            g.CurveTo(
                2.0 / 3.0 * x1 + 1.0 / 3.0 * x0,
                2.0 / 3.0 * y1 + 1.0 / 3.0 * y0,
                2.0 / 3.0 * x1 + 1.0 / 3.0 * x2,
                2.0 / 3.0 * y1 + 1.0 / 3.0 * y2,
                x2, y2);
        }

        internal static void StrokeRectangle(this Cairo.Context g,
            Rect rect,
            Cairo.Color topColor,
            Cairo.Color rightColor,
            Cairo.Color bottomColor,
            Cairo.Color leftColor)
        {
            g.NewPath();
            g.MoveTo(rect.TopLeft.X, rect.TopLeft.Y);
            g.SetSourceColor(topColor);
            g.LineTo(rect.TopRight.X, rect.TopRight.Y); //Top
            g.SetSourceColor(rightColor);
            g.LineTo(rect.BottomRight.X, rect.BottomRight.Y); //Right
            g.SetSourceColor(bottomColor);
            g.LineTo(rect.BottomLeft.X, rect.BottomRight.Y); //Bottom
            g.SetSourceColor(leftColor);
            g.LineTo(rect.TopLeft.X, rect.TopLeft.Y); //Left
            g.ClosePath();
            g.Stroke();
        }

        internal static void StrokeRectangle(this Cairo.Context g,
            Rect rect,
            Cairo.Color color)
        {
            g.NewPath();
            g.MoveTo(rect.TopLeft.X, rect.TopLeft.Y);
            g.SetSourceColor(color);
            g.LineTo(rect.TopRight.X, rect.TopRight.Y); //Top
            g.LineTo(rect.BottomRight.X, rect.BottomRight.Y); //Right
            g.LineTo(rect.BottomLeft.X, rect.BottomLeft.Y); //Bottom
            g.LineTo(rect.TopLeft.X, rect.TopLeft.Y); //Left
            g.ClosePath();
            g.Stroke();
        }

        internal static void FillRectangle(this Cairo.Context g,
            Rect rect,
            Cairo.Color color)
        {
            g.NewPath();
            g.MoveTo(rect.TopLeft.X, rect.TopLeft.Y);
            g.SetSourceColor(color);
            g.LineTo(rect.TopRight.X, rect.TopRight.Y);
            g.LineTo(rect.BottomRight.X, rect.BottomRight.Y);
            g.LineTo(rect.BottomLeft.X, rect.BottomLeft.Y);
            g.LineTo(rect.TopLeft.X, rect.TopLeft.Y);
            g.ClosePath();
            g.Fill();
        }

        internal static void DrawArrow(this Cairo.Context g, Point p0, Point p1)
        {
            var x0 = p0.X;
            var y0 = p0.Y;
            var x1 = p1.X;
            var y1 = p1.Y;

            var dx = x1 - x0;
            var dy = y1 - y0;

            if (MathEx.AmostZero(dx) && MathEx.AmostZero(dy))
            {
                return;
            }

            var n0 = new Vector(-dy, dx); n0.Normalize();
            var n1 = new Vector(dy, -dx); n1.Normalize();

            var B = new Point(x1, y1);
            var d = new Vector(x0 - x1, y0 - y1); d.Normalize();

            var arrowEnd0 = B + 20 * (d + n0);
            var arrowEnd1 = B + 20 * (d + n1);
            g.MoveTo(x1, y1);
            g.LineTo(new Cairo.PointD(arrowEnd0.X, arrowEnd0.Y));
            g.MoveTo(x1, y1);
            g.LineTo(new Cairo.PointD(arrowEnd1.X, arrowEnd1.Y));
            g.MoveTo(x1, y1);
        }


        #region Color

        public static readonly Cairo.Color ColorClear = ColorArgb(0, 0, 0, 0);
        public static readonly Cairo.Color ColorBlack = ColorRgb(0, 0, 0);
        public static readonly Cairo.Color ColorWhite = ColorRgb(255, 255, 255);
        public static readonly Cairo.Color ColorMetal = ColorRgb(192, 192, 192);
        public static readonly Cairo.Color ColorRed = ColorRgb(255, 0, 0);
        public static readonly Cairo.Color ColorGreen = ColorRgb(0, 255, 0);
        public static readonly Cairo.Color ColorBlue = ColorRgb(0, 0, 255);
        public static readonly Cairo.Color ColorLightBlue = ColorRgb(46, 167, 224);
        public static readonly Cairo.Color ColorDarkBlue = ColorRgb(3, 110, 184);
        public static readonly Cairo.Color ColorPink = ColorRgb(255, 192, 203);
        public static readonly Cairo.Color ColorOrange = ColorRgb(255, 165, 0);

        public static void SetSourceColor(this Cairo.Context context, Cairo.Color color)
        {
            context.SetSourceRGBA(color.R,color.G,color.B,color.A);
        }

        public static Cairo.Color ColorRgb(byte r, byte g, byte b)
        {
            return new Cairo.Color(r/255.0,g/255.0,b/255.0,1.0);
        }

        public static Cairo.Color ColorArgb(byte a, byte r, byte g, byte b)
        {
            return new Cairo.Color(r/255.0,g/255.0,b/255.0,a/255.0);
        }

        public static Cairo.Color ColorArgb(uint colorValue)
        {
            return ColorArgb(
                (byte) ((colorValue >> 24) & 0xff),
                (byte) ((colorValue >> 16) & 0xff),
                (byte) ((colorValue >> 8) & 0xff),
                (byte) (colorValue & 0xff)
                );
        }

        public static Cairo.Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if(hi == 0)
                return CairoEx.ColorArgb(255, v, t, p);
            if(hi == 1)
                return CairoEx.ColorArgb(255, q, v, p);
            if(hi == 2)
                return CairoEx.ColorArgb(255, p, v, t);
            if(hi == 3)
                return CairoEx.ColorArgb(255, p, q, v);
            if(hi == 4)
                return CairoEx.ColorArgb(255, t, p, v);

            return CairoEx.ColorArgb(255, v, p, q);
        }

        public static Cairo.Color AlphaBlend(Cairo.Color backgroundColor, Cairo.Color foregroundColor)
        {
            double k = foregroundColor.A;
            Cairo.Color outputColor = new Cairo.Color(
                foregroundColor.R*k + backgroundColor.R*(1.0 - k),
                foregroundColor.G*k + backgroundColor.G*(1.0 - k),
                foregroundColor.B*k + backgroundColor.B*(1.0 - k)
                );
            return outputColor;
        }
        #endregion

        public static Cairo.Color ToCairoColor(this Color color)
        {
            return new Cairo.Color(color.R, color.G, color.B, color.A);
        }

        public static PointD ToPointD(this Point point)
        {
            return new PointD(point.x, point.y);
        }
    }
}
