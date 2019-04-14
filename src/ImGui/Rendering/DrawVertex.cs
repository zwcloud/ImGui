using System.Diagnostics;

namespace ImGui
{
    [DebuggerDisplay("{pos} {uv} {color}")]
    internal struct DrawVertex
    {
        public Point pos;
        public Point uv;
        public Color color;
    }
}