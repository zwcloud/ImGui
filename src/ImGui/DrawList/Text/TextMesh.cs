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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendVertex(DrawVertex vertex)
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
            Vector positionOffset, Vector glyphOffset, double scale, bool flipY)
        {
            float glyphOffsetX = (float)glyphOffset.X;
            float glyphOffsetY = (float)glyphOffset.Y;
            float positionOffsetX = (float)positionOffset.X;
            float positionOffsetY = (float)positionOffset.Y;
            float scaleF = (float)scale;

            foreach (var polygon in polygons)
            {
                if (polygon == null || polygon.Count < 3)
                {
                    continue;
                }
                var p0 = polygon[0];
                p0.x += glyphOffsetX;
                p0.y += glyphOffsetY;
                ApplyOffsetScale(ref p0, positionOffsetX, positionOffsetY, scaleF, flipY);
                PrimReserve(3 * (polygon.Count - 1), 3 * (polygon.Count - 1));
                for (int i = 0; i < polygon.Count - 1; i++)
                {
                    var p1 = polygon[i];
                    p1.x += glyphOffsetX;
                    p1.y += glyphOffsetY;
                    ApplyOffsetScale(ref p1, positionOffsetX, positionOffsetY, scaleF, flipY);
                    var p2 = polygon[i + 1];
                    p2.x += glyphOffsetX;
                    p2.y += glyphOffsetY;
                    ApplyOffsetScale(ref p2, positionOffsetX, positionOffsetY, scaleF, flipY);
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

        public void AddBezierSegments(IList<(Point, Point, Point)> segments, Color color, Vector positionOffset, Vector glyphOffset, double scale = 1, bool flipY = true)
        {
            PrimReserve(segments.Count * 3, segments.Count * 3);
            var uv0 = new Point(0, 0);
            var uv1 = new Point(0.5, 0);
            var uv2 = new Point(1, 1);
            float glyphOffsetX = (float)glyphOffset.X;
            float glyphOffsetY = (float)glyphOffset.Y;
            float positionOffsetX = (float)positionOffset.X;
            float positionOffsetY = (float)positionOffset.Y;
            float scaleF = (float)scale;

            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];

                var startPoint = segment.Item1;
                startPoint.x += glyphOffsetX;
                startPoint.y += glyphOffsetY;
                ApplyOffsetScale(ref startPoint, positionOffsetX, positionOffsetY, scaleF, flipY);

                var controlPoint = segment.Item2;
                controlPoint.x += glyphOffsetX;
                controlPoint.y += glyphOffsetY;
                ApplyOffsetScale(ref controlPoint, positionOffsetX, positionOffsetY, scaleF, flipY);

                var endPoint = segment.Item3;
                endPoint.x += glyphOffsetX;
                endPoint.y += glyphOffsetY;
                ApplyOffsetScale(ref endPoint, positionOffsetX, positionOffsetY, scaleF, flipY);

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
            //return new Point(point.x * scale + offsetX, point.y * scale * (flipY ? -1 : 1)+ offsetY);
        }

        internal void Build(Point position, ITextContext textContext)
        {
            textContext.Build(position);
        }

        public void Append(Vector positionOffset, GlyphData glyphData, Vector glyphOffset, double scale, Color color, bool flipY)
        {
            var polygons = glyphData.Polygons;
            var segments = glyphData.QuadraticCurveSegments;

            // triangles
            AddTriangles(polygons, color, positionOffset, glyphOffset, scale, flipY);
            // quadratic bezier segments
            AddBezierSegments(segments, color, positionOffset, glyphOffset, scale, flipY);
        }
    }
}
