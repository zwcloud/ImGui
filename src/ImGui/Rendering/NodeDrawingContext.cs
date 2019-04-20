using System;
using System.Diagnostics;

namespace ImGui.Rendering
{
    internal class NodeDrawingContext : IDisposable
    {
        public NodeDrawingContext(Node node)
        {
            ownerNode = node;
            dc = new VisualDrawingContext(node);
        }

        public void DrawLine(Point point0, Point point1)
        {
            var rule = ownerNode.RuleSet;
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            dc.DrawLine(pen, point0, point1);
        }

        public void DrawRectangle(Rect rectangle)
        {
            var rule = ownerNode.RuleSet;
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            Brush brush = new Brush(rule.FillColor);
            dc.DrawRectangle(brush, pen, rectangle);
        }

        public void DrawRoundedRectangle(Rect rect)
        {
            var rule = ownerNode.RuleSet;
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            Brush brush = new Brush(rule.FillColor);
            var cornerRadius = rule.BorderRadius;
            DrawRoundedRectangle(brush, pen, rect, cornerRadius);
        }

        public void DrawRoundedRectangle(Brush brush, Pen pen, Rect rect,
            (double TopLeft, double TopRight, double BottomRight, double BottomLeft) cornerRadius)
        {
            var geometry = new PathGeometry();
            using (var context = geometry.Open())
            {
                context.ArcFast(
                    new Point(rect.TopLeft.X + cornerRadius.TopLeft, rect.TopLeft.Y + cornerRadius.TopLeft),
                    cornerRadius.TopLeft, 6, 9);
                context.LineTo(rect.TopRight - new Vector(cornerRadius.TopRight, 0));
                context.ArcFast(
                    new Point(rect.TopRight.X - cornerRadius.TopRight, rect.TopRight.Y + cornerRadius.TopRight),
                    cornerRadius.TopRight, 9, 12);
                context.LineTo(rect.BottomRight - new Vector(0, cornerRadius.BottomRight));
                context.ArcFast(
                    new Point(rect.BottomRight.X - cornerRadius.BottomRight, rect.BottomRight.Y - cornerRadius.BottomRight),
                    cornerRadius.BottomRight, 0, 3);
                context.LineTo(rect.BottomLeft + new Vector(cornerRadius.BottomLeft, 0));
                context.ArcFast(new Point(rect.BottomLeft.X + cornerRadius.BottomLeft, rect.BottomLeft.Y - cornerRadius.BottomLeft),
                    cornerRadius.BottomLeft, 3, 6);
                context.Finish();
            }
            var rule = ownerNode.RuleSet;
            dc.DrawGeometry(brush, pen, geometry);
        }

        public void DrawBoxModel(Rect rect)
        {
            var style = ownerNode.RuleSet;
            GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            this.DrawBackground(paddingBoxRect);

            //Content
            //Content-box
            //no content

            this.DrawBorder(borderBoxRect, paddingBoxRect);
            this.DrawOutline(borderBoxRect);

            this.DrawDebug(paddingBoxRect, contentBoxRect);
        }

        private void DrawOutline(Rect borderBoxRect)
        {
            var style = ownerNode.RuleSet;
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth);
            if (MathEx.AmostZero(outlineWidth)) return;
            var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor);
            if (MathEx.AmostZero(outlineColor.A)) return;
            dc.DrawRectangle(null, new Pen(outlineColor, outlineWidth), borderBoxRect);
        }

        private void DrawBorder(Rect borderBoxRect, Rect paddingBoxRect)
        {
            var style = ownerNode.RuleSet;
            // draw border between border-box and padding-box
            var borderImageSource = style.BorderImageSource;
            if (borderImageSource != null)
            {
                var rule = style.GetRule<string>(GUIStyleName.BorderImageSource);
                if (rule.Geometry == null)
                {
                    rule.Geometry = new ImageGeometry(borderImageSource);
                }

                Debug.Assert(rule.Geometry is ImageGeometry);
                //TODO this.DrawSlicedImage((ImageGeometry) rule.Geometry, borderBoxRect, style);
            }
            else
            {
                //  Top
                if (!MathEx.AmostZero(borderBoxRect.Top))
                {
                    var borderTopColor = style.Get<Color>(GUIStyleName.BorderTopColor);
                    if (!MathEx.AmostZero(borderTopColor.A))
                    {
                        PathGeometry geometry = new PathGeometry();
                        PathGeometryContext ctx = new PathGeometryContext(geometry);
                        ctx.LineTo(paddingBoxRect.TopLeft);
                        ctx.LineTo(borderBoxRect.TopLeft);
                        ctx.LineTo(borderBoxRect.TopRight);
                        ctx.LineTo(paddingBoxRect.TopRight);
                        ctx.Finish();
                        dc.DrawGeometry(new Brush(borderTopColor), null, geometry);
                    }
                }

                //  Right
                if (!MathEx.AmostZero(borderBoxRect.Right))
                {
                    var borderRightColor = style.Get<Color>(GUIStyleName.BorderRightColor);
                    if (!MathEx.AmostZero(borderRightColor.A))
                    {
                        PathGeometry geometry = new PathGeometry();
                        PathGeometryContext ctx = new PathGeometryContext(geometry);
                        ctx.LineTo(paddingBoxRect.TopRight);
                        ctx.LineTo(borderBoxRect.TopRight);
                        ctx.LineTo(borderBoxRect.BottomRight);
                        ctx.LineTo(paddingBoxRect.BottomRight);
                        ctx.Finish();
                        dc.DrawGeometry(new Brush(borderRightColor), null, geometry);
                    }
                }

                //  Bottom
                if (!MathEx.AmostZero(borderBoxRect.Bottom))
                {
                    var borderBottomColor = style.Get<Color>(GUIStyleName.BorderBottomColor);
                    if (!MathEx.AmostZero(borderBottomColor.A))
                    {
                        PathGeometry geometry = new PathGeometry();
                        PathGeometryContext ctx = new PathGeometryContext(geometry);
                        ctx.LineTo(paddingBoxRect.BottomRight);
                        ctx.LineTo(borderBoxRect.BottomRight);
                        ctx.LineTo(borderBoxRect.BottomLeft);
                        ctx.LineTo(paddingBoxRect.BottomLeft);
                        ctx.Finish();
                        dc.DrawGeometry(new Brush(borderBottomColor), null, geometry);
                    }
                }

                //  Left
                if (!MathEx.AmostZero(borderBoxRect.Left))
                {
                    var borderLeftColor = style.Get<Color>(GUIStyleName.BorderLeftColor);
                    if (!MathEx.AmostZero(borderLeftColor.A))
                    {
                        PathGeometry geometry = new PathGeometry();
                        PathGeometryContext ctx = new PathGeometryContext(geometry);
                        ctx.LineTo(paddingBoxRect.BottomLeft);
                        ctx.LineTo(borderBoxRect.BottomLeft);
                        ctx.LineTo(borderBoxRect.TopLeft);
                        ctx.LineTo(paddingBoxRect.TopLeft);
                        ctx.Finish();
                        dc.DrawGeometry(new Brush(borderLeftColor), null, geometry);
                    }
                }
            }
        }

        private void DrawBackground(Rect paddingBoxRect)
        {
            var style = ownerNode.RuleSet;
            // draw background in padding-box
            var gradient = (Gradient) style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.Get<Color>(GUIStyleName.BackgroundColor);
                var borderRadius = style.BorderRadius;
                DrawRoundedRectangle(new Brush(bgColor), null, paddingBoxRect, borderRadius);
            }
            else if (gradient == Gradient.TopBottom)
            {
                var topColor = style.Get<Color>(GUIStyleName.GradientTopColor);
                var bottomColor = style.Get<Color>(GUIStyleName.GradientBottomColor);
                //TODO this.AddRectFilledGradient(paddingBoxRect, topColor, bottomColor);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        public void Close()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(VisualDrawingContext));
            }

            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            if (!disposed)
            {
                dc.Close();
                disposed = true;
            }
        }

        private void DrawDebug(Rect paddingBoxRect, Rect contentBoxRect)
        {
#if DrawPaddingBox
            this.PathRect(paddingBoxRect.TopLeft, paddingBoxRect.BottomRight);
            this.PathStroke(Color.Rgb(0, 100, 100), true, 1);
#endif

#if DrawContentBox
            this.PathRect(contentBoxRect.TopLeft, cbr);
            this.PathStroke(Color.Rgb(100, 0, 100), true, 1);
#endif
        }

        //TODO remove out parameter borderBoxRect
        private static void GetBoxes(Rect rect, StyleRuleSet style, out Rect borderBoxRect, out Rect paddingBoxRect,
            out Rect contentBoxRect)
        {
            //Widths of border
            var bt = style.Get<double>(GUIStyleName.BorderTop);
            var br = style.Get<double>(GUIStyleName.BorderRight);
            var bb = style.Get<double>(GUIStyleName.BorderBottom);
            var bl = style.Get<double>(GUIStyleName.BorderLeft);

            //Widths of padding
            var pt = style.Get<double>(GUIStyleName.PaddingTop);
            var pr = style.Get<double>(GUIStyleName.PaddingRight);
            var pb = style.Get<double>(GUIStyleName.PaddingBottom);
            var pl = style.Get<double>(GUIStyleName.PaddingLeft);

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var btr = new Point(rect.Right, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);
            var bbl = new Point(rect.Left, rect.Bottom);
            borderBoxRect = new Rect(btl, bbr);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);
            Debug.Assert(ptl.X < ptr.X);//TODO what if (ptl.X > ptr.X) happens?
            paddingBoxRect = new Rect(ptl, pbr);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            if (ctl.X >= ctr.X)
            {
                Log.Warning("Content box is zero-sized.");
                contentBoxRect = new Rect(ctl, Size.Zero);
            }
            else
            {
                contentBoxRect = new Rect(ctl, cbr);
            }
        }

        private bool disposed;
        private readonly Node ownerNode;
        private readonly VisualDrawingContext dc;
    }
}
