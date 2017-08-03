using System.Collections.Generic;

namespace ImGui
{
    interface ITextGeometryContainer
    {
        void AddContour(List<Point> points);
        void AddBezier((Point,Point,Point) segments);
    }
}
