using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui
{
    interface ITextGeometryContainer
    {
        void AddContour(List<Point> points);
        void AddBezier((Point,Point,Point) segments);
        void Clear();
    }
}
