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

        public void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
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
            var figure = new PathFigure();
            figure.StartPoint = new Point(rect.TopLeft.X, rect.TopLeft.Y + cornerRadius.TopLeft);
            figure.Segments.Add(new ArcSegment(
                point: new Point(rect.TopLeft.X + cornerRadius.TopLeft, rect.TopLeft.Y),
                size: new Size(cornerRadius.TopLeft, cornerRadius.TopLeft),
                rotationAngle: 0,
                isLargeArc: false,
                sweepDirection: SweepDirection.Clockwise,
                isStroked: true
            ));
            figure.Segments.Add(new LineSegment(rect.TopRight - new Vector(cornerRadius.TopRight, 0), true));
            figure.Segments.Add(new ArcSegment(
                point: new Point(rect.TopRight.X, rect.TopRight.Y + cornerRadius.TopRight),
                size: new Size(cornerRadius.TopRight, cornerRadius.TopRight),
                rotationAngle: 0,
                isLargeArc: false,
                sweepDirection: SweepDirection.Clockwise,
                isStroked: true
            ));
            figure.Segments.Add(new LineSegment(rect.BottomRight - new Vector(0, cornerRadius.BottomRight), true));
            figure.Segments.Add(new ArcSegment(
                point: new Point(rect.BottomRight.X - cornerRadius.BottomRight, rect.BottomRight.Y),
                size: new Size(cornerRadius.BottomRight, cornerRadius.BottomRight),
                rotationAngle: 0,
                isLargeArc: false,
                sweepDirection: SweepDirection.Clockwise,
                isStroked: true
            ));
            figure.Segments.Add(new LineSegment(rect.BottomLeft + new Vector(cornerRadius.BottomLeft, 0), true));
            figure.Segments.Add(new ArcSegment(
                point: new Point(rect.BottomLeft.X, rect.BottomLeft.Y - cornerRadius.BottomLeft),
                size: new Size(cornerRadius.BottomLeft, cornerRadius.BottomLeft),
                rotationAngle: 0,
                isLargeArc: false,
                sweepDirection: SweepDirection.Clockwise,
                isStroked: true
            ));
            figure.Segments.Add(new LineSegment(new Point(rect.TopLeft.X, rect.TopLeft.Y + cornerRadius.TopLeft), true));

            dc.DrawGeometry(brush, pen, geometry);
        }

        public void DrawBoxModel()
        {
            var rect = ownerNode.Rect;
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
                var border = style.Border;
                var borderColor = style.BorderColor;
                var borderRadius = style.BorderRadius;
#if TODO
                PathGeometry geometry = new PathGeometry();
                PathGeometryContext g = new PathGeometryContext(geometry);

                //start from top-left
                var bl = border.left;
                var bt = border.top;
                Point topLeftLowerEndPoint;
                {
                    var r = borderRadius.TopLeft;
                    var arcCenter = borderBoxRect.TopLeft + new Vector(r, r);
                    var paddingIntersectionPoint = borderBoxRect.TopLeft + new Vector(bl, bt);
                    var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(-135));
                    if (bl < r && r < bt || bt < r && r < bl || r < bl && bl < bt || r < bt && bt < bl
                        || r == bt && bt == bl)
                    {
                        g.MoveTo(paddingIntersectionPoint);
                        g.LineTo(halfArcPoint);
                        g.Arc(arcCenter, r, MathEx.Deg2Rad(-135), MathEx.Deg2Rad(-90));
                        topLeftLowerEndPoint = paddingIntersectionPoint;
                    }
                    else// if (bl < bt && bt < r || bt<bl && bl <r)//inner ellipse curve occurs
                    {
                        var ellipseCenter = arcCenter;
                        var ellipseXRadius = r - bl;
                        var ellipseYRadius = r - bt;
                        topLeftLowerEndPoint = paddingIntersectionPoint + new Vector(ellipseXRadius, 0);
                        g.MoveTo(topLeftLowerEndPoint);
                        g.Ellipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-90), MathEx.Deg2Rad(-135));
                        g.LineTo(halfArcPoint);
                        g.Arc(arcCenter, r, MathEx.Deg2Rad(-135), MathEx.Deg2Rad(-90));
                    }
                }

                //top upper line: connect left to right
                g.LineTo(borderBoxRect.TopRight + new Vector(-borderRadius.TopRight, 0));

                //to top-right
                var br = border.right;
                {
                    var r = borderRadius.TopRight;
                    var arcCenter = borderBoxRect.TopRight + new Vector(-r, r);
                    var paddingIntersectionPoint = borderBoxRect.TopRight + new Vector(-bl, bt);
                    if (br < r && r < bt || bt < r && r < br || r < br && br < bt || r < bt && bt < br
                        || r == bt && bt == br)
                    {
                        g.Arc(arcCenter, r, MathEx.Deg2Rad(-90), MathEx.Deg2Rad(-45));
                        g.LineTo(paddingIntersectionPoint);
                    }
                    else //(br < bt && bt < r || bt<br && br <r)//inner ellipse curve occurs
                    {
                        var ellipseCenter = arcCenter;
                        var ellipseXRadius = r - br;
                        var ellipseYRadius = r - bt;
                        g.Arc(arcCenter, r, MathEx.Deg2Rad(-90), MathEx.Deg2Rad(-45));
                        g.LineTo(MathEx.EvaluateEllipse(arcCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-45)));
                        g.Ellipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-45), MathEx.Deg2Rad(-90));
                    }
                }

                //top lower line: connect left to right
                g.LineTo(topLeftLowerEndPoint);

                dc.DrawGeometry(new Brush(borderColor.top), new Pen(Color.Black, 2), geometry);
#endif
            }
        }

        private void DrawBackground(Rect paddingBoxRect)
        {
            var style = ownerNode.RuleSet;
            // draw background in padding-box
            var gradient = (Gradient) style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.BackgroundColor;
                DrawRectangle(new Brush(bgColor), null, paddingBoxRect);
                //TODO apply border-radius clip, which is determined by how border corners are rendered
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
