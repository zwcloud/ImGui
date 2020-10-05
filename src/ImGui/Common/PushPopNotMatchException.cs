using System;

namespace ImGui
{
    public class PushPopNotMatchException : Exception
    {
        public PushPopNotMatchException()
        {
        }

        public PushPopNotMatchException(string message) : base(message)
        {
        }

        public PushPopNotMatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}