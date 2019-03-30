using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;

namespace ImGui.Rendering
{
    /// <summary>
    /// The minimal rendering element: tree hierarchy, clipping and how to render itself.
    /// </summary>
    /// <remarks>
    /// Persisting rendering data for controls.
    /// </remarks>
    internal abstract class Visual
    {
        public static bool DefaultUseBoxModel = false;

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
            this.Id = name.Substring(0, idIndex).GetHashCode();
            this.Name = name.Substring(idIndex);
        }

        protected Visual(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// The rectangle this visual occupies. Act as the border-box when using box-model.
        /// </summary>
        public Rect Rect;

        /// <summary>
        /// identifier number
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// string identifier
        /// </summary>
        public string Name { get; set; }

        public double X
        {
            get => this.Rect.X;
            set => this.Rect.X = value;
        }

        public double Y
        {
            get => this.Rect.Y;
            set => this.Rect.Y = value;
        }

        public double Width
        {
            get => this.Rect.Width;
            set => this.Rect.Width = value;
        }

        public double Height
        {
            get => this.Rect.Height;
            set => this.Rect.Height = value;
        }

        public Visual Parent { get; set; }
        protected List<Visual> Children { get; set; }
            = new List<Visual>();
        public int ChildCount => this.Children.Count;

        internal bool ActiveInTree
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

        public bool ActiveSelf { get; set; } = true;
        internal Primitive Primitive { get; set; }

        /// <summary>
        /// Is this visual clipped?
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
        /// <param name="rootClipRect">The root clip rect: client area of the window</param>
        public Rect GetClipRect(Rect rootClipRect)
        {
            Rect clipRect;
            if (this.Parent != null)
            {
                var parentNode = this.Parent;
                if (this.UseBoxModel)
                {
                    clipRect = Utility.GetContentBox(parentNode.Rect, parentNode.RuleSet);//TODO decuple RuleSet and ContentBox
                }
                else
                {
                    clipRect = parentNode.Rect;
                }
                clipRect.Intersect(rootClipRect);
            }
            else
            {
                clipRect = rootClipRect;
            }

            return clipRect;
        }

        internal bool UseBoxModel { get; set; } = DefaultUseBoxModel;

        public void AppendChild(Visual child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            Node.CheckNodeType(this, child);

            child.Parent = this;
            this.Children.Add(child);
        }

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

        public bool RemoveChild(Visual visual)
        {
            return this.Children.Remove(visual);
        }

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
        /// Redraw the node's primitive.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="meshList"></param>
        /// <remarks>A visual can only have one single primitive.</remarks>
        public void Draw(IPrimitiveRenderer renderer, MeshList meshList)
        {
            //TEMP regard all renderer as the built-in renderer
            var r = renderer as GraphicsImplementation.BuiltinPrimitiveRenderer;
            Debug.Assert(r != null);
            r.DrawPrimitive(this.Primitive, this.UseBoxModel, this.Rect, this.RuleSet, meshList);
        }

        public StyleRuleSet RuleSet { get; } = new StyleRuleSet();

        public GUIState State
        {
            get => this.state;
            set
            {
                if (this.state == value)
                {
                    return;
                }

                this.state = value;
                this.RuleSet.SetState(value);
            }
        }

        private GUIState state = GUIState.Normal;
    }

}