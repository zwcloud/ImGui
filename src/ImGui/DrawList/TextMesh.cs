using System;
using System.Collections.Generic;
using LibTessDotNet;

namespace ImGui
{
    interface ITextPathBuilder
    {
        void PathClear();
        void PathMoveTo(Point point);
        void PathLineTo(Point pos);
        void PathClose();
        void PathAddBezier(Point start, Point control, Point end);

        /// <summary>
        /// Append contour
        /// </summary>
        /// <param name="color"></param>
        void AddContour(Color color);
    }

    /// <summary>
    /// Text mesh
    /// </summary>
    /// <remarks>
    /// A text mesh contains two parts:
    ///   1. triangles: generated from glyph contours (line segment part)
    ///   2. bezier segments: generated from glyph bezier curves
    /// </remarks>
    class TextMesh : ITextPathBuilder
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

        /// <summary>
        /// Append contour
        /// </summary>
        /// <param name="color"></param>
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

        internal void Build(Point position, GUIStyle style, ITextContext textContext)
        {
            var color = style.Get<Color>(GUIStyleName.FontColor);
            textContext.Build(position, this);
            this.PathTessPolygon(color);
        }
    }

    class TextMeshUtil
    {
        static readonly Dictionary<int, TextMesh> TextMeshCache = new Dictionary<int, TextMesh>();
        static readonly Dictionary<int, ITextContext> TextContextCache = new Dictionary<int, ITextContext>();

        static int GetTextId(string text, Size size, GUIStyle style, GUIState state)
        {
            int hash = 17;
            hash = hash * 23 + text.GetHashCode();
            hash = hash * 23 + size.GetHashCode();
            hash = hash * 23 + style.GetHashCode();
            hash = hash * 23 + state.GetHashCode();
            return hash;
        }

        /// <summary>
        /// build the text context against the size and style
        /// </summary>
        internal static TextMesh GetTextMesh(string text, Size size, GUIStyle style, GUIState state)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (style == null) throw new ArgumentNullException(nameof(style));

            //TODO re-think text mesh caching method and when to rebuild

            int textMeshId = GetTextId(text, size, style, state);

            TextMesh mesh;
            if (TextMeshCache.TryGetValue(textMeshId, out mesh))
            {
                return mesh;
            }
            else
            {
                // create a text mesh
                ITextContext textContext = GetTextContext(text, size, style, state);
                mesh = new TextMesh();
                mesh.Build(Point.Zero, style, textContext);
                TextMeshCache.Add(textMeshId, mesh);
            }

            return mesh;
        }

        internal static ITextContext GetTextContext(string text, Size size, GUIStyle style, GUIState state)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (style == null) throw new ArgumentNullException(nameof(style));

            int textMeshId = GetTextId(text, size, style, state);

            ITextContext textContext;
            if (TextContextCache.TryGetValue(textMeshId, out textContext))
            {
                return textContext;
            }
            else
            {
                // create a TextContent for the text
                var fontFamily = style.Get<string>(GUIStyleName.FontFamily, state);
                var fontSize = style.Get<double>(GUIStyleName.FontSize, state);
                var fontStretch = (FontStretch)style.Get<int>(GUIStyleName.FontStretch, state);
                var fontStyle = (FontStyle)style.Get<int>(GUIStyleName.FontStyle, state);
                var fontWeight = (FontWeight)style.Get<int>(GUIStyleName.FontWeight, state);
                var textAlignment = (TextAlignment)style.Get<int>(GUIStyleName.TextAlignment, state);
                textContext = Application.platformContext.CreateTextContext(
                    text,
                    fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                    (int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height),
                    textAlignment);
                textContext.Build(Point.Zero, null);
                TextContextCache.Add(textMeshId, textContext);
            }
            return textContext;
        }
    }
}
