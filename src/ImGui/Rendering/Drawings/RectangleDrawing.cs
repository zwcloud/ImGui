using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class RectangleDrawing : GeometryDrawing
    {
        public RectangleDrawing()
        {
            this.Primitive = new PathPrimitive();
        }

        public Rect Rect { get; set; }

        public void UpdatePrimitive(Rect rect)
        {
            PathPrimitive p = this.Primitive as PathPrimitive;
            p.PathRect(rect);
        }
    }
}