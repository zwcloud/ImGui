using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImGui
{
    internal class GUIPrimitive
    {
        internal void RenderFrame(Point p_min, Point p_max, Color fill_col, bool border, float rounding)
        {
            Form window = Form.current;
            window.DrawList.AddRectFilled(p_min, p_max, fill_col, (int)rounding);
            if (border)
            {
                window.DrawList.AddRect(p_min + new Vector(1, 1), p_max + new Vector(1, 1), Color.Black, rounding);
                window.DrawList.AddRect(p_min, p_max, Color.Black, rounding);
            }
        }

        /// <summary>
        /// Draw a box model
        /// </summary>
        /// <param name="g">the Cairo context</param>
        /// <param name="rect">the rect (of the border-box) in which to draw this box model </param>
        /// <param name="content">content of the box mode</param>
        /// <param name="style">style of the box model</param>
        public static void DrawBoxModel(Rect rect, Content content, Style style)
        {
            Form window = Form.current;
            var drawList = window.DrawList;

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

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var btr = new Point(rect.Right, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);
            var bbl = new Point(rect.Left, rect.Bottom);
            var borderBoxRect = new Rect(btl, bbr);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);
            var paddingBoxRect = new Rect(ptl, pbr);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            var contentBoxRect = new Rect(ctl, cbr);

            /*
             * Render order ??
             * 1. from inner to outer: Content, Border, Outline
             * 2. from background to foreground
             */

            // draw background in padding-box
            drawList.AddRectFilled(paddingBoxRect.TopLeft, paddingBoxRect.BottomRight, style.BackgroundStyle.Color);

            //Content
            //Content-box
            if (content != null)
            {
                if (content.Image != null)
                {
                    DrawImage(contentBoxRect, content, style);
                }
                if (content.Text != null)
                {
                    DrawText(contentBoxRect, content, style);
                }
            }

            //Border
            //  Top
            if (!MathEx.AmostZero(bt) && !MathEx.AmostZero(style.BorderTopColor.A))
            {
                drawList.PathLineTo(ptl);
                drawList.PathLineTo(btl);
                drawList.PathLineTo(btr);
                drawList.PathLineTo(ptr);
                drawList.PathFill(style.BorderTopColor);
            }
            //  Right
            if (!MathEx.AmostZero(br) && !MathEx.AmostZero(style.BorderTopColor.A))
            {
                drawList.PathLineTo(ptr);
                drawList.PathLineTo(btr);
                drawList.PathLineTo(bbr);
                drawList.PathLineTo(pbr);
                drawList.PathFill(style.BorderRightColor);
            }
            //  Bottom
            if (!MathEx.AmostZero(bb) && !MathEx.AmostZero(style.BorderTopColor.A))
            {
                drawList.PathLineTo(pbr);
                drawList.PathLineTo(bbr);
                drawList.PathLineTo(bbl);
                drawList.PathLineTo(pbl);
                drawList.PathFill(style.BorderBottomColor);
            }
            //  Left
            if (!MathEx.AmostZero(bl) && !MathEx.AmostZero(style.BorderTopColor.A))
            {
                drawList.PathLineTo(pbl);
                drawList.PathLineTo(bbl);
                drawList.PathLineTo(btl);
                drawList.PathLineTo(ptl);
                drawList.PathFill(style.BorderBottomColor);
            }

            //Outline
            if (!MathEx.AmostZero(style.OutlineWidth) && !MathEx.AmostZero(style.BorderTopColor.A))
            {
                drawList.PathRect(btl, bbr);
                drawList.PathStroke(style.OutlineColor, true, (float)style.OutlineWidth);
            }

#if DrawPaddingBox
            drawList.PathRect(ptl, pbr);
            drawList.PathStroke(Color.ColorRgb(0, 100, 100), true, 1);
#endif

#if DrawContentBox
            drawList.PathRect(ctl, cbr);
            drawList.PathStroke(Color.ColorRgb(100, 0, 100), true, 1);
#endif
        }

        public static void DrawText(Rect rect, Content content, Style style)
        {
            content.BuildText(rect, style);
            Form window = Form.current;
            var drawList = window.DrawList;

            drawList.Append(content.TextMesh);
        }

        public static void DrawImage(Rect rect, Content content, Style style)
        {
            var drawList = Form.current.DrawList;
            drawList.AddImage(content.Image, rect.TopLeft, rect.BottomRight, Point.Zero, new Point(1, 1), Color.Clear);
        }

    }
}
