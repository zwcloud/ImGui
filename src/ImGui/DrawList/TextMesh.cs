using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui
{
    /// <summary>
    /// Text mesh
    /// </summary>
    /// <remarks>
    /// A text mesh contains two parts:
    ///   1. triangle strip: generated from glyph contours
    ///   2. quadratic bezier segments: generated from glyph bezier curves
    /// </remarks>
    class TextMesh
    {
        // triangle strip will be rendered as triangle strip
        ImGui.Internal.UnsafeList<DrawVertex> vertexBuffer = new ImGui.Internal.UnsafeList<DrawVertex>();
        ImGui.Internal.UnsafeList<DrawIndex> indexBuffer = new ImGui.Internal.UnsafeList<DrawIndex>();

        // quadratic bezier segments will be rendered as triangle list
        ImGui.Internal.UnsafeList<DrawVertex> bezierVertexBuffer = new ImGui.Internal.UnsafeList<DrawVertex>();
        ImGui.Internal.UnsafeList<DrawIndex> bezierIndexBuffer = new ImGui.Internal.UnsafeList<DrawIndex>();

        DrawCommand triangleStripCommand = new DrawCommand { ClipRect = Rect.Big, PrimitiveType = PrimitiveType.TriangleStrip };
        DrawCommand segmentCommand = new DrawCommand { ClipRect = Rect.Big, PrimitiveType = PrimitiveType.TriangleList };

        public int _vtxWritePosition;
        public int _idxWritePosition;
        public int _currentIdx;
        private int _bezier_vtxWritePosition;
        private int _bezier_idxWritePosition;
        private int _bezier_currentIdx;

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

        public void PrimBezierReserve(int segment_point_count)
        {
            if (segment_point_count == 0)
            {
                return;
            }

            int vtx_buffer_size = this.BezierVertexBuffer.Count;
            this._bezier_vtxWritePosition = vtx_buffer_size;
            this.BezierVertexBuffer.Resize(vtx_buffer_size + segment_point_count);

            int idx_buffer_size = this.BezierIndexBuffer.Count;
            this._bezier_idxWritePosition = idx_buffer_size;
            this.BezierIndexBuffer.Resize(idx_buffer_size + segment_point_count);

            var command = Command1;
            triangleStripCommand.ElemCount += segment_point_count;
            Command1 = command;
        }

        /// <summary>
        /// Vertex buffer
        /// </summary>
        public ImGui.Internal.UnsafeList<DrawVertex> VertexBuffer
        {
            get { return vertexBuffer; }
            set { vertexBuffer = value; }
        }

        public ImGui.Internal.UnsafeList<DrawIndex> IndexBuffer
        {
            get { return indexBuffer; }
            set { indexBuffer = value; }
        }
        
        /// <summary>
        /// Index buffer for bezier curves
        /// </summary>
        public ImGui.Internal.UnsafeList<DrawIndex> BezierIndexBuffer
        {
            get { return bezierIndexBuffer; }
            set { bezierIndexBuffer = value; }
        }

        /// <summary>
        /// Vertex buffer for beziers curves
        /// </summary>
        public ImGui.Internal.UnsafeList<DrawVertex> BezierVertexBuffer
        {
            get { return bezierVertexBuffer; }
        }

        public DrawCommand Command0
        {
            get => triangleStripCommand;
            set => triangleStripCommand = value;
        }
        public DrawCommand Command1
        {
            get => segmentCommand;
            set => segmentCommand = value;
        }

        public void Clear()
        {
            _Path.Clear();

            // triangles
            this.VertexBuffer.Clear();
            this.IndexBuffer.Clear();
            this._vtxWritePosition = 0;
            this._idxWritePosition = 0;
            _currentIdx = 0;
            Command0 = new DrawCommand { ClipRect = Rect.Big, PrimitiveType = PrimitiveType.TriangleStrip };

            // bezier segments
            this.BezierIndexBuffer.Clear();
            this.BezierVertexBuffer.Clear();
            _bezier_vtxWritePosition = 0;
            _bezier_idxWritePosition = 0;
            _bezier_currentIdx = 0;
            Command1 = new DrawCommand { ClipRect = Rect.Big, PrimitiveType = PrimitiveType.TriangleList };
        }

        private void AppendVertex(DrawVertex vertex)
        {
            vertexBuffer[_vtxWritePosition] = vertex;
            _vtxWritePosition++;
        }

        /// <summary>
        /// Append an index to the IndexBuffer
        /// </summary>
        /// <remarks>The value to insert is `_currentIdx + offsetToCurrentIndex`.</remarks>
        public void AppendIndex(int offsetToCurrentIndex)
        {
            indexBuffer[_idxWritePosition] = new DrawIndex { Index = _currentIdx + offsetToCurrentIndex };
            _idxWritePosition++;
        }

        public void PrimReserve(int vtx_count, int idx_count)
        {
            if (vtx_count == 0)
            {
                return;
            }

            int vtx_buffer_size = this.VertexBuffer.Count;
            this._vtxWritePosition = vtx_buffer_size;
            this.VertexBuffer.Resize(vtx_buffer_size + vtx_count);

            int idx_buffer_size = this.IndexBuffer.Count;
            this._idxWritePosition = idx_buffer_size;
            this.IndexBuffer.Resize(idx_buffer_size + idx_count);

            var command = this.Command0;
            command.ElemCount += vtx_count;
            this.Command0 = command;
        }

        private static readonly List<Point> _Path = new List<Point>();

        public void PathClear()
        {
            _Path.Clear();
        }

        public void PathMoveTo(Point point)
        {
            _Path.Add(point);
        }

        public void PathLineTo(Point pos)
        {
            _Path.Add(pos);
        }

        public void PathClose()
        {
            _Path.Add(_Path[0]);
        }

        public void PathAddBezier(Point start, Point control, Point end)
        {
            _Path.Add(start);
            _Path.Add(control);
            _Path.Add(end);
        }

        public void AddTriangle(Point a, Point b, Point c, Color color)
        {
            PrimReserve(3, 3);
            AppendVertex(new DrawVertex { pos = (PointF)a, uv = PointF.Zero, color = (ColorF)color });
            AppendVertex(new DrawVertex { pos = (PointF)b, uv = PointF.Zero, color = (ColorF)color });
            AppendVertex(new DrawVertex { pos = (PointF)c, uv = PointF.Zero, color = (ColorF)color });
            AppendIndex(0);
            AppendIndex(1);
            AppendIndex(2);
            _currentIdx += 3;
        }

        public void AddBezierSegments(IList<(Point, Point, Point)> segments, Color color)
        {
            PrimReserve(segments.Count * 3, segments.Count * 3);
            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];
                var startPoint = segment.Item1;
                var controlPoint = segment.Item2;
                var endPoint = segment.Item3;
                var uv0 = new PointF(0, 0);
                var uv1 = new PointF(0.5, 0);
                var uv2 = new PointF(1, 1);
                AppendVertex(new DrawVertex { pos = (PointF)startPoint, uv = uv0, color = (ColorF)color });
                AppendVertex(new DrawVertex { pos = (PointF)controlPoint, uv = uv1, color = (ColorF)color });
                AppendVertex(new DrawVertex { pos = (PointF)endPoint, uv = uv2, color = (ColorF)color });

                AppendIndex(0);
                AppendIndex(1);
                AppendIndex(2);
                _currentIdx += 3;
            }
        }

        TextGeometryContainer textGeometryContainer = new TextGeometryContainer();
        internal void Build(Point position, GUIStyle style, ITextContext textContext)
        {
            var color = style.Get<Color>(GUIStyleName.FontColor);
            textContext.Build(position, color, textGeometryContainer);

            // create mesh data

            // triangles
            Color _color = new Color(1.01 / 255, 0, 0, 1);
            foreach (var polygon in textGeometryContainer.Polygons)
            {
                if (polygon == null || polygon.Count < 3) { continue; }
                for (int i = 0; i < polygon.Count-1; i++)
                {
                    AddTriangle(polygon[0], polygon[i], polygon[i + 1], _color);
                }
            }
            // bezier segments
            AddBezierSegments(textGeometryContainer.CurveSegments, _color);
        }

        public void Append(TextMesh textMesh, Vector offset)
        {
            var oldVertexCount = this.VertexBuffer.Count;
            var oldIndexCount = this.IndexBuffer.Count;
            var oldBezierVertexCount = this.BezierVertexBuffer.Count;
            var oldBezierIndexCount = this.BezierIndexBuffer.Count;

            // Append mesh data
            {
                this.VertexBuffer.AddRange(textMesh.VertexBuffer);
                this.IndexBuffer.AddRange(textMesh.IndexBuffer);
                var newIndexCount = this.IndexBuffer.Count;
                for (int i = oldIndexCount; i < newIndexCount; i++)
                {
                    var index = this.IndexBuffer[i].Index;
                    index += oldVertexCount;
                    this.IndexBuffer[i] = new DrawIndex { Index = index };
                }
                var command = this.Command0;
                command.ElemCount = this.IndexBuffer.Count;
                this.Command0 = command;
            }
            {
                this.BezierVertexBuffer.AddRange(textMesh.BezierVertexBuffer);
                this.BezierIndexBuffer.AddRange(textMesh.BezierIndexBuffer);
                var newBezierIndexCount = this.BezierIndexBuffer.Count;
                for (int i = oldBezierIndexCount; i < newBezierIndexCount; i++)
                {
                    var index = this.BezierIndexBuffer[i].Index;
                    index += oldBezierVertexCount;
                    this.BezierIndexBuffer[i] = new DrawIndex { Index = index };
                }
                var command = this.Command1;
                command.ElemCount = bezierIndexBuffer.Count;
                this.Command1 = command;
            }

            // Apply offset to appended part
            if (!MathEx.AmostZero(offset.X) || !MathEx.AmostZero(offset.Y))
            {
                for (int i = oldVertexCount; i < this.VertexBuffer.Count; i++)
                {
                    var vertex = this.vertexBuffer[i];
                    vertex.pos = new PointF(vertex.pos.X + offset.X, vertex.pos.Y + offset.Y);
                    this.vertexBuffer[i] = vertex;
                }
                for (int i = oldBezierVertexCount; i < this.BezierVertexBuffer.Count; i++)
                {
                    var vertex = this.bezierVertexBuffer[i];
                    vertex.pos = new PointF(vertex.pos.X + offset.X, vertex.pos.Y + offset.Y);
                    this.bezierVertexBuffer[i] = vertex;
                }
            }
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

            //TODO re-think text mesh caching method and when to rebuild and remove unused text mesh

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
                textContext.Build(Point.Zero, Color.Clear, null);
                TextContextCache.Add(textMeshId, textContext);
            }
            return textContext;
        }
    }
}
