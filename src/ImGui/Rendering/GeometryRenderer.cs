using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui.Rendering.Composition
{
    internal abstract class GeometryRenderer : RecordReader
    {
        protected Stack<Rect> ClipRectStack { get; } = new Stack<Rect>(new[] { Rect.Big });

        public void PushClipRect(Rect rect)
        {
            if (rect.IsEmpty || rect.IsZero)
            {
                throw new ArgumentOutOfRangeException(nameof(rect), "Invalid Clip Rect: empty or zero-sized");
            }
            ClipRectStack.Push(rect);
        }

        public void PopClipRect()
        {
            if (ClipRectStack.Count <= 1)
            {
                throw new InvalidOperationException("Push/Pop doesn't match.");
            }

            ClipRectStack.Pop();
        }
    }
}
