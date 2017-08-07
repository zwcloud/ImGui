using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui.Layout
{
    internal class StackLayout
    {
        private bool dirty;

        private readonly Stack<LayoutGroup> groupStack = new Stack<LayoutGroup>();

        public StackLayout(int rootId, Size size)
        {
            var rootGroup = new LayoutGroup(true, GUIStyle.Default, GUILayout.Width(size.Width), GUILayout.Height(size.Height)) { Id = rootId };
            this.dirty = true;
            rootGroup.Id = rootId;
            this.groupStack.Push(rootGroup);
        }

        public LayoutGroup TopGroup => this.groupStack.Peek();

        public bool InsideVerticalGroup => this.TopGroup.IsVertical;

        public Rect GetRect(int id, Size contentSize, GUIStyle style, LayoutOption[] options)
        {
            return DoGetRect(id, contentSize, style, options);
        }

        public LayoutGroup FindLayoutGroup(int id)
        {
            return FindLayoutGroup(this.TopGroup, id);
        }

        private static LayoutGroup FindLayoutGroup(LayoutGroup layoutGroup, int id)
        {
            foreach (var entry in layoutGroup.Entries)
            {
                var group = entry as LayoutGroup;
                if(group != null && group.Id == id)
                {
                    return group;
                }
            }
            //not found
            return null;
        }

        private static LayoutEntry FindLayoutEntry(LayoutGroup layoutGroup, int id)
        {
            foreach (var entry in layoutGroup.Entries)
            {
                var group = entry as LayoutGroup;
                if (group == null)
                {
                    if (entry.Id == id)
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
            var group = FindLayoutGroup(id);
            if(group == null)
            {
                group = new LayoutGroup(isVertical, style, options) { Id = id};
                this.TopGroup.Add(group);
                this.dirty = true;
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
            var layoutEntry = FindLayoutEntry(this.TopGroup, id);
            if (layoutEntry == null)
            {
                layoutEntry = new LayoutEntry(style, options) { Id = id, ContentWidth = contentZize.Width, ContentHeight = contentZize.Height };
                this.TopGroup.Add(layoutEntry);
                this.dirty = true;
                return new Rect(9999,9999);
            }
            //TODO check if layoutEntry' size/style/option changed

            return layoutEntry.Rect;
        }

        public void Begin()
        {
            this.TopGroup.ResetCursor();
        }

        /// <summary>
        /// Calculate positions and sizes of every LayoutGroup and LayoutEntry
        /// </summary>
        public void Layout(Size size)
        {
            if (!this.dirty) return;
            this.TopGroup.CalcWidth();
            this.TopGroup.CalcHeight();
            this.TopGroup.SetX(0);
            this.TopGroup.SetY(0);
            this.dirty = false;
        }
    }
}
