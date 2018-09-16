using System.Collections.Generic;
using ImGui.Common;
using System.Runtime.CompilerServices;
using ImGui.Rendering;

namespace ImGui
{
    internal class Mesh
    {
        private int vtxWritePosition;
        private int idxWritePosition;
        public int currentIdx;

        public bool Visible { get; set; } = true;

        public Node Node { get; set; }

        /// <summary>
        /// Commands. Typically 1 command = 1 gpu draw call.
        /// </summary>
        /// <remarks>Every command corresponds to 1 sub-mesh.</remarks>
        public List<DrawCommand> CommandBuffer { get; } = new List<DrawCommand>();

        /// <summary>
        /// Index buffer. Each command consume DrawCommand.ElemCount of those
        /// </summary>
        public IndexBuffer IndexBuffer { get; } = new IndexBuffer(128);

        /// <summary>
        /// Vertex buffer
        /// </summary>
        public VertexBuffer VertexBuffer { get; } = new VertexBuffer(128);

        /// <summary>
        /// Append a vertex to the VertexBuffer
        /// </summary>
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
            var vertexBuffer = mesh.VertexBuffer;
            var indexBuffer = mesh.IndexBuffer;
            var commandBuffer = mesh.CommandBuffer;

            if (indexBuffer.Count == 0 || vertexBuffer.Count == 0)
            {
                return;
            }

            foreach (var command in commandBuffer)
            {
                DrawCommand previousCommand = this.CommandBuffer[this.CommandBuffer.Count - 1];
                //TODO check if clip rect is the same
                if (command.TextureData != previousCommand.TextureData)
                {
                    this.CommandBuffer.Add(command);
                }
                else//same texture, only add element count to previous command
                {
                    previousCommand.ElemCount += indexBuffer.Count;
                    this.CommandBuffer[this.CommandBuffer.Count - 1] = previousCommand;//write back
                }

                var originalVertexCount = this.VertexBuffer.Count;

                int vtxBufferSize = this.VertexBuffer.Count;
                this.vtxWritePosition = vtxBufferSize + vertexBuffer.Count;
                this.VertexBuffer.Append(vertexBuffer);

                int idxBufferSize = this.IndexBuffer.Count;
                this.idxWritePosition = idxBufferSize + indexBuffer.Count;

                var sizeBefore = this.IndexBuffer.Count;
                this.IndexBuffer.Append(indexBuffer);
                var sizeAfter = this.IndexBuffer.Count;

                if (originalVertexCount != 0)
                {
                    for (int i = sizeBefore; i < sizeAfter; i++)
                    {
                        this.IndexBuffer[i] = new DrawIndex
                        {
                            Index = this.IndexBuffer[i].Index + originalVertexCount
                        };
                    }
                }

                this.currentIdx += vertexBuffer.Count;
            }
        }
    }
}