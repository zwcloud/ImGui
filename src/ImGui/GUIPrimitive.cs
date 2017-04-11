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
        public static void DrawBoxModel(Rect rect, Content content, GUIStyle style, GUIState state = GUIState.Normal)
        {
            Form window = Form.current;
            var drawList = window.DrawList;

            //Widths of border
            var bt = style.Get<double>(GUIStyleName.BorderTop, state);
            var br = style.Get<double>(GUIStyleName.BorderRight, state);
            var bb = style.Get<double>(GUIStyleName.BorderBottom, state);
            var bl = style.Get<double>(GUIStyleName.BorderLeft, state);

            //Widths of padding
            var pt = style.Get<double>(GUIStyleName.PaddingTop, state);
            var pr = style.Get<double>(GUIStyleName.PaddingRight, state);
            var pb = style.Get<double>(GUIStyleName.PaddingBottom, state);
            var pl = style.Get<double>(GUIStyleName.PaddingLeft, state);

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
            drawList.AddRectFilled(paddingBoxRect.TopLeft, paddingBoxRect.BottomRight, style.Get<Color>(GUIStyleName.BackgroundColor, state));

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
                    DrawText(contentBoxRect, content, style, state);
                }
            }

            //Border
            //  Top
            if (!MathEx.AmostZero(bt))
            {
                var borderTopColor = style.Get<Color>(GUIStyleName.BorderTopColor, state);
                if (!MathEx.AmostZero(borderTopColor.A))
                {
                    drawList.PathLineTo(ptl);
                    drawList.PathLineTo(btl);
                    drawList.PathLineTo(btr);
                    drawList.PathLineTo(ptr);
                    drawList.PathFill(borderTopColor);
                }
            }
            //  Right
            if (!MathEx.AmostZero(br))
            {
                var borderRightColor = style.Get<Color>(GUIStyleName.BorderRightColor, state);
                if(!MathEx.AmostZero(borderRightColor.A))
                {
                    drawList.PathLineTo(ptr);
                    drawList.PathLineTo(btr);
                    drawList.PathLineTo(bbr);
                    drawList.PathLineTo(pbr);
                    drawList.PathFill(borderRightColor);
                }
            }
            //  Bottom
            if (!MathEx.AmostZero(bb))
            {
                var borderBottomColor = style.Get<Color>(GUIStyleName.BorderBottomColor, state);
                if (!MathEx.AmostZero(borderBottomColor.A))
                {
                    drawList.PathLineTo(pbr);
                    drawList.PathLineTo(bbr);
                    drawList.PathLineTo(bbl);
                    drawList.PathLineTo(pbl);
                    drawList.PathFill(borderBottomColor);
                }
            }
            //  Left
            if (!MathEx.AmostZero(bl))
            {
                var borderLeftColor = style.Get<Color>(GUIStyleName.BorderLeftColor, state);
                if (!MathEx.AmostZero(borderLeftColor.A))
                {
                    drawList.PathLineTo(pbl);
                    drawList.PathLineTo(bbl);
                    drawList.PathLineTo(btl);
                    drawList.PathLineTo(ptl);
                    drawList.PathFill(borderLeftColor);
                }
            }

            //Outline
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth, state);
            if (!MathEx.AmostZero(outlineWidth))
            {
                var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor, state);
                if(!MathEx.AmostZero(outlineColor.A))
                {
                    drawList.PathRect(btl, bbr);
                    drawList.PathStroke(outlineColor, true, outlineWidth);
                }
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

        public static void DrawText(Rect rect, Content content, GUIStyle style, GUIState state)
        {
            content.BuildText(rect, style, state);
            Form window = Form.current;
            var drawList = window.DrawList;

            drawList.Append(content.TextMesh);
        }

        public static void DrawImage(Rect rect, Content content, GUIStyle style)
        {
            var drawList = Form.current.DrawList;
            drawList.AddImage(content.Image, rect.TopLeft, rect.BottomRight, Point.Zero, new Point(1, 1), Color.White);
        }

    }
}
