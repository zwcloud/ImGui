using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui.Layout
{
    internal class StackLayout
    {
        public bool dirty;

        private readonly Stack<LayoutGroup> groupStack = new Stack<LayoutGroup>();

        public StackLayout(int rootId, Size size)
        {
            var rootGroup = new LayoutGroup(true, GUIStyle.Default, GUILayout.Width(size.Width), GUILayout.Height(size.Height)) { Id = rootId };
            rootGroup.Activated = true;
            this.dirty = true;
            rootGroup.Id = rootId;
            this.groupStack.Push(rootGroup);
        }

        public LayoutGroup TopGroup => this.groupStack.Peek();

        public bool InsideVerticalGroup => this.TopGroup.IsVertical;

        public Rect GetRect(int id, Size contentSize, GUIStyle style = null, LayoutOption[] options = null)
        {
            if (contentSize.Height < 1 || contentSize.Height < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(contentSize), "Content size is too small.");
            }
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

        public LayoutGroup BeginLayoutGroup(int id, bool isVertical, GUIStyle style = null, LayoutOption[] options = null)
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
            group.Activated = true;
            this.groupStack.Push(group);
            return group;
        }

        public void EndLayoutGroup()
        {
            this.groupStack.Pop();
        }

        private Rect DoGetRect(int id, Size contentZize, GUIStyle style, LayoutOption[] options)
        {
            var entry = FindLayoutEntry(this.TopGroup, id);
            if (entry == null)
            {
                entry = new LayoutEntry(style, options) { Id = id, ContentWidth = contentZize.Width, ContentHeight = contentZize.Height };
                this.TopGroup.Add(entry);
                this.dirty = true;
                entry.Activated = true;
                return new Rect(9999,9999);
            }
            //TODO check if layoutEntry' size/style/option changed
            entry.Activated = true;
            return entry.Rect;
        }

        public void Begin()
        {
            this.TopGroup.ResetCursor();
            DeactiveAllEntries(this.TopGroup);
            // Following `BeginLayoutGroup` and `GetRect` calls will activate groups and entries.
        }

        /// <summary>
        /// Calculate positions and sizes of every LayoutGroup and LayoutEntry
        /// </summary>
        public void Layout()
        {
            if (RemoveInactiveEntries(this.TopGroup))
            {
                this.dirty = true;
            }
            if (this.dirty)
            {
                this.TopGroup.CalcWidth();
                this.TopGroup.CalcHeight();
                this.TopGroup.SetX(0);
                this.TopGroup.SetY(0);
                this.dirty = false;
            }
        }

        private void DeactiveAllEntries(LayoutGroup targetGroup)
        {
            foreach (var entry in targetGroup.Entries)
            {
                entry.Activated = false;
                var group = entry as LayoutGroup;
                if (group != null)
                {
                    DeactiveAllEntries(group);
                }
            }
        }

        private bool RemoveInactiveEntries(LayoutGroup targetGroup)
        {
            bool removed = false;
            for (int i = targetGroup.Entries.Count - 1; i >= 0; i--)
            {
                var entry = targetGroup.Entries[i];
                if (entry.Activated)
                {
                    var group = entry as LayoutGroup;
                    if (group != null)
                    {
                        removed = RemoveInactiveEntries(group);
                    }
                }
                else
                {
                    targetGroup.Entries.RemoveAt(i);
                    removed = true;
                }
            }
            return removed;
        }
    }
}
