using System.Collections.Generic;

namespace ImGui
{
    internal struct DrawIndex
    {
        short index;

        public short Index
        {
            get { return index; }
            set { this.index = value; }
        }
    }
}