using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    public class LayoutStyle
    {
        public double MinWidth = -2;
        public double MaxWidth = -1;
        public double MinHeight = -2;
        public double MaxHeight = -1;
        public int HorizontalStretchFactor = -1;
        public int VerticalStretchFactor = -1;

        public void Reset()
        {
            this.MinWidth = -2;
            this.MaxWidth = -1;
            this.MinHeight = -2;
            this.MaxHeight = -1;
            this.HorizontalStretchFactor = -1;
            this.VerticalStretchFactor = -1;
        }
    }

    public partial class GUILayout
    {
        static Stack<LayoutStyle> _overrideLayoutStyleStack = new Stack<LayoutStyle>();

        public static void PushStyle(LayoutStyle layoutStyle)
        {
            _overrideLayoutStyleStack.Push(layoutStyle);
        }

        public static void PopStyle()
        {
            _overrideLayoutStyleStack.Pop();
        }

        public static void PushHorizontalStretchFactor(int factor)
        {
            var style = new LayoutStyle()
            {
                HorizontalStretchFactor = factor
            };
            PushStyle(style);
        }

        public static void PopHorizontalStretchFactor()
        {
            PopStyle();
        }

        public static int GetOverrideHorizontalStretchFactor()
        {
            if (_overrideLayoutStyleStack.Count == 0) return -1;
            return _overrideLayoutStyleStack.Peek().HorizontalStretchFactor;
        }

        public static int GetOverrideVerticalStretchFactor()
        {
            if (_overrideLayoutStyleStack.Count == 0) return -1;
            return _overrideLayoutStyleStack.Peek().VerticalStretchFactor;
        }
    }
}
