using System.Collections.Generic;

namespace ImGui.Rendering
{
    /// <summary>
    /// Represents a set of line segments defined by a list of Point with each Point specifying the end point of a line segment.
    /// </summary>
    internal class PolyLineSegment : PathSegment
    {
        /// <summary>
        /// Gets or sets the list of Point structures that defines this PolyLineSegment object.
        /// </summary>
        public List<Point> Points { get; set; }

        public PolyLineSegment()
        {

        }

        public PolyLineSegment(IEnumerable<Point> points, bool isStroked)
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