using System;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    public class PathFigure
    {
        public Point StartPoint { get; set; }

        public List<PathSegment> Segments { get; set; } = new List<PathSegment>();

        public PathFigure()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">The path's startpoint</param>
        /// <param name="segments">A collection of segments</param>
        public PathFigure(Point start, IEnumerable<PathSegment> segments)
        {
            if (segments == null)
            {
                throw new ArgumentNullException(nameof(segments));
            }

            StartPoint = start;
            Segments.AddRange(segments);
        }

    }
}