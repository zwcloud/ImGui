using System;
using System.Collections;
using System.Collections.Generic;
using ImGui.GraphicsAbstraction;

namespace ImGui.Rendering
{
    /// <summary>
    /// The minimal rendering element: tree hierarchy, clipping and how to render itself.
    /// </summary>
    /// <remarks>
    /// Persisting rendering data for controls.
    /// </remarks>
    internal abstract class Visual : IEnumerable<Visual>
    {
        protected Visual(int id)
        {
            this.Id = id;
        }

        protected Visual(string name)
        {
            var idIndex = name.IndexOf('#');
            if (idIndex < 0)
            {
                throw new ArgumentException("No id is specfied in the name.", nameof(name));
            }
            this.Id = name.Substring(idIndex).GetHashCode();
            this.Name = name.Substring(0, idIndex);
        }

        protected Visual(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// The rectangle this visual occupies. Act as the border-box when using box-model.
        /// </summary>
        public Rect Rect
        {
            get;
            set;
        }

        /// <summary>
        /// identifier number
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// string identifier
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Shortcut of Visual.Rect.X
        /// </summary>
        public double X
        {
            get => this.Rect.X;
            set
            {
                var r = this.Rect;
                r.X = value;
                this.Rect = r;
            }
        }

        /// <summary>
        /// Shortcut of Visual.Rect.Y
        /// </summary>
        public double Y
        {
            get => this.Rect.Y;
            set
            {
                var r = this.Rect;
                r.Y = value;
                this.Rect = r;
            }
        }

        /// <summary>
        /// Shortcut of Visual.Rect.Width
        /// </summary>
        public double Width
        {
            get => this.Rect.Width;
            set
            {
                var r = this.Rect;
                r.Width = value;
                this.Rect = r;
            }
        }

        /// <summary>
        /// Shortcut of Visual.Rect.Height
        /// </summary>
        public double Height
        {
            get => this.Rect.Height;
            set
            {
                var r = this.Rect;
                r.Height = value;
                this.Rect = r;
            }
        }

        /// <summary>
        /// Parent
        /// </summary>
        public Visual Parent { get; set; }

        /// <summary>
        /// Children list
        /// </summary>
        protected List<Visual> Children { get; } = new List<Visual>();

        /// <summary>
        /// The number of visuals in the list of children
        /// </summary>
        public int ChildCount => this.Children.Count;

        public IEnumerator<Visual> GetEnumerator()
        {
            return this.Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Defines whether the Visual is active in the render tree.
        /// </summary>
        public bool ActiveInTree
        {
            get
            {
                //already deactived
                if (!this.ActiveSelf)
                {
                    return false;
                }

                //check if all ancestors are active
                Visual ancestorNode = this;
                do
                {
                    ancestorNode = ancestorNode.Parent;
                    if (ancestorNode == null)
                    {
                        break;
                    }
                    if (!ancestorNode.ActiveSelf)
                    {
                        return false;
                    }
                } while (ancestorNode.ActiveSelf);

                return true;
            }
        }

        /// <summary>
        /// The local active state of this Visual
        /// </summary>
        public bool ActiveSelf { get; set; } = true;

        /// <summary>
        /// Whether this visual is clipped: it doesn't intersect with the clip rectangle.
        /// </summary>
        public bool IsClipped(Rect clipRect)
        {
            if (clipRect.IntersectsWith(this.Rect))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the clip rect that applies to this visual.
        /// </summary>
        public abstract Rect GetClipRect();

        private int insertIndex = 0;

        /// <summary>
        /// Adds a visual to the end of the list of children.
        /// If the visual is already a child, it will be moved to the last position
        /// in the children list.
        /// </summary>
        public void AppendChild(Visual child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (child is Node)
            {
                Node.CheckNodeType(this, child);
            }

            child.Parent = this;
            if(this.Children.Count == 0)
            {
                this.insertIndex = 0;
            }

            var existingIndex = Children.FindIndex(v => v.Id == child.Id);
            if(existingIndex < 0)
            {
                if(this.insertIndex > 0)
                {
                    this.Children.Insert(this.insertIndex, child);
                }
                else
                {
                    this.Children.Add(child);
                }
            }
            else
            {
                this.insertIndex = existingIndex + 1;
                this.insertIndex = 0;
                //Move found child to last element
                this.Children.RemoveAt(existingIndex);
                this.Children.Add(child);
            }
        }

        public Visual GetVisualByIndex(int i)
        {
            if (this.Children == null)
            {
                return null;
            }

            if (this.Children.Count - 1 < i)
            {
                throw new System.IndexOutOfRangeException();
            }

            return this.Children[i];
        }

        /// <summary>
        /// Find a visual in the list of children recursively.
        /// </summary>
        public Visual GetVisualById(int id)
        {
            if (this.Children == null)
            {
                return null;
            }
            foreach (var visual in this.Children)
            {
                if (visual.Id == id)
                {
                    return visual;
                }

                Visual child = visual.GetVisualById(id);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// Remove a direct child visual from the list of children.
        /// </summary>
        public bool RemoveChildVisual(Visual visual)
        {
            return this.Children.Remove(visual);
        }

        /// <summary>
        /// Executes the provided callback once for each element present in the list of children recursively.
        /// Recursion will be stopped if the callback returns false: the callback won't be called on the child.
        /// </summary>
        public void Foreach(Func<Visual, bool> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var visual in this.Children)
            {
                var continueWithChildren = func(visual);
                if (continueWithChildren && visual.Children != null && visual.Children.Count != 0)
                {
                    visual.Foreach(func);
                }
            }
        }

        /// <summary>
        /// Rule Set
        /// </summary>
        public StyleRuleSet RuleSet { get; } = new StyleRuleSet();

        internal VisualFlags Flags { get; set; } = VisualFlags.None;

        internal void SetFlags(bool value, VisualFlags flags)
        {
            Flags = value ? (Flags | flags) : (Flags & (~flags));
        }

        internal virtual void Render(RenderContext context)
        {
            RenderRecursive(context);
        }

        internal virtual void RenderAfterChildren(RenderContext context)
        {
        }

        internal void RenderRecursive(RenderContext context)
        {
            if (context.ClipRect.IsEmpty)//completely clipped
            {
                return;
            }

            var oldClipRect = context.ClipRect;
            var clipRect = this.GetClipRect();
            clipRect.Intersect(oldClipRect);
            if (clipRect.IsEmpty)//completely clipped by context-decided clip rectangle
            {
                return;
            }
            context.ClipRect = clipRect;
            if (!RenderContent(context))//NOTE RenderContent will use context.ClipRect
            {
                return;
            }
            for (var i = 0; i < this.ChildCount; i++)
            {
                var child = GetVisualByIndex(i);
                if (child.IsClipped(clipRect))
                {
                    continue;
                }
                child.RenderRecursive(context);
            }
            RenderAfterChildren(context);
            context.ClipRect = oldClipRect;
        }

        /// <summary>
        /// Convert content into GPU renderable resources: Mesh/TextMesh
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true: content is rendered; false: content is not rendered</returns>
        internal abstract bool RenderContent(RenderContext context);

        /// <summary>
        /// Called from the DrawingContext when the DrawingContext is closed.
        /// </summary>
        /// <param name="newContent"></param>
        internal abstract void RenderClose(DrawingContent newContent);
    }

}