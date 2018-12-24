namespace ImGui.Rendering
{
    /// <summary>
    /// Render context for Node
    /// </summary>
    internal class RenderContext
    {
        public Mesh shapeMesh;

        public Mesh imageMesh;

        public TextMesh textMesh;

        /// <summary>
        /// check render context for shape mesh
        /// </summary>
        public void CheckShapeMesh(Node node)
        {
            if (this.shapeMesh == null)
            {
                this.shapeMesh = MeshPool.ShapeMeshPool.Get();
                this.shapeMesh.Node = node;
            }
        }

        /// <summary>
        /// check render context for textMesh
        /// </summary>
        public void CheckTextMesh(Node node)
        {
            if (this.textMesh == null)
            {
                this.textMesh = MeshPool.TextMeshPool.Get();
                this.textMesh.Node = node;
            }
        }

        /// <summary>
        /// check render context for image mesh
        /// </summary>
        /// <param name="node"></param>
        public void CheckImageMesh(Node node)
        {
            if (this.imageMesh == null)
            {
                this.imageMesh = MeshPool.ImageMeshPool.Get();
                this.imageMesh.Node = node;
            }
        }

        /// <summary>
        /// clear shape mesh
        /// </summary>
        public void ClearShapeMesh()
        {
            this.shapeMesh.Clear();
            this.shapeMesh.CommandBuffer.Add(DrawCommand.Default);
        }

        /// <summary>
        /// clear image mesh
        /// </summary>
        public void ClearImageMesh()
        {
            this.imageMesh.Clear();
        }

        /// <summary>
        /// clear text mesh
        /// </summary>
        public void ClearTextMesh()
        {
            this.textMesh.Clear();
        }
    }
}