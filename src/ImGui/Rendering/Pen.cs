using System.Diagnostics;

namespace ImGui.Rendering
{
    /// <summary>
    /// Describes how to stroke a shape.
    /// </summary>
    [DebuggerStepThrough]
    public class Pen
    {
        public Color LineColor { get; set; } = Color.Black;
        public double LineWidth { get; set; } = 1;

        /// <summary>
        /// Initialize a new instance of Pen class with black color and 1 line-width.
        /// </summary>
        public Pen()
        {
        }

        public Pen(Color lineColor, double lineWidth)
        {
            this.LineColor = lineColor;
            this.LineWidth = lineWidth;
        }

        public override bool Equals(object obj)
        {
            Pen other = obj as Pen;
            if (other == null)
            {
                return false;
            }

            return other.LineColor.Equals(this.LineColor)
                && System.Math.Abs(this.LineWidth - other.LineWidth) < 0.001;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.LineColor.GetHashCode() * 397) ^ this.LineWidth.GetHashCode();
            }
        }
    }
}