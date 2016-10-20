using System.Collections.Generic;

namespace ImGui
{
    /// <summary>
    // This is what you have to render
    /// </summary>
    partial class DrawList
    {
        List<DrawCommand> commandBuffer;
        List<DrawIndex> indexBuffer;
        List<DrawVertex> vertexBuffer;

        /// <summary>
        /// Commands. Typically 1 command = 1 gpu draw call.
        /// </summary>
        public List<DrawCommand> CommandBuffer
        {
            get { return commandBuffer; }
            set { commandBuffer = value; }
        }

        /// <summary>
        /// Index buffer. Each command consume DrawCommand.ElemCount of those
        /// </summary>
        public List<DrawIndex> IndexBuffer
        {
            get { return indexBuffer; }
            set { indexBuffer = value; }
        }

        /// <summary>
        /// Vertex buffer
        /// </summary>
        public List<DrawVertex> VertexBuffer
        {
            get { return vertexBuffer; }
            set { vertexBuffer = value; }
        }
    }
}
