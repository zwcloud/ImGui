using System.Collections.Generic;
using ImGui.Common.Primitive;
using System.Diagnostics;

namespace ImGui.Layout
{
    internal partial class StackLayout
    {
        public StackLayout(int rootId, Size size)
        {
        }

        public Rect GetRect(int id, Size contentSize, LayoutOptions? options = null, string str_id = null)
        {
            return new Rect();
        }

        public void BeginLayoutGroup(int id, bool isVertical, LayoutOptions? options = null, string str_id = null)
        {
        }

        public void EndLayoutGroup()
        {
        }

        public void Begin()
        {
        }

        public void Layout()
        {
        }

        public void SetRootSize(Size size)
        {
        }

        public static Rect DummyRect = new Rect(1, 1);
    }
}
