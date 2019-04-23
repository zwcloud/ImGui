namespace ImGui.Rendering
{
    /// <summary>
    /// Creates a quadratic Bezier curve between two points in a PathFigure.
    /// </summary>
    internal class QuadraticBezierSegment : PathSegment
    {
        /// <summary>
        /// Gets or sets the control Point of the curve.
        /// </summary>
        public Point ControlPoint { get; set; }

        /// <summary>
        /// Gets or sets the end Point of this QuadraticBezierSegment.
        /// </summary>
        public Point EndPoint { get; set; }

        public QuadraticBezierSegment(Point controlPoint, Point endPoint)
        {
            ControlPoint = controlPoint;
            EndPoint = endPoint;
        }
    }
}