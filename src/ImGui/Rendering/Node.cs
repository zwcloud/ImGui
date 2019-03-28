using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.GraphicsImplementation;

namespace ImGui.Rendering
{
    /// <summary>
    /// The minimal layout and style element: styling and layout.
    /// </summary>
    /// <remarks>
    /// Persisting styling and layout data for <see cref="Visual"/>s of a control.
    /// </remarks>
    [DebuggerDisplay("{" + nameof(ActiveSelf) + "?\"[*]\":\"[ ]\"}" + "#{" + nameof(Id) + "} " + "{" + nameof(Name) + "}")]
    internal class Node : Visual, IStyleRuleSet, ILayoutGroup
    {
        /// <summary>
        /// Create a node.
        /// </summary>
        public Node(int id)
        {
            this.Id = id;
            this.RuleSet = new StyleRuleSet();
        }

        public Node(int id, Rect rect) : this(id)
        {
            this.Rect = rect;
        }

        /// <summary>
        /// Create a node.
        /// </summary>
        public Node(int id, string name) : this(id)
        {
            this.Name = name;
        }

        /// <summary>
        /// Create a node.
        /// </summary>
        public Node(int id, string name, Rect rect) : this(id, name)
        {
            this.Rect = rect;
        }

        /// <summary>
        /// Create a node.
        /// </summary>
        public Node(string name)
        {
            var idIndex = name.IndexOf('#');
            if (idIndex < 0)
            {
                throw new ArgumentException("No id is specfied in the name.", nameof(name));
            }
            this.Id = name.Substring(idIndex).GetHashCode();
            this.Name = name;
            this.RuleSet = new StyleRuleSet();
        }

        /// <summary>
        /// Create a node.
        /// </summary>
        public Node(string name, Rect rect) : this(name)
        {
            this.Rect = rect;
        }

        #region Layout

        public LayoutEntry LayoutEntry { get; private set; }
        public LayoutGroup LayoutGroup => this.LayoutEntry as LayoutGroup;

        /// <summary>
        /// Make this node a group
        /// </summary>
        public void AttachLayoutGroup(bool isVertical)
        {
            this.LayoutEntry = new LayoutGroup(this, isVertical);
            this.Children = new List<Node>();
        }

        /// <summary>
        /// Make this node a layout entry.
        /// </summary>
        public void AttachLayoutEntry(Size contentSize)
        {
            this.LayoutEntry = new LayoutEntry(this, contentSize);
            this.Children = null;
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
            this.LayoutGroup.CalcWidth(this.LayoutEntry.ContentWidth);
            this.LayoutGroup.CalcHeight(this.LayoutEntry.ContentHeight);
            this.LayoutGroup.SetX(p.X);
            this.LayoutGroup.SetY(p.Y);
        }

        /// <summary>
        /// Layout the sub-tree rooted at this node
        /// </summary>
        public void Layout()
        {
            this.LayoutGroup.CalcWidth(this.LayoutEntry.ContentWidth);
            this.LayoutGroup.CalcHeight(this.LayoutEntry.ContentHeight);
            this.LayoutGroup.SetX(this.Rect.X);
            this.LayoutGroup.SetY(this.Rect.Y);
        }
        #endregion

        #region Hierarchy
        public IEnumerator<ILayoutEntry> GetEnumerator()
        {
            return this.Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        enum NodeType
        {
            Plain,
            LayoutEntry,
            LayoutGroup
        }

        private static NodeType GetNodeType(Node node)
        {
            NodeType nodeType = NodeType.Plain;
            do
            {
                if (node.LayoutEntry is LayoutGroup)
                {
                    nodeType = NodeType.LayoutGroup;
                    break;
                }

                if (node.LayoutEntry != null)
                {
                    nodeType = NodeType.LayoutEntry;
                    break;
                }
            } while (false);

            return nodeType;
        }

        public void AppendChild(Node node)
        {
            NodeType thisNodeType = GetNodeType(this);
            NodeType nodeType = GetNodeType(node);

            /* Rules:
             * 1. Plain nodes are not allowed to be added to a layout-ed node tree,
             * which should only contain LayoutEntry and LayoutGroup;
             * 2. LayoutEntry nodes should always be a children of a LayoutGroup;
             * 3. LayoutEntry nodes are always leaf nodes.
             */

            if (thisNodeType == NodeType.Plain && nodeType != NodeType.Plain)
            {
                throw new LayoutException("It's not allowed to append a Plain node to a non-Plain node");
            }

            if (thisNodeType == NodeType.LayoutEntry)
            {
                throw new LayoutException("It's not allowed to append any node to a LayoutEntry node");
            }

            if (thisNodeType == NodeType.LayoutGroup)
            {
                switch (nodeType)
                {
                    case NodeType.Plain:
                        throw new LayoutException("It's not allowed to append a Plain node to a LayoutGroup node");
                    case NodeType.LayoutEntry:
                    case NodeType.LayoutGroup:
                        if (this.RuleSet.IsDefaultWidth && node.RuleSet.IsStretchedWidth)
                        {
                            throw new LayoutException("It's not allowed to append a stretched node to a default-sized LayoutGroup node");
                        }
                        if (this.RuleSet.IsDefaultHeight && node.RuleSet.IsStretchedHeight)
                        {
                            throw new LayoutException("It's not allowed to append a stretched node to a default-sized LayoutGroup node");
                        }
                        this.LayoutGroup.OnAddLayoutEntry(node);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            this.SetUpParentChildren(node);
        }

        private void SetUpParentChildren(Node childNode)
        {
            childNode.Parent = this;
            if (this.Children == null)
            {
                this.Children = new List<Node>();
            }
            this.Children.Add(childNode);
        }

        //TODO maybe we should use an extra dictionary to retrive node by id, O(1) but occupies more memory
        public Node GetNodeById(int id)
        {
            if (this.Children == null)
            {
                return null;
            }
            foreach (var node in this.Children)
            {
                if (node.Id == id)
                {
                    return node;
                }

                Node child = node.GetNodeById(id);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the first child node of the specified name
        /// </summary>
        public Node GetNodeByName(string name)
        {
            if (this.Children == null)
            {
                return null;
            }
            foreach (var node in this.Children)
            {
                if (node.Name == name)
                {
                    return node;
                }

                Node child = node.GetNodeByName(name);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        public void Foreach(Func<Node, bool> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var node in this.Children)
            {
                var continueWithChildren = func(node);
                if (continueWithChildren && node.Children != null && node.Children.Count != 0)
                {
                    node.Foreach(func);
                }
            }
        }

        public bool RemoveChild(Node node)
        {
            return this.Children.Remove(node);
        }
        #endregion

        #region Draw

        public StyleRuleSet RuleSet { get; }

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

        /// <summary>
        /// Redraw the node's primitive.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="meshList"></param>
        /// <remarks>A node can only have one single primitive.</remarks>
        public void Draw(IPrimitiveRenderer renderer, MeshList meshList)
        {
            //TEMP regard all renderer as the built-in renderer
            var r = renderer as BuiltinPrimitiveRenderer;
            Debug.Assert(r != null);
            r.DrawPrimitive(this.Primitive, this.UseBoxModel, this.Rect, this.RuleSet, meshList);
        }
        #endregion

        private GUIState state = GUIState.Normal;
    }
}
