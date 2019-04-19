using System;

namespace ImGui.Rendering
{
    internal class RectangleGeometry : Geometry
    {
        public RectangleGeometry(Rect rect)
        {
            this.Rect = rect;
        }

        public double RadiusX { get; set; }

        public double RadiusY { get; set; }

        public Rect Rect { get; set; }

        public bool IsEmpty() => this.Rect.IsEmpty;

        internal static bool IsRounded(double radiusX, double radiusY)
        {
            return (radiusX != 0.0) && (radiusY != 0.0);
        }

        internal bool IsRounded()
        {
            return RadiusX != 0.0 && RadiusY != 0.0;
        }

        internal override PathGeometryData GetPathGeometryData()
        {
            throw new NotImplementedException();
        }
    }
}