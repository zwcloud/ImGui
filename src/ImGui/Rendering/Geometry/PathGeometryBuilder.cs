using System;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    public class PathGeometryBuilder
    {
        /// <summary>
        /// Starts a new path by emptying the list of sub-paths.
        /// Call this method when you want to create a new path.
        /// </summary>
        public void BeginPath()
        {
            if(Figure == null)
            {
                Figure = new PathFigure();
            }
            else
            {
                Figure.Segments.Clear();
            }
        }

        /// <summary>
        /// Causes the point of the pen to move back to the start of the current sub-path.
        /// It tries to draw a straight line from the current point to the start.
        /// If the shape has already been closed or has only one point, this function does nothing.
        /// </summary>
        public void ClosePath()
        {
            if (Figure.Segments.Count <= 1)
            {//has no or only one point
                return;
            }
            if (Figure.IsClosed)
            {//already closed
                return;
            }
            LineTo(Figure.StartPoint);
            Figure.IsClosed = true;
        }

        /// <summary>
        /// Moves the starting point of a new sub-path to the (x, y) coordinates.
        /// </summary>
        public void MoveTo(double x, double y)
        {
            MoveTo(new Point(x, y));
        }

        /// <summary>
        /// Connects the last point in the current sub-path to the specified (x, y) coordinates with a straight line.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void LineTo(double x, double y)
        {
            LineTo(new Point(x, y));
        }

        /// <summary>
        /// Adds a cubic Bézier curve to the current path.
        /// </summary>
        public void BezierCurveTo(double cp1x, double cp1y, double cp2x, double cp2y, double x, double y)
        {
            BezierCurveTo(cp1x, cp1y, cp2x, cp2y, x, y, false);
        }

        /// <summary>
        /// Adds a cubic Bézier curve to the current path.
        /// </summary>
        public void BezierCurveTo(double cp1x, double cp1y, double cp2x, double cp2y, double x, double y, bool isStroked)
        {
            BezierCurveTo(new Point(cp1x, cp1y), new Point(cp2x, cp2y), new Point(x, y), isStroked);
        }

        public void BezierCurveTo(Point controlPoint1, Point controlPoint2, Point endPoint)
        {
            BezierCurveTo(controlPoint1, controlPoint2, endPoint, false);
        }

        /// <summary>
        /// Adds a cubic Bézier spline to the path from the current point to position end, using c1 and c2 as the control points.
        /// </summary>
        public void BezierCurveTo(Point controlPoint1, Point controlPoint2, Point endPoint, bool isStroked)
        {
            Figure.Segments.Add(new CubicBezierSegment(controlPoint1, controlPoint2, endPoint, isStroked));
        }

        /// <summary>
        /// Adds a quadratic Bézier curve to the current path.
        /// </summary>
        public void QuadraticCurveTo(double cpx, double cpy, double x, double y)
        {
            QuadraticCurveTo(new Point(cpx, cpy), new Point(x, y));
        }

        public void QuadraticCurveTo(Point controlPoint, Point point)
        {
            Figure.Segments.Add(new QuadraticBezierSegment(controlPoint, point));
        }

        /// <summary>
        /// Adds a circular arc to the current path.
        /// </summary>
        public void Arc(double x, double y, double radius, double startAngle, double endAngle, bool anticlockwise)
        {
            Arc(new Point(x, y), radius, startAngle, endAngle, anticlockwise);
        }

        public void Arc(Point center, double radius, double startAngle, double endAngle, bool anticlockwise)
        {
            //TODO apply anticlockwise
            Ellipse(center, radius, radius, startAngle, endAngle);
        }

        public void Circle(Point center, double radius)
        {
            Ellipse(center, radius, radius);
        }

        /// <summary>
        /// Create a circular arc from current point to (x,y) with specified parameters.
        /// </summary>
        /// <remarks>
        /// Note `CanvasRenderingContext2D.arcTo` is implemented in a different approach compared with WPF's ArcSegment.
        /// WPF's approach is used here. See <see cref="ArcSegmentExtension.Flatten"/>.
        /// </remarks>
        public void ArcTo(double x, double y, double radius,
            double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked)
        {
            ArcTo(new Point(x, y), radius, rotationAngle, isLargeArc, sweepDirection, isStroked);
        }

        public void ArcTo(Point point, double radius, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked)
        {
            ArcSegment segment = new ArcSegment(point, new Size(radius, radius), rotationAngle, isLargeArc, sweepDirection, isStroked);
            Figure.Segments.Add(segment);
        }

        /// <summary>
        /// Create an elliptical arc from current point to (x,y) with specified parameters.
        /// </summary>
        public void EllipseArcTo(double x, double y, double radiusX, double radiusY,
            double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked)
        {
            EllipseArcTo(new Point(x, y), new Size(radiusX, radiusY), rotationAngle, isLargeArc, sweepDirection, isStroked);
        }

        public void EllipseArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked)
        {
            ArcSegment segment = new ArcSegment(point, size, rotationAngle, isLargeArc, sweepDirection, isStroked);
            Figure.Segments.Add(segment);
        }

        /// <summary>
        /// Adds an elliptical arc to the current path.
        /// </summary>
        /// <param name="x">The x-axis (horizontal) coordinate of the ellipse's center.</param>
        /// <param name="y">The y-axis (vertical) coordinate of the ellipse's center.</param>
        /// <param name="radiusX">The ellipse's major-axis radius. Must be non-negative.</param>
        /// <param name="radiusY">The ellipse's minor-axis radius. Must be non-negative.</param>
        /// <param name="startAngle">The angle at which the ellipse starts, measured clockwise from the positive x-axis
        /// and expressed in radians.</param>
        /// <param name="endAngle">The angle at which the ellipse ends, measured clockwise from the positive x-axis
        /// and expressed in radians.</param>
        public void Ellipse(double x, double y, double radiusX, double radiusY, double startAngle, double endAngle)
        {
            Ellipse(x, y, radiusX, radiusY, startAngle, endAngle);
        }

        /// <summary>
        /// Adds an elliptical arc to the current path.
        /// </summary>
        /// <param name="center">the ellipse's center.</param>
        /// <param name="radiusX">The ellipse's major-axis radius. Must be non-negative.</param>
        /// <param name="radiusY">The ellipse's minor-axis radius. Must be non-negative.</param>
        /// <param name="startAngle">The angle at which the ellipse starts, measured clockwise from the positive x-axis
        /// and expressed in radians.</param>
        /// <param name="endAngle">The angle at which the ellipse ends, measured clockwise from the positive x-axis
        /// and expressed in radians.</param>
        public void Ellipse(Point center, double radiusX, double radiusY, double startAngle, double endAngle)
        {
            throw new NotImplementedException();
        }

        public void Ellipse(Point center, double radiusX, double radiusY)
        {
            //TODO don't create eg and points here
            EllipseGeometry eg = new EllipseGeometry(center, radiusX, radiusY);
            Point[] points = eg.GetPointList();

            PathFigure figure = new PathFigure();
            figure.StartPoint = points[0];
            // i == 0, 3, 6, 9
            for (int i = 0; i < 12; i += 3)
            {
                figure.Segments.Add(new CubicBezierSegment(points[i + 1], points[i + 2], points[i + 3], false));
            }
            this.Figure = figure;
        }

        /// <summary>
        /// Creates a path for a rectangle at position (x, y) with a size that is determined by width and height.
        /// </summary>
        /// <param name="x">The x-axis coordinate of the rectangle's starting point.</param>
        /// <param name="y">The y-axis coordinate of the rectangle's starting point.</param>
        /// <param name="width">The rectangle's width. Positive values are to the right, and negative to the left.</param>
        /// <param name="height">The rectangle's height. Positive values are down, and negative are up.</param>
        public void Rect(double x, double y, double width, double height, bool isStroked)
        {
            Rect(new Rect(x, y, width, height), isStroked);
        }

        /// <summary>
        /// Creates a path for a rectangle at position (x, y) with a size that is determined by width and height.
        /// </summary>
        /// <param name="rect">the rectangle</param>
        public void Rect(Rect rect, bool isStroked)
        {
            MoveTo(rect.TopLeft);
            PolyLineSegment segment = new PolyLineSegment();
            segment.Points = new List<Point>
            {
                rect.TopRight,
                rect.BottomRight,
                rect.BottomLeft,
                rect.TopLeft
            };
            segment.IsStroked = isStroked;
            Figure.Segments.Add(segment);
        }

        /// <summary>
        /// Moves the starting point of a new sub-path to the point coordinates.
        /// </summary>
        public void MoveTo(Point point)
        {
            Figure = new PathFigure();
            Figure.StartPoint = point;
        }

        /// <summary>
        /// Adds a line to the path from the current point to position p.
        /// </summary>
        /// <param name="p">next point</param>
        public void LineTo(Point p)
        {
            Figure.Segments.Add(new LineSegment(p, false));
        }

        /// <summary>
        /// Strokes the current path.
        /// </summary>
        public void Stroke()
        {
            //When stroking current figure, override all segments in the figure.
            foreach (var segment in Figure.Segments)
            {
                segment.IsStroked = true;
            }
            Figure.IsFilled = false;
            Geometry.Figures.Add(Figure);
            Figure = null;
        }

        /// <summary>
        /// Fills the current sub-path. The path must be a convex.
        /// </summary>
        public void Fill()
        {
            Figure.IsFilled = true;
            Geometry.Figures.Add(Figure);
            Figure = null;
        }

        public void StrokeAndFill()
        {
            //When stroking current figure, override all segments in the figure.
            foreach (var segment in Figure.Segments)
            {
                segment.IsStroked = true;
            }
            Figure.IsFilled = true;
            Geometry.Figures.Add(Figure);
            Figure = null;
        }

        public Geometry ToGeometry()
        {
            var result = Geometry;
            return result;
        }

        #region private implementation

        private PathGeometry Geometry = new PathGeometry();
        private PathFigure Figure;

        #endregion

    }
}
