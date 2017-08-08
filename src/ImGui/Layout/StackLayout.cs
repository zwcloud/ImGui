using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ImGui.Common.Primitive;

namespace ImGui.Layout
{
    internal class StackLayout
    {
        private int rootId;
        public bool dirty;

        private readonly Stack<LayoutGroup> stackA = new Stack<LayoutGroup>();
        private readonly Stack<LayoutGroup> stackB = new Stack<LayoutGroup>();

        private Stack<LayoutGroup> WritingStack { get; set; }
        public Stack<LayoutGroup> ReadingStack { get; private set; }

        public LayoutGroup TopGroup => this.ReadingStack.Peek();


        private string GetStringId(int id)
        {
            if (id == this.rootId)
            {
                return "root";
            }
            string str;
            if(GUILayout.stringIdMap.TryGetValue(id, out str))
            {
                return str;
            }
            return id.ToString();
        }
        StringBuilder sb = new StringBuilder();
        private string GetStackString(Stack<LayoutGroup> stack , int id)
        {
            foreach (var group in stack)
            {
                if (group == null)
                {
                    this.sb.AppendLine("<null>");
                    continue;
                }
                sb.AppendFormat("{0}, Rect = {1} {2}\n", GetStringId(group.Id), group.Rect, id==group.Id? "<-" : "");
                foreach (var entry in group.Entries)
                {
                    sb.AppendFormat("\t{0}, Rect = {1} {2}\n", GetStringId(entry.Id), entry.Rect, id == entry.Id ? "<-" : "");
                    var innerGroup = entry as LayoutGroup;
                    if (innerGroup != null)
                    {
                        foreach (var e in innerGroup.Entries)
                        {
                            sb.AppendFormat("\t\t{0}, Rect = {1} {2}\n", GetStringId(e.Id), e.Rect, id == e.Id ? "<-" : "");
                        }
                    }
                }
            }
            var result = sb.ToString();
            sb.Clear();
            return result;
        }

        private void WriteStacks(int id)
        {
            Console.Clear();
            Console.WriteLine("Reading stack:");
            Console.WriteLine(GetStackString(this.ReadingStack, id));
            Console.WriteLine("Writing stack:");
            Console.WriteLine(GetStackString(this.WritingStack, id));
            Console.WriteLine("--------------------------");
        }

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
            this.rootId = rootId;
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
            Console.WriteLine("GetRect({0})", id);
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
                    WriteStacks(id);
                    return new Rect(100, 100);//dummy
                }
                var entry = group.GetEntry(id);
                if(entry == null)
                {
                    WriteStacks(id);
                    return new Rect(100, 100);//dummy
                }

                WriteStacks(id);
                return entry.Rect;
            }
        }

        public void BeginLayoutGroup(int id, bool isVertical, GUIStyle style = null, LayoutOption[] options = null)
        {
            Console.WriteLine("BeginLayoutGroup({0})", id);
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
            WriteStacks(id);
        }

        public void EndLayoutGroup()
        {
            Console.WriteLine("EndLayoutGroup");
            this.WritingStack.Pop();
            this.ReadingStack.Pop();
            WriteStacks(-1);
        }

        public void Begin()
        {
            Console.WriteLine("Begin");
            this.ReadingStack.Peek().ResetCursor();//reset reading cursor of root group
            this.WritingStack.Peek().Entries.Clear();//remove all children of root group
            WriteStacks(-1);
        }

        /// <summary>
        /// Calculate positions and sizes of every LayoutGroup and LayoutEntry
        /// </summary>
        public void Layout()
        {
            Console.WriteLine("Layout");
            this.WritingStack.Peek().CalcWidth();
            this.WritingStack.Peek().CalcHeight();
            this.WritingStack.Peek().SetX(0);
            this.WritingStack.Peek().SetY(0);

            this.SwapStack();
            WriteStacks(-1);
        }
    }
}
