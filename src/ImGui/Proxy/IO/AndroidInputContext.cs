using System;

namespace ImGui
{
    public class AndroidInputContext : IInputContext
    {
        public bool IsMouseLeftButtonDown
        {
            get
            {
                return false;//dummy
            }
        }

        public bool IsMouseMiddleButtonDown
        {
            get
            {
                return false;//dummy
            }
        }

        public bool IsMouseRightButtonDown
        {
            get
            {
                return false;//dummy
            }
        }

        public Point MousePosition
        {
            get
            {
                return Point.Zero;//dummy
            }
        }
    }
}