namespace ImGui
{
    class DrawBuffer
    {
        private readonly ImGui.Internal.List<DrawCommand> commandBuffer = new ImGui.Internal.List<DrawCommand>();
        private readonly ImGui.Internal.List<DrawIndex> indexBuffer = new ImGui.Internal.List<DrawIndex>();
        private readonly ImGui.Internal.List<DrawVertex> vertexBuffer = new ImGui.Internal.List<DrawVertex>();

        public int _vtxWritePosition;
        public int _idxWritePosition;
        public int _currentIdx;

        /// <summary>
        /// Commands. Typically 1 command = 1 gpu draw call.
        /// </summary>
        public ImGui.Internal.List<DrawCommand> CommandBuffer
        {
            get { return commandBuffer; }
        }

        /// <summary>
        /// Index buffer. Each command consume DrawCommand.ElemCount of those
        /// </summary>
        public ImGui.Internal.List<DrawIndex> IndexBuffer
        {
            get { return indexBuffer; }
        }

        /// <summary>
        /// Vertex buffer
        /// </summary>
        public ImGui.Internal.List<DrawVertex> VertexBuffer
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

            if (CommandBuffer.Count == 0)
            {
                CommandBuffer.Add(DrawCommand.Default);
            }
            DrawCommand newDrawCommand = this.CommandBuffer[CommandBuffer.Count - 1];
            newDrawCommand.ElemCount += idx_count;
            this.CommandBuffer[CommandBuffer.Count - 1] = newDrawCommand;

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
    }
}