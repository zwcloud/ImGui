namespace ImGui.Rendering
{
    /// <summary>
    /// Describes how to stroke a shape.
    /// </summary>
    internal class Pen
    {
        public Color LineColor { get; set; } = Color.Black;
        public double LineWidth { get; set; } = 1;

        public Pen()
        {
        }

        public Pen(Color lineColor, double lineWidth)
        {
            LineColor = lineColor;
            LineWidth = lineWidth;
        }
    }
}