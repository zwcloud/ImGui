using System;

namespace ImGui
{
    internal class AndroidInputContext : IInputContext
    {
        public Cursor MouseCursor
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }
    }
}