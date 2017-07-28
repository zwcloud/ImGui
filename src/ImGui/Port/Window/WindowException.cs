using System;

namespace ImGui
{
    internal abstract class WindowException : Exception
    {
        protected WindowException()
        {
        }

        protected WindowException(string message) : base(message)
        {
        }

        protected WindowException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    internal class WindowCreateException : WindowException
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

    internal class WindowUpdateException : WindowException
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
