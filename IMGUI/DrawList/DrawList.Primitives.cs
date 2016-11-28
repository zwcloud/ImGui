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
                DrawBuffer.PrimReserve(idx_count, vtx_count);

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
                    DrawBuffer.AppendVertex(vertex0);
                    DrawBuffer.AppendVertex(vertex1);
                    DrawBuffer.AppendVertex(vertex2);
                    DrawBuffer.AppendVertex(vertex3);

                    DrawBuffer.AppendIndex(0);
                    DrawBuffer.AppendIndex(1);
                    DrawBuffer.AppendIndex(2);
                    DrawBuffer.AppendIndex(0);
                    DrawBuffer.AppendIndex(2);
                    DrawBuffer.AppendIndex(3);

                    DrawBuffer._currentIdx += 4;
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
                DrawBuffer.PrimReserve(idx_count, vtx_count);
                for (int i = 0; i < vtx_count; i++)
                {
                    DrawBuffer.AppendVertex(new DrawVertex { pos = (PointF)points[i], uv = PointF.Zero, color = (ColorF)col });
                }
                for (int i = 2; i < points_count; i++)
                {
                    DrawBuffer.AppendIndex(0);
                    DrawBuffer.AppendIndex(i-1);
                    DrawBuffer.AppendIndex(i);
                }
                DrawBuffer._currentIdx += vtx_count;
            }
        }
        
        // Fully unrolled with inline call to keep our debug builds decently fast.
        public void PrimRect(Point a, Point c, Color col)
        {
            Point b = new Point(c.X, a.Y);
            Point d = new Point(a.X, c.Y);
            Point uv = Point.Zero;

            DrawBuffer.AppendVertex(new DrawVertex { pos = (PointF)a, uv = PointF.Zero, color = (ColorF)col });
            DrawBuffer.AppendVertex(new DrawVertex { pos = (PointF)b, uv = PointF.Zero, color = (ColorF)col });
            DrawBuffer.AppendVertex(new DrawVertex { pos = (PointF)c, uv = PointF.Zero, color = (ColorF)col });
            DrawBuffer.AppendVertex(new DrawVertex { pos = (PointF)d, uv = PointF.Zero, color = (ColorF)col });

            DrawBuffer.AppendIndex(0);
            DrawBuffer.AppendIndex(1);
            DrawBuffer.AppendIndex(2);
            DrawBuffer.AppendIndex(0);
            DrawBuffer.AppendIndex(2);
            DrawBuffer.AppendIndex(3);

            DrawBuffer._currentIdx += 4;
        }

        void PrimRectUV(Point a, Point c, Point uv_a, Point uv_c, Color col)
        {
            Point b = new Point(c.X, a.Y);
            Point d = new Point(a.X, c.Y);
            Point uv_b = new Point(uv_c.X, uv_a.Y);
            Point uv_d = new Point(uv_a.X, uv_c.Y);

            DrawBuffer.AppendVertex(new DrawVertex { pos = (PointF)a, uv = (PointF)uv_a, color = (ColorF)col });
            DrawBuffer.AppendVertex(new DrawVertex { pos = (PointF)b, uv = (PointF)uv_b, color = (ColorF)col });
            DrawBuffer.AppendVertex(new DrawVertex { pos = (PointF)c, uv = (PointF)uv_c, color = (ColorF)col });
            DrawBuffer.AppendVertex(new DrawVertex { pos = (PointF)d, uv = (PointF)uv_d, color = (ColorF)col });

            DrawBuffer.AppendIndex(0);
            DrawBuffer.AppendIndex(1);
            DrawBuffer.AppendIndex(2);
            DrawBuffer.AppendIndex(0);
            DrawBuffer.AppendIndex(2);
            DrawBuffer.AppendIndex(3);

            DrawBuffer._currentIdx += 4;
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
                DrawBuffer.PrimReserve(6, 4);
                PrimRect(a, b, col);
            }
        }

        void AddImage(Texture texture, Point a, Point b, Point uv0, Point uv1, Color col)
        {
            if (MathEx.AmostZero(col.A))
                return;
            
            ImageBuffer.CommandBuffer.Add(
                new DrawCommand
                {
                    ClipRect = new Rect(a, b),
                    ElemCount = 6,
                    TextureData = texture
                }
            );

            ImageBuffer.PrimReserve(6, 4);
            AddImageRect(a, b, uv0, uv1, col);
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

                DrawList.PathBezierToCasteljau(path, x1, y1, x12, y12, x123, y123, x1234, y1234, tess_tol, level + 1);
                DrawList.PathBezierToCasteljau(path, x1234, y1234, x234, y234, x34, y34, x4, y4, tess_tol, level + 1);
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
                var r0 = (rounding_corners & 1) != 0 ? r : 0.0f;
                var r1 = (rounding_corners & 2) != 0 ? r : 0.0f;
                var r2 = (rounding_corners & 4) != 0 ? r : 0.0f;
                var r3 = (rounding_corners & 8) != 0 ? r : 0.0f;
                PathArcToFast(new Point(rect_min.X+r0, rect_min.Y+r0), r0, 6, 9);
                PathArcToFast(new Point(rect_max.X-r1, rect_min.Y+r1), r1, 9, 12);
                PathArcToFast(new Point(rect_max.X-r2, rect_max.Y-r2), r2, 0, 3);
                PathArcToFast(new Point(rect_min.X+r3, rect_max.Y-r3), r3, 3, 6);
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

        private static readonly Point[] circle_vtx = InitCircleVtx();

        private static Point[] InitCircleVtx()
        {
            Point[] result = new Point[12];
            for (int i = 0; i < 12; i++)
            {
                var a = (float) i/12*2*Math.PI;
                result[i].X = Math.Cos(a);
                result[i].Y = Math.Sin(a);
            }
            return result;
        }

        public void PathArcToFast(Point center, double radius, int amin, int amax)
        {
            if (amin > amax) return;
            if (MathEx.AmostZero(radius))
            {
                _Path.Add(center);
            }
            else
            {
                _Path.Capacity = _Path.Count + amax - amin + 1;
                for (int a = amin; a <= amax; a++)
                {
                    Point c = circle_vtx[a % circle_vtx.Length];
                    _Path.Add(new Point(center.X + c.X* radius, center.Y + c.Y* radius));
                }
            }
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
                if (DrawBuffer.CommandBuffer.Count == 0)
                {
                    DrawBuffer.CommandBuffer.Add(DrawCommand.Default);
                }
                DrawCommand newDrawCommand = DrawBuffer.CommandBuffer[DrawBuffer.CommandBuffer.Count - 1];
                var idx_count = textMesh.IndexBuffer.Count;
                var vtx_count = textMesh.VertexBuffer.Count;
                if (idx_count != 0 && vtx_count != 0)
                {
                    newDrawCommand.ElemCount += idx_count;
                    DrawBuffer.CommandBuffer[DrawBuffer.CommandBuffer.Count - 1] = newDrawCommand;

                    var vertexCountBefore = DrawBuffer.VertexBuffer.Count;

                    int vtx_buffer_size = DrawBuffer.VertexBuffer.Count;
                    DrawBuffer._vtxWritePosition = vtx_buffer_size + vtx_count;
                    DrawBuffer.VertexBuffer.AddRange(textMesh.VertexBuffer);

                    int idx_buffer_size = DrawBuffer.IndexBuffer.Count;
                    DrawBuffer._idxWritePosition = idx_buffer_size + idx_count;

                    var sizeBefore = DrawBuffer.IndexBuffer.Count;
                    DrawBuffer.IndexBuffer.AddRange(textMesh.IndexBuffer);
                    var sizeAfter = DrawBuffer.IndexBuffer.Count;

                    if (vertexCountBefore != 0)
                    {
                        for (int i = sizeBefore; i < sizeAfter; i++)
                        {
                            DrawBuffer.IndexBuffer[i] = new DrawIndex { Index = DrawBuffer.IndexBuffer[i].Index + vertexCountBefore };
                        }
                    }
                    DrawBuffer._currentIdx += vtx_count;
                }
            }

            // append beziers
            {
                // TODO This is a temp hack, need to be moved to a suitable place.
                if (this.BezierBuffer.CommandBuffer.Count == 0)
                {
                    this.BezierBuffer.CommandBuffer.Add(DrawCommand.Default);
                }
                DrawCommand newDrawCommand = this.BezierBuffer.CommandBuffer[this.BezierBuffer.CommandBuffer.Count - 1];
                var idx_count = textMesh.BezierIndexBuffer.Count;
                var vtx_count = textMesh.BezierVertexBuffer.Count;
                if (idx_count != 0 && vtx_count != 0)
                {
                    newDrawCommand.ElemCount += idx_count;
                    this.BezierBuffer.CommandBuffer[this.BezierBuffer.CommandBuffer.Count - 1] = newDrawCommand;

                    var vertexCountBefore = this.BezierBuffer.VertexBuffer.Count;

                    int vtx_buffer_size = this.BezierBuffer.VertexBuffer.Count + vtx_count;
                    this.BezierBuffer._vtxWritePosition = vtx_buffer_size;
                    this.BezierBuffer.VertexBuffer.AddRange(textMesh.BezierVertexBuffer);

                    int idx_buffer_size = this.BezierBuffer.IndexBuffer.Count + idx_count;
                    this.BezierBuffer._idxWritePosition = idx_buffer_size;

                    var sizeBefore = this.BezierBuffer.IndexBuffer.Count;
                    this.BezierBuffer.IndexBuffer.AddRange(textMesh.BezierIndexBuffer);
                    var sizeAfter = this.BezierBuffer.IndexBuffer.Count;

                    if (vertexCountBefore != 0)
                    {
                        for (int i = sizeBefore; i < sizeAfter; i++)
                        {
                            this.BezierBuffer.IndexBuffer[i] = new DrawIndex { Index = this.BezierBuffer.IndexBuffer[i].Index + vertexCountBefore };
                        }
                    }
                    this.BezierBuffer._currentIdx += vtx_count;
                }
            }
        }


    }
}
