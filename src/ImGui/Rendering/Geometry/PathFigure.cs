using System;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    /// <summary>
    /// Represents a subsection of a geometry, a single connected series of two-dimensional geometric segments.
    /// </summary>
    /// <remarks>A PathGeometry is made up of one or more figures, represented by the PathFigure class. Each figure is itself made up of one or more segments, defined by the PathSegment class.</remarks>
    internal class PathFigure
    {
        /// <summary>
        /// Gets or sets the Point where the PathFigure begins.
        /// </summary>
        /// <value>The Point where the PathFigure begins. The default value is 0,0.</value>
        public Point StartPoint { get; set; }

        /// <summary>
        /// Gets or sets the collection of segments that define the shape of this PathFigure object.
        /// </summary>
        /// <value>The list of segments that define the shape of this PathFigure object. The default value is an empty list.</value>
        public List<PathSegment> Segments { get; set; } = new List<PathSegment>();

        /// <summary>
        /// Gets or sets whether the contained area of this PathFigure is to be used for hit-testing, rendering, and clipping.
        /// </summary>
        /// <value>Determines whether the contained area of this PathFigure is to be used for hit-testing, rendering, and clipping. The default value is true.</value>
        public bool IsFilled { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether this figures first and last segments are connected.
        /// </summary>
        /// <value>true if this figure's first and last segments are connected; otherwise, false. The default value is false.</value>
        public bool IsClosed { get; set; }

        public PathFigure()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">The path's startpoint</param>
        /// <param name="segments">A collection of segments</param>
        public PathFigure(Point start, IEnumerable<PathSegment> segments, bool closed)
        {
            if (segments == null)
            {
                throw new ArgumentNullException(nameof(segments));
            }

            StartPoint = start;
            Segments.AddRange(segments);
            IsClosed = closed;
        }

    }
}