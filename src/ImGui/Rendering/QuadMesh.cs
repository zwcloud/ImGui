using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui.Rendering
{
    class QuadMesh : Mesh
    {
        double Width
        {
            get
            {
                return this.VertexBuffer[1].pos.X;
            }
            set
            {
                var A = this.VertexBuffer[1];
                A.pos.X = value;
                this.VertexBuffer[1] = A;

                var B = this.VertexBuffer[2];
                B.pos.X = value;
                this.VertexBuffer[2] = B;
            }
        }

        double Height
        {
            get
            {
                return this.VertexBuffer[3].pos.Y;
            }
            set
            {
                var B = this.VertexBuffer[2];
                B.pos.Y = value;
                this.VertexBuffer[2] = B;

                var C = this.VertexBuffer[3];
                C.pos.Y = value;
                this.VertexBuffer[3] = C;
            }
        }

        public QuadMesh(double width, double height)
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


    }
}
