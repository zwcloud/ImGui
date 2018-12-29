using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.GraphicsImplementation;

namespace ImGui.Rendering
{
    [DebuggerDisplay("{" + nameof(ActiveSelf) + "?\"[*]\":\"[ ]\"}" + "#{" + nameof(Id) + "} " + "{" + nameof(Name) + "}")]
    internal class Node : IStyleRuleSet, ILayoutGroup
    {
        public static bool DefaultUseBoxModel = false;

        /// <summary>
        /// identifier number
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// string identifier
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The rectangle this node occupies. Act as the border-box when using box-model.
        /// </summary>
        public Rect Rect;

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
        public Node Parent { get; set; }

        public List<Node> Children { get; set; }

        public int ChildCount => this.Children.Count;

        public IEnumerator<ILayoutEntry> GetEnumerator()
        {
            return this.Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

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
                Node ancestorNode = this;
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

        public bool ActiveSelf
        {
            get => this.activeSelf;
            set
            {
                this.activeSelf = value;
                if (this.RenderContext.shapeMesh != null)
                {
                    this.RenderContext.shapeMesh.Visible = value;
                }
                if (this.RenderContext.textMesh != null)
                {
                    this.RenderContext.textMesh.Visible = value;
                }
                if (this.RenderContext.imageMesh != null)
                {
                    this.RenderContext.imageMesh.Visible = value;
                }
            }
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
        internal Primitive Primitive
        {
            get
            {
                if (this.PrimitiveList.Count == 0)
                {
                    return null;
                }

                return this.PrimitiveList[0];
            }
            set
            {
                if (this.PrimitiveList.Count == 0)
                {
                    this.PrimitiveList.Add(value);
                }
                else
                {
                    this.PrimitiveList[0] = value;
                }
            }
        }

        internal List<Primitive> PrimitiveList { get; set; } = new List<Primitive>(1);

        internal bool UseBoxModel { get; set; } = DefaultUseBoxModel;

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

        public RenderContext RenderContext { get; } = new RenderContext();

        public void Draw(IPrimitiveRenderer renderer, MeshList meshList) => this.Draw(renderer, Vector.Zero, meshList);

        /// <summary>
        /// Redraw the node's primitive.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="meshList"></param>
        /// <remarks>A node can only have one single primitive.</remarks>
        public void Draw(IPrimitiveRenderer renderer, Vector offset, MeshList meshList)
        {
            //TEMP regard all renderer as the built-in renderer
            var r = renderer as BuiltinPrimitiveRenderer;
            Debug.Assert(r != null);

            var renderContext = this.RenderContext;

            //special common case: an empty box-model
            if (this.Primitive == null && this.UseBoxModel)
            {
                renderContext.CheckShapeMesh();
                renderContext.CheckImageMesh();

                renderContext.ClearShapeMesh();
                renderContext.ClearImageMesh();

                r.SetShapeMesh(renderContext.shapeMesh);
                r.SetImageMesh(renderContext.imageMesh);
                r.DrawBoxModel(Rect.Offset(this.Rect, offset), this.RuleSet);
                r.SetShapeMesh(null);
                r.SetImageMesh(null);

                meshList.AddOrUpdateShapeMesh(renderContext.shapeMesh);
                meshList.AddOrUpdateImageMesh(renderContext.imageMesh);
            }
            else
            {
                foreach (var primitive in this.PrimitiveList)
                {
                    switch (primitive)
                    {
                        case PathPrimitive p:
                            if (this.UseBoxModel)
                            {
                                throw new NotImplementedException();
                            }
                            else
                            {
                                renderContext.CheckShapeMesh();
                                renderContext.ClearShapeMesh();

                                r.DrawPathPrimitive(renderContext.shapeMesh, p, (Vector)this.Rect.Location);

                                meshList.AddOrUpdateShapeMesh(renderContext.shapeMesh);
                            }

                            break;
                        case TextPrimitive t:
                            if (this.UseBoxModel)
                            {
                                renderContext.CheckTextMesh();
                                renderContext.CheckShapeMesh();
                                renderContext.CheckImageMesh();

                                renderContext.ClearTextMesh();
                                renderContext.ClearImageMesh();
                                renderContext.ClearShapeMesh();

                                r.SetShapeMesh(renderContext.shapeMesh);
                                r.SetTextMesh(renderContext.textMesh);
                                r.SetImageMesh(renderContext.imageMesh);
                                r.DrawBoxModel(t, Rect.Offset(this.Rect, offset), this.RuleSet);
                                r.SetShapeMesh(null);
                                r.SetTextMesh(null);
                                r.SetImageMesh(null);

                                meshList.AddOrUpdateShapeMesh(renderContext.shapeMesh);
                                meshList.AddOrUpdateImageMesh(renderContext.imageMesh);
                                meshList.AddOrUpdateTextMesh(renderContext.textMesh);
                            }
                            else
                            {
                                renderContext.CheckTextMesh();
                                renderContext.ClearTextMesh();

                                r.DrawTextPrimitive(renderContext.textMesh, t, this.Rect, this.RuleSet, offset);

                                meshList.AddOrUpdateTextMesh(renderContext.textMesh);
                            }

                            break;
                        case ImagePrimitive i:
                            if (this.UseBoxModel)
                            {
                                renderContext.CheckShapeMesh();
                                renderContext.CheckImageMesh();

                                renderContext.ClearShapeMesh();
                                renderContext.ClearImageMesh();

                                r.SetImageMesh(renderContext.imageMesh);
                                r.SetShapeMesh(renderContext.shapeMesh);
                                r.DrawBoxModel(i, Rect.Offset(this.Rect, offset), this.RuleSet);
                                r.SetShapeMesh(null);
                                r.SetImageMesh(null);

                                meshList.AddOrUpdateShapeMesh(renderContext.shapeMesh);
                                meshList.AddOrUpdateImageMesh(renderContext.imageMesh);
                            }
                            else
                            {
                                renderContext.CheckImageMesh();
                                renderContext.ClearImageMesh();

                                r.DrawImagePrimitive(renderContext.imageMesh, i, this.Rect, this.RuleSet, offset);

                                meshList.AddOrUpdateImageMesh(renderContext.imageMesh);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        #endregion

        private bool activeSelf = true;
        private GUIState state = GUIState.Normal;

        /// <summary>
        /// Is this node clipped?
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
        /// Get the clip rect that applies to this node.
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
                    clipRect = Utility.GetContentBox(parentNode.Rect, parentNode.RuleSet);
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
    }
}
