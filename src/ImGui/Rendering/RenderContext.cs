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
        public void CheckShapeMesh()
        {
            if (this.shapeMesh == null)
            {
                this.shapeMesh = MeshPool.ShapeMeshPool.Get();
            }
        }

        /// <summary>
        /// check render context for textMesh
        /// </summary>
        public void CheckTextMesh()
        {
            if (this.textMesh == null)
            {
                this.textMesh = MeshPool.TextMeshPool.Get();
            }
        }

        /// <summary>
        /// check render context for image mesh
        /// </summary>
        public void CheckImageMesh()
        {
            if (this.imageMesh == null)
            {
                this.imageMesh = MeshPool.ImageMeshPool.Get();
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