using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    /// <summary>
    /// Describes how to stroke a shape.
    /// </summary>
    internal class Pen
    {
        public Color LineColor { get; set; } = Color.Black;
        public float LineWidth { get; set; } = 1;
    }
}