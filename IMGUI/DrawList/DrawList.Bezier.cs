using System;
using System.Collections.Generic;

namespace ImGui
{
    partial class DrawList
    {
        private List<int> _BezierControlPointIndex = new List<int>();

        public void AddBezier(Point start, Point control, Point end, Color col)
        {
            int idx_count = 3;
            int vtx_count = 3;
            BezierBuffer.PrimReserve(idx_count, vtx_count);

            var uv0 = new PointF(0, 0);
            var uv1 = new PointF(0.5, 0);
            var uv2 = new PointF(1, 1);

            var p0 = start;
            var p1 = control;
            var p2 = end;

            BezierBuffer.AppendVertex(new DrawVertex { pos = (PointF)p0, uv = uv0, color = (ColorF)col });
            BezierBuffer.AppendVertex(new DrawVertex { pos = (PointF)p1, uv = uv1, color = (ColorF)col });
            BezierBuffer.AppendVertex(new DrawVertex { pos = (PointF)p2, uv = uv2, color = (ColorF)col });

            BezierBuffer.AppendIndex(0);
            BezierBuffer.AppendIndex(1);
            BezierBuffer.AppendIndex(2);

            BezierBuffer._currentIdx += 3;
        }

    }
}
