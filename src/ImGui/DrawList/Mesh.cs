using System.Collections.Generic;
using ImGui.Common;

namespace ImGui
{
    internal class Mesh
    {
        private int vtxWritePosition;
        private int idxWritePosition;
        public int currentIdx;

        /// <summary>
        /// Commands. Typically 1 command = 1 gpu draw call.
        /// </summary>
        /// <remarks>Every command corresponds to 1 sub-mesh.</remarks>
        public List<DrawCommand> CommandBuffer { get; } = new List<DrawCommand>();

        /// <summary>
        /// Index buffer. Each command consume DrawCommand.ElemCount of those
        /// </summary>
        public UnsafeList<DrawIndex> IndexBuffer { get; } = new UnsafeList<DrawIndex>();

        /// <summary>
        /// Vertex buffer
        /// </summary>
        public UnsafeList<DrawVertex> VertexBuffer { get; } = new UnsafeList<DrawVertex>();

        /// <summary>
        /// Append a vertex to the VertexBuffer
        /// </summary>
        public void AppendVertex(DrawVertex vertex)
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

        /// <summary>
        /// Pre-allocate space for a number of indexes and vertexes.
        /// </summary>
        /// <param name="idxCount">the number of indexes to add</param>
        /// <param name="vtxCount">the number of vertexes to add</param>
        public void PrimReserve(int idxCount, int vtxCount)
        {
            if (idxCount == 0)
            {
                return;
            }

            DrawCommand drawCommand = this.CommandBuffer[this.CommandBuffer.Count - 1];
            drawCommand.ElemCount += idxCount;
            this.CommandBuffer[this.CommandBuffer.Count - 1] = drawCommand;

            int vtxBufferSize = this.VertexBuffer.Count;
            this.vtxWritePosition = vtxBufferSize;
            this.VertexBuffer.Resize(vtxBufferSize + vtxCount);

            int idxBufferSize = this.IndexBuffer.Count;
            this.idxWritePosition = idxBufferSize;
            this.IndexBuffer.Resize(idxBufferSize + idxCount);
        }

        /// <summary>
        /// Clear the buffers and reset states of vertex and index writer.
        /// </summary>
        /// <remarks>
        /// The capacity of buffers is not changed.
        /// So no OS-level memory allocation will happen if the buffers don't get bigger than their capacity.
        /// </remarks>
        public void Clear()
        {
            this.CommandBuffer.Clear();
            this.IndexBuffer.Clear();
            this.VertexBuffer.Clear();
            this.vtxWritePosition = 0;
            this.idxWritePosition = 0;
            this.currentIdx = 0;
        }

        /// <summary>
        /// Append a mesh to this mesh
        /// </summary>
        /// <param name="mesh"></param>
        public void Append(Mesh mesh)
        {
            UnsafeList<DrawVertex> vertexBuffer = mesh.VertexBuffer;
            UnsafeList<DrawIndex> indexBuffer = mesh.IndexBuffer;

            DrawCommand drawCommand = this.CommandBuffer[this.CommandBuffer.Count - 1];
            var idxCount = indexBuffer.Count;
            var vtxCount = vertexBuffer.Count;
            if (idxCount != 0 && vtxCount != 0)
            {
                drawCommand.ElemCount += idxCount;
                this.CommandBuffer[this.CommandBuffer.Count - 1] = drawCommand;

                var vertexCountBefore = this.VertexBuffer.Count;

                int vtxBufferSize = this.VertexBuffer.Count;
                this.vtxWritePosition = vtxBufferSize + vtxCount;
                this.VertexBuffer.AddRange(vertexBuffer);

                int idxBufferSize = this.IndexBuffer.Count;
                this.idxWritePosition = idxBufferSize + idxCount;

                var sizeBefore = this.IndexBuffer.Count;
                this.IndexBuffer.AddRange(indexBuffer);
                var sizeAfter = this.IndexBuffer.Count;

                if (vertexCountBefore != 0)
                {
                    for (int i = sizeBefore; i < sizeAfter; i++)
                    {
                        this.IndexBuffer[i] = new DrawIndex
                        {
                            Index = this.IndexBuffer[i].Index + vertexCountBefore
                        };
                    }
                }
                this.currentIdx += vtxCount;
            }
        }

    }
}