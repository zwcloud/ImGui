using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    class GUIDrawContext
    {
        public Point CurrentPos;

        Stack<LayoutEntry> LayoutStack;
        LayoutEntry Layouts;
    }
}
