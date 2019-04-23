namespace ImGui.Rendering
{
    /// <summary>
    /// Creates a line between two points in a PathFigure.
    /// </summary>
    internal class LineSegment : PathSegment
    {
        /// <summary>
        /// Gets or sets the end point of the line segment.
        /// </summary>
        public Point Point { get; set; }

        public LineSegment(Point point, bool isStroked)
        {
            Point = point;
            IsStroked = isStroked;
        }
    }
}