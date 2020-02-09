using ImGui.Rendering.Composition;

namespace ImGui.Rendering
{
    /// <summary>
    /// This class accumulates state during a render pass of the scene.
    /// </summary>
    internal class RenderContext
    {
        public RenderContext(GeometryRenderer renderer, MeshList meshList)
        {
            this.renderer = renderer;
            this.meshList = meshList;
        }

        public Rect ClipRect { get; set; } = Rect.Big;

        public void ConsumeContent(DrawingContent content)
        {
            this.renderer.PushClipRect(ClipRect);
            this.renderer.OnBeforeRead();
            content.ReadAllRecords(this.renderer);
            this.renderer.OnAfterRead(this.meshList);
            this.renderer.PopClipRect();
        }

        private readonly MeshList meshList;
        private readonly GeometryRenderer renderer;
    }
}