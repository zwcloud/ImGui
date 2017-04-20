using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ImGui
{
    internal class StackList<T> : Stack<T>
    {
        public bool Empty => this.Count <= 0;

        public T this[int index]
        {
            get => this.ElementAt<T>(index);
        }
    }
}
