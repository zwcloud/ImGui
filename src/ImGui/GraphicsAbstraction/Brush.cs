using ImGui.Common.Primitive;

namespace ImGui.GraphicsAbstraction
{
    internal class Brush
    {
        public Color LineColor { get; set; } = Color.Black;
        public float LineWidth { get; set; } = 1;
        public Color FillColor { get; set; } = Color.Black;
    }
}