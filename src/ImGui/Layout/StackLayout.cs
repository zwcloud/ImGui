using System;
using System.Collections.Generic;
using System.Linq;

namespace ImGui.Layout
{
    class StackLayout
    {
        private bool Dirty = false;

        internal Stack<LayoutGroup> groupStack = new Stack<LayoutGroup>();

        public StackLayout(int rootId, Size size)
        {
            var rootGroup = new LayoutGroup(true, GUIStyle.Default, GUILayout.Width(size.Width), GUILayout.Height(size.Height)) { id = rootId };
            this.Dirty = true;
            rootGroup.id = rootId;
            this.groupStack.Push(rootGroup);
        }

        public LayoutGroup topGroup { get { return this.groupStack.Peek(); } }

        public bool InsideVerticalGroup => this.topGroup.isVertical;

        public Rect GetRect(int id, Size contentSize, GUIStyle style, LayoutOption[] options)
        {
            return DoGetRect(id, contentSize, style, options);
        }

        public LayoutGroup FindLayoutGroup(int id)
        {
            return FindLayoutGroup(topGroup, id);
        }

        private LayoutGroup FindLayoutGroup(LayoutGroup layoutGroup, int id)
        {
            foreach (var entry in layoutGroup.entries)
            {
                var group = entry as LayoutGroup;
                if(group != null)
                {
                    if(group.id == id)
                    {
                        return group;
                    }
                }
            }
            //not found
            return null;
        }

        private LayoutEntry FindLayoutEntry(int id)
        {
            return FindLayoutEntry(topGroup, id);
        }

        private LayoutEntry FindLayoutEntry(LayoutGroup layoutGroup, int id)
        {
            foreach (var entry in layoutGroup.entries)
            {
                var group = entry as LayoutGroup;
                if (group == null)
                {
                    if (entry.id == id)
                    {
                        return entry;
                    }
                }
            }
            //not found
            return null;
        }

        public LayoutGroup BeginLayoutGroup(int id, bool isVertical, GUIStyle style, LayoutOption[] options)
        {
            LayoutGroup group = FindLayoutGroup(id);
            if(group == null)
            {
                group = new LayoutGroup(isVertical, style, options) { id = id};
                topGroup.Add(group);
                this.Dirty = true;
            }
            else
            {
                group.ResetCursor();
            }
            this.groupStack.Push(group);
            return group;
        }

        public void EndLayoutGroup()
        {
            this.groupStack.Pop();
        }

        private Rect DoGetRect(int id, Size contentZize, GUIStyle style, LayoutOption[] options)
        {
            var layoutEntry = FindLayoutEntry(topGroup, id);
            if (layoutEntry == null)
            {
                layoutEntry = new LayoutEntry(style, options) { id = id, contentWidth = contentZize.Width, contentHeight = contentZize.Height };
                this.topGroup.Add(layoutEntry);
                this.Dirty = true;
                return new Rect(9999,9999);
            }
            //TODO check if layoutEntry' size/style/option changed

            return layoutEntry.rect;
        }

        public void Begin()
        {
            this.topGroup.ResetCursor();
        }

        /// <summary>
        /// Calculate positions and sizes of every LayoutGroup and LayoutEntry
        /// </summary>
        public void Layout(Size size)
        {
            if (!Dirty) return;
            this.topGroup.CalcWidth();
            this.topGroup.CalcHeight();
            this.topGroup.SetX(0);
            this.topGroup.SetY(0);
            this.Dirty = false;
        }
    }
}
