namespace ImGui.Rendering
{
    /// <summary>
    /// Represents a segment of a PathFigure object.
    /// </summary>
    internal abstract class PathSegment
    {
        /// <summary>
        /// Gets or sets a value that indicates whether the segment is stroked.
        /// </summary>
        /// <value>true if the segment is stroked when a Pen is used to render the segment; otherwise, the segment is not stroked. The default is true.</value>
        public bool IsStroked { get; set; }
    }
}
