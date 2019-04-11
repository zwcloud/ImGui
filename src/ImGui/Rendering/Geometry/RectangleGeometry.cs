using System;
using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class RectangleGeometry : PathGeometry
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
            if (IsEmpty())
            {
                return Geometry.GetEmptyPathGeometryData();
            }

            PathGeometryData data = new PathGeometryData();
            data.FillRule = FillRule.EvenOdd;
            data.Offset = Offset;

            double radiusX = RadiusX;
            double radiusY = RadiusY;
            Rect rect = Rect;

            PathGeometryContext context = new PathGeometryContext();

            double r = Math.Max(radiusX, radiusY);
            var a = rect.Min;
            var b = rect.Max;
            if (r <= 0)
            {
                context.PathMoveTo(a);
                context.PathLineTo(new Point(b.X, a.Y));
                context.PathLineTo(b);
                context.PathLineTo(new Point(a.X, b.Y));
                context.PathLineTo(a);
            }
            else
            {
                context.PathArcFast(new Point(a.X + r, a.Y + r), r, 6, 9);
                context.PathArcFast(new Point(b.X - r, a.Y + r), r, 9, 12);
                context.PathArcFast(new Point(b.X - r, b.Y - r), r, 0, 3);
                context.PathArcFast(new Point(a.X + r, b.Y - r), r, 3, 6);
            }

            return data;
        }

        public override void UpdateContent(RenderContext context)
        {

        }
    }
}