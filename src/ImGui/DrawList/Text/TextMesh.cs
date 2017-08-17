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
    ///   1. triangles: generated from glyph contours
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
        /// <remarks>
        /// Specifiy a big capacity to forbid frequent GC due to List reallocation
        /// when adding Glyph triangles and curve segments _frequently_ to the TextMesh.
        /// See important not below.
        /// </remarks>
        public UnsafeList<DrawVertex> VertexBuffer { get; set; } = new UnsafeList<DrawVertex>(100000);

        /// <summary>
        /// index buffer
        /// </summary>
        /// <remarks>
        /// (same as vertex buffer)
        /// </remarks>
        public UnsafeList<DrawIndex> IndexBuffer { get; set; } = new UnsafeList<DrawIndex>(100000);

        /*
         * Important Note:
         * 
         * The initial capacity of VertexBuffer and IndexBuffer should be big enough.
         * Otherwise, when adding Glyph triangles and curve segments _frequently_ to the TextMesh, 
         * List reallocation will happen frequently and creates many garbages(discarded old `UsafeList._items` buffer of about 200KB-600KB).
         * Those garbages will lead to Generation 2 GC which uses much CPU time and stuck the application.
         */

        public List<DrawCommand> Commands { get; set; } = new List<DrawCommand>(2);

        public void Clear()
        {
            this.VertexBuffer.Clear();
            this.IndexBuffer.Clear();
            this.vtxWritePosition = 0;
            this.idxWritePosition = 0;
            this.currentIdx = 0;

            this.Commands.Clear();
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

        public void AddBezierSegments(IList<(Point, Point, Point)> segments, Color color,  Vector positionOffset = new Vector(), Vector glyphOffset = new Vector(), double scale = 1, bool flipY = true)
        {
            PrimReserve(segments.Count * 3, segments.Count * 3);
            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];
                var startPoint = segment.Item1;
                startPoint += glyphOffset;
                startPoint = ApplyOffsetScale(startPoint, positionOffset.X, positionOffset.Y, scale, flipY);
                var controlPoint = segment.Item2;
                controlPoint += glyphOffset;
                controlPoint = ApplyOffsetScale(controlPoint, positionOffset.X, positionOffset.Y, scale, flipY);
                var endPoint = segment.Item3;
                endPoint += glyphOffset;
                endPoint = ApplyOffsetScale(endPoint, positionOffset.X, positionOffset.Y, scale, flipY);
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

        private static Point ApplyOffsetScale(Point point, double offsetX, double offsetY, double scale, bool flipY)
        {
            return new Point(point.X * scale + offsetX, point.Y * scale * (flipY ? -1 : 1)+ offsetY);
        }

        internal void Build(Point position, ITextContext textContext)
        {
            textContext.Build(position);
        }

        public void Append(TextMesh textMesh, Vector offset)// must use after DrawList.AddText
        {
            var oldVertexCount = this.VertexBuffer.Count;
            var oldIndexCount = this.IndexBuffer.Count;
            // Update added command
            var command = this.Commands[this.Commands.Count - 1];
            command.ElemCount = textMesh.IndexBuffer.Count;
            this.Commands[this.Commands.Count - 1] = command;

            // TODO merge command with previous one if they share the same clip rect.

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

        public void Append(Vector positionOffset, GlyphData glyphData, Vector glyphOffset, double scale, Color color, bool flipY)
        {
            var polygons = glyphData.Polygons;
            var segments = glyphData.QuadraticCurveSegments;

            // triangles
            foreach (var polygon in polygons)
            {
                if (polygon == null || polygon.Count < 3) { continue; }
                var p0 = polygon[0] + glyphOffset;
                p0 = ApplyOffsetScale(p0, positionOffset.X, positionOffset.Y, scale, flipY);
                for (int i = 0; i < polygon.Count - 1; i++)
                {
                    var p1 = polygon[i] + glyphOffset;
                    p1 = ApplyOffsetScale(p1, positionOffset.X, positionOffset.Y, scale, flipY);
                    var p2 = polygon[i + 1] + glyphOffset;
                    p2 = ApplyOffsetScale(p2, positionOffset.X, positionOffset.Y, scale, flipY);
                    AddTriangle(p0, p1, p2, color);
                }
            }
            // quadratic bezier segments
            AddBezierSegments(segments, color, positionOffset, glyphOffset, scale, flipY);
        }
    }
}
