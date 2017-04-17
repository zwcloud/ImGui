using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    class GUIDrawContext
    {
        public Point CurrentPos = Point.Zero;

        Stack<LayoutEntry> LayoutStack = new Stack<LayoutEntry>();
        Dictionary<int, LayoutEntry> Layouts = new Dictionary<int, LayoutEntry>();

        public GUIDrawContext()
        {

        }
    }
}
