namespace ImGui.Rendering
{
    /// <summary>
    /// Drawing content contains drawing instructions.
    /// </summary>
    /// <remarks>
    /// Drawing content contains intermediate rendering data from two sources:
    /// 1. <see cref="Geometry.GetPathGeometryData"/>,
    /// 2. <see cref="Node.RenderData"/>
    /// Further, DrawingContent will be converted to Mesh or TextMesh for rendering with GPU.
    /// </remarks>
    internal class DrawingContent
    {


        //TODO image, text, etc.
        //TODO consider ImageGeometryData, how Brush related to ImageGeometry
    }

    namespace ImGui.Rendering.Internal
    {
        internal enum CommandType
        {
            DrawLine,
            DrawRectangle,
            DrawGlyphRun,
        }

        internal struct Instruction
        {
            public CommandType CommandType;
        }
    }

}