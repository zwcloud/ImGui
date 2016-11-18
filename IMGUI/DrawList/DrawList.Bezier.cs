using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImGui
{
    partial class DrawList
    {
        private int _bezier_vtxWritePosition;
        private int _bezier_idxWritePosition;
        private short _bezier_currentIdx;

        private List<int> _BezierControlPointIndex = new List<int>();

        private void AppendBezierVertex(DrawVertex vertex)
        {
            bezierVertexBuffer[_bezier_vtxWritePosition] = vertex;
            _bezier_vtxWritePosition++;
        }

        private void AppendBezierIndex(short offsetToCurrentIndex)
        {
            bezierIndexBuffer[_bezier_idxWritePosition] = new DrawIndex { Index = (short)(_bezier_currentIdx + offsetToCurrentIndex) };
            _bezier_idxWritePosition++;
        }

        public void PrimBezierReserve(int idx_count, int vtx_count)
        {
            if (idx_count == 0)
            {
                return;
            }

            if (BezierCommandBuffer.Count == 0)
            {
                BezierCommandBuffer.Add(new DrawCommand());
            }
            DrawCommand draw_cmd = this.BezierCommandBuffer[BezierCommandBuffer.Count - 1];
            draw_cmd.ElemCount += idx_count;

            int vtx_buffer_size = this.BezierVertexBuffer.Count;
            this._bezier_vtxWritePosition = vtx_buffer_size;
            this.BezierVertexBuffer.Resize(vtx_buffer_size + vtx_count);

            int idx_buffer_size = this.BezierIndexBuffer.Count;
            this._bezier_idxWritePosition = idx_buffer_size;
            this.BezierIndexBuffer.Resize(idx_buffer_size + idx_count);
        }

        public void AddBezier(Point start, Point control, Point end, Color col)
        {
            int idx_count = 3;
            int vtx_count = 3;
            PrimBezierReserve(idx_count, vtx_count);

            var uv0 = new PointF(0, 0);
            var uv1 = new PointF(0.5, 0);
            var uv2 = new PointF(1, 1);

            var p0 = start;
            var p1 = control;
            var p2 = end;

            AppendBezierVertex(new DrawVertex { pos = (PointF)p0, uv = uv0, color = (ColorF)col });
            AppendBezierVertex(new DrawVertex { pos = (PointF)p1, uv = uv1, color = (ColorF)col });
            AppendBezierVertex(new DrawVertex { pos = (PointF)p2, uv = uv2, color = (ColorF)col });

            AppendBezierIndex(0);
            AppendBezierIndex(1);
            AppendBezierIndex(2);

            _bezier_currentIdx += 3;
        }

    }
}
