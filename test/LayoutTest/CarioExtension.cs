using System;
using ImGui;

namespace Test
{
    public static class CairoEx
    {
        #region basic types

        public static Cairo.PointD ToPointD(this Point p)
        {
            return new Cairo.PointD(p.X, p.Y);
        }

        #endregion


        #region primitive

        internal static void StrokeRectangle(this Cairo.Context g,
            Rect rect,
            Color topColor,
            Color rightColor,
            Color bottomColor,
            Color leftColor)
        {
            g.NewPath();
            g.MoveTo(rect.TopLeft.ToPointD());
            g.SetSourceColor(topColor);
            g.LineTo(rect.TopRight.ToPointD()); //Top
            g.SetSourceColor(rightColor);
            g.LineTo(rect.BottomRight.ToPointD()); //Right
            g.SetSourceColor(bottomColor);
            g.LineTo(rect.BottomLeft.ToPointD()); //Bottom
            g.SetSourceColor(leftColor);
            g.LineTo(rect.TopLeft.ToPointD()); //Left
            g.ClosePath();
            g.Stroke();
        }

        internal static void StrokeRectangle(this Cairo.Context g,
            Rect rect,
            Color color)
        {
            g.NewPath();
            g.MoveTo(rect.TopLeft.ToPointD());
            g.SetSourceColor(color);
            g.LineTo(rect.TopRight.ToPointD()); //Top
            g.LineTo(rect.BottomRight.ToPointD()); //Right
            g.LineTo(rect.BottomLeft.ToPointD()); //Bottom
            g.LineTo(rect.TopLeft.ToPointD()); //Left
            g.ClosePath();
            g.Stroke();
        }

        internal static void FillRectangle(this Cairo.Context g,
            Rect rect,
            Color color)
        {
            g.NewPath();
            g.MoveTo(rect.TopLeft.ToPointD());
            g.SetSourceColor(color);
            g.LineTo(rect.TopRight.ToPointD());
            g.LineTo(rect.BottomRight.ToPointD());
            g.LineTo(rect.BottomLeft.ToPointD());
            g.LineTo(rect.TopLeft.ToPointD());
            g.ClosePath();
            g.Fill();
        }

        internal static void FillPolygon(this Cairo.Context g, Point[] vPoint, Color color)
        {
            if (vPoint.Length <= 2)
            {
                throw new ArgumentException("vPoint should contains 3 ore more points!");
            }

            g.NewPath();
            g.SetSourceColor(color);
            g.MoveTo(vPoint[0].ToPointD());
            foreach (var t in vPoint)
            {
                g.LineTo(t.ToPointD());
            }
            g.ClosePath();
            g.Fill();
        }

        internal static void StrokeLineStrip(this Cairo.Context g, Point[] vPoint, Color color)
        {
            if (vPoint.Length <= 2)
            {
                throw new ArgumentException("vPoint should contains 3 ore more points!");
            }

            g.NewPath();
            g.SetSourceColor(color);
            g.MoveTo(vPoint[0].ToPointD());
            foreach (var t in vPoint)
            {
                g.LineTo(t.ToPointD());
            }
            g.Stroke();
        }

        internal static void StrokePolygon(this Cairo.Context g, Point[] vPoint, Color color)
        {
            if (vPoint.Length <= 2)
            {
                throw new ArgumentException("vPoint should contains 3 ore more points!");
            }

            g.NewPath();
            g.SetSourceColor(color);
            g.MoveTo(vPoint[0].ToPointD());
            foreach (var t in vPoint)
            {
                g.LineTo(t.ToPointD());
            }
            g.ClosePath();
            g.Stroke();
        }

        internal static void StrokeCircle(this Cairo.Context g, Point center, float radius, Color color)
        {
            g.NewPath();
            g.LineWidth = 1;
            g.SetSourceColor(color);
            g.Arc(center.X, center.Y, radius, 0, 2 * Math.PI);
            g.Stroke();
        }

        internal static void FillCircle(this Cairo.Context g, Point center, float radius, Color color)
        {
            g.NewPath();
            g.SetSourceColor(color);
            g.Arc(center.X, center.Y, radius, 0, 2 * Math.PI);
            g.Fill();
        }

        internal static void DrawLine(this Cairo.Context g, Point p0, Point p1, float width, Color color)
        {
            g.NewPath();
            g.Save();
            g.SetSourceColor(color);
            g.LineWidth = width;
            g.MoveTo(p0.ToPointD());
            g.LineTo(p1.ToPointD());
            g.Stroke();
            g.Restore();
        }
        #endregion

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

        #region Image
        //TODO specfiy image fill method (fill, unscale, 9-box, etc.) and alignment
        //public static void DrawImage(this Cairo.Context g, Rect destRect, Rect srcRect, Texture image)
        //{
        //    float xScale = (float)destRect.Width / (float)srcRect.Width;
        //    float yScale = (float)destRect.Height / (float)srcRect.Height;
        //    g.Save();
        //    g.Translate(destRect.X, destRect.Y);
        //    g.Scale(xScale, yScale);
        //    g.SetSource(image._surface, 0, 0);
        //    g.Rectangle(destRect.TopLeft.ToPointD(), destRect.Width, destRect.Height);
        //    g.Paint();
        //    g.Restore();
        //}

        //public static void DrawImage(this Cairo.Context g, Rect rect, Texture image)
        //{
        //    g.DrawImage(rect, new Rect(new Size(image.Width, image.Height)), image);
        //}

        public static Cairo.ImageSurface CreateImage(string pngFilePath)
        {
            Cairo.ImageSurface surface;
            try
            {
                surface = new Cairo.ImageSurface(pngFilePath);
            }
            catch (Exception)
            {
                return null;
            }
            return surface;
        } 

        #endregion

        static CairoEx()
        {
        }

        /// <summary>
        /// Build a ImageSurface
        /// </summary>
        /// <param name="Width">width</param>
        /// <param name="Height">height</param>
        /// <param name="Color">color</param>
        /// <param name="format">surface format</param>
        /// <returns>the created ImageSurface</returns>
        public static Cairo.ImageSurface BuildSurface(int Width, int Height, Color Color, Cairo.Format format)
        {
            var surface = new Cairo.ImageSurface(format, Width, Height);
            var c = new Cairo.Context(surface);
            c.Rectangle(0, 0, Width, Height);
            c.SetSourceColor(Color);
            c.Fill();
            c.Dispose();
            return surface;
        }
    }

}