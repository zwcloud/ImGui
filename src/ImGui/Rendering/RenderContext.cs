using ImGui.GraphicsImplementation;

namespace ImGui.Rendering
{
    /// <summary>
    /// This class accumulates state during a render pass of the scene.
    /// </summary>
    internal class RenderContext
    {
        public RenderContext(BuiltinGeometryRenderer renderer, MeshList meshList)
        {
            this.renderer = renderer;
            this.meshList = meshList;
        }

        public void ConsumeContent(DrawingContent content)
        {
            var shapeMesh = MeshPool.ShapeMeshPool.Get();
            shapeMesh.Clear();
            shapeMesh.CommandBuffer.Add(DrawCommand.Default);
            var textMesh = MeshPool.TextMeshPool.Get();
            textMesh.Clear();
            var imageMesh = MeshPool.ImageMeshPool.Get();
            imageMesh.Clear();

            renderer.SetShapeMesh(shapeMesh);
            renderer.SetTextMesh(textMesh);
            renderer.SetImageMesh(imageMesh);
            content.ReadAllRecords(renderer);
            renderer.SetShapeMesh(null);
            renderer.SetTextMesh(null);
            renderer.SetImageMesh(null);

            meshList.AddOrUpdateShapeMesh(shapeMesh);
            meshList.AddOrUpdateTextMesh(textMesh);
            meshList.AddOrUpdateImageMesh(imageMesh);
        }

        private readonly MeshList meshList;
        private readonly BuiltinGeometryRenderer renderer;
    }
}