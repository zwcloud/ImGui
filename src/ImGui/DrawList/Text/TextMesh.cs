using System.Collections.Generic;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Text;
using System.Runtime.CompilerServices;

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
        
        public bool Visible { get; set; }

        /// <summary>
        /// vertex buffer
        /// </summary>
        /// <remarks>
        /// Specifiy a big capacity to forbid frequent GC due to List reallocation
        /// when adding Glyph triangles and curve segments _frequently_ to the TextMesh.
        /// See *important note* below.
        /// </remarks>
        public VertexBuffer VertexBuffer { get; set; } = new VertexBuffer(100000);

        /// <summary>
        /// index buffer
        /// </summary>
        /// <remarks>
        /// (same as vertex buffer)
        /// </remarks>
        public IndexBuffer IndexBuffer { get; set; } = new IndexBuffer(100000);

        /*
         * Important Note:
         * 
         * The initial capacity of VertexBuffer and IndexBuffer should be big enough.
         * Otherwise, when adding Glyph triangles and curve segments _frequently_ to the TextMesh, 
         * List reallocation will happen frequently and creates many garbages(discarded old `UsafeList._items` buffer of about 200KB-600KB).
         * Those garbages will lead to Generation 2 GC which uses much CPU time and stuck the application.
         *
         * TODO check if this is still a problem after the render-tree based rendering is implemented.
         * It's ridiculous to use a initial capacity of 100000 for the vertex/index buffer
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendVertex(DrawVertex vertex)
        {
            this.VertexBuffer[this.vtxWritePosition] = vertex;
            this.vtxWritePosition++;
        }

        /// <summary>
        /// Append an index to the IndexBuffer
        /// </summary>
        /// <remarks>The value to insert is `_currentIdx + offsetToCurrentIndex`.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendIndex(int offsetToCurrentIndex)
        {
            this.IndexBuffer[this.idxWritePosition] = new DrawIndex { Index = this.currentIdx + offsetToCurrentIndex };
            this.idxWritePosition++;
        }

        //FIXME use the same parameter order as Mesh.PrimReserve
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

        private void AddTriangles(List<List<Point>> polygons, Color color,
            Vector positionOffset, Vector glyphOffset, float scale, bool flipY)
        {
            float positionOffsetX = positionOffset._x;
            float positionOffsetY = positionOffset._y;
            float glyphOffsetX = glyphOffset._x;
            float glyphOffsetY = glyphOffset._y;

            for (var i = 0; i < polygons.Count; ++i)
            {
                var polygon = polygons[i];
                var p0 = polygon[0];
                p0.x += glyphOffsetX;
                p0.y += glyphOffsetY;
                ApplyOffsetScale(ref p0, positionOffsetX, positionOffsetY, scale, flipY);
                PrimReserve(3 * (polygon.Count - 1), 3 * (polygon.Count - 1));
                for (int k = 0; k < polygon.Count - 1; k++)
                {
                    var p1 = polygon[k];
                    p1.x += glyphOffsetX;
                    p1.y += glyphOffsetY;
                    ApplyOffsetScale(ref p1, positionOffsetX, positionOffsetY, scale, flipY);
                    var p2 = polygon[k + 1];
                    p2.x += glyphOffsetX;
                    p2.y += glyphOffsetY;
                    ApplyOffsetScale(ref p2, positionOffsetX, positionOffsetY, scale, flipY);
                    AppendVertex(new DrawVertex { pos = p0, uv = Point.Zero, color = color });
                    AppendVertex(new DrawVertex { pos = p1, uv = Point.Zero, color = color });
                    AppendVertex(new DrawVertex { pos = p2, uv = Point.Zero, color = color });
                    AppendIndex(0);
                    AppendIndex(1);
                    AppendIndex(2);
                    this.currentIdx += 3;
                }
            }
        }

        public void AddBezierSegments(IList<(Point, Point, Point)> segments, Color color, Vector positionOffset, Vector glyphOffset, float scale = 1, bool flipY = true)
        {
            PrimReserve(segments.Count * 3, segments.Count * 3);
            var uv0 = new Point(0, 0);
            var uv1 = new Point(0.5, 0);
            var uv2 = new Point(1, 1);

            float positionOffsetX = positionOffset._x;
            float positionOffsetY = positionOffset._y;
            float glyphOffsetX = glyphOffset._x;
            float glyphOffsetY = glyphOffset._y;

            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];

                var startPoint = segment.Item1;
                startPoint.x += glyphOffsetX;
                startPoint.y += glyphOffsetY;
                ApplyOffsetScale(ref startPoint, positionOffsetX, positionOffsetY, scale, flipY);

                var controlPoint = segment.Item2;
                controlPoint.x += glyphOffsetX;
                controlPoint.y += glyphOffsetY;
                ApplyOffsetScale(ref controlPoint, positionOffsetX, positionOffsetY, scale, flipY);

                var endPoint = segment.Item3;
                endPoint.x += glyphOffsetX;
                endPoint.y += glyphOffsetY;
                ApplyOffsetScale(ref endPoint, positionOffsetX, positionOffsetY, scale, flipY);

                AppendVertex(new DrawVertex { pos = startPoint, uv = uv0, color = color });
                AppendVertex(new DrawVertex { pos = controlPoint, uv = uv1, color = color });
                AppendVertex(new DrawVertex { pos = endPoint, uv = uv2, color = color });

                AppendIndex(0);
                AppendIndex(1);
                AppendIndex(2);
                this.currentIdx += 3;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyOffsetScale(ref Point point, float offsetX, float offsetY, float scale, bool flipY)
        {
            point._x = point._x * scale + offsetX;
            point._y = point._y * scale * (flipY ? -1 : 1) + offsetY;
        }

        internal void Build(Point position, ITextContext textContext)
        {
            textContext.Build(position);
        }

        public void Append(Vector positionOffset, GlyphData glyphData, Vector glyphOffset, float scale, Color color, bool flipY)
        {
            var polygons = glyphData.Polygons;
            var segments = glyphData.QuadraticCurveSegments;

            // triangles
            AddTriangles(polygons, color, positionOffset, glyphOffset, scale, flipY);
            // quadratic bezier segments
            AddBezierSegments(segments, color, positionOffset, glyphOffset, scale, flipY);
        }

        public void Append(TextMesh textMesh, Vector offset)
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
                this.VertexBuffer.Append(textMesh.VertexBuffer);
                this.IndexBuffer.Append(textMesh.IndexBuffer);
                var newIndexCount = this.IndexBuffer.Count;
                for (int i = oldIndexCount; i < newIndexCount; i++)
                {
                    var index = this.IndexBuffer[i].Index;
                    index += oldVertexCount;
                    this.IndexBuffer[i] = new DrawIndex {Index = index};
                }
            }

            // Apply offset to appended part
            if (!MathEx.AmostZero(offset.X) || !MathEx.AmostZero(offset.Y))
            {
                for (int i = oldVertexCount; i < this.VertexBuffer.Count; i++)
                {
                    var vertex = this.VertexBuffer[i];
                    vertex.pos = new Point(vertex.pos.X + offset.X, vertex.pos.Y + offset.Y);
                    this.VertexBuffer[i] = vertex;
                }
            }
        }
    }
}
