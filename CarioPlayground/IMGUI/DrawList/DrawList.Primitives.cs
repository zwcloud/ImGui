using System.Numerics;

namespace ImGui
{
    partial class DrawList
    {
        private int _vtxWritePosition;
        private int _idxWritePosition;
#if false
        //primitives part
        void AddPolyline(Point[] points, int points_count, Color col, bool closed, double thickness, bool anti_aliased)
        {
            if (points_count < 2)
                return;

            Point uv = Point.Zero;

            int count = points_count;
            if (!closed)
                count = points_count - 1;

            bool thick_line = thickness > 1.0f;

            if (anti_aliased)
            {

            }
            else
            {
                // Non Anti-aliased Stroke
                int idx_count = count*6;
                int vtx_count = count*4;      // FIXME-OPT: Not sharing edges
                PrimReserve(idx_count, vtx_count);

                //for (int i1 = 0; i1 < count; i1++)
                //{
                //    int i2 = (i1+1) == points_count ? 0 : i1+1;
                //    Vector2 p1 = points[i1];
                //    ImVec2& p2 = points[i2];
                //    ImVec2 diff = p2 - p1;
                //    diff *= ImInvLength(diff, 1.0f);
                //
                //    const float dx = diff.x * (thickness * 0.5f);
                //    const float dy = diff.y * (thickness * 0.5f);
                //    _VtxWritePtr[0].pos.x = p1.x + dy; _VtxWritePtr[0].pos.y = p1.y - dx; _VtxWritePtr[0].uv = uv; _VtxWritePtr[0].col = col;
                //    _VtxWritePtr[1].pos.x = p2.x + dy; _VtxWritePtr[1].pos.y = p2.y - dx; _VtxWritePtr[1].uv = uv; _VtxWritePtr[1].col = col;
                //    _VtxWritePtr[2].pos.x = p2.x - dy; _VtxWritePtr[2].pos.y = p2.y + dx; _VtxWritePtr[2].uv = uv; _VtxWritePtr[2].col = col;
                //    _VtxWritePtr[3].pos.x = p1.x - dy; _VtxWritePtr[3].pos.y = p1.y + dx; _VtxWritePtr[3].uv = uv; _VtxWritePtr[3].col = col;
                //    _VtxWritePtr += 4;
                //
                //    _IdxWritePtr[0] = (ImDrawIdx)(_VtxCurrentIdx); _IdxWritePtr[1] = (ImDrawIdx)(_VtxCurrentIdx+1); _IdxWritePtr[2] = (ImDrawIdx)(_VtxCurrentIdx+2);
                //    _IdxWritePtr[3] = (ImDrawIdx)(_VtxCurrentIdx); _IdxWritePtr[4] = (ImDrawIdx)(_VtxCurrentIdx+2); _IdxWritePtr[5] = (ImDrawIdx)(_VtxCurrentIdx+3);
                //    _IdxWritePtr += 6;
                //    _VtxCurrentIdx += 4;
                //}
            }
        }
#endif
        void PrimReserve(int idx_count, int vtx_count)
        {
            DrawCommand draw_cmd = this.CommandBuffer[CommandBuffer.Count - 1];
            draw_cmd.ElemCount += idx_count;

            int vtx_buffer_size = this.VertexBuffer.Count;
            this.VertexBuffer.Resize(vtx_buffer_size + vtx_count);
            this._vtxWritePosition = vtx_buffer_size;

            int idx_buffer_size = this.IndexBuffer.Count;
            this.IndexBuffer.Resize(idx_buffer_size + idx_count);
            this._idxWritePosition = idx_buffer_size;
        }

    }
}
