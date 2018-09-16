using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;

namespace ImGui.Rendering
{
    [DebuggerDisplay("{"+ nameof(ActiveSelf) +"?\"[*]\":\"[ ]\"}"+"#{" + nameof(Id) + "} " + "{" + nameof(Name) +"}")]
    internal class Node
    {
        /// <summary>
        /// identifier number of the node
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// string identifier of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// border-box, the layout result
        /// </summary>
        public Rect Rect;

        /// <summary>
        /// Create a manually positioned node.
        /// </summary>
        public Node(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Create a manually positioned node.
        /// </summary>
        public Node(int id, string name) : this(id)
        {
            this.Name = name;
        }
        
        /// <summary>
        /// Create a manually positioned node.
        /// </summary>
        public Node(int id, string name, Rect rect) : this(id, name)
        {
            this.Rect = rect;
        }



        #region Layout

        public LayoutEntry LayoutEntry { get; private set; }
        public LayoutGroup LayoutGroup => this.LayoutEntry as LayoutGroup;

        /// <summary>
        /// Make this node a group
        /// </summary>
        public void AttachLayoutGroup(bool isVertical, LayoutOptions? options = null)
        {
            this.LayoutEntry = new LayoutGroup(this);
            this.LayoutGroup.Group_Init(isVertical, options);
        }

        /// <summary>
        /// Make this node an entry
        /// </summary>
        public void AttachLayoutEntry(Size contentSize, LayoutOptions? options = null)
        {
            this.LayoutEntry = new LayoutEntry(this);
            this.LayoutEntry.Entry_Init(contentSize, options);
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

        public void CalcWidth(double unitPartWidth = -1)
        {
            this.LayoutEntry.CalcWidth(unitPartWidth);
        }

        public void CalcHeight(double unitPartHeight = -1)
        {
            this.LayoutEntry.CalcHeight(unitPartHeight);
        }

        public void SetX(double x)
        {
            this.LayoutEntry.SetX(x);
        }

        public void SetY(double y)
        {
            this.LayoutEntry.SetY(y);
        }
        #endregion

        #region Hierarchy
        public Node Parent { get; set; }

        public List<Node> Children { get; set; }
        
        internal bool ActiveInTree
        {
            get
            {
                //already deactived
                if (!ActiveSelf)
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

        internal bool ActiveSelf
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
                        throw new LayoutException("It's not allowed to append a Plain node to a node");
                    case NodeType.LayoutEntry:
                    case NodeType.LayoutGroup:
                        this.LayoutGroup.OnAddLayoutEntry(node.LayoutEntry);
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
                if (continueWithChildren && node.Children!=null && node.Children.Count != 0)
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

        internal Primitive Primitive { get; set; }

        internal bool UseBoxModel { get; set; }

        /// <summary>
        /// Redraw the node's primitive.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="meshList"></param>
        /// <remarks>A node can only have one single primitive.</remarks>
        public void Draw(IPrimitiveRenderer renderer, MeshList meshList)
        {
            if (this.Primitive == null)
            {
                return;
            }

            //TEMP regard all renderer as the built-in renderer
            var r = renderer as GraphicsImplementation.BuiltinPrimitiveRenderer;
            Debug.Assert(r != null);

            switch (this.Primitive)
            {
                case PathPrimitive p:
                {
                    //check render context for shape mesh
                    if (this.RenderContext.shapeMesh == null)
                    {
                        this.RenderContext.shapeMesh = MeshPool.ShapeMeshPool.Get();
                        this.RenderContext.shapeMesh.Node = this;
                    }

                    //get shape mesh
                    var shapeMesh = this.RenderContext.shapeMesh;
                    shapeMesh.Clear();
                    shapeMesh.CommandBuffer.Add(DrawCommand.Default);

                    //draw
                    r.SetShapeMesh(shapeMesh);
                    renderer.DrawPath(p);
                    r.SetShapeMesh(null);

                    //save to mesh list
                    var foundNode = meshList.ShapeMeshes.Find(shapeMesh);
                    if (foundNode == null)
                    {
                        meshList.ShapeMeshes.AddLast(shapeMesh);
                    }
                }
                break;
                case TextPrimitive t:
                {
                    var style = GUIStyle.Default;//FIXME TEMP
                    if (this.UseBoxModel)
                    {
                        //check render context for textMesh
                        if (this.RenderContext.textMesh == null)
                        {
                            this.RenderContext.textMesh = MeshPool.TextMeshPool.Get();
                            this.RenderContext.textMesh.Node = this;
                        }

                        //get text mesh
                        var textMesh = this.RenderContext.textMesh;
                        textMesh.Clear();

                        //check render context for shape mesh
                        if (this.RenderContext.shapeMesh == null)
                        {
                            this.RenderContext.shapeMesh = MeshPool.ShapeMeshPool.Get();
                            this.RenderContext.shapeMesh.Node = this;
                        }

                        //get shape mesh
                        var shapeMesh = this.RenderContext.shapeMesh;
                        shapeMesh.Clear();
                        shapeMesh.CommandBuffer.Add(DrawCommand.Default);

                        //draw
                        r.SetShapeMesh(shapeMesh);
                        r.SetTextMesh(textMesh);
                        r.DrawBoxModel(t, this.Rect, style);
                        r.SetShapeMesh(null);
                        r.SetTextMesh(null);

                        //save to mesh list
                        if (!meshList.ShapeMeshes.Contains(shapeMesh))
                        {
                            meshList.ShapeMeshes.AddLast(shapeMesh);
                        }
                        if (!meshList.TextMeshes.Contains(textMesh))
                        {
                            meshList.TextMeshes.AddLast(textMesh);
                        }
                    }
                    else
                    {
                        //check render context for textMesh
                        if (this.RenderContext.textMesh == null)
                        {
                            this.RenderContext.textMesh = MeshPool.TextMeshPool.Get();
                            this.RenderContext.textMesh.Node = this;
                        }

                        //clear text mesh
                        var textMesh = this.RenderContext.textMesh;
                        textMesh.Clear();

                        //draw
                        r.SetTextMesh(textMesh);
                        r.DrawText(t, this.Rect, style);
                        r.SetTextMesh(null);

                        //save to mesh list
                        if (!meshList.TextMeshes.Contains(textMesh))
                        {
                            meshList.TextMeshes.AddLast(textMesh);
                        }
                    }
                }
                break;
                case ImagePrimitive i:
                {
                    var style = GUIStyle.Default;//FIXME TEMP
                    style.BackgroundColor = Color.White;//TEMP (default Clear won't show the image!!)
                    if (this.UseBoxModel)
                    {
                        //check render context for image mesh
                        if (this.RenderContext.imageMesh == null)
                        {
                            this.RenderContext.imageMesh = MeshPool.ImageMeshPool.Get();
                            this.RenderContext.imageMesh.Node = this;
                        }

                        //clear image mesh
                        var imageMesh = this.RenderContext.imageMesh;
                        imageMesh.Clear();

                        //check render context for shape mesh
                        if (this.RenderContext.shapeMesh == null)
                        {
                            this.RenderContext.shapeMesh = MeshPool.ShapeMeshPool.Get();
                            this.RenderContext.shapeMesh.Node = this;
                        }

                        //clear shape mesh
                        var shapeMesh = this.RenderContext.shapeMesh;
                        shapeMesh.Clear();
                        shapeMesh.CommandBuffer.Add(DrawCommand.Default);

                        //draw
                        r.SetImageMesh(imageMesh);
                        r.SetShapeMesh(shapeMesh);
                        r.DrawBoxModel(i, this.Rect, style);
                        r.SetShapeMesh(null);
                        r.SetImageMesh(null);

                        //save to mesh list
                        if (!meshList.ShapeMeshes.Contains(shapeMesh))
                        {
                            meshList.ShapeMeshes.AddLast(shapeMesh);
                        }
                        if (!meshList.ImageMeshes.Contains(imageMesh))
                        {
                            meshList.ImageMeshes.AddLast(imageMesh);
                        }
                    }
                    else
                    {
                        //check render context for image mesh
                        if (this.RenderContext.imageMesh == null)
                        {
                            this.RenderContext.imageMesh = MeshPool.ImageMeshPool.Get();
                            this.RenderContext.imageMesh.Node = this;
                        }

                        //clear image mesh
                        var imageMesh = this.RenderContext.imageMesh;
                        imageMesh.Clear();

                        r.SetImageMesh(imageMesh);
                        renderer.DrawImage(i, this.Rect, style);
                        r.SetImageMesh(null);

                        //save to mesh list
                        if (!meshList.ImageMeshes.Contains(imageMesh))
                        {
                            meshList.ImageMeshes.AddLast(imageMesh);
                        }
                    }
                }
                break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        /// <summary>
        /// internal render context refers to a context object.
        /// </summary>
        internal (Mesh shapeMesh, Mesh imageMesh, TextMesh textMesh) RenderContext;

        private bool activeSelf = true;
    }
}
