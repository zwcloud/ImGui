using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Layout;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;

namespace ImGui.Rendering
{
    [DebuggerDisplay("{"+ nameof(ActiveSelf) +"?\"[*]\":\"[ ]\"}"+"#{" + nameof(Id) + "} " + "{" + nameof(Name) +"}")]
    internal partial class Node
    {
        /// <summary>
        /// identifier number of the node
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// string identifier of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// border-box, the layout result
        /// </summary>
        public Rect Rect;

        public Node(int id)
        {
            this.Id = id;
        }
        
        public Node(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        #region Layout
        /// <summary>
        /// Make this node a group
        /// </summary>
        public void AttachLayoutGroup(bool isVertical, LayoutOptions? options = null)
        {
            this.Group_Init(this.Id, isVertical, options);
            //TODO delete entry?
        }

        /// <summary>
        /// Make this node an entry
        /// </summary>
        public void AttachLayoutEntry(Size contentSize, LayoutOptions? options = null)
        {
            this.Entry_Init(Id, contentSize, options);
        }

        /// <summary>
        /// Layout the sub-tree rooted at this node
        /// </summary>
        public void Layout()
        {
            this.Group_CalcWidth(this.ContentWidth);
            this.Group_CalcHeight(this.ContentHeight);
            this.Group_SetX(this.Rect.X);
            this.Group_SetY(this.Rect.Y);
        }

        private void CalcWidth(double unitPartWidth = -1)
        {
            if (this.Children == null)
            {
                this.Entry_CalcWidth(unitPartWidth);
            }
            else
            {
                this.Group_CalcWidth(unitPartWidth);
            }
        }

        private void CalcHeight(double unitPartHeight = -1)
        {
            if (this.Children == null)
            {
                this.Entry_CalcHeight(unitPartHeight);
            }
            else
            {
                this.Group_CalcHeight(unitPartHeight);
            }
        }

        private void SetX(double x)
        {
            if (this.Children == null)
            {
                this.Entry_SetX(x);
            }
            else
            {
                this.Group_SetX(x);
            }
        }

        
        private void SetY(double y)
        {
            if (this.Children == null)
            {
                this.Entry_SetY(y);
            }
            else
            {
                this.Group_SetY(y);
            }
        }
        #endregion

        #region Hierarchy
        public Node Parent { get; set; }

        public List<Node> Children { get; set; }

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
                if (this.RenderContext is Mesh mesh)
                {
                    mesh.Visible = value;
                }
            }
        }

        internal Primitive Primitive { get; set; }
        internal bool IsFill { get; set; } = false;
        internal Brush Brush { get; set; } = new Brush();
        internal StrokeStyle StrokeStyle { get; set; } = new StrokeStyle();

        /// <summary>
        /// Redraw the node's primitive.
        /// </summary>
        /// <param name="renderer"></param>
        /// <remarks>A node can only have one single primitive.</remarks>
        public void Draw(IPrimitiveRenderer renderer)
        {
            if (this.Primitive == null)
            {
                return;
            }
            //TEMP regard all renderer as the built-in renderer
            var builtinPrimitiveRenderer = renderer as GraphicsImplementation.BuiltinPrimitiveRenderer;
            Debug.Assert(builtinPrimitiveRenderer != null);
            switch (this.Primitive)
            {
                case PathPrimitive p:
                {
                    Mesh mesh = null;

                    if (this.RenderContext == null)
                    {
                        mesh = MeshPool.ShapeMeshPool.Get();
                        this.RenderContext = mesh;
                    }
                    else
                    {
                        mesh = (Mesh) this.RenderContext;
                    }
                    
                    mesh.Clear();
                    mesh.CommandBuffer.Add(DrawCommand.Default);
                    mesh.Node = this;

                    builtinPrimitiveRenderer.SetShapeMesh(mesh);

                    if (this.IsFill)
                    {
                        renderer.Fill(p, this.Brush);
                    }
                    else
                    {
                        renderer.Stroke(p, this.Brush, this.StrokeStyle);
                    }
                    var foundNode = MeshList.ShapeMeshes.Find(mesh);
                    if (foundNode == null)
                    {
                        MeshList.ShapeMeshes.AddLast(mesh);
                    }

                    builtinPrimitiveRenderer.SetShapeMesh(null);
                }
                break;
                case TextPrimitive t:
                {
                    TextMesh mesh = null;

                    if (this.RenderContext == null)
                    {
                        mesh = MeshPool.TextMeshPool.Get();
                        this.RenderContext = mesh;
                    }
                    else
                    {
                        mesh = (TextMesh) this.RenderContext;
                    }
                    mesh.Clear();
                    builtinPrimitiveRenderer.SetTextMesh(mesh);
                    mesh.Node = this;

                    var style = GUIStyle.Default;//FIXME TEMP
                    renderer.DrawText(t, style.FontFamily, style.FontSize, style.FontColor, style.FontStyle, style.FontWeight);
                    var foundNode = MeshList.TextMeshes.Find(mesh);
                    if (foundNode == null)
                    {
                        MeshList.TextMeshes.AddLast(mesh);
                    }

                    builtinPrimitiveRenderer.SetTextMesh(null);
                }
                break;
                case ImagePrimitive i:
                {
                    Mesh mesh = null;

                    if (this.RenderContext == null)
                    {
                        mesh = MeshPool.ImageMeshPool.Get();
                        this.RenderContext = mesh;
                    }
                    else
                    {
                        mesh = (Mesh) this.RenderContext;
                    }

                    mesh.Clear();
                    builtinPrimitiveRenderer.SetImageMesh(mesh);
                    mesh.Node = this;

                    renderer.DrawImage(i, this.Brush);
                    var foundNode = MeshList.ImageMeshes.Find(mesh);
                    if (foundNode == null)
                    {
                        MeshList.ImageMeshes.AddLast(mesh);
                    }
                    
                    builtinPrimitiveRenderer.SetImageMesh(null);
                }
                break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        /// <summary>
        /// internal render context refers to a context object. For built-in renderer, this is a <see cref="Mesh"/>.
        /// </summary>
        /// <remarks>
        /// This object is used as the context between the node and _Layer 4 basic rendering API implementation_.
        /// </remarks>
        internal object RenderContext;

        private bool activeSelf = true;
    }
}
