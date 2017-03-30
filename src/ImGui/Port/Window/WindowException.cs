using System;

namespace ImGui
{
    abstract class WindowException : Exception
    {
        public WindowException()
        {
        }

        public WindowException(string message) : base(message)
        {
        }

        public WindowException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    class WindowCreateException : WindowException
    {
        public WindowCreateException()
        {
        }

        public WindowCreateException(string message) : base(message)
        {
        }

        public WindowCreateException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    class WindowUpdateException : WindowException
    {
        public WindowUpdateException()
        {
        }

        public WindowUpdateException(string message) : base(message)
        {
        }

        public WindowUpdateException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
