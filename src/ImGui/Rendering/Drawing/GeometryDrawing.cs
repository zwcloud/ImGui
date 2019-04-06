namespace ImGui.Rendering
{
    internal class GeometryDrawing : Drawing
    {
        public Geometry Geometry { get; protected set; }

        public Brush Brush { get; protected set; }

        public Pen Pen { get; protected set; }
    }
}