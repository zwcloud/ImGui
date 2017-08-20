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
            var rootGroup = new LayoutGroup(rootId, true, size);
            rootGroup.ContentWidth = size.Width;
            rootGroup.ContentHeight = size.Height;
            rootGroup.HorizontalStretchFactor = 1;
            //rootGroup.VerticalStretchFactor = 1;
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

        public Rect GetRect(int id, Size contentSize)
        {
            // FIXME This should only be checked if the rect's width or height is not stretched.
            //if (contentSize.Height < 1 || contentSize.Width < 1)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(contentSize), "Content size is too small.");
            //}

            // build entry for next frame
            {
                var entry = new LayoutEntry(id, contentSize);
                this.WritingStack.Peek().Add(entry);
            }

            // read from built group
            {
                var group = this.ReadingStack.Peek();
                var entry = group.GetEntry(id);
                if(entry == null)
                {
                    return DummyRect;//dummy
                }

                return entry.Rect;
            }
        }

        public void BeginLayoutGroup(int id, bool isVertical, Size size, GUIStyle style = null)
        {
            // build group for next frame
            {
                var group = new LayoutGroup(id, isVertical, size);
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
#if DrawGroup
                    if(group!= null)
                    {
                        var window = Form.current.uiContext.WindowManager.CurrentWindow;
                        var rect = window.GetRect(group.Rect);
                        window.DrawList.AddRectFilled(rect, group.IsVertical ? Color.Argb(30, 255, 0, 0) : Color.Argb(30, 0, 0, 255));
                    }
#endif
                }
                if(group == null)// this happens when new group is added in previous frame
                {
                    group = DummyGroup;//dummy (HACK added to reading stack to forbid NRE)
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
            {
                var rootGroup = this.ReadingStack.Peek();
                rootGroup.MinWidth = rootGroup.MaxWidth = rootGroup.ContentWidth = size.Width;
                rootGroup.ContentHeight = size.Height;
            }
            {
                var rootGroup = this.WritingStack.Peek();
                rootGroup.MinWidth = rootGroup.MaxWidth = rootGroup.ContentWidth = size.Width;
                rootGroup.ContentHeight = size.Height;
            }
        }

        public static Rect DummyRect = new Rect(1, 1);
        public static LayoutGroup DummyGroup = new LayoutGroup(-9999, false, Size.Zero);
    }
}
