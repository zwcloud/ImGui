using System;
using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.Layout;

namespace ImGui.Rendering
{
    /// <summary>
    /// The minimal layout element.
    /// </summary>
    /// <remarks>
    /// Persisting styling and layout data for <see cref="Visual"/>s of a control.
    /// </remarks>
    [DebuggerDisplay("{" + nameof(ActiveSelf) + "?\"[*]\":\"[ ]\"}" + "#{" + nameof(Id) + "} " + "{" + nameof(Name) + "}")]
    internal partial class Node : Visual, IStyleRuleSet
    {
        public Node(int id) : base(id)
        {
        }

        public Node(int id, Rect rect) : this(id)
        {
            this.Rect = rect;
        }

        public Node(string name) : base(name)
        {
        }

        public Node(string name, Rect rect) : this(name)
        {
            this.Rect = rect;
        }

        public Node(int id, string name) : base(id, name)
        {
        }

        public Node(int id, string name, Rect rect) : this(id, name)
        {
            this.Rect = rect;
        }

        #region Layout

        public static bool DefaultUseBoxModel = false;

        /// <summary>
        /// Whether box-model be applied when rendering this Node.
        /// </summary>
        internal bool UseBoxModel { get; set; } = DefaultUseBoxModel;

        public bool IsGroup { get; set; }

        public bool IsVertical { get; set; }

        public Rect ContentRect;

        public Size ContentSize
        {
            get => this.ContentRect.Size;
            set => this.ContentRect.Size = value;
        }

        public double ContentWidth
        {
            get => this.ContentRect.Width;
            set => this.ContentRect.Width = value;
        }

        public double ContentHeight
        {
            get => this.ContentRect.Height;
            set => this.ContentRect.Height = value;
        }

        /// <summary>
        /// Make this node a group
        /// </summary>
        public void AttachLayoutGroup(bool isVertical)
        {
            this.IsGroup = true;
            this.IsVertical = isVertical;
        }

        /// <summary>
        /// Make this node a layout entry.
        /// </summary>
        public void AttachLayoutEntry(Size contentSize)
        {
            this.ContentRect.Size = contentSize;
        }

        /// <summary>
        /// Make this node a layout entry.
        /// </summary>
        public void AttachLayoutEntry() => this.AttachLayoutEntry(Size.Zero);

        /// <summary>
        /// Layout the sub-tree rooted at this node: the root node is placed at the specified position.
        /// </summary>
        public void Layout(Point p)
        {
            StackLayout.Layout(this, p);
        }

        /// <summary>
        /// Layout the sub-tree rooted at this node
        /// </summary>
        public void Layout()
        {
            StackLayout.Layout(this, this.Rect.Location);
        }

        enum NodeType
        {
            LayoutEntry,
            LayoutGroup
        }

        private static NodeType GetNodeType(Node node)
        {
            return node.IsGroup ? NodeType.LayoutGroup : NodeType.LayoutEntry;
        }

        internal static void CheckNodeType(Visual parentVisual, Visual v)
        {
            var parent = (Node)parentVisual;
            var node = (Node)v;

            NodeType thisNodeType = GetNodeType(parent);
            NodeType nodeType = GetNodeType(node);

            /* Rules:
             * 1. LayoutEntry nodes should always be a children of a LayoutGroup;
             * 2. LayoutEntry nodes are always leaf nodes.
             * 3. Layout related rules.
             */

            if (thisNodeType == NodeType.LayoutEntry)
            {
                throw new LayoutException("It's not allowed to append any node to a LayoutEntry node");
            }

            if (thisNodeType == NodeType.LayoutGroup)
            {
                switch (nodeType)
                {
                    case NodeType.LayoutEntry:
                    case NodeType.LayoutGroup:
                        if (parent.RuleSet.IsDefaultWidth && node.RuleSet.IsStretchedWidth)
                        {
                            throw new LayoutException(
                                "It's not allowed to append a stretched node to a default-sized LayoutGroup node");
                        }

                        if (parent.RuleSet.IsDefaultHeight && node.RuleSet.IsStretchedHeight)
                        {
                            throw new LayoutException(
                                "It's not allowed to append a stretched node to a default-sized LayoutGroup node");
                        }

                        parent.CheckRuleSetForLayout_Group(node);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Node GetNodeById(int id)
        {
            var visual = GetVisualById(id);
            return (Node) visual;
        }

        public Node GetDirectNodeById(int id)
        {
            return (Node) this.Children.Find(n => n.Id == id);
        }

        public bool Contains(Node node)
        {
            return this.Children.Contains(node);
        }

        #endregion

        public override Rect GetClipRect(Rect rootClipRect)
        {
            Rect clipRect;
            if (this.Parent != null)
            {
                var parentNode = (Node)this.Parent;
                clipRect = parentNode.UseBoxModel ? parentNode.ContentRect : parentNode.Rect;
                clipRect.Intersect(rootClipRect);
            }
            else
            {
                clipRect = rootClipRect;
            }

            return clipRect;
        }

        public override void Draw(IPrimitiveRenderer renderer, MeshList meshList)
        {
            //TEMP regard all renderer as the built-in renderer
            var r = renderer as GraphicsImplementation.BuiltinPrimitiveRenderer;
            Debug.Assert(r != null);
            r.DrawPrimitive(this.Primitive, this.UseBoxModel, this.Rect, this.RuleSet, meshList);
        }

        /// <summary>
        /// UI state
        /// </summary>
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
