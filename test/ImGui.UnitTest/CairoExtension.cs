using System;
using System.Collections.Generic;
using System.Text;
using Cairo;

namespace ImGui.UnitTest
{
    public static class CairoExtension
    {
        public static void QuadraticTo(this Context g,
                     double x1, double y1,
                     double x2, double y2)
        {
            var currentPoint = g.CurrentPoint;
            var x0 = currentPoint.X;
            var y0 = currentPoint.Y;
            g.CurveTo(
                2.0 / 3.0 * x1 + 1.0 / 3.0 * x0,
                2.0 / 3.0 * y1 + 1.0 / 3.0 * y0,
                2.0 / 3.0 * x1 + 1.0 / 3.0 * x2,
                2.0 / 3.0 * y1 + 1.0 / 3.0 * y2,
                x2, y2);
        }
    }
}
