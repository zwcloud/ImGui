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
            //TODO
        }

        /// <summary>
        /// Causes the point of the pen to move back to the start of the current sub-path.
        /// It tries to draw a straight line from the current point to the start.
        /// If the shape has already been closed or has only one point, this function does nothing.
        /// </summary>
        public void ClosePath()
        {
            if (Path.Count <= 1)
            {//has no or only one point
                return;
            }
            if (Point.AlmostEqual(Path[Path.Count - 1], Path[0]))
            {//already closed
                return;
            }
            Path.Add(Path[0]);
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
            BezierCurveTo(new Point(cp1x, cp1y), new Point(cp2x, cp2y), new Point(x, y));
        }

        public void BezierCurveTo(Point controlPoint1, Point controlPoint2, Point endPoint)
        {
            BezierCurveTo(controlPoint1, controlPoint2, endPoint, 0);
        }

        /// <summary>
        /// Adds a cubic Bézier spline to the path from the current point to position end, using c1 and c2 as the control points.
        /// </summary>
        /// <param name="c1">first control point</param>
        /// <param name="c2">second control point</param>
        /// <param name="end">end point</param>
        /// <param name="numSegments">number of segments used when tessellating the curve. Use 0 to do automatic tessellation.</param>
        private void BezierCurveTo(Point c1, Point c2, Point end, int numSegments = 0)
        {
            Point p1 = Path[Path.Count - 1];
            if (numSegments == 0)
            {
                const double CurveTessellationTol = 1.25;

                // Auto-tessellated
                PathBezierToCasteljau(Path, p1.X, p1.Y, c1.X, c1.Y, c2.X, c2.Y, end.X, end.Y, CurveTessellationTol, 0);
            }
            else
            {
                float tStep = 1.0f / (float)numSegments;
                for (int iStep = 1; iStep <= numSegments; iStep++)
                {
                    float t = tStep * iStep;
                    float u = 1.0f - t;
                    float w1 = u * u * u;
                    float w2 = 3 * u * u * t;
                    float w3 = 3 * u * t * t;
                    float w4 = t * t * t;
                    Path.Add(new Point(w1 * p1.X + w2 * c1.X + w3 * c2.X + w4 * end.X, w1 * p1.Y + w2 * c1.Y + w3 * c2.Y + w4 * end.Y));
                }
            }
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
            double x, y;
            double x1 = controlPoint.X;
            double y1 = controlPoint.Y;
            double x2 = point.X;
            double y2 = point.Y;
            GetCurrentPoint(out x, out y);
            BezierCurveTo( x + 2.0 / 3.0 * (x1 - x), y + 2.0 / 3.0 * (y1 - y),
                           x2 + 2.0 / 3.0 * (x1 - x2), y2 + 2.0 / 3.0 * (y1 - y2),
                           x2, y2);
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

        /// <summary>
        /// The x-axis coordinate of the first control point.
        /// </summary>
        /// <param name="x1">The y-axis coordinate of the first control point.</param>
        /// <param name="y1">The y-axis coordinate of the first control point.</param>
        /// <param name="x2">The x-axis coordinate of the second control point.</param>
        /// <param name="y2">The y-axis coordinate of the second control point.</param>
        /// <param name="radius">The arc's radius. Must be non-negative.</param>
        public void ArcTo(double x1, double y1, double x2, double y2, double radius)
        {
            //TODO
            //NOTE CanvasRenderingContext2D.arcTo is implemented in a different approach compared with WPF's arcSegment
        }

        public void ArcTo(Point controlPoint0, Point controlPoint2, double radius)
        {
            //TODO
        }

        public void EllipseArcTo(Point startPoint, Point endPoint, double radiusX, double radiusY, double rotationAngle,
            bool isLargeArc, SweepDirection sweepDirection)
        {
            //TODO refactor this: originally copied from ArcSegmentExtension::Flatten

            var start = startPoint;
            var end = endPoint;
            var rx = radiusX;
            var ry = radiusY;
            var x_axis_rotation = rotationAngle;
            var large_arc_flag = isLargeArc ? 1 : 0;
            var sweep_flag = (int)sweepDirection;

            // Ensure radii are positive
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);

            // Compute (x1′, y1′)
            var dx2 = (start.x - end.x) / 2.0;
            var dy2 = (start.y - end.y) / 2.0;
            var x1p = Math.Cos(x_axis_rotation) * dx2 + Math.Sin(x_axis_rotation) * dy2;
            var y1p = -Math.Sin(x_axis_rotation) * dx2 + Math.Cos(x_axis_rotation) * dy2;

            // Compute (cx′, cy′)
            var rxs = rx * rx;
            var rys = ry * ry;
            var x1ps = x1p * x1p;
            var y1ps = y1p * y1p;

            // Ensure radii are large enough
            var cr = x1ps / rxs + y1ps / rys;

            if (cr > 1)
            {
                // scale up rx,ry equally so cr == 1
                var s = Math.Sqrt(cr);
                rx = s * rx;
                ry = s * ry;
                rxs = rx * rx;
                rys = ry * ry;
            }

            var dq = (rxs * y1ps + rys * x1ps);
            var pq = (rxs * rys - dq) / dq;
            var q = Math.Sqrt(Math.Max(0, pq));
            if (large_arc_flag == sweep_flag) q = -q;
            var cxp = q * rx * y1p / ry;
            var cyp = -q * ry * x1p / rx;

            // Step 3: Compute (cx, cy) from (cx′, cy′)
            var cx = Math.Cos(x_axis_rotation) * cxp - Math.Sin(x_axis_rotation) * cyp + (start.x + end.x) / 2;
            var cy = Math.Sin(x_axis_rotation) * cxp + Math.Cos(x_axis_rotation) * cyp + (start.y + end.y) / 2;

            static double svgAngle(double ux, double uy, double vx, double vy)
            {
                var dot = ux * vx + uy * vy;
                var len = Math.Sqrt(ux * ux + uy * uy) * Math.Sqrt(vx * vx + vy * vy);
                var ang = Math.Acos(Math.Max(-1, Math.Min(1, dot / len))); // floating point precision, slightly over values appear
                if ((ux * vy - uy * vx) < 0) ang = -ang;
                return ang;
            }

            // Step 4: Compute θ1 and Δθ
            var theta = svgAngle(1, 0, (x1p - cxp) / rx, (y1p - cyp) / ry);
            var delta = svgAngle((x1p - cxp) / rx, (y1p - cyp) / ry, (-x1p - cxp) / rx, (-y1p - cyp) / ry) % (Math.PI * 2);

            var curve = new EllipseCurve(cx, cy, rx, ry, theta, theta + delta, sweep_flag == 0, x_axis_rotation);

            // Calculate number of points for polyline approximation
            var max = (int)(Math.Abs(delta) / MathEx.Deg2Rad(3));
            for (double i = 0; i <= max; i++)
            {
                var p = curve.getPoint(i / max);
                LineTo(p);
            }
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
            //TODO support rotation

            if (startAngle == endAngle) return;
            if (MathEx.AmostZero(radiusX) && MathEx.AmostZero(radiusY))
            {
                return;
            }
            var segmentCount = Math.Max((int)(radiusX + radiusY - 1), 1);

            if (startAngle < endAngle)
            {
                var unit = (endAngle - startAngle) / segmentCount;
                for (int i = 0; i <= segmentCount; i++)
                {
                    var angle = startAngle + unit * i;
                    var p = MathEx.EvaluateEllipse(center, radiusX, radiusY, angle);
                    LineTo(p);
                }
            }
            else
            {
                var unit = (startAngle - endAngle) / segmentCount;
                for (int i = 0; i <= segmentCount; i++)
                {
                    var angle = startAngle - unit * i;
                    var p = MathEx.EvaluateEllipse(center, radiusX, radiusY, angle);
                    LineTo(p);
                }
            }
        }

        /// <summary>
        /// Creates a path for a rectangle at position (x, y) with a size that is determined by width and height.
        /// </summary>
        /// <param name="x">The x-axis coordinate of the rectangle's starting point.</param>
        /// <param name="y">The y-axis coordinate of the rectangle's starting point.</param>
        /// <param name="width">The rectangle's width. Positive values are to the right, and negative to the left.</param>
        /// <param name="height">The rectangle's height. Positive values are down, and negative are up.</param>
        public void Rect(double x, double y, double width, double height)
        {
            Rect(new Rect(x, y, width, height));
        }

        /// <summary>
        /// Creates a path for a rectangle at position (x, y) with a size that is determined by width and height.
        /// </summary>
        /// <param name="rect">the rectangle</param>
        public void Rect(Rect rect)
        {
            var a = rect.Min;
            var b = rect.Max;
            this.MoveTo(a);
            this.LineTo(new Point(b.X, a.Y));
            this.LineTo(b);
            this.LineTo(new Point(a.X, b.Y));
        }

        /// <summary>
        /// Moves the starting point of a new sub-path to the point coordinates.
        /// </summary>
        public void MoveTo(Point point)
        {
            if (Path.Count == 0)
            {
                Path.Add(point);
            }
            else
            {
                Path[Path.Count - 1] = point;
            }
        }

        /// <summary>
        /// Adds a line to the path from the current point to position p.
        /// </summary>
        /// <param name="p">next point</param>
        public void LineTo(Point p)
        {
            Path.Add(p);
        }
        /// <summary>
        /// Clears the current path.
        /// </summary>
        public void PathClear()
        {
            Path.Clear();
        }

        /// <summary>
        /// Strokes the current path.
        /// </summary>
        /// <param name="color">color</param>
        /// <param name="close">Set to true if you want the path be closed. A line segment from the last point to first point will be added if this is true.</param>
        /// <param name="thickness">thickness</param>
        public void Stroke(Color color, bool close, double thickness = 1)
        {
            //TODO

            //this.AddPolyline(Path, color, close, thickness);
            this.PathClear();
        }

        /// <summary>
        /// Fills the current sub-paths with the current brush. The path must be a convex.
        /// </summary>
        /// <param name="color">fill color</param>
        public void Fill(Color color)
        {
            //TODO

            //this.AddConvexPolyFilled(Path, color, true);
            this.PathClear();
        }

        #region private implementation

        private readonly List<Point> Path = new List<Point>();

        private void GetCurrentPoint(out Point point)
        {
            point = Path[Path.Count - 1];
        }

        private void GetCurrentPoint(out double x, out double y)
        {
            GetCurrentPoint(out var p);
            x = p.x;
            y = p.y;
        }

        private void PathBezierToCasteljau(IList<Point> path, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double tessTol, int level)
        {
            double dx = x4 - x1;
            double dy = y4 - y1;
            double d2 = ((x2 - x4) * dy - (y2 - y4) * dx);
            double d3 = ((x3 - x4) * dy - (y3 - y4) * dx);
            d2 = (d2 >= 0) ? d2 : -d2;
            d3 = (d3 >= 0) ? d3 : -d3;
            if ((d2 + d3) * (d2 + d3) < tessTol * (dx * dx + dy * dy))
            {
                path.Add(new Point(x4, y4));
            }
            else if (level < 10)
            {
                double x12 = (x1 + x2) * 0.5f, y12 = (y1 + y2) * 0.5f;
                double x23 = (x2 + x3) * 0.5f, y23 = (y2 + y3) * 0.5f;
                double x34 = (x3 + x4) * 0.5f, y34 = (y3 + y4) * 0.5f;
                double x123 = (x12 + x23) * 0.5f, y123 = (y12 + y23) * 0.5f;
                double x234 = (x23 + x34) * 0.5f, y234 = (y23 + y34) * 0.5f;
                double x1234 = (x123 + x234) * 0.5f, y1234 = (y123 + y234) * 0.5f;

                PathBezierToCasteljau(path, x1, y1, x12, y12, x123, y123, x1234, y1234, tessTol, level + 1);
                PathBezierToCasteljau(path, x1234, y1234, x234, y234, x34, y34, x4, y4, tessTol, level + 1);
            }
        }
        #endregion

    }
}
