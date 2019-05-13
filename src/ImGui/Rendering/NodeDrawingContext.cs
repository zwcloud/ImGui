using System.Diagnostics;

namespace ImGui.Rendering
{
    internal partial class NodeDrawingContext : VisualDrawingContext
    {
        public NodeDrawingContext(Node node) : base(node)
        {
            this.rect = node.Rect;
            this.ruleSet = node.RuleSet;
        }

        public void DrawLine(Point point0, Point point1)
        {
            var rule = this.ruleSet;
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            base.DrawLine(pen, point0, point1);
        }

        public void DrawRectangle(Rect rectangleRect)
        {
            var rule = this.ruleSet;
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            Brush brush = new Brush(rule.FillColor);
            base.DrawRectangle(brush, pen, rectangleRect);
        }

        public void DrawRoundedRectangle(Rect rectangle)
        {
            var rule = this.ruleSet;
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            Brush brush = new Brush(rule.FillColor);
            var cornerRadius = rule.BorderRadius;
            DrawRoundedRectangle(brush, pen, rectangle, cornerRadius);
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
            geometry.Figures.Add(figure);
            DrawGeometry(brush, pen, geometry);
        }

        public void DrawBoxModel()
        {
            var rectangle = this.rect;
            var style = this.ruleSet;
            GetBoxes(rectangle, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            DrawBackground(paddingBoxRect);

            //Content
            //Content-box
            //no content

            DrawBorder(borderBoxRect, paddingBoxRect);
            DrawOutline(borderBoxRect);

            DrawDebug(paddingBoxRect, contentBoxRect);
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

        private readonly StyleRuleSet ruleSet;
        private readonly Rect rect;
    }
}
