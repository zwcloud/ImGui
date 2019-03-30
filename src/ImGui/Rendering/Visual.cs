using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    /// <summary>
    /// The minimal rendering element: tree hierarchy, clipping and how to render itself.
    /// </summary>
    /// <remarks>
    /// Persisting rendering data for controls.
    /// </remarks>
    internal class Visual
    {
        public static bool DefaultUseBoxModel = false;

        public Visual(int id)
        {
            this.Id = id;
        }

        public Visual(string name)
        {
            var idIndex = name.IndexOf('#');
            if (idIndex < 0)
            {
                throw new ArgumentException("No id is specfied in the name.", nameof(name));
            }
            this.Id = name.Substring(0, idIndex).GetHashCode();
            this.Name = name.Substring(idIndex);
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
        public List<Visual> Children { get; set; }
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
                var parentNode = this.Parent as Node;
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
            if (this.Children == null)
            {
                this.Children = new List<Visual>();
            }
            this.Children.Add(child);
        }

    }

}