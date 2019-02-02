using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal struct RoundedRect
    {
        public Rect Rect { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
    }
}