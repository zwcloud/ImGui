using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Common.Primitive;
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
            //TODO check if all children is Node

            var parent = (Node)parentVisual;
            var node = (Node)v;

            NodeType thisNodeType = GetNodeType(parent);
            NodeType nodeType = GetNodeType(node);

            /* Rules:
             * 1. Plain nodes are not allowed to be added to a layout-ed node tree,
             * which should only contain LayoutEntry and LayoutGroup;
             * 2. LayoutEntry nodes should always be a children of a LayoutGroup;
             * 3. LayoutEntry nodes are always leaf nodes.
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

                        parent.CheckRuleSetForLayout_Group(node);//TODO handle this
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
    }
}
