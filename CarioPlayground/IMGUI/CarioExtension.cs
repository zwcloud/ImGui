using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cairo;

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
        public static void DrawBoxModel(this Context g, Rect rect, Content content, Style style, Font font, StyleStateType type)
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
            var btl = new PointD(rect.Left, rect.Top);
            var btr = new PointD(rect.Right, rect.Top);
            var bbr = new PointD(rect.Right, rect.Bottom);
            var bbl = new PointD(rect.Left, rect.Bottom);

            //4 corner of the padding-box
            var ptl = new PointD(btl.X + bl, btl.Y + bt);
            var ptr = new PointD(btr.X - br, btr.Y + bt);
            var pbr = new PointD(bbr.X - br, bbr.Y - bb);
            var pbl = new PointD(bbl.X + bl, bbl.Y - bb);

            //4 corner of the content-box
            var ctl = new PointD(ptl.X + pl, ptl.Y + pt);
            var ctr = new PointD(ptr.X - pr, ptr.Y + pr);
            var cbr = new PointD(pbr.X - pr, pbr.Y - pb);
            var cbl = new PointD(pbl.X + pl, pbl.Y - pb);

            /*
             * Render from inner to outer: Content, Padding, Border, Margin, Outline
             */

            //Content(draw as a filled rectangle now)
            Color backgroundColor;
            switch (type)
            {
                case StyleStateType.Active:
                    backgroundColor = style.Active.BackgroundColor;
                    break;
                case StyleStateType.Hover:
                    backgroundColor = style.Hover.BackgroundColor;
                    break;
                default:
                    backgroundColor = style.Normal.BackgroundColor;
                    break;
            }
            FillRectangle(g, rect, backgroundColor);

            //Border
            //  Top
            FillPolygon(g,
                new PointD[] { ptl, btl, btr, ptr }, style.BorderTopColor);
            //  Right
            FillPolygon(g,
                new PointD[] { ptr, btr, bbr, pbr }, style.BorderRightColor);
            //  Bottom
            FillPolygon(g,
                new PointD[] { pbr, bbr, bbl, pbl }, style.BorderBottomColor);
            //  Left
            FillPolygon(g,
                new PointD[] { pbl, bbl, btl, ptl }, style.BorderLeftColor);


            //Check state of the style
            switch (type)
            {
                case StyleStateType.Active:
                    font.Color = style.Active.FontColor;
                    font.Weight = style.Active.FontWeight;
                    break;
                case StyleStateType.Hover:
                    font.Color = style.Hover.FontColor;
                    font.Weight = style.Hover.FontWeight;
                    break;
                default:
                    font.Color = style.Normal.FontColor;
                    font.Weight = style.Normal.FontWeight;
                    break;
            }


            if (content.Image != null)
            {
                g.SetSourceSurface(content.Image._surface, (int)rect.TopLeft.X, (int)rect.TopLeft.Y );
                g.Paint();
            }

            /*
             * TODO Replace DrawText with proper method
             */
            if (content.Text != null)
            {
                g.DrawText(rect, content.Text, font, style.TextStyle);
            }
        }

        #region primitive draw helpers

        private static void MakeRectangePath(this Context g, Rect rect)
        {
            g.MoveTo(rect.TopLeft);
            g.LineTo(rect.TopRight); //Top
            g.LineTo(rect.BottomRight); //Right
            g.LineTo(rect.BottomLeft); //Bottom
            g.LineTo(rect.TopLeft); //Left
            g.ClosePath();
        }

        public static void StrokeRectangle(Context g,
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
        
        private static void FillRectangle(Context g,
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

        public static void FillPolygon(Context g, PointD[] vPoint, Color color)
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

        public static void FillExtentsOfRectangle(Context g,
            Color topColor,
            Color rightColor,
            Color bottomColor,
            Color leftColor,
            PointD topLeft,
            PointD topRight,
            PointD bottomRight,
            PointD bottomLeft,
            float top,
            float Right,
            float bottom,
            float Left)
        {
            //TODO See WinFormCario project!
            //Top
            
            g.MoveTo(topLeft);
            g.SetSourceColor(topColor);
            g.LineTo(topRight);
            g.LineTo(bottomRight);
            g.LineTo(bottomLeft);
            g.LineTo(topLeft);
            g.ClosePath();
            g.FillExtents();

            g.Fill();
        }
#endregion


        #region text

        public static void DrawText(this Context g, Rect rect, string text, Font font)
        {
            //drawing text's bottom-left corner is in the bottom-left of the rect
            g.SelectFontFace(font.Family, font.Slant, font.Weight);
            g.SetFontSize(font.Size);
            g.SetSourceColor(font.Color);
            g.MoveTo(rect.Left, rect.Y + 0.5 * rect.Height + 0.5 * font.Size);
            g.ShowText(text);
        }

        public static void DrawText(this Context g, Rect rect, string text, Font font, TextStyle textStyle)
        {
            g.SelectFontFace(font.Family, font.Slant, font.Weight);
            g.SetFontSize(font.Size);
            g.SetSourceColor(font.Color);
            Point p;
            if (textStyle.TextAlign == TextAlignment.Left)
            {
                //drawing text's bottom-left corner is in the bottom-left of the rect
                p = new Point(rect.Left, rect.Y + 0.5*rect.Height + 0.5*font.Size);
            }
            else
            {
                var extents = g.TextExtents(text);
                p = new Point(
                    rect.X + rect.Width/2 - (extents.Width/2 + extents.XBearing),
                    rect.Y + rect.Height/2 - (extents.Height/2 + extents.YBearing));
            }
            g.MoveTo(p);
            g.ShowText(text);
        }

        #endregion

        #endregion

        #region Basic definitions

        public static readonly Color ColorBlack = new Color(0, 0, 0, 0xff);
        public static readonly Color ColorWhite = new Color(0xff, 0xff, 0xff, 0xff);
        public static readonly Color ColorMetal = new Color();
        
        

        #endregion
        
        #region Color
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
#endregion

        #region Image


        public static ImageSurface CreateImage(this Context g, string pngFilePath)
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
