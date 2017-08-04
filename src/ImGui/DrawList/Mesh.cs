using ImGui.Common;

namespace ImGui
{
    internal class Mesh
    {
        private readonly UnsafeList<DrawCommand> commandBuffer = new UnsafeList<DrawCommand>();
        private readonly UnsafeList<DrawIndex> indexBuffer = new UnsafeList<DrawIndex>();
        private readonly UnsafeList<DrawVertex> vertexBuffer = new UnsafeList<DrawVertex>();

        public int _vtxWritePosition;
        public int _idxWritePosition;
        public int _currentIdx;

        /// <summary>
        /// Commands. Typically 1 command = 1 gpu draw call.
        /// </summary>
        /// <remarks>Every command corresponds to 1 sub-mesh.</remarks>
        public UnsafeList<DrawCommand> CommandBuffer
        {
            get { return commandBuffer; }
        }

        /// <summary>
        /// Index buffer. Each command consume DrawCommand.ElemCount of those
        /// </summary>
        public UnsafeList<DrawIndex> IndexBuffer
        {
            get { return indexBuffer; }
        }

        /// <summary>
        /// Vertex buffer
        /// </summary>
        public UnsafeList<DrawVertex> VertexBuffer
        {
            get { return vertexBuffer; }
        }

        /// <summary>
        /// Append a vertex to the VertexBuffer
        /// </summary>
        public void AppendVertex(DrawVertex vertex)
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

        /// <summary>
        /// Pre-allocate space for a number of indexes and vertexes.
        /// </summary>
        /// <param name="idx_count">the number of indexes to add</param>
        /// <param name="vtx_count">the number of vertexes to add</param>
        public void PrimReserve(int idx_count, int vtx_count)
        {
            if (idx_count == 0)
            {
                return;
            }

            DrawCommand drawCommand = this.CommandBuffer[CommandBuffer.Count - 1];
            drawCommand.ElemCount += idx_count;
            this.CommandBuffer[CommandBuffer.Count - 1] = drawCommand;

            int vtx_buffer_size = this.VertexBuffer.Count;
            this._vtxWritePosition = vtx_buffer_size;
            this.VertexBuffer.Resize(vtx_buffer_size + vtx_count);

            int idx_buffer_size = this.IndexBuffer.Count;
            this._idxWritePosition = idx_buffer_size;
            this.IndexBuffer.Resize(idx_buffer_size + idx_count);
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
            this._vtxWritePosition = 0;
            this._idxWritePosition = 0;
            this._currentIdx = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexBuffer"></param>
        /// <param name="vertexBuffer"></param>
        public void Fill(UnsafeList<DrawIndex> indexBuffer, UnsafeList<DrawVertex> vertexBuffer)
        {
            DrawCommand drawCommand = this.CommandBuffer[this.CommandBuffer.Count - 1];
            var idx_count = indexBuffer.Count;
            var vtx_count = vertexBuffer.Count;
            if (idx_count != 0 && vtx_count != 0)
            {
                drawCommand.ElemCount += idx_count;
                this.CommandBuffer[this.CommandBuffer.Count - 1] = drawCommand;

                var vertexCountBefore = this.VertexBuffer.Count;

                int vtx_buffer_size = this.VertexBuffer.Count;
                this._vtxWritePosition = vtx_buffer_size + vtx_count;
                this.VertexBuffer.AddRange(vertexBuffer);

                int idx_buffer_size = this.IndexBuffer.Count;
                this._idxWritePosition = idx_buffer_size + idx_count;

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
                this._currentIdx += vtx_count;
            }
        }

    }
}