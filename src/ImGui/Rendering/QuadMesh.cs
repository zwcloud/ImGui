namespace ImGui.Rendering
{
    class QuadMesh : Mesh
    {
        private Point p0 = new Point(-1, 1);
        private Point p1 = new Point(1, 1);
        private Point p2 = new Point(1, -1);
        private Point p3 = new Point(-1, -1);

        public QuadMesh()
        {
            this.CommandBuffer.Add(new DrawCommand { });
            this.PrimReserve(6, 4);

            var vertex0 = new DrawVertex { pos = p0, uv = new Point(0, 1), color = Color.White };
            var vertex1 = new DrawVertex { pos = p1, uv = new Point(1, 1), color = Color.White };
            var vertex2 = new DrawVertex { pos = p2, uv = new Point(1, 0), color = Color.White };
            var vertex3 = new DrawVertex { pos = p3, uv = new Point(0, 0), color = Color.White };
            this.AppendVertex(vertex0);
            this.AppendVertex(vertex1);
            this.AppendVertex(vertex2);
            this.AppendVertex(vertex3);

            this.AppendIndex(0);
            this.AppendIndex(1);
            this.AppendIndex(2);
            this.AppendIndex(0);
            this.AppendIndex(2);
            this.AppendIndex(3);

            this.currentIdx += 4;
        }
        
        public void SetOffset(Vector ndcOffset)
        {
            VertexBuffer.Data[0].pos = p0 + ndcOffset;
            VertexBuffer.Data[1].pos = p1 + ndcOffset;
            VertexBuffer.Data[2].pos = p2 + ndcOffset;
            VertexBuffer.Data[3].pos = p3 + ndcOffset;
        }

    }
}
