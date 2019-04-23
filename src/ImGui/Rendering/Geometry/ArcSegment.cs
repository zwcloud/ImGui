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
}