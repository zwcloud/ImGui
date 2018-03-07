using System;
using Cairo;
using ImGui.Common.Primitive;
using Color = Cairo.Color;

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
            Color topColor,
            Color rightColor,
            Color bottomColor,
            Color leftColor)
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
            Color color)
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
            Color color)
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
        
        #region Color

        public static readonly Color ColorClear = ColorArgb(0, 0, 0, 0);
        public static readonly Color ColorBlack = ColorRgb(0, 0, 0);
        public static readonly Color ColorWhite = ColorRgb(255, 255, 255);
        public static readonly Color ColorMetal = ColorRgb(192, 192, 192);
        public static readonly Color ColorBlue = ColorRgb(0, 0, 255);
        public static readonly Color ColorLightBlue = ColorRgb(46, 167, 224);
        public static readonly Color ColorDarkBlue = ColorRgb(3, 110, 184);
        public static readonly Color ColorPink = ColorRgb(255, 192, 203);
        public static readonly Color ColorOrange = ColorRgb(255, 165, 0);

        public static void SetSourceColor(this Cairo.Context context, Color color)
        {
            context.SetSourceRGBA(color.R,color.G,color.B,color.A);
        }

        public static Color ColorRgb(byte r, byte g, byte b)
        {
            return new Color(r/255.0,g/255.0,b/255.0,1.0);
        }

        public static Color ColorArgb(byte a, byte r, byte g, byte b)
        {
            return new Color(r/255.0,g/255.0,b/255.0,a/255.0);
        }

        public static Color ColorArgb(uint colorValue)
        {
            return ColorArgb(
                (byte) ((colorValue >> 24) & 0xff),
                (byte) ((colorValue >> 16) & 0xff),
                (byte) ((colorValue >> 8) & 0xff),
                (byte) (colorValue & 0xff)
                );
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
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

        public static Color AlphaBlend(Color backgroundColor, Color foregroundColor)
        {
            double k = foregroundColor.A;
            Color outputColor = new Color(
                foregroundColor.R*k + backgroundColor.R*(1.0 - k),
                foregroundColor.G*k + backgroundColor.G*(1.0 - k),
                foregroundColor.B*k + backgroundColor.B*(1.0 - k)
                );
            return outputColor;
        }
        #endregion


    }
}
