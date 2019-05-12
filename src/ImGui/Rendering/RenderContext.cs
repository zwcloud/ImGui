using ImGui.Rendering.Composition;

namespace ImGui.Rendering
{
    /// <summary>
    /// This class accumulates state during a render pass of the scene.
    /// </summary>
    internal class RenderContext
    {
        public RenderContext(RecordReader renderer, MeshList meshList)
        {
            this.renderer = renderer;
            this.meshList = meshList;
        }

        public void ConsumeContent(DrawingContent content)
        {
            this.renderer.OnBeforeRead();
            content.ReadAllRecords(this.renderer);
            this.renderer.OnAfterRead(this.meshList);
        }

        private readonly MeshList meshList;
        private readonly RecordReader renderer;
    }
}