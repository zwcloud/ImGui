using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ImGui
{
    partial class DrawList
    {
        public void PopClipRect()
        {
            System.Diagnostics.Debug.Assert(_ClipRectStack.Count > 0);
            _ClipRectStack.RemoveAt(_ClipRectStack.Count-1);
            //UpdateClipRect();
        }
    }
}
