using System.Diagnostics;

namespace ImGui.Rendering
{
    /// <summary>
    /// Defines how to paint an area.
    /// </summary>
    [DebuggerStepThrough]
    public class Brush
    {
        public Color FillColor { get; set; } = Color.Black;

        public Brush()
        {
        }

        public Brush(Color fillColor)
        {
            FillColor = fillColor;
        }

        public override bool Equals(object obj)
        {
            Brush other = obj as Brush;
            if (other == null)
            {
                return false;
            }

            return other.FillColor.Equals(this.FillColor);
        }

        public override int GetHashCode()
        {
            return this.FillColor.GetHashCode();
        }
    }
}