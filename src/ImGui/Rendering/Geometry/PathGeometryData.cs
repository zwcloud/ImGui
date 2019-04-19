using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal class PathGeometryData : IPathList
    {
        public List<PathCommand> Path { get; set; } = new List<PathCommand>();

        public FillRule FillRule { get; set; }

        public Vector Offset { get; set; }//TODO make this a matrix, namely transformation
    }
}