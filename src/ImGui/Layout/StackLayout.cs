using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui.Layout
{
    internal class StackLayout
    {
        public bool dirty;

        private readonly Stack<LayoutGroup> stackA = new Stack<LayoutGroup>();
        private readonly Stack<LayoutGroup> stackB = new Stack<LayoutGroup>();

        private Stack<LayoutGroup> WritingStack { get; set; }
        public Stack<LayoutGroup> ReadingStack { get; private set; }

        public LayoutGroup TopGroup => this.ReadingStack.Peek();

        private void SwapStack()
        {
            var t = this.ReadingStack;
            this.ReadingStack = this.WritingStack;
            this.WritingStack = t;
        }

        private LayoutGroup CreateRootGroup(int rootId, Size size)
        {
            var rootGroup = new LayoutGroup(true, GUIStyle.Default, GUILayout.Width(size.Width), GUILayout.Height(size.Height)) { Id = rootId };
            rootGroup.Id = rootId;
            return rootGroup;
        }

        public StackLayout(int rootId, Size size)
        {
            var rootGroup = CreateRootGroup(rootId, size);
            this.stackA.Push(rootGroup);
            this.WritingStack = this.stackA;

            var rootGroupX = CreateRootGroup(rootId, size);
            this.stackB.Push(rootGroupX);
            this.ReadingStack = this.stackB;
        }

        public Rect GetRect(int id, Size contentSize, GUIStyle style = null, LayoutOption[] options = null)
        {
            // FIXME This should only be checked if the rect's width or height is not stretched.
            //if (contentSize.Height < 1 || contentSize.Width < 1)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(contentSize), "Content size is too small.");
            //}

            // build entry for next frame
            {
                var entry = new LayoutEntry(style, options) { Id = id, ContentWidth = contentSize.Width, ContentHeight = contentSize.Height };
                this.WritingStack.Peek().Add(entry);
            }

            // read from built group
            {
                var group = this.ReadingStack.Peek();
                if (group == null)
                {
                    return new Rect(100, 100);//dummy
                }
                var entry = group.GetEntry(id);
                if(entry == null)
                {
                    return new Rect(100, 100);//dummy
                }

                return entry.Rect;
            }
        }

        public void BeginLayoutGroup(int id, bool isVertical, GUIStyle style = null, LayoutOption[] options = null)
        {
            // build group for next frame
            {
                var group = new LayoutGroup(isVertical, style, options) { Id = id };
                this.WritingStack.Peek().Add(group);
                this.WritingStack.Push(group);
            }

            // read from built group
            {
                var parentGroup = this.ReadingStack.Peek();
                LayoutGroup group = null;
                if (parentGroup != null)
                {
                    group = parentGroup.GetEntry(id) as LayoutGroup;
                    group?.ResetCursor();
                }
                this.ReadingStack.Push(group);
            }
        }

        public void EndLayoutGroup()
        {
            this.WritingStack.Pop();
            this.ReadingStack.Pop();
        }

        public void Begin()
        {
            this.ReadingStack.Peek().ResetCursor();//reset reading cursor of root group
            this.WritingStack.Peek().Entries.Clear();//remove all children of root group
        }

        /// <summary>
        /// Calculate positions and sizes of every LayoutGroup and LayoutEntry
        /// </summary>
        public void Layout()
        {
            this.WritingStack.Peek().CalcWidth();
            this.WritingStack.Peek().CalcHeight();
            this.WritingStack.Peek().SetX(0);
            this.WritingStack.Peek().SetY(0);

            this.SwapStack();
        }
    }
}
