using System.Collections.Generic;

namespace ImGui
{
    /// <summary>
    // This is what you have to render
    /// </summary>
    partial class DrawList
    {
        ImGui.Internal.List<DrawCommand> commandBuffer = new ImGui.Internal.List<DrawCommand>();
        ImGui.Internal.List<DrawIndex> indexBuffer = new ImGui.Internal.List<DrawIndex>();
        ImGui.Internal.List<DrawVertex> vertexBuffer = new ImGui.Internal.List<DrawVertex>();

        ImGui.Internal.List<DrawCommand> bezierCommandBuffer = new ImGui.Internal.List<DrawCommand>();
        ImGui.Internal.List<DrawIndex> bezierIndexBuffer = new ImGui.Internal.List<DrawIndex>();
        ImGui.Internal.List<DrawVertex> bezierVertexBuffer = new ImGui.Internal.List<DrawVertex>();

        /// <summary>
        /// Commands. Typically 1 command = 1 gpu draw call.
        /// </summary>
        public ImGui.Internal.List<DrawCommand> CommandBuffer
        {
            get { return commandBuffer; }
            set { commandBuffer = value; }
        }

        /// <summary>
        /// Index buffer. Each command consume DrawCommand.ElemCount of those
        /// </summary>
        public ImGui.Internal.List<DrawIndex> IndexBuffer
        {
            get { return indexBuffer; }
            set { indexBuffer = value; }
        }

        /// <summary>
        /// Vertex buffer
        /// </summary>
        public ImGui.Internal.List<DrawVertex> VertexBuffer
        {
            get { return vertexBuffer; }
            set { vertexBuffer = value; }
        }

        /// <summary>
        /// Command buffer for bezier curves
        /// </summary>
        public ImGui.Internal.List<DrawCommand> BezierCommandBuffer
        {
            get { return bezierCommandBuffer; }
            set { bezierCommandBuffer = value; }
        }

        /// <summary>
        /// Index buffer for bezier curves
        /// </summary>
        public ImGui.Internal.List<DrawIndex> BezierIndexBuffer
        {
            get { return bezierIndexBuffer; }
            set { bezierIndexBuffer = value; }
        }

        /// <summary>
        /// Vertex buffer for beziers curves
        /// </summary>
        public ImGui.Internal.List<DrawVertex> BezierVertexBuffer
        {
            get { return bezierVertexBuffer; }
        }

        public void Clear()
        {
            // triangles
            this.CommandBuffer.Clear();
            this.IndexBuffer.Clear();
            this.VertexBuffer.Clear();

            _vtxWritePosition = 0;
            _idxWritePosition = 0;
            _currentIdx = 0;

            _Path.Clear();

            // beziers
            this.BezierCommandBuffer.Clear();
            this.BezierIndexBuffer.Clear();
            this.BezierVertexBuffer.Clear();
            
            _bezier_vtxWritePosition = 0;
            _bezier_idxWritePosition = 0;
            _bezier_currentIdx = 0;

            _BezierControlPointIndex.Clear();
        }
    }
}
