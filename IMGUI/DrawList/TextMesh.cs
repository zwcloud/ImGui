using System;
using System.Collections.Generic;
using LibTessDotNet;

namespace ImGui
{
    class TextMesh
    {
        ImGui.Internal.List<DrawIndex> indexBuffer = new ImGui.Internal.List<DrawIndex>();
        ImGui.Internal.List<DrawVertex> vertexBuffer = new ImGui.Internal.List<DrawVertex>();

        ImGui.Internal.List<DrawIndex> bezierIndexBuffer = new ImGui.Internal.List<DrawIndex>();
        ImGui.Internal.List<DrawVertex> bezierVertexBuffer = new ImGui.Internal.List<DrawVertex>();

        private int _bezier_vtxWritePosition;
        private int _bezier_idxWritePosition;
        private int _bezier_currentIdx;

        private List<int> _BezierControlPointIndex = new List<int>();

        private void AppendBezierVertex(DrawVertex vertex)
        {
            bezierVertexBuffer[_bezier_vtxWritePosition] = vertex;
            _bezier_vtxWritePosition++;
        }

        private void AppendBezierIndex(int offsetToCurrentIndex)
        {
            bezierIndexBuffer[_bezier_idxWritePosition] = new DrawIndex { Index = _bezier_currentIdx + offsetToCurrentIndex };
            _bezier_idxWritePosition++;
        }

        public void PrimBezierReserve(int idx_count, int vtx_count)
        {
            if (idx_count == 0)
            {
                return;
            }

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


        /// <summary>
        /// Index buffer. Each command consume DrawCommand.ElemCount of those
        /// </summary>
        public ImGui.Internal.List<DrawIndex> IndexBuffer
        {
            get { return indexBuffer; }
            set { indexBuffer = value; }
        }

        /// <summary>
        /// Vertex buffer
        /// </summary>
        public ImGui.Internal.List<DrawVertex> VertexBuffer
        {
            get { return vertexBuffer; }
            set { vertexBuffer = value; }
        }

        /// <summary>
        /// Index buffer for bezier curves
        /// </summary>
        public ImGui.Internal.List<DrawIndex> BezierIndexBuffer
        {
            get { return bezierIndexBuffer; }
            set { bezierIndexBuffer = value; }
        }

        /// <summary>
        /// Vertex buffer for beziers curves
        /// </summary>
        public ImGui.Internal.List<DrawVertex> BezierVertexBuffer
        {
            get { return bezierVertexBuffer; }
        }

        public void Clear()
        {
            // triangles
            this.IndexBuffer.Clear();
            this.VertexBuffer.Clear();

            _vtxWritePosition = 0;
            _idxWritePosition = 0;
            _currentIdx = 0;

            _Path.Clear();

            // beziers
            this.BezierIndexBuffer.Clear();
            this.BezierVertexBuffer.Clear();

            _bezier_vtxWritePosition = 0;
            _bezier_idxWritePosition = 0;
            _bezier_currentIdx = 0;

            _BezierControlPointIndex.Clear();
        }

        #region buffer writing

        private int _vtxWritePosition;
        private int _idxWritePosition;
        private int _currentIdx;

        private void AppendVertex(DrawVertex vertex)
        {
            vertexBuffer[_vtxWritePosition] = vertex;
            _vtxWritePosition++;
        }

        private void AppendIndex(int offsetToCurrentIndex)
        {
            indexBuffer[_idxWritePosition] = new DrawIndex { Index = _currentIdx + offsetToCurrentIndex };
            _idxWritePosition++;
        }

        public void PrimReserve(int idx_count, int vtx_count)
        {
            if (idx_count == 0)
            {
                return;
            }

            int vtx_buffer_size = this.VertexBuffer.Count;
            this._vtxWritePosition = vtx_buffer_size;
            this.VertexBuffer.Resize(vtx_buffer_size + vtx_count);

            int idx_buffer_size = this.IndexBuffer.Count;
            this._idxWritePosition = idx_buffer_size;
            this.IndexBuffer.Resize(idx_buffer_size + idx_count);
        }
        #endregion

        #region primitives

        private static readonly List<Point> _Path = new List<Point>();


        #endregion

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

        #region polygon tessellation

        public void AddContour(Color color)
        {
            // determine the winding of the path
            //var pathIsClockwise = IsClockwise(_Path);//no need

            var contour = new List<LibTessDotNet.ContourVertex>();

            int j = 0;
            for (int i = 0; i < _Path.Count; i++)
            {
                var p = _Path[i];

                //check if p is a control point of a quadratic bezier curve
                bool isControlPoint = false;
                if (j <= _BezierControlPointIndex.Count - 1 && i == _BezierControlPointIndex[j])
                {
                    j++;
                    isControlPoint = true;
                }

                if (isControlPoint)
                {
                    var start = _Path[i - 1];
                    var control = p;
                    var end = _Path[i + 1];

                    var bezierIsClockwise = IsClockwise(start, control, end);

                    if (bezierIsClockwise)//bezier 'triangle' is clockwise
                    {
                        //[picture]
                        contour.Add(new LibTessDotNet.ContourVertex
                        {
                            Position = new LibTessDotNet.Vec3
                            {
                                X = (float)control.X,
                                Y = (float)control.Y,
                                Z = 0.0f
                            }
                        });
                    }

                    // add this bezier to bezier buffer
                    AddBezier(start, control, end, color);
                }
                else//not control point of a bezier
                {
                    contour.Add(new LibTessDotNet.ContourVertex
                    {
                        Position = new LibTessDotNet.Vec3
                        {
                            X = (float)p.X,
                            Y = (float)p.Y,
                            Z = 0.0f
                        }
                    });
                }

            }
            _BezierControlPointIndex.Clear();
            // Add the contour with a specific orientation, use "Original" if you want to keep the input orientation.
            tess.AddContour(contour.ToArray()/* TODO remove this copy here!!  */, LibTessDotNet.ContourOrientation.Original);
        }

        static LibTessDotNet.Tess tess = new LibTessDotNet.Tess();// Create an instance of the tessellator. Can be reused.

        public void PathTessPolygon(Color color)
        {
            tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3, null);
            if (tess.Elements == null || tess.Elements.Length == 0)
            {
                return;
            }
            int numTriangles = tess.ElementCount;
            int idx_count = numTriangles * 3;
            int vtx_count = numTriangles * 3;
            PrimReserve(idx_count, vtx_count);
            for (int i = 0; i < numTriangles; i++)
            {
                var index0 = tess.Elements[i * 3];
                var index1 = tess.Elements[i * 3 + 1];
                var index2 = tess.Elements[i * 3 + 2];
                var v0 = tess.Vertices[index0].Position;
                var v1 = tess.Vertices[index1].Position;
                var v2 = tess.Vertices[index2].Position;

                AppendVertex(new DrawVertex { pos = new PointF(v0.X, v0.Y), uv = PointF.Zero, color = (ColorF)color });
                AppendVertex(new DrawVertex { pos = new PointF(v1.X, v1.Y), uv = PointF.Zero, color = (ColorF)color });
                AppendVertex(new DrawVertex { pos = new PointF(v2.X, v2.Y), uv = PointF.Zero, color = (ColorF)color });
                AppendIndex(0);
                AppendIndex(1);
                AppendIndex(2);
                _currentIdx += 3;
            }
            
        }

#if false //to be moved to unit test
        // for rendering path in the immediate window
        public void Debug_DrawToCairoSurface()
        {
            if(_Path.Count == 0) return;

            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, (int)Form.current.Size.Width, (int)Form.current.Size.Height))
            using (Cairo.Context g = new Cairo.Context(surface))
            {
                g.MoveTo(_Path[0].X, _Path[0].Y);
                for (int i = 1; i < _Path.Count; i++)
                {
                    var p = _Path[i];
                    g.LineTo(p.X, p.Y);
                }

                g.Stroke();
                surface.WriteToPng(@"D:\_Path.png");
            }
        }
#endif

        // unused
        private static bool IsClockwise(IList<Point> vertices)
        {
            double sum = 0.0;
            for (int i = 0; i < vertices.Count; i++)
            {
                Point v1 = vertices[i];
                Point v2 = vertices[(i + 1) % vertices.Count]; // % is the modulo operator
                sum += (v2.X - v1.X) * (v2.Y + v1.Y);
            }
            return sum > 0.0;
        }

        private static bool IsClockwise(Point v0, Point v1, Point v2)
        {
            var vA = v1 - v0; // .normalize()
            var vB = v2 - v1;
            var z = vA.X * vB.Y - vA.Y * vB.X; // z component of cross Production
            var wind = z < 0; // clockwise/anticlock wind
            return wind;
        }

        #endregion

        internal void Build(Point position, Style style, ITextContext textContext)
        {
            var color = style.Font.Color;

            Point lastPoint = Point.Zero;
            textContext.Build(position,
                // point adder
                (x, y) =>
                {
                    this.PathLineTo(new Point(x, y));
                    lastPoint = new Point(x, y);
                },

                // bezier adder
                (c0x, c0y, c1x, c1y, p1x, p1y) =>
                {
                    var p0 = lastPoint;//The start point of the cubic Bezier segment.
                    var c0 = new Point(c0x, c0y);//The first control point of the cubic Bezier segment.
                    var p = new Point((c0x + c1x) / 2, (c0y + c1y) / 2);
                    var c1 = new Point(c1x, c1y);//The second control point of the cubic Bezier segment.
                    var p1 = new Point(p1x, p1y);//The end point of the cubic Bezier segment.

                    this.PathAddBezier(p0, c0, p);
                    this.PathAddBezier(p, c1, p1);

                    //set last point for next bezier
                    lastPoint = p1;
                },

                // path closer
                () =>
                {
                },

                // figure beginner
                (x, y) =>
                {
                    lastPoint = new Point(x, y);
                    this.PathMoveTo(lastPoint);
                },

                // figure ender
                () =>
                {
                    this.PathClose();
                    this.AddContour(color);
                    this.PathClear();
                }
            );

            this.PathTessPolygon(color);

        }
    }
}
