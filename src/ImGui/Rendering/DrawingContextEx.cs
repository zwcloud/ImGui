﻿using ImGui.GraphicsAbstraction;
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
            var texture = TextureCache.Default.GetOrAdd(path);
            dc.DrawImage(texture, new Rect(texture.Width, texture.Height));
        }

        public static void DrawImage(this DrawingContext dc, string path, Rect rect)
        {
            var texture = TextureCache.Default.GetOrAdd(path);
            dc.DrawImage(texture, rect);
        }

        //TODO consider cache GlyphRun and FormattedText

        internal static void DrawGlyphRun(this DrawingContext dc, string text,
            double fontSize, string fontFamily, Color fontColor, Point topLeft)
        {
            var ascent = OSImplentation.TypographyTextContext.GetAscent(fontFamily, fontSize);
            var baselineOrigin = new Point(topLeft.X, topLeft.Y + ascent);
            dc.DrawGlyphRun(new Brush(fontColor),
                new GlyphRun(baselineOrigin, text, fontFamily, fontSize));
        }

        public static void DrawGlyphRun(this DrawingContext dc, StyleRuleSet rule, string text, Point topLeft)
        {
            var ascent = OSImplentation.TypographyTextContext.GetAscent(rule.FontFamily, rule.FontSize);
            var baselineOrigin = new Point(topLeft.X, topLeft.Y + ascent);
            dc.DrawGlyphRun(new Brush(rule.FontColor),
                new GlyphRun(baselineOrigin, text, rule.FontFamily, rule.FontSize));
        }

        public static void DrawText(this DrawingContext dc, StyleRuleSet rule, string text, Rect rect)
        {
            var ascent = OSImplentation.TypographyTextContext.GetAscent(rule.FontFamily, rule.FontSize);
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

            var style = rule;
            GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            DrawBackground(dc, style, paddingBoxRect);

            //Content
            //Content-box
            //no content

            DrawBorder(dc, style, borderBoxRect, paddingBoxRect);
            DrawOutline(dc, style, borderBoxRect);

            DrawDebug(dc, style, paddingBoxRect, contentBoxRect);
        }

        public static void DrawBoxModel(this DrawingContext dc, string text, StyleRuleSet rule, Rect rect)
        {
            if (rect.IsZero)
            {
                return;
            }

            var style = rule;
            GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            DrawBackground(dc, style, paddingBoxRect);

            //Content
            //Content-box
            //FIXME Use FormattedText instead of GlyphRun for multi-line text
            dc.DrawGlyphRun(rule, text, contentBoxRect.TopLeft);

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

            GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            DrawBackground(dc, style, paddingBoxRect);

            //Content
            //Content-box
            dc.DrawImage(texture, contentBoxRect, style.BorderImageSlice);

            DrawBorder(dc, style, borderBoxRect, paddingBoxRect);
            DrawOutline(dc, style, borderBoxRect);

            DrawDebug(dc, style, paddingBoxRect, contentBoxRect);
        }

        public static void DrawBoxModel(this DrawingContext dc, Node node)
        {
            DrawBoxModel(dc, node.RuleSet, node.Rect);
        }
    }
}
