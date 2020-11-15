using System;
using System.Diagnostics;
using ImGui.Layout;

namespace ImGui.Rendering
{
    /// <summary>
    /// The minimal layout element.
    /// </summary>
    /// <remarks>
    /// Persisting styling and layout data for <see cref="Visual"/>s of a control.
    /// </remarks>
    [DebuggerDisplay("{ActiveSelf?\"☑️\":\"☐\",nq} Id:{Id} Name:{Name,nq} Rect:{Rect}")]
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

        public double NaturalWidth { get; set; }

        public double NaturalHeight { get; set; }

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
            this.NaturalWidth = contentSize.Width;
            this.NaturalHeight = contentSize.Height;

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

        public override Rect GetClipRect()
        {
            Rect clipRect = Rect.Big;
            if (this.Parent != null)
            {
                var parentNode = (Node)this.Parent;

                //check all ancestor nodes
                while (parentNode != null)
                {
                    if (parentNode.HorizontallyOverflow || parentNode.VerticallyOverflow)
                    {
                        //TODO consider in two aspects: horizontal and vertical
                        var clipRectFromParent = parentNode.UseBoxModel ?
                            parentNode.ContentRect : parentNode.Rect;
                        clipRect = Rect.Intersect(clipRect, clipRectFromParent);
                    }
                    parentNode = (Node)parentNode.Parent;
                }
            }

            return clipRect;
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

        #region new rendering pipeline

        internal override void Render(RenderContext context)
        {
            base.Render(context);
        }

        internal override void RenderAfterChildren(RenderContext context)
        {
            if (HScrollBarRoot != null)
            {
                HScrollBarRoot.Render(context);
            }
            if (VScrollBarRoot != null)
            {
                VScrollBarRoot.Render(context);
            }
        }

        internal override bool RenderContent(RenderContext context)
        {
            if (content != null && ActiveInTree)
            {
                context.ConsumeContent(content);
            }

            //empty active node renders empty content
            return ActiveInTree;
        }

        /// <summary>
        /// Opens the Node for static rendering. The returned DrawingContext can be used to
        /// render into the Node: populate the content.
        /// </summary>
        internal DrawingContext RenderOpen()
        {
            this.isAppendingContent = false;
            return new VisualDrawingContext(this);
        }

        internal DrawingContext RenderAppend()
        {
            this.isAppendingContent = true;
            return new VisualDrawingContext(this);
        }

        internal override void RenderClose(DrawingContent newContent)
        {
            if (isAppendingContent)
            {
                content.AppendRecords(newContent);
                isAppendingContent = false;
            }
            else
            {
                content = newContent;
            }
        }

        private bool isAppendingContent = false;
        private DrawingContent content;

        #endregion
    }
}
