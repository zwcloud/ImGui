//#define Use_Fill_Layout
#define Use_Stretch_Layout
using System;
using System.Collections.Generic;
namespace ImGui
{
    internal enum EventType
    {
        Layout,
        Repaint,
        Used,

        MaximizeWindow,
        MinimizeWindow,
        NormalizeWindow,
    }

    internal class Event
    {
        public static Event current;

        public EventType type;
    }

    internal class LayoutUtility
    {
        internal static Rect GetRect(Content content, Style style, LayoutOption[] options)
        {
            return DoGetRect(content, style, options);
        }

        internal static Rect GetRect(Size contentSize, Style style, LayoutOption[] options)
        {
            return DoGetRect(contentSize, style, options);
        }

        internal static LayoutCache current
        {
            get { return Form.current.LayoutCache;}
        }

        internal static LayoutGroup BeginLayoutGroup(bool isVertical, Style style, LayoutOption[] options)
        {
            EventType type = Event.current.type;
            if (type == EventType.Layout)
            {
                LayoutGroup layoutGroup = new LayoutGroup(isVertical, style, options);
                layoutGroup.isForm = false;
                current.topGroup.Add(layoutGroup);
                current.Push(layoutGroup);
                return layoutGroup;
            }
            else
            {
                LayoutGroup layoutGroup = current.topGroup.GetNext() as LayoutGroup;
                if (layoutGroup == null)
                {
                    throw new InvalidOperationException("GUILayout mis-matched LayoutGroup");
                }
                layoutGroup.ResetCursor();
                current.Push(layoutGroup);
                return layoutGroup;
            }
        }

        internal static void EndLayoutGroup()
        {
            current.Pop();
        }

        private static Rect DoGetRect(Size contentZize, Style style, LayoutOption[] options)
        {
            if (Event.current.type == EventType.Layout)
            {
                current.topGroup.Add(new LayoutEntry(style, options) { contentWidth = contentZize.Width, contentHeight = contentZize.Height });
                return Rect.Empty;
            }
            return current.topGroup.GetNext().rect;
        }

        private static Rect DoGetRect(Content content, Style style, LayoutOption[] options)
        {
            if (Event.current.type == EventType.Layout)
            {
                Size size = style.CalcSize(content, options);
                current.topGroup.Add(new LayoutEntry(style, options) { contentWidth = size.Width, contentHeight = size.Height });
                return Rect.Empty;
            }
            return current.topGroup.GetNext().rect;
        }


        internal static void Begin()
        {
            current.topGroup.ResetCursor();
        }

        /// <summary>
        /// Calculate positions and sizes of every LayoutGroup and layoutEntry
        /// </summary>
        internal static void Layout()
        {
#if Use_Stretch_Layout
            current.topGroup.CalcWidth();
            current.topGroup.CalcHeight();
            current.topGroup.SetX(0);
            current.topGroup.SetY(0);
#elif Use_Filled_Layout
            current.topGroup.CalcRect();
#endif
        }
    }
}
