using System.Diagnostics;

namespace ImGui
{
    [DebuggerDisplay("{Index}")]
    internal struct DrawIndex
    {
        public int Index { get; set; }

        public static implicit operator int(DrawIndex v)
        {
            return v.Index;
        }
    }
}