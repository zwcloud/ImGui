namespace ImGui.Rendering
{
    /// <summary>
    /// Represents a cubic Bezier curve drawn between two points.
    /// </summary>
    internal class CubicBezierSegment : PathSegment
    {
        /// <summary>
        /// Gets or sets the first control point of the curve.
        /// </summary>
        public Point ControlPoint1 { get; set; }

        /// <summary>
        /// Gets or sets the second control point of the curve.
        /// </summary>
        public Point ControlPoint2 { get; set; }

        /// <summary>
        /// Gets or sets the end point of the curve.
        /// </summary>
        public Point EndPoint { get; set; }

        public CubicBezierSegment(Point controlPoint1, Point controlPoint2, Point endPoint, bool isStroked)
        {
            ControlPoint1 = controlPoint1;
            ControlPoint2 = controlPoint2;
            EndPoint = endPoint;
            IsStroked = isStroked;
        }
    }
}