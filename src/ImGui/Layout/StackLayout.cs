using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui.Layout
{
    internal partial class StackLayout
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
            var rootGroup = new LayoutGroup(rootId, true, GUIStyle.Default, size);
            rootGroup.HorizontalStretchFactor = 1;
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

        public Rect GetRect(int id, Size contentSize, GUIStyle style = null)
        {
            // FIXME This should only be checked if the rect's width or height is not stretched.
            //if (contentSize.Height < 1 || contentSize.Width < 1)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(contentSize), "Content size is too small.");
            //}

            // build entry for next frame
            {
                var entry = new LayoutEntry(id, style, contentSize);
                ApplyOverridedStyle(entry);
                this.WritingStack.Peek().Add(entry);
            }

            // read from built group
            {
                var group = this.ReadingStack.Peek();
                var entry = group.GetEntry(id);
                if(entry == null)
                {
                    return new Rect(100, 100);//dummy
                }

                return entry.Rect;
            }
        }

        public void BeginLayoutGroup(int id, bool isVertical, Size size, GUIStyle style = null)
        {
            // build group for next frame
            {
                var group = new LayoutGroup(id, isVertical, style, size);
                ApplyOverridedStyle(group);
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
                }
                if(group == null)// this happens when new group is added in previous frame
                {
                    group = new LayoutGroup(id, isVertical, style, size);//dummy (HACK added to reading stack to forbid NRE)
                }
                group.ResetCursor();
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
            this.WritingStack.Peek().CalcWidth(TopGroup.ContentWidth);
            this.WritingStack.Peek().CalcHeight(TopGroup.ContentHeight);
            this.WritingStack.Peek().SetX(0);
            this.WritingStack.Peek().SetY(0);

            this.SwapStack();
        }

        public void SetRootSize(Size size)
        {
            var rootGroup = this.ReadingStack.Peek();
            rootGroup.ContentWidth = size.Width;
            rootGroup.ContentHeight = size.Height;
        }

        private void ApplyOverridedStyle(LayoutEntry entry)
        {
            if (this.MinWidth > 0) entry.MinWidth = this.MinWidth;
            if (this.MaxWidth  > 0) entry.MaxWidth = this.MaxWidth;
            if (this.MinHeight > 0) entry.MinHeight = this.MinHeight;
            if (this.MaxHeight > 0) entry.MaxHeight = this.MaxHeight;
            if (this.HorizontalStretchFactor > 0) entry.HorizontalStretchFactor = this.HorizontalStretchFactor;
            if (this.VerticalStretchFactor > 0) entry.VerticalStretchFactor = this.VerticalStretchFactor;
            if (this.Border.Item1 > 0) entry.Border = this.Border;
            if (this.Padding.Item1 > 0) entry.Padding = this.Padding;
        }

        private void ApplyOverridedStyle(LayoutGroup group)
        {
            ApplyOverridedStyle(group as LayoutEntry);
            if (this.CellSpacingHorizontal > 0) group.CellSpacingHorizontal = this.CellSpacingHorizontal;
            if (this.CellSpacingVertical > 0) group.CellSpacingVertical = this.CellSpacingVertical;
            if (this.AlignmentHorizontal != Alignment.Undefined) group.AlignmentHorizontal = this.AlignmentHorizontal;
            if (this.AlignmentVertical != Alignment.Undefined) group.AlignmentVertical = this.AlignmentVertical;
        }
    }
}
