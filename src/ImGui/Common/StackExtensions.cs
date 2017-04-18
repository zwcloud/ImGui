using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    internal static class StackExtensions
    {
        public static bool Empty<T>(this Stack<T> stack)
        {
            return stack.Count == 0;
        }
    }
}
