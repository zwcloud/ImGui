namespace ImGui.Rendering
{
    /// <summary>
    /// Defines how to paint an area.
    /// </summary>
    internal class Brush
    {
        public Color FillColor { get; set; } = Color.Black;

        public Brush()
        {
        }

        public Brush(Color fillColor)
        {
            FillColor = fillColor;
        }
    }
}