using Xunit;

namespace ImGui.UnitTest.DrawList
{
    public class TextMeshFacts
    {
        public class TheAppendMethod
        {
            [Fact]
            public void AppendOneToAnother()
            {
                const int elementCount0 = 3;
                const int elementCount1 = 6;

                var mesh = new TextMesh();
                mesh.Commands.Add(new DrawCommand { ElemCount = elementCount0 });
                mesh.PrimReserve(3, 3);
                mesh.AppendIndex(0);
                mesh.AppendIndex(1);
                mesh.AppendIndex(2);
                mesh.AppendVertex(new DrawVertex{pos=new Point(0,0)});
                mesh.AppendVertex(new DrawVertex{pos=new Point(1,0)});
                mesh.AppendVertex(new DrawVertex{pos=new Point(2,0)});

                var meshToAppend = new TextMesh();
                meshToAppend.Commands.Add(new DrawCommand { ElemCount = elementCount1 });
                meshToAppend.PrimReserve(6, 3);
                meshToAppend.AppendIndex(0);
                meshToAppend.AppendIndex(1);
                meshToAppend.AppendIndex(2);
                meshToAppend.AppendIndex(1);
                meshToAppend.AppendIndex(2);
                meshToAppend.AppendIndex(0);
                meshToAppend.AppendVertex(new DrawVertex{pos=new Point(3,0)});
                meshToAppend.AppendVertex(new DrawVertex{pos=new Point(4,0)});
                meshToAppend.AppendVertex(new DrawVertex{pos=new Point(5,0)});

                mesh.Append(meshToAppend, Vector.Zero);

                Assert.Single(mesh.Commands);
                Assert.Equal(elementCount0 + elementCount1, mesh.Commands[0].ElemCount);

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
        }
    }
}