using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui.Layout
{
    internal partial class StackLayout
    {
        Stack<int> horizontalStretchFactorStack = new Stack<int>();
        Stack<int> verticalStretchFactorStack = new Stack<int>();

        public int HorizontalStretchFactor { get; set; } = 0;
        public int VerticalStretchFactor { get; set; } = 0;

        public void PushStretchFactor(bool isVertical, int factor)
        {
            if (isVertical)
            {
                this.verticalStretchFactorStack.Push(factor);
                this.VerticalStretchFactor = factor;
            }
            else
            {
                this.horizontalStretchFactorStack.Push(factor);
                this.HorizontalStretchFactor = factor;
            }
        }

        public void PopStretchFactor(bool isVertical)
        {
            if (isVertical)
            {
                verticalStretchFactorStack.Pop();
                this.VerticalStretchFactor = verticalStretchFactorStack.Count == 0 ? -1 : verticalStretchFactorStack.Peek();
            }
            else
            {
                horizontalStretchFactorStack.Pop();
                this.HorizontalStretchFactor = horizontalStretchFactorStack.Count == 0 ? -1 : horizontalStretchFactorStack.Peek();
            }
        }
    }
}
