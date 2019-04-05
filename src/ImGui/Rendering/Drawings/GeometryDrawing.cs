namespace ImGui.Rendering
{
    internal class GeometryDrawing : Drawing
    {
        public Primitive Primitive { get; protected set; }

        public Brush Brush { get; protected set; }

        public Pen Pen { get; protected set; }
    }
}