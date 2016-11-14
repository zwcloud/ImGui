using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui
{
    [DebuggerDisplay("{pos} {uv} {color}")]
    internal struct DrawVertex
    {
        public PointF pos;
        public PointF uv;
        public ColorF color;
    }
}