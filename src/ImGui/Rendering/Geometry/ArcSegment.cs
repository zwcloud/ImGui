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
        /// <summary>
        /// Flatten the ArcSegment into a List of Points
        /// </summary>
        /// <param name="segment">the arc segment</param>
        /// <param name="startPoint">start point</param>
        /// <param name="tolerance">tolerance</param>
        /// <returns>flattened point list</returns>
        /// <remarks>
        /// ref: http://www.charlespetzold.com/blog/2008/01/Mathematics-of-ArcSegment.html
        /// </remarks>
        public static IList<Point> Flatten(this ArcSegment segment, Point startPoint/*start point*/,
                double tolerance = 1)
        {
            var pt1 = startPoint;
            var pt2 = segment.Point;
            var radiusX = segment.Size.Width;
            var radiusY = segment.Size.Height;
            var angleRotation = segment.RotationAngle;
            var isLargeArc = segment.IsLargeArc;
            var isCounterclockwise = segment.SweepDirection == SweepDirection.Counterclockwise;

            // Adjust for different radii and rotation angle
            Matrix matx = new Matrix();
            matx.Rotate(-angleRotation);
            matx.Scale(radiusY / radiusX, 1);
            pt1 = matx.Transform(pt1);
            pt2 = matx.Transform(pt2);

            // Get info about chord that connects both points
            Point midPoint = new Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
            Vector vect = pt2 - pt1;
            double halfChord = vect.Length / 2;

            // Get vector from chord to center
            Vector vectRotated;

            // (comparing two Booleans here!)
            if (isLargeArc == isCounterclockwise)
                vectRotated = new Vector(-vect.Y, vect.X);
            else
                vectRotated = new Vector(vect.Y, -vect.X);

            vectRotated.Normalize();

            // Distance from chord to center
            double centerDistance = Math.Sqrt(Math.Abs(radiusY * radiusY - halfChord * halfChord));

            // Calculate center point
            Point center = midPoint + centerDistance * vectRotated;

            // Get angles from center to the two points
            double angle1 = Math.Atan2(pt1.Y - center.Y, pt1.X - center.X);
            double angle2 = Math.Atan2(pt2.Y - center.Y, pt2.X - center.X);

            // (another comparison of two Booleans!)
            if (isLargeArc == (Math.Abs(angle2 - angle1) < Math.PI))
            {
                if (angle1 < angle2)
                    angle1 += 2 * Math.PI;
                else
                    angle2 += 2 * Math.PI;
            }

            // Invert matrix for final point calculation
            matx.Invert();

            // Calculate number of points for polyline approximation
            int max = (int)((4 * (radiusX + radiusY) * Math.Abs(angle2 - angle1) / (2 * Math.PI)) / tolerance);

            IList<Point> points = new List<Point>(max);
            // Loop through the points
            for (int i = 0; i <= max; i++)
            {
                double angle = ((max - i) * angle1 + i * angle2) / max;
                double x = center.X + radiusY * Math.Cos(angle);
                double y = center.Y + radiusY * Math.Sin(angle);

                // Transform the point back
                Point pt = matx.Transform(new Point(x, y));
                points.Add(pt);
            }

            return points;
        }
    }
}