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
            var context = Form.current.uiContext;
            var styleStack = context.StyleStack;

            var minWidth = styleStack.MinWidth;
            var maxWidth = styleStack.MaxWidth;
            var minHeight = styleStack.MinHeight;
            var maxHeight = styleStack.MaxHeight;
            var horizontalStretchFactor = styleStack.HorizontalStretchFactor;
            var verticalStretchFactor = styleStack.VerticalStretchFactor;
            var border = styleStack.Border;
            var padding = styleStack.Padding;

            if (minWidth > 0) entry.MinWidth = minWidth;
            if (maxWidth  > 0) entry.MaxWidth =  maxWidth;
            if (minHeight > 0) entry.MinHeight = minHeight;
            if (maxHeight > 0) entry.MaxHeight = maxHeight;
            if (horizontalStretchFactor > 0) entry.HorizontalStretchFactor = horizontalStretchFactor;
            if (verticalStretchFactor > 0) entry.VerticalStretchFactor = verticalStretchFactor;
            if (border.Item1 > 0) entry.Border = border;
            if (padding.Item1 > 0) entry.Padding = padding;
        }

        private void ApplyOverridedStyle(LayoutGroup group)
        {
            ApplyOverridedStyle(group as LayoutEntry);

            var context = Form.current.uiContext;
            var styleStack = context.StyleStack;

            var cellSpacingHorizontal = styleStack.CellSpacingHorizontal;
            var cellSpacingVertical = styleStack.CellSpacingVertical;
            var alignmentHorizontal = styleStack.AlignmentHorizontal;
            var alignmentVertical = styleStack.AlignmentVertical;

            if (cellSpacingHorizontal > 0) group.CellSpacingHorizontal = cellSpacingHorizontal;
            if (cellSpacingVertical > 0) group.CellSpacingVertical = cellSpacingVertical;
            if (alignmentHorizontal != Alignment.Undefined) group.AlignmentHorizontal = alignmentHorizontal;
            if (alignmentVertical != Alignment.Undefined) group.AlignmentVertical = alignmentVertical;
        }
    }
}
