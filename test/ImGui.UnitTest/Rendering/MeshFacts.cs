using Xunit;

namespace ImGui.UnitTest.DrawList
{
    public class MeshFacts
    {
        public class TheAppendMethod
        {
            [Fact]
            public void AppendOneToAnother()
            {
                var mesh = new Mesh();
                mesh.CommandBuffer.Add(DrawCommand.Default);
                mesh.PrimReserve(3, 3);
                mesh.AppendIndex(0);
                mesh.AppendIndex(1);
                mesh.AppendIndex(2);
                mesh.AppendVertex(new DrawVertex { pos = new Point(0, 0) });
                mesh.AppendVertex(new DrawVertex { pos = new Point(1, 0) });
                mesh.AppendVertex(new DrawVertex { pos = new Point(2, 0) });

                var meshToAppend = new Mesh();
                meshToAppend.CommandBuffer.Add(DrawCommand.Default);
                meshToAppend.PrimReserve(6, 3);
                meshToAppend.AppendIndex(0);
                meshToAppend.AppendIndex(1);
                meshToAppend.AppendIndex(2);
                meshToAppend.AppendIndex(1);
                meshToAppend.AppendIndex(2);
                meshToAppend.AppendIndex(0);
                meshToAppend.AppendVertex(new DrawVertex { pos = new Point(3, 0) });
                meshToAppend.AppendVertex(new DrawVertex { pos = new Point(4, 0) });
                meshToAppend.AppendVertex(new DrawVertex { pos = new Point(5, 0) });

                mesh.Append(meshToAppend);

                Assert.Equal(6, mesh.VertexBuffer.Count);
                Assert.Equal(9, mesh.IndexBuffer.Count);

                Assert.Equal(3, mesh.IndexBuffer[3]);
                Assert.Equal(4, mesh.IndexBuffer[4]);
                Assert.Equal(5, mesh.IndexBuffer[5]);
                Assert.Equal(4, mesh.IndexBuffer[6]);
                Assert.Equal(5, mesh.IndexBuffer[7]);
                Assert.Equal(3, mesh.IndexBuffer[8]);

                Assert.Equal(0, mesh.VertexBuffer[0].pos.x);
                Assert.Equal(1, mesh.VertexBuffer[1].pos.x);
                Assert.Equal(2, mesh.VertexBuffer[2].pos.x);
                Assert.Equal(3, mesh.VertexBuffer[3].pos.x);
                Assert.Equal(4, mesh.VertexBuffer[4].pos.x);
                Assert.Equal(5, mesh.VertexBuffer[5].pos.x);
            }

            [Fact]
            public void AppendTexturedMesh()
            {
                var mesh = new Mesh();
                mesh.CommandBuffer.Add(DrawCommand.Default);
                mesh.PrimReserve(3, 3);
                mesh.AppendIndex(0);
                mesh.AppendIndex(1);
                mesh.AppendIndex(2);
                mesh.AppendVertex(new DrawVertex { pos = new Point(0, 0) });
                mesh.AppendVertex(new DrawVertex { pos = new Point(1, 0) });
                mesh.AppendVertex(new DrawVertex { pos = new Point(2, 0) });

                var meshToAppend = new Mesh();

                DrawCommand cmd = new DrawCommand();
                cmd.ClipRect = Rect.Big;
                var dummyTexture = new FakeTexture();
                cmd.TextureData = dummyTexture;//dummy, only the reference is needed
                meshToAppend.CommandBuffer.Add(cmd);

                meshToAppend.PrimReserve(6, 4);
                Point a = new Point(10, 10);
                Point c = new Point(300, 300);
                Point b = new Point(c.X, a.Y);
                Point d = new Point(a.X, c.Y);
                Point uvA = new Point(0, 0);
                Point uvC = new Point(1, 1);
                Point uvB = new Point(uvC.X, uvA.Y);
                Point uvD = new Point(uvA.X, uvC.Y);

                meshToAppend.AppendVertex(new DrawVertex { pos = a, uv = uvA, color = Color.AliceBlue });
                meshToAppend.AppendVertex(new DrawVertex { pos = b, uv = uvB, color = Color.AliceBlue });
                meshToAppend.AppendVertex(new DrawVertex { pos = c, uv = uvC, color = Color.AliceBlue });
                meshToAppend.AppendVertex(new DrawVertex { pos = d, uv = uvD, color = Color.AliceBlue });
                meshToAppend.AppendIndex(0);
                meshToAppend.AppendIndex(1);
                meshToAppend.AppendIndex(2);
                meshToAppend.AppendIndex(0);
                meshToAppend.AppendIndex(2);
                meshToAppend.AppendIndex(3);
                meshToAppend.currentIdx += 4;

                mesh.Append(meshToAppend);

                Assert.Equal(dummyTexture, mesh.CommandBuffer[mesh.CommandBuffer.Count - 1].TextureData);

                Assert.Equal(7, mesh.VertexBuffer.Count);
                Assert.Equal(9, mesh.IndexBuffer.Count);

                Assert.Equal(0, mesh.IndexBuffer[0]);
                Assert.Equal(1, mesh.IndexBuffer[1]);
                Assert.Equal(2, mesh.IndexBuffer[2]);
                Assert.Equal(3, mesh.IndexBuffer[3]);
                Assert.Equal(4, mesh.IndexBuffer[4]);
                Assert.Equal(5, mesh.IndexBuffer[5]);
                Assert.Equal(3, mesh.IndexBuffer[6]);
                Assert.Equal(5, mesh.IndexBuffer[7]);
                Assert.Equal(6, mesh.IndexBuffer[8]);

                Assert.Equal(0, mesh.VertexBuffer[0].pos.x);
                Assert.Equal(1, mesh.VertexBuffer[1].pos.x);
                Assert.Equal(2, mesh.VertexBuffer[2].pos.x);
                Assert.Equal(10, mesh.VertexBuffer[3].pos.x);
                Assert.Equal(300, mesh.VertexBuffer[4].pos.x);
                Assert.Equal(300, mesh.VertexBuffer[5].pos.x);
                Assert.Equal(10, mesh.VertexBuffer[6].pos.x);
            }
        }
    }
}