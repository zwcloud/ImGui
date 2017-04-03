using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui.Port.Render
{
    class RendererException : Exception
    {
        public RendererException()
        {
        }

        public RendererException(string message) : base(message)
        {
        }

        public RendererException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    class RendererCreateException : RendererException
    {
        public RendererCreateException()
        {
        }

        public RendererCreateException(string message) : base(message)
        {
        }

        public RendererCreateException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
