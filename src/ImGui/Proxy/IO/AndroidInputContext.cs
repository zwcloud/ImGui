using System;

namespace ImGui
{
    public class AndroidInputContext : IInputContext
    {
        public bool IsMouseLeftButtonDown
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        public bool IsMouseMiddleButtonDown
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        public bool IsMouseRightButtonDown
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        public Point MousePosition
        {
            get
            {
                throw new InvalidOperationException();
            }
        }
    }
}