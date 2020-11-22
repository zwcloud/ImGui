using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;

namespace ImGui.Rendering
{
    internal static partial class DrawingContextEx
    {
        public static void DrawLine(this DrawingContext dc, StyleRuleSet rule, Point point0, Point point1)
        {
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            dc.DrawLine(pen, point0, point1);
        }

        public static void DrawRectangle(this DrawingContext dc, StyleRuleSet rule, Rect rectangle)
        {
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            Brush brush = new Brush(rule.FillColor);
            dc.DrawRectangle(brush, pen, rectangle);
        }

        public static void DrawRectangle(this DrawingContext dc, StyleRuleSet rule, Brush brush, Pen pen, Rect rectangle)
        {
            dc.DrawRectangle(brush, pen, rectangle);
        }

        public static void DrawRoundedRectangle(this DrawingContext dc, StyleRuleSet rule, Rect rect)
        {
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            Brush brush = new Brush(rule.FillColor);
            var cornerRadius = rule.BorderRadius;
            dc.DrawRoundedRectangle(brush, pen, rect, cornerRadius);
        }

        public static void DrawRoundedRectangle(this DrawingContext dc, Brush brush, Pen pen, Rect rect,
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
            dc.DrawGeometry(brush, pen, geometry);
        }

        public static void DrawImage(this DrawingContext dc, string path)
        {
            var texture = TextureCache.Default.GetOrAdd(path, Form.current.renderer);
            dc.DrawImage(texture, new Rect(texture.Width, texture.Height), Point.Zero, Point.One);
        }

        public static void DrawImage(this DrawingContext dc, string path, Rect rect)
        {
            var texture = TextureCache.Default.GetOrAdd(path, Form.current.renderer);
            dc.DrawImage(texture, rect, Point.Zero, Point.One);
        }

        //TODO consider cache GlyphRun and FormattedText

        internal static void DrawGlyphRun(this DrawingContext dc, string text,
            double fontSize, string fontFamily, Color fontColor, Point topLeft)
        {
            var ascent = OSImplementation.TypographyTextContext.GetAscent(fontFamily, fontSize);
            var baselineOrigin = new Point(topLeft.X, topLeft.Y + ascent);
            dc.DrawGlyphRun(new Brush(fontColor),
                new GlyphRun(baselineOrigin, text, fontFamily, fontSize));
        }

        public static void DrawGlyphRun(this DrawingContext dc, StyleRuleSet rule, string text, Point topLeft)
        {
            var ascent = OSImplementation.TypographyTextContext.GetAscent(rule.FontFamily, rule.FontSize);
            var baselineOrigin = new Point(topLeft.X, topLeft.Y + ascent);
            dc.DrawGlyphRun(new Brush(rule.FontColor),
                new GlyphRun(baselineOrigin, text, rule.FontFamily, rule.FontSize));
        }

        public static void DrawText(this DrawingContext dc, StyleRuleSet rule, string text, Rect rect)
        {
            var ascent = OSImplementation.TypographyTextContext.GetAscent(rule.FontFamily, rule.FontSize);
            var baselineOrigin = new Point(rect.X, rect.Y + ascent);
            dc.DrawText(new Brush(rule.FontColor),
                new FormattedText(baselineOrigin, text, rule.FontFamily, rule.FontSize));
        }

        public static void DrawBoxModel(this DrawingContext dc, StyleRuleSet rule, Rect rect)
        {
            if (rect.IsZero)
            {
                return;
            }

            BoxModelUtil.GetBoxes(rect, rule, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            DrawBackground(dc, rule, paddingBoxRect);

            //Content
            //Content-box
            //no content

            DrawBorder(dc, rule, borderBoxRect, paddingBoxRect);
            DrawOutline(dc, rule, borderBoxRect);

            DrawDebug(dc, rule, paddingBoxRect, contentBoxRect);
        }

        public static void DrawBoxModel(this DrawingContext dc, string text, StyleRuleSet rule, Rect rect)
        {
            if (rect.IsZero)
            {
                return;
            }

            var style = rule;
            BoxModelUtil.GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            DrawBackground(dc, style, paddingBoxRect);

            //Content
            //Content-box
            if (!string.IsNullOrEmpty(text))
            {
                //TODO reuse alignment logic in StackLayout to layout text content inside the content box
                //See Metrics.LayoutTextInRectCentered
                if (text.Contains('\n'))
                {
                    dc.DrawText(rule, text, contentBoxRect);
                }
                else
                {
                    dc.DrawGlyphRun(rule, text, contentBoxRect.TopLeft);
                }
            }

            DrawBorder(dc, style, borderBoxRect, paddingBoxRect);
            DrawOutline(dc, style, borderBoxRect);

            DrawDebug(dc, style, paddingBoxRect, contentBoxRect);
        }

        public static void DrawBoxModel(this DrawingContext dc, ImGui.OSAbstraction.Graphics.ITexture texture, StyleRuleSet style, Rect rect)
        {
            if (rect.IsZero)
            {
                return;
            }

            BoxModelUtil.GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            DrawBackground(dc, style, paddingBoxRect);

            //Content
            //Content-box
            var slice = style.BorderImageSlice;
            if (slice.Item1 != 0 || slice.Item2 != 0 || slice.Item3 != 0 || slice.Item4 != 0)
            {
                dc.DrawImage(texture, contentBoxRect, style.BorderImageSlice);
            }
            else
            {
                var objectPosition = style.ObjectPosition;
                (double offsetX, double offsetY) = objectPosition;
                if (!double.IsInfinity(offsetX) && !double.IsInfinity(offsetY))
                {
                    (int w, int h) = (texture.Width, texture.Height);
                    var uvMin = new Point(offsetX / w, offsetY / h);
                    var uvMax = new Point((offsetX + contentBoxRect.Width) / w,
                        (offsetY + contentBoxRect.Height) / h);
                    dc.DrawImage(texture, contentBoxRect, uvMin, uvMax);
                }
                else
                {
                    dc.DrawImage(texture, contentBoxRect, Point.Zero, Point.One);
                }
            }

            DrawBorder(dc, style, borderBoxRect, paddingBoxRect);
            DrawOutline(dc, style, borderBoxRect);

            DrawDebug(dc, style, paddingBoxRect, contentBoxRect);
        }

        public static void DrawBoxModel(this DrawingContext dc, Node node)
        {
            DrawBoxModel(dc, node.RuleSet, node.Rect);
        }

        public static void DrawRectangleRing(this DrawingContext dc, Rect outerRect, Rect innerRect,
            Pen pen, Brush brush)
        {
            PathGeometryBuilder b = new PathGeometryBuilder();
            var A = outerRect;
            var B = innerRect; 
            b.MoveTo(B.TopLeft);
            b.LineTo(A.TopLeft);
            b.LineTo(A.TopRight);
            b.LineTo(B.TopRight);
            b.LineTo(B.TopLeft);
            b.Fill();

            b.MoveTo(B.TopRight);
            b.LineTo(A.TopRight);
            b.LineTo(A.BottomRight);
            b.LineTo(B.BottomRight);
            b.LineTo(B.TopRight);
            b.Fill();

            b.MoveTo(B.BottomRight);
            b.LineTo(A.BottomRight);
            b.LineTo(A.BottomLeft);
            b.LineTo(B.BottomLeft);
            b.LineTo(B.BottomRight);
            b.Fill();

            b.MoveTo(B.BottomLeft);
            b.LineTo(A.BottomLeft);
            b.LineTo(A.TopLeft);
            b.LineTo(B.TopLeft);
            b.LineTo(B.BottomLeft);

            var geometry = b.ToGeometry();
            dc.DrawGeometry(brush, null, geometry);

            dc.DrawRectangle(null, pen, outerRect);
            dc.DrawRectangle(null, pen, innerRect);
        }

        public static void PushClip(this DrawingContext dc, Rect clipRect)
        {
            dc.PushClip(new RectangleGeometry(clipRect));
        }
    }
}
