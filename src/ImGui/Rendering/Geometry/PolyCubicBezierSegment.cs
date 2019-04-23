using System.Collections.Generic;

namespace ImGui.Rendering
{
    /// <summary>
    /// Represents one or more cubic Bezier curves.
    /// </summary>
    internal class PolyCubicBezierSegment : PathSegment
    {
        /// <summary>
        /// Gets or sets the list of Points that define this PolyCubicBezierSegment object.
        /// </summary>
        public List<Point> Points { get; set; }

        public PolyCubicBezierSegment(IEnumerable<Point> points, bool isStroked)
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