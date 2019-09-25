using System;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    /// <summary>
    /// Represents an elliptical arc between two points.
    /// </summary>
    internal class ArcSegment : PathSegment
    {
        /// <summary>
        /// Gets or sets the endpoint of the elliptical arc.
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// Gets or sets the x- and y-radius of the arc as a Size structure.
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether the arc is drawn in the <see cref="SweepDirection.Clockwise"/> or <see cref="SweepDirection.Counterclockwise"/> direction.
        /// </summary>
        public SweepDirection SweepDirection { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the arc should be greater than 180 degrees.
        /// </summary>
        public bool IsLargeArc { get; set; }

        /// <summary>
        /// Gets or sets the amount (in degrees) by which the ellipse is rotated about the x-axis.
        /// </summary>
        public double RotationAngle { get; set; }

        public ArcSegment(
            Point point,
            Size size,
            double rotationAngle,
            bool isLargeArc,
            SweepDirection sweepDirection,
            bool isStroked)
        {
            Size = size;
            RotationAngle = rotationAngle;
            IsLargeArc = isLargeArc;
            SweepDirection = sweepDirection;
            Point = point;
            IsStroked = isStroked;
        }
    }

    /// <summary>
    /// Defines the direction an elliptical arc is drawn.
    /// </summary>
    public enum SweepDirection
    {
        /// <summary>
        /// Specifies that arcs are drawn in a counter clockwise (negative-angle) direction.
        /// </summary>
        Counterclockwise = 0,

        /// <summary>
        /// Specifies that arcs are drawn in a clockwise (positive-angle) direction.
        /// </summary>
        Clockwise = 1,
    }

    internal static class ArcSegmentExtension
    {
        //port from https://github.com/mrdoob/three.js/blob/7e0a78beb9317e580d7fa4da9b5b12be051c6feb/examples/js/loaders/SVGLoader.js#L569
        public static IList<Point> Flatten(this ArcSegment segment, Point startPoint)
        {
            var start = startPoint;
            var end = segment.Point;
            var rx = segment.Size.Width;
            var ry = segment.Size.Height;
            var x_axis_rotation = segment.RotationAngle;
            var large_arc_flag = segment.IsLargeArc ? 1 : 0;
            var sweep_flag = (int)segment.SweepDirection;

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

            // Step 4: Compute θ1 and Δθ
            var theta = svgAngle(1, 0, (x1p - cxp) / rx, (y1p - cyp) / ry);
            var delta = svgAngle((x1p - cxp) / rx, (y1p - cyp) / ry, (-x1p - cxp) / rx, (-y1p - cyp) / ry) % (Math.PI * 2);

            var curve = new EllipseCurve(cx, cy, rx, ry, theta, theta + delta, sweep_flag == 0, x_axis_rotation);

            // Calculate number of points for polyline approximation
            var max = (int)(Math.Abs(delta)/MathEx.Deg2Rad(3));
            IList<Point> points = new List<Point>(max);
            for (double i = 0; i <= max; i++)
            {
                var p = curve.getPoint(i/max);
                points.Add(p);
            }
            return points;
        }

        static double svgAngle(double ux, double uy, double vx, double vy)
        {
            var dot = ux * vx + uy * vy;
            var len = Math.Sqrt(ux * ux + uy * uy) * Math.Sqrt(vx * vx + vy * vy);
            var ang = Math.Acos(Math.Max(-1, Math.Min(1, dot / len))); // floating point precision, slightly over values appear
            if ((ux * vy - uy * vx) < 0) ang = -ang;
            return ang;
        }
    }

    class EllipseCurve
    {
        private double aX;
        private double aY;
        private double xRadius;
        private double yRadius;
        private double aStartAngle;
        private double aEndAngle;
        private bool aClockwise;
        private double aRotation;

        public EllipseCurve(
            double aX, double aY,
            double xRadius, double yRadius,
            double aStartAngle, double aEndAngle,
            bool aClockwise,
            double aRotation)
        {
            this.aX = aX;
            this.aY = aY;

            this.xRadius = xRadius;
            this.yRadius = yRadius;

            this.aStartAngle = aStartAngle;
            this.aEndAngle = aEndAngle;

            this.aClockwise = aClockwise;

            this.aRotation = aRotation;

        }

        public Point getPoint(double t)
        {
            var twoPi = Math.PI * 2;
            var deltaAngle = this.aEndAngle - this.aStartAngle;
            var samePoints = Math.Abs(deltaAngle) < double.Epsilon;

            // ensures that deltaAngle is 0 .. 2 PI
            while (deltaAngle < 0) deltaAngle += twoPi;
            while (deltaAngle > twoPi) deltaAngle -= twoPi;

            if (deltaAngle < double.Epsilon)
            {
                if (samePoints)
                {
                    deltaAngle = 0;
                }
                else
                {
                    deltaAngle = twoPi;
                }
            }

            if (this.aClockwise == true && !samePoints)
            {
                if (deltaAngle == twoPi)
                {
                    deltaAngle = -twoPi;
                }
                else
                {
                    deltaAngle = deltaAngle - twoPi;
                }

            }

            var angle = this.aStartAngle + t * deltaAngle;
            var x = this.aX + this.xRadius * Math.Cos(angle);
            var y = this.aY + this.yRadius * Math.Sin(angle);

            if (this.aRotation != 0)
            {
                var cos = Math.Cos(this.aRotation);
                var sin = Math.Sin(this.aRotation);

                var tx = x - this.aX;
                var ty = y - this.aY;

                // Rotate the point about the center of the ellipse.
                x = tx * cos - ty * sin + this.aX;
                y = tx * sin + ty * cos + this.aY;
            }

            return new Point(x, y);

        }
    }

}