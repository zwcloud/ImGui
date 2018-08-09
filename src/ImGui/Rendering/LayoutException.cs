using System;

namespace ImGui.Rendering
{
    public class LayoutException : Exception
    {
        public LayoutException(string message) : base(message)
        {
        }
    }
}