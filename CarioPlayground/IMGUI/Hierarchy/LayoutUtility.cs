#define Use_Fill_Layout
using System;
using System.Collections.Generic;
namespace ImGui
{
    public enum EventType
    {
        Layout,
        Repaint,
        Used,
    }

    public class Event
    {
        public static Event current;

        public EventType type;
    }

    public class LayoutUtility
    {
        public static Rect GetRect(Content content, Style style)
        {
            return DoGetRect(content, style);
        }

        private static LayoutCache current
        {
            get { return Form.current.LayoutCache;}
        }

        internal static LayoutGroup BeginLayoutGroup(Style style)
        {
            EventType type = Event.current.type;
            if (type == EventType.Layout)
            {
                LayoutGroup layoutGroup = new LayoutGroup(style);
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
                    throw new InvalidOperationException("GUILayout mis matched LayoutGroup");
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

        private static Rect DoGetRect(Content content, Style style)
        {
            if (Event.current.type == EventType.Layout)
            {
                Size size = style.CalcSize(content);
                current.topGroup.Add(new LayoutEntry(size.Width, size.Width, size.Height, size.Height, style));
                return Rect.Empty;
            }
            //if (Event.current.type != EventType.Used)
            //{
                return current.topGroup.GetNext().rect;
            //}
            //return Rect.Empty;
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
#if Use_Fill_Layout
            current.topGroup.CalcRect();
#else
            current.topGroup.CalcWidth();
            current.topGroup.SetHorizontal(0f, current.topGroup.maxWidth);
            current.topGroup.CalcHeight();
            current.topGroup.SetVertical(0f, current.topGroup.maxHeight);
#endif
        }
    }
}