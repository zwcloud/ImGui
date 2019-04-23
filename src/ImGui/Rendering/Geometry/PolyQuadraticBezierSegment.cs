using System.Collections.Generic;

namespace ImGui.Rendering
{
    /// <summary>
    /// Represents a set of quadratic Bezier segments.
    /// </summary>
    internal class PolyQuadraticBezierSegment : PathSegment
    {
        /// <summary>
        /// Gets or sets the list of Points that define this PolyQuadraticBezierSegment object.
        /// </summary>
        public List<Point> Points { get; set; }

        public PolyQuadraticBezierSegment(IEnumerable<Point> points, bool isStroked)
        {
            if (points == null)
            {
                throw new System.ArgumentNullException(nameof(points));
            }

            Points = new List<Point>(points);
            IsStroked = isStroked;
        }
    }
}