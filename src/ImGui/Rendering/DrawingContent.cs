namespace ImGui.Rendering
{
    /// <summary>
    /// Drawing content contains intermediate rendering data that converted from <see cref="Geometry"/>,
    /// and it will be further converted to rendering resources -Mesh or TextMesh- for rendering in GPU in
    /// <see cref="RenderContext.ConsumeContent(DrawingContent)"/>.
    /// </summary>
    internal class DrawingContent
    {
        //TODO image, text, etc.
        //TODO consider ImageGeometryData, how Brush related to ImageGeometry

        public PathGeometryData PathContent { get; } = new PathGeometryData();
    }
}