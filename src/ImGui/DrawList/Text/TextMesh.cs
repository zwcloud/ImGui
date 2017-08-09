using System.Collections.Generic;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Text;

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
        private int vtxWritePosition;
        private int idxWritePosition;
        private int currentIdx;

        /// <summary>
        /// vertex buffer
        /// </summary>
        public UnsafeList<DrawVertex> VertexBuffer { get; set; } = new UnsafeList<DrawVertex>();

        /// <summary>
        /// index buffer
        /// </summary>
        public UnsafeList<DrawIndex> IndexBuffer { get; set; } = new UnsafeList<DrawIndex>();

        /// <summary>
        /// draw command
        /// </summary>
        public DrawCommand Command { get; set; } = new DrawCommand { ClipRect = Rect.Big };

        public void Clear()
        {
            Path.Clear();

            this.VertexBuffer.Clear();
            this.IndexBuffer.Clear();
            this.vtxWritePosition = 0;
            this.idxWritePosition = 0;
            this.currentIdx = 0;

            this.Command = new DrawCommand { ClipRect = Rect.Big };
        }

        private void AppendVertex(DrawVertex vertex)
        {
            this.VertexBuffer[this.vtxWritePosition] = vertex;
            this.vtxWritePosition++;
        }

        /// <summary>
        /// Append an index to the IndexBuffer
        /// </summary>
        /// <remarks>The value to insert is `_currentIdx + offsetToCurrentIndex`.</remarks>
        public void AppendIndex(int offsetToCurrentIndex)
        {
            this.IndexBuffer[this.idxWritePosition] = new DrawIndex { Index = this.currentIdx + offsetToCurrentIndex };
            this.idxWritePosition++;
        }

        public void PrimReserve(int vtxCount, int idxCount)
        {
            if (vtxCount == 0)
            {
                return;
            }

            int vtxBufferSize = this.VertexBuffer.Count;
            this.vtxWritePosition = vtxBufferSize;
            this.VertexBuffer.Resize(vtxBufferSize + vtxCount);

            int idxBufferSize = this.IndexBuffer.Count;
            this.idxWritePosition = idxBufferSize;
            this.IndexBuffer.Resize(idxBufferSize + idxCount);

            var command = this.Command;
            command.ElemCount += vtxCount;
            this.Command = command;
        }

        private static readonly List<Point> Path = new List<Point>();

        public void PathClear()
        {
            Path.Clear();
        }

        public void PathMoveTo(Point point)
        {
            Path.Add(point);
        }

        public void PathLineTo(Point pos)
        {
            Path.Add(pos);
        }

        public void PathClose()
        {
            Path.Add(Path[0]);
        }

        public void PathAddBezier(Point start, Point control, Point end)
        {
            Path.Add(start);
            Path.Add(control);
            Path.Add(end);
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
            this.currentIdx += 3;
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
                this.currentIdx += 3;
            }
        }

        private readonly TextGeometryContainer textGeometryContainer = new TextGeometryContainer();
        internal void Build(Point position, GUIStyle style, ITextContext textContext)
        {
            var color = style.Get<Color>(GUIStyleName.FontColor);
            this.textGeometryContainer.Clear();
            textContext.Build(position, color, this.textGeometryContainer);

            // create mesh data
            // triangles
            foreach (var polygon in this.textGeometryContainer.Polygons)
            {
                if (polygon == null || polygon.Count < 3) { continue; }
                for (int i = 0; i < polygon.Count-1; i++)
                {
                    AddTriangle(polygon[0], polygon[i], polygon[i + 1], color);
                }
            }
            // bezier segments
            AddBezierSegments(this.textGeometryContainer.CurveSegments, color);
        }

        public void Append(TextMesh textMesh, Vector offset)
        {
            var oldVertexCount = this.VertexBuffer.Count;
            var oldIndexCount = this.IndexBuffer.Count;

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
                var command = this.Command;
                command.ElemCount = this.IndexBuffer.Count;
                this.Command = command;
            }

            // Apply offset to appended part
            if (!MathEx.AmostZero(offset.X) || !MathEx.AmostZero(offset.Y))
            {
                for (int i = oldVertexCount; i < this.VertexBuffer.Count; i++)
                {
                    var vertex = this.VertexBuffer[i];
                    vertex.pos = new PointF(vertex.pos.X + offset.X, vertex.pos.Y + offset.Y);
                    this.VertexBuffer[i] = vertex;
                }
            }
        }
    }
}
