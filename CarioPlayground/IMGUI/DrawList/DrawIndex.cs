using System.Diagnostics;

namespace ImGui
{
    [DebuggerDisplay("{Index}")]
    internal struct DrawIndex
    {
        short index;

        public short Index
        {
            get { return index; }
            set { this.index = value; }
        }

        public static implicit operator short(DrawIndex v)
        {
            return v.Index;
        }
    }
}