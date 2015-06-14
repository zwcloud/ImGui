using System;
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
        public static void DrawBoxModel(this Context g, Rect rect, Content content, Style style, Font font)
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
            FillRectangle(g, rect, style.BackgroundColor);

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

            /*
             * TODO Show picture here
             */
            g.DrawText(rect, content.Text, font);
        }

        #region primitive draw helpers
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
            //TODO draw text in the middle of the rect
            g.SelectFontFace(font.Family, font.Slant, font.Weight);
            g.SetFontSize(font.Size);
            g.SetSourceColor(font.Color);
            g.MoveTo(rect.BottomLeft);
            g.ShowText(text);
            g.Stroke();
        }
        #endregion

#endregion

        #region Basic definitions

        public static readonly Color ColorBlack = new Color(0, 0, 0, 0xff);
        public static readonly Color ColorWhite = new Color(0xff, 0xff, 0xff, 0xff);
        
        #endregion
    }
}
