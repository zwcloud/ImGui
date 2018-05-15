using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Layout;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;

namespace ImGui.Rendering
{
    [DebuggerDisplay("#{" + nameof(Id) + "}")]
    internal partial class Node
    {
        /// <summary>
        /// identifier number of the node
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// string identifier of the node
        /// </summary>
        public string StrId { get; set; }

        /// <summary>
        /// Dirty flag: Should this node be re-drawn, default value: true.
        /// </summary>
        public bool Dirty { get; set; } = true;

        /// <summary>
        /// border-box, the layout result
        /// </summary>
        public Rect Rect;

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

        public void Foreach(Action<Node> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var node in this.Children)
            {
                action(node);
                if (node.Children!=null && node.Children.Count != 0)
                {
                    node.Foreach(action);
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
        internal bool IsFill { get; set; }
        internal Brush Brush { get; set; }
        internal StrokeStyle StrokeStyle { get; set; }

        /// <summary>
        /// Redraw the node's primitive.
        /// </summary>
        /// <param name="renderer"></param>
        /// <remarks>A node can only have one single primitive.</remarks>
        public void Draw(IPrimitiveRenderer renderer)
        {
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
                        MeshList.ShapeMeshes.AddFirst(mesh);
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

                    var style = GUIStyle.Default;//FIXME TEMP
                    renderer.DrawText(t, this.Rect, style.FontFamily, style.FontSize, style.FontColor, style.FontStyle, style.FontWeight);
                    var foundNode = MeshList.TextMeshes.Find(mesh);
                    if (foundNode == null)
                    {
                        MeshList.TextMeshes.AddFirst(mesh);
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

                    renderer.DrawImage(i, this.Brush);
                    var foundNode = MeshList.ImageMeshes.Find(mesh);
                    if (foundNode == null)
                    {
                        MeshList.ImageMeshes.AddFirst(mesh);
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
    }
}
