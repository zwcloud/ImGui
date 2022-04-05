namespace ImGui.Rendering
{
    class QuadMesh : Mesh
    {
        public QuadMesh()
        {
            this.CommandBuffer.Add(new DrawCommand { });
            this.PrimReserve(6, 4);

            var vertex0 = new DrawVertex { pos = new Point(-1, 1), uv = new Point(0, 1), color = Color.White };
            var vertex1 = new DrawVertex { pos = new Point(1, 1), uv = new Point(1, 1), color = Color.White };
            var vertex2 = new DrawVertex { pos = new Point(1, -1), uv = new Point(1, 0), color = Color.White };
            var vertex3 = new DrawVertex { pos = new Point(-1, -1), uv = new Point(0, 0), color = Color.White };
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ndcRectNormalized">new rect in ndc space in [0, 1]</param>
        public void Resize(Rect ndcRectNormalized)
        {
            var r = ndcRectNormalized;
            VertexBuffer.Data[0].uv = new Point(r.X, r.Y + r.Height);
            VertexBuffer.Data[1].uv = new Point(r.X+r.Width, r.Y + r.Height);
            VertexBuffer.Data[2].uv = new Point(r.X+r.Width, r.Y);
            VertexBuffer.Data[3].uv = new Point(r.X, r.Y);
        }

    }
}
