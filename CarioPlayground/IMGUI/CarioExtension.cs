using System;
using Cairo;
using TinyIoC;
using Context = Cairo.Context;
using Layout = IMGUI.ITextLayout;

namespace IMGUI
{
    public static class CairoEx
    {
        #region Basic element rendering

        /// <summary>
        /// Draw a box model
        /// </summary>
        /// <param name="g">the Cairo context</param>
        /// <param name="rect">the rect (of the border-box) in which to draw this box model </param>
        /// <param name="content">content of the box mode</param>
        /// <param name="style">style of the box model</param>
        public static void DrawBoxModel(this Context g, Rect rect, Content content, Style style)
        {
            //Widths of border
            var bt = style.BorderTop;
            var br = style.BorderRight;
            var bb = style.BorderBottom;
            var bl = style.BorderLeft;

            //Widths of padding
            var pt = style.PaddingTop;
            var pr = style.PaddingRight;
            var pb = style.PaddingBottom;
            var pl = style.PaddingLeft;
            
            /*
             * TODO Outline is temporarily not used.
             */

            /*
             * TODO Margin is temporarily not used.
             */
            var mt = style.MarginTop;
            var mr = style.MarginRight;
            var mb = style.MarginBottom;
            var ml = style.MarginLeft;

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var btr = new Point(rect.Right, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);
            var bbl = new Point(rect.Left, rect.Bottom);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            var contentBoxRect = new Rect(ctl, cbr);

            /*
             * Render from inner to outer: Content, Padding, Border, Margin, Outline
             */

            //Content(draw as a filled rectangle now)
            g.FillRectangle(rect, style.BackgroundStyle.Color);

            //Border
            //  Top
            g.FillPolygon(new Point[] { ptl, btl, btr, ptr }, style.BorderTopColor);
            //  Right
            g.FillPolygon(new Point[] { ptr, btr, bbr, pbr }, style.BorderRightColor);
            //  Bottom
            g.FillPolygon(new Point[] { pbr, bbr, bbl, pbl }, style.BorderBottomColor);
            //  Left
            g.FillPolygon(new Point[] { pbl, bbl, btl, ptl }, style.BorderLeftColor);

            if(content != null)
            {
                if (content.Image != null)
                {
                    //g.DrawImage(contentBoxRect, content.Image);
                }
                if (content.Text!=null)
                {
                    g.DrawText(contentBoxRect, content.Text, style.Font, style.TextStyle);
                }
                if (content.Layout != null)
                {
                    g.DrawText(contentBoxRect, content.Layout, style.Font, style.TextStyle);
                }
            }
        }

        #region primitive draw helpers

        internal static void StrokeRectangle(this Context g,
            Rect rect,
            Color topColor,
            Color rightColor,
            Color bottomColor,
            Color leftColor)
        {
            g.MoveTo(rect.TopLeft);
            g.SetSourceColor(topColor);
            g.LineTo(rect.TopRight); //Top
            g.SetSourceColor(rightColor);
            g.LineTo(rect.BottomRight); //Right
            g.SetSourceColor(bottomColor);
            g.LineTo(rect.BottomLeft); //Bottom
            g.SetSourceColor(leftColor);
            g.LineTo(rect.TopLeft); //Left
            g.ClosePath();
            g.Stroke();
        }

        internal static void FillRectangle(this Context g,
            Rect rect,
            Color color)
        {
            g.MoveTo(rect.TopLeft);
            g.SetSourceColor(color);
            g.LineTo(rect.TopRight);
            g.LineTo(rect.BottomRight);
            g.LineTo(rect.BottomLeft);
            g.LineTo(rect.TopLeft);
            g.ClosePath();
            g.Fill();
        }

        internal static void FillPolygon(this Context g, Point[] vPoint, Color color)
        {
            if (vPoint.Length <= 2)
            {
                throw new ArgumentException("vPoint should contains 3 ore more points!");
            }

            g.SetSourceColor(color);
            g.MoveTo(vPoint[0]);
            foreach (var t in vPoint)
            {
                g.LineTo(t);
            }
            g.ClosePath();
            g.Fill();
        }

        internal static void StrokePolygon(this Context g, Point[] vPoint, Color color)
        {
            if (vPoint.Length <= 2)
            {
                throw new ArgumentException("vPoint should contains 3 ore more points!");
            }

            g.SetSourceColor(color);
            g.MoveTo(vPoint[0]);
            foreach (var t in vPoint)
            {
                g.LineTo(t);
            }
            g.ClosePath();
            g.Stroke();
        }

        internal static void StrokeCircle(this Context g, PointD center, float radius, Color color)
        {
            g.NewPath();
            g.SetSourceColor(color);
            g.Arc(center.X, center.Y, radius, 0, 2 * Math.PI);
            g.Stroke();
        }

        internal static void FillCircle(this Context g, PointD center, float radius, Color color)
        {
            g.NewPath();
            g.SetSourceColor(color);
            g.Arc(center.X, center.Y, radius, 0, 2 * Math.PI);
            g.Fill();
        }

        internal static void DrawLine(this Context g, Point p0, Point p1, float width, Color color)
        {
            g.Save();
            g.SetSourceColor(color);
            g.LineWidth = width;
            g.MoveTo(p0);
            g.LineTo(p1);
            g.Stroke();
            g.Restore();
        }
        #endregion


        #region text
        /// <summary>
        /// Draw short text
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect">region to draw the text</param>
        /// <param name="text">text</param>
        /// <param name="font">font</param>
        /// <param name="textStyle">text styles</param>
        public static void DrawText(this Context g, Rect rect, string text, Font font, TextStyle textStyle)
        {
            using(ITextFormat textFormat = Application.IocContainer.Resolve<ITextFormat>(
                new NamedParameterOverloads
                {
                    { "fontFamilyName", font.FontFamily },
                    { "fontWeight", font.FontWeight },
                    { "fontStyle", font.FontStyle },
                    { "fontStretch", font.FontStretch },
                    { "fontSize", (float)font.Size}
                }))
            using(
            ITextLayout textLayout = Application.IocContainer.Resolve<ITextLayout>(
                new NamedParameterOverloads
                {
                    { "text", text },
                    { "textFormat", textFormat },
                    { "maxWidth", (float)rect.Width },
                    { "maxHeight", (float)rect.Height }
                }))
            {
                g.SetSourceColor(font.Color);
                Point p = rect.TopLeft;
                g.MoveTo(p);
                textLayout.Show(g);
            }
        }

        /// <summary>
        /// Draw layouted text
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="layout"></param>
        /// <param name="font"></param>
        /// <param name="textStyle"></param>
        public static void DrawText(this Context g, Rect rect, Layout layout, Font font, TextStyle textStyle)
        {
            g.SetSourceColor(font.Color);
            Point p = rect.TopLeft;
            g.MoveTo(p);
            layout.Show(g);
        }
        #endregion

        #endregion

        #region Basic definitions

        public static readonly Color ColorBlack = new Color(0, 0, 0, 0xff);
        public static readonly Color ColorWhite = new Color(0xff, 0xff, 0xff, 0xff);
        public static readonly Color ColorMetal = new Color(0xff, 192, 192, 192);
        
        

        #endregion
        
        #region Color

        public static void SetSourceColor(this Context context, Color color)
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
#endregion

        #region Image
        //TODO specfiy image fill method (fill, unscale, 9-box, etc.) and alignment
        public static void DrawImage(this Context g, Rect destRect, Rect srcRect, Texture image)
        {
            float xScale = (float)destRect.Width / (float)srcRect.Width;
            float yScale = (float)destRect.Height / (float)srcRect.Height;
            g.Translate(destRect.X, destRect.Y);
            g.Scale(xScale, yScale);
            g.SetSourceSurface(image._surface, 0, 0);
            g.Rectangle(destRect.TopLeft, destRect.Width, destRect.Height);
            g.ClosePath();
            g.Paint();
            g.IdentityMatrix();
        }

        public static void DrawImage(this Context g, Rect rect, Texture image)
        {
            g.DrawImage(rect, new Rect(new Size(image.Width, image.Height)), image);
        }

        public static ImageSurface CreateImage(string pngFilePath)
        {
            ImageSurface surface;
            try
            {
                surface = new ImageSurface(pngFilePath);
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

    }

}
