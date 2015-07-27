using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cairo;
using Pango;
using Color = Cairo.Color;
using Context = Cairo.Context;

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
            g.FillPolygon(new PointD[] { ptl, btl, btr, ptr }, style.BorderTopColor);
            //  Right
            g.FillPolygon(new PointD[] { ptr, btr, bbr, pbr }, style.BorderRightColor);
            //  Bottom
            g.FillPolygon(new PointD[] { pbr, bbr, bbl, pbl }, style.BorderBottomColor);
            //  Left
            g.FillPolygon(new PointD[] { pbl, bbl, btl, ptl }, style.BorderLeftColor);

            if (content.Image != null)
            {
                //TODO Draw the image at a proper position
                g.DrawImage(contentBoxRect, content.Image);
            }

            if (content.Text != null)
            {
                //TODO Draw the text at a proper position
                g.DrawText(contentBoxRect, content.Text, style.Font, style.TextStyle);
            }

            if(content.Text!=null && content.CaretIndex!=0)
            {
                g.DrawTextEx(contentBoxRect, content.Text, style.Font, style.TextStyle, content.CaretIndex);
            }

        }

        #region primitive draw helpers

        public static void StrokeRectangle(this Context g,
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
        
        private static void FillRectangle(this Context g,
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

        public static void FillPolygon(this Context g, PointD[] vPoint, Color color)
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

        public static void StrokePolygon(this Context g, PointD[] vPoint, Color color)
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

        public static void StrokeCircle(this Context g, PointD center, float radius, Color color)
        {
            g.NewPath();
            g.SetSourceColor(color);
            g.Arc(center.X, center.Y, radius, 0, 2 * Math.PI);
            g.Stroke();
        }

        public static void FillCircle(this Context g, PointD center, float radius, Color color)
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
        public static void DrawText(this Context g, Rect rect, string text, Font font, TextStyle textStyle)
        {
#if USE_TOY
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
#else
            //TODO solve these code mess
            Layout l = CairoHelper.CreateLayout(g);
            l.SetText(text);
            l.FontDescription = font.Description;
            CairoHelper.UpdateLayout(g, l);
            l.Alignment = textStyle.TextAlign;
            l.Width = (int)(rect.Width*Pango.Scale.PangoScale);
            g.SetSourceColor(font.Color);
            Point p = rect.TopLeft;
            g.MoveTo(p);
            CairoHelper.ShowLayout(g, l);
#endif
        }

        public static void DrawTextEx(this Context g, Rect rect, string text, Font font, TextStyle textStyle,
            int caretIndex)
        {
            //TODO solve these code mess
            Layout l = CairoHelper.CreateLayout(g);
            l.SetText(text);
            l.FontDescription = font.Description;
            CairoHelper.UpdateLayout(g, l);
            l.Alignment = textStyle.TextAlign;
            l.Width = (int)(rect.Width * Pango.Scale.PangoScale);
            g.SetSourceColor(font.Color);
            Point p = rect.TopLeft;
            g.MoveTo(p);
            CairoHelper.ShowLayout(g, l);

            //TODO draw selection
            //Pango.Rectangle rightStrongRect, rightRect;
            //l.GetCursorPos(caretIndex, out rightStrongRect, out rightRect);
            //Pango.Rectangle leftStrongRect, leftRect;
            //l.GetCursorPos(caretIndex, out leftStrongRect, out leftRect);
            //var r = new Cairo.Rectangle(Pango.Units.ToPixels(leftRect.X), Pango.Units.ToPixels(leftRect.Y),
            //    Pango.Units.ToPixels(rightRect.X + rightRect.Width - leftRect.X),
            //    Pango.Units.ToPixels(leftRect.Height));

            #region Draw caret
            //BUG caret not show up when caretIndex is 0 or the length of text
            //TODO make caret flash
            Pango.Rectangle strongCursorPosFromPango, weakCursorPosFromPango;
            l.GetCursorPos(caretIndex,
                out strongCursorPosFromPango, out weakCursorPosFromPango);
            var caretTopPoint = new Point(Pango.Units.ToPixels(strongCursorPosFromPango.X), Pango.Units.ToPixels(strongCursorPosFromPango.Y));
            var caretBottomPoint = new Point(Pango.Units.ToPixels(strongCursorPosFromPango.X), Pango.Units.ToPixels(strongCursorPosFromPango.Y + strongCursorPosFromPango.Height));
            caretTopPoint.Offset(p.X, p.Y);
            caretBottomPoint.Offset(p.X, p.Y);
            g.DrawLine(caretTopPoint, caretBottomPoint, 1.0f, CairoEx.ColorBlack);
            #endregion
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
#endregion

        #region Image

        public static void DrawImage(this Context g, Rect rect, Texture image)
        {
            g.SetSourceSurface(image._surface, (int)rect.X, (int)rect.Y);
            g.MoveTo(rect.TopLeft);
            g.LineTo(rect.TopRight); //Top
            g.LineTo(rect.BottomRight); //Right
            g.LineTo(rect.BottomLeft); //Bottom
            g.LineTo(rect.TopLeft); //Left
            g.ClosePath();
            g.Fill();
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
