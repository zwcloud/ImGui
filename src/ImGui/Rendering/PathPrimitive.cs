using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui.Rendering
{
    internal class PathPrimitive : Primitive
    {
        public List<PathData> Path { get; set; } = new List<PathData>();
    }
}
