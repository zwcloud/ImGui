using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ImGui
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void PointAdder(float x, float y);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void BezierAdder(float x0, float y0, float x1, float y1, float x2, float y2);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void PathCloser();
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void FigureBeginner(float x, float y);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void FigureEnder(); 

    partial class DrawList
    {
        #region buffer writing

        private int _vtxWritePosition;
        private int _idxWritePosition;
        private short _currentIdx;

        private void AppendVertex(DrawVertex vertex)
        {
            vertexBuffer[_vtxWritePosition] = vertex;
            _vtxWritePosition++;
        }

        private void AppendIndex(short offsetToCurrentIndex)
        {
            indexBuffer[_idxWritePosition] = new DrawIndex { Index = (short)(_currentIdx + offsetToCurrentIndex) };
            _idxWritePosition++;
        }
        
        public void PrimReserve(int idx_count, int vtx_count)
        {
            if (idx_count == 0)
            {
                return;
            }

            if (CommandBuffer.Count == 0)
            {
                CommandBuffer.Add(new DrawCommand());
            }
            DrawCommand draw_cmd = this.CommandBuffer[CommandBuffer.Count - 1];
            draw_cmd.ElemCount += idx_count;

            int vtx_buffer_size = this.VertexBuffer.Count;
            this._vtxWritePosition = vtx_buffer_size;
            this.VertexBuffer.Resize(vtx_buffer_size + vtx_count);

            int idx_buffer_size = this.IndexBuffer.Count;
            this._idxWritePosition = idx_buffer_size;
            this.IndexBuffer.Resize(idx_buffer_size + idx_count);
        }

        #endregion

        #region primitives

        private List<Point> _Path = new List<Point>();

        //primitives part
        public void AddPolyline(IList<Point> points, Color col, bool closed, double thickness, bool anti_aliased = false)
        {
            var points_count = points.Count;
            if (points_count < 2)
                return;

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

                for (int i1 = 0; i1 < count; i1++)
                {
                    int i2 = (i1+1) == points_count ? 0 : i1+1;
                    Point p1 = points[i1];
                    Point p2 = points[i2];
                    Vector diff = p2 - p1;
                    diff *= MathEx.InverseLength(diff, 1.0f);
                
                    float dx = (float)(diff.X * (thickness * 0.5f));
                    float dy = (float)(diff.Y * (thickness * 0.5f));
                    var vertex0 = new DrawVertex { pos = new PointF(p1.X + dy, p1.Y - dx), uv = PointF.Zero, color = (ColorF)col };
                    var vertex1 = new DrawVertex { pos = new PointF(p2.X + dy, p2.Y - dx), uv = PointF.Zero, color = (ColorF)col };
                    var vertex2 = new DrawVertex { pos = new PointF(p2.X - dy, p2.Y + dx), uv = PointF.Zero, color = (ColorF)col };
                    var vertex3 = new DrawVertex { pos = new PointF(p1.X - dy, p1.Y + dx), uv = PointF.Zero, color = (ColorF)col };
                    AppendVertex(vertex0);
                    AppendVertex(vertex1);
                    AppendVertex(vertex2);
                    AppendVertex(vertex3);
                    
                    AppendIndex(0);
                    AppendIndex(1);
                    AppendIndex(2);
                    AppendIndex(0);
                    AppendIndex(2);
                    AppendIndex(3);

                    _currentIdx += 4;
                }
            }
        }
        
        public void AddConvexPolyFilled(IList<Point> points, Color col, bool anti_aliased)
        {
            var points_count = points.Count;
            anti_aliased = false;
            //if (.KeyCtrl) anti_aliased = false; // Debug

            if (anti_aliased)
            {

            }
            else
            {
                // Non Anti-aliased Fill
                int idx_count = (points_count-2)*3;
                int vtx_count = points_count;
                PrimReserve(idx_count, vtx_count);
                for (int i = 0; i < vtx_count; i++)
                {
                    AppendVertex(new DrawVertex { pos = (PointF)points[i], uv = PointF.Zero, color = (ColorF)col });
                }
                for (int i = 2; i < points_count; i++)
                {
                    AppendIndex(0);
                    AppendIndex((short)(i-1));
                    AppendIndex((short)i);
                }
                _currentIdx += (short)vtx_count;
            }
        }
        
        // Fully unrolled with inline call to keep our debug builds decently fast.
        public void PrimRect(Point a, Point c, Color col)
        {
            Point b = new Point(c.X, a.Y);
            Point d = new Point(a.X, c.Y);
            Point uv = Point.Zero;
            
            AppendVertex(new DrawVertex { pos = (PointF)a, uv = PointF.Zero, color = (ColorF)col });
            AppendVertex(new DrawVertex { pos = (PointF)b, uv = PointF.Zero, color = (ColorF)col });
            AppendVertex(new DrawVertex { pos = (PointF)c, uv = PointF.Zero, color = (ColorF)col });
            AppendVertex(new DrawVertex { pos = (PointF)d, uv = PointF.Zero, color = (ColorF)col });

            AppendIndex(0);
            AppendIndex(1);
            AppendIndex(2);
            AppendIndex(0);
            AppendIndex(2);
            AppendIndex(3);
            
            _currentIdx += 4;
        }

        // a: upper-left, b: lower-right. we don't render 1 px sized rectangles properly.
        public void AddRect(Point a, Point b, Color col, float rounding = 0.0f, int rounding_corners = 0x0F, float thickness = 1.0f)
        {
            if (MathEx.AmostZero(col.A))
                return;
            PathRect(a + new Vector(0.5f,0.5f), b - new Vector(0.5f,0.5f), rounding, rounding_corners);
            PathStroke(col, true, thickness);
        }

        public void AddRectFilled(Point a, Point b, Color col, float rounding = 0.0f, int rounding_corners = 0x0F)
        {
            if (MathEx.AmostZero(col.A))
                return;
            if (rounding > 0.0f)
            {
                //PathRect(a, b, rounding, rounding_corners);
                //PathFill(col);
            }
            else
            {
                PrimReserve(6, 4);
                PrimRect(a, b, col);
            }
        }

        #endregion

        #region stateful path constructing methods

        static void PathBezierToCasteljau(IList<Point> path, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double tess_tol, int level)
        {
            double dx = x4 - x1;
            double dy = y4 - y1;
            double d2 = ((x2 - x4) * dy - (y2 - y4) * dx);
            double d3 = ((x3 - x4) * dy - (y3 - y4) * dx);
            d2 = (d2 >= 0) ? d2 : -d2;
            d3 = (d3 >= 0) ? d3 : -d3;
            if ((d2 + d3) * (d2 + d3) < tess_tol * (dx * dx + dy * dy))
            {
                path.Add(new Point(x4, y4));
            }
            else if (level < 10)
            {
                double x12 = (x1 + x2) * 0.5f, y12 = (y1 + y2) * 0.5f;
                double x23 = (x2 + x3) * 0.5f, y23 = (y2 + y3) * 0.5f;
                double x34 = (x3 + x4) * 0.5f, y34 = (y3 + y4) * 0.5f;
                double x123 = (x12 + x23) * 0.5f, y123 = (y12 + y23) * 0.5f;
                double x234 = (x23 + x34) * 0.5f, y234 = (y23 + y34) * 0.5f;
                double x1234 = (x123 + x234) * 0.5f, y1234 = (y123 + y234) * 0.5f;

                PathBezierToCasteljau(path, x1, y1, x12, y12, x123, y123, x1234, y1234, tess_tol, level + 1);
                PathBezierToCasteljau(path, x1234, y1234, x234, y234, x34, y34, x4, y4, tess_tol, level + 1);
            }
        }

        const double CurveTessellationTol = 1.25;
        void PathBezierCurveTo(Point p2, Point p3, Point p4, int num_segments = 0)
        {
            Point p1 = _Path[_Path.Count-1];
            if (num_segments == 0)
            {
                // Auto-tessellated
                PathBezierToCasteljau(_Path, p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, CurveTessellationTol, 0);
            }
            else
            {
                float t_step = 1.0f / (float)num_segments;
                for (int i_step = 1; i_step <= num_segments; i_step++)
                {
                    float t = t_step * i_step;
                    float u = 1.0f - t;
                    float w1 = u * u * u;
                    float w2 = 3 * u * u * t;
                    float w3 = 3 * u * t * t;
                    float w4 = t * t * t;
                    _Path.Add(new Point(w1* p1.X + w2* p2.X + w3* p3.X + w4* p4.X, w1* p1.Y + w2* p2.Y + w3* p3.Y + w4* p4.Y));
                }
            }
        }

        public void PathRect(Point rect_min, Point rect_max, float rounding = 0.0f, int rounding_corners = 0x0F)
        {
            double r = rounding;
            r = System.Math.Min(r, System.Math.Abs(rect_max.X-rect_min.X) * ( ((rounding_corners&(1|2))==(1|2)) || ((rounding_corners&(4|8))==(4|8)) ? 0.5f : 1.0f ) - 1.0f);
            r = System.Math.Min(r, System.Math.Abs(rect_max.Y-rect_min.Y) * ( ((rounding_corners&(1|8))==(1|8)) || ((rounding_corners&(2|4))==(2|4)) ? 0.5f : 1.0f ) - 1.0f);

            if (r <= 0.0f || rounding_corners == 0)
            {
                PathLineTo(rect_min);
                PathLineTo(new Point(rect_max.X, rect_min.Y));
                PathLineTo(rect_max);
                PathLineTo(new Point(rect_min.X, rect_max.Y));
            }
            else
            {
                //const float r0 = (rounding_corners & 1) ? r : 0.0f;
                //const float r1 = (rounding_corners & 2) ? r : 0.0f;
                //const float r2 = (rounding_corners & 4) ? r : 0.0f;
                //const float r3 = (rounding_corners & 8) ? r : 0.0f;
                //PathArcToFast(ImVec2(a.x+r0, a.y+r0), r0, 6, 9);
                //PathArcToFast(ImVec2(b.x-r1, a.y+r1), r1, 9, 12);
                //PathArcToFast(ImVec2(b.x-r2, b.y-r2), r2, 0, 3);
                //PathArcToFast(ImVec2(a.x+r3, b.y-r3), r3, 3, 6);
            }
        }
        
        //inline
        public void PathStroke(Color col, bool closed, float thickness = 1.0f)
        {
            AddPolyline(_Path, col, closed, thickness);
            PathClear();
        }

        //inline
        public void PathFill(Color col)
        {
            AddConvexPolyFilled(_Path, col, true);
            PathClear();
        }

        public void PathClear()
        {
            _Path.Clear();
        }

        public void PathMoveTo(Point point)
        {
            _Path.Add(point);
        }

        //inline
        public void PathLineTo(Point pos)
        {
            _Path.Add(pos);
        }

        public void PathClose()
        {
            _Path.Add(_Path[0]);
        }

        #region filled bezier curve

        public void PathAddBezier(Point start, Point control, Point end)
        {
            _Path.Add(start);

            _Path.Add(control);
            _BezierControlPointIndex.Add(_Path.Count - 1);

            _Path.Add(end);
        }

        #endregion

        #endregion

        #region TODO data-based path api

#if false
        struct Path
        {
            PathData[] data;
        }

        struct PathData
        {
            public PathType type;
        }

        enum PathType
        {
            MoveTo,
            LineTo,
            Close,
            AddBezier,
            Clear,
        }
#endif

        #endregion

        /// <summary>
        /// Append a text mesh to this drawlist
        /// </summary>
        /// <param name="textMesh"></param>
        public void Append(TextMesh textMesh)
        {
            if (textMesh == null)
            {
                return;
            }
            
            // append triangles
            {
                // TODO This is a temp hack, need to be moved to a suitable place.
                if (CommandBuffer.Count == 0)
                {
                    CommandBuffer.Add(new DrawCommand());
                }
                DrawCommand draw_cmd = this.CommandBuffer[this.CommandBuffer.Count - 1];
                var idx_count = textMesh.IndexBuffer.Count;
                var vtx_count = textMesh.VertexBuffer.Count;
                if (idx_count != 0 && vtx_count != 0)
                {
                    draw_cmd.ElemCount += idx_count;

                    var vertexCountBefore = this.VertexBuffer.Count;

                    int vtx_buffer_size = this.VertexBuffer.Count + vtx_count;
                    this._vtxWritePosition = vtx_buffer_size;
                    this.VertexBuffer.AddRange(textMesh.VertexBuffer);

                    int idx_buffer_size = this.IndexBuffer.Count + idx_count;
                    this._idxWritePosition = idx_buffer_size;

                    var sizeBefore = this.IndexBuffer.Count;
                    this.IndexBuffer.AddRange(textMesh.IndexBuffer);
                    var sizeAfter = this.IndexBuffer.Count;

                    if (vertexCountBefore != 0)
                    {
                        for (int i = sizeBefore; i < sizeAfter - 1; i++)
                        {
                            this.IndexBuffer[i] = new DrawIndex { Index = (short)(this.IndexBuffer[i].Index + vertexCountBefore) };
                        }
                    }
                }
            }

            // append beziers
            {
                // TODO This is a temp hack, need to be moved to a suitable place.
                if (BezierCommandBuffer.Count == 0)
                {
                    BezierCommandBuffer.Add(new DrawCommand());
                }
                DrawCommand draw_cmd = this.BezierCommandBuffer[this.BezierCommandBuffer.Count - 1];
                var idx_count = textMesh.BezierIndexBuffer.Count;
                var vtx_count = textMesh.BezierVertexBuffer.Count;
                if (idx_count != 0 && vtx_count != 0)
                {
                    draw_cmd.ElemCount += idx_count;

                    var vertexCountBefore = this.BezierVertexBuffer.Count;

                    int vtx_buffer_size = this.BezierVertexBuffer.Count + vtx_count;
                    this._bezier_vtxWritePosition = vtx_buffer_size;
                    this.BezierVertexBuffer.AddRange(textMesh.BezierVertexBuffer);

                    int idx_buffer_size = this.BezierIndexBuffer.Count + idx_count;
                    this._bezier_idxWritePosition = idx_buffer_size;

                    var sizeBefore = this.BezierIndexBuffer.Count;
                    this.BezierIndexBuffer.AddRange(textMesh.BezierIndexBuffer);
                    var sizeAfter = this.BezierIndexBuffer.Count;

                    if (vertexCountBefore != 0)
                    {
                        for (int i = sizeBefore; i < sizeAfter - 1; i++)
                        {
                            this.BezierIndexBuffer[i] = new DrawIndex { Index = (short)(this.BezierIndexBuffer[i].Index + vertexCountBefore) };
                        }
                    }
                }
            }
        }

    }
}
