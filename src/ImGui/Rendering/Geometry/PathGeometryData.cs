using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class PathGeometryData
    {
        public List<PathCommand> Path { get; set; } = new List<PathCommand>();

        public FillRule FillRule { get; set; }

        public Vector Offset { get; set; }//TODO make this a matrix, namely transformation
    }
}