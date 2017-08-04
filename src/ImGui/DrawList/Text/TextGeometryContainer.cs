using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui
{
    internal class TextGeometryContainer : ITextGeometryContainer
    {
        List<(Point, Point, Point)> quadraticCurveSegments = new List<(Point, Point, Point)>();
        List<List<Point>> polygons = new List<List<Point>>();

        public List<(Point, Point, Point)> CurveSegments => quadraticCurveSegments;
        public List<List<Point>> Polygons => polygons;

        public void AddBezier((Point, Point, Point) segment)
        {
            quadraticCurveSegments.Add(segment);
        }

        public void AddContour(List<Point> points)
        {
            polygons.Add(points);
        }

        public void Clear()
        {
            quadraticCurveSegments.Clear();
            polygons.Clear();
        }
    }
}
