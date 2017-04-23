using System;
using System.Collections.Generic;
using System.Linq;

namespace ImGui.Layout
{
    class StackLayout
    {
        public bool Dirty = false;

        internal Stack<LayoutGroup> groupStack = new Stack<LayoutGroup>();

        public StackLayout(int rootId, Size size)
        {
            var rootGroup = new LayoutGroup(true, GUIStyle.Default, GUILayout.Width(size.Width), GUILayout.Height(size.Height));
            rootGroup.id = rootId;
            Push(rootGroup);
        }

        public LayoutGroup topGroup { get { return this.groupStack.Peek(); } }

        public void Push(LayoutGroup group)
        {
            this.groupStack.Push(group);
        }

        public void Pop()
        {
            this.groupStack.Pop();
        }

        public void Clear()
        {
            this.groupStack.Clear();
        }

        internal Rect GetRect(int id, Size contentSize, GUIStyle style, LayoutOption[] options)
        {
            return DoGetRect(id, contentSize, style, options);
        }

        internal Rect GetLastRect()
        {
            Rect last = topGroup.GetLast();
            return last;
        }

        public LayoutGroup FindLayoutGroup(int id)
        {
            var layoutGroup = topGroup;
            return FindLayoutGroup(layoutGroup, id);
        }

        private LayoutGroup FindLayoutGroup(LayoutGroup layoutGroup, int id)
        {
            if (layoutGroup.id == id)
            {
                return layoutGroup;
            }
            foreach (var entry in layoutGroup.entries)
            {
                return FindLayoutGroup((LayoutGroup)entry, id);
            }
            //not found
            return null;
        }

        private LayoutEntry FindLayoutEntry(int id)
        {
            var layoutGroup = topGroup;
            foreach (var entry in layoutGroup.entries)
            {
                var group = entry as LayoutGroup;
                if (group != null)
                {
                    return FindLayoutGroup(group, id);
                }
                else
                {
                    if(entry.id == id)
                    {
                        return entry;
                    }
                }
            }
            //not found
            return null;
        }

        internal LayoutGroup BeginLayoutGroup(int id, bool isVertical, GUIStyle style, LayoutOption[] options)
        {
            LayoutGroup layoutGroup = FindLayoutGroup(id);
            if(layoutGroup == null)
            {
                layoutGroup = new LayoutGroup(isVertical, style, options);
                this.Dirty = true;
            }
            topGroup.Add(layoutGroup);
            this.Push(layoutGroup);
            return layoutGroup;
        }

        internal void EndLayoutGroup()
        {
            this.Pop();
        }

        private Rect DoGetRect(int id, Size contentZize, GUIStyle style, LayoutOption[] options)
        {
            var layoutEntry = FindLayoutEntry(id);
            if (layoutEntry == null)
            {
                layoutEntry = new LayoutEntry(style, options) { id = id, contentWidth = contentZize.Width, contentHeight = contentZize.Height };
                this.topGroup.Add(layoutEntry);
                return new Rect(100,100);
            }
            return layoutEntry.rect;
        }

        internal void Begin()
        {
            this.topGroup.ResetCursor();
        }

        /// <summary>
        /// Calculate positions and sizes of every LayoutGroup and LayoutEntry
        /// </summary>
        internal void Layout()
        {
            this.topGroup.CalcWidth();
            this.topGroup.CalcHeight();
            this.topGroup.SetX(0);
            this.topGroup.SetY(0);
        }
    }
}
