using Xunit;

namespace ImGui.UnitTest.DrawList
{
    public class VertexBufferFacts
    {
        public class TheAppendMethod
        {
            [Fact]
            public void AppendOneToAnother()
            {
                VertexBuffer buffer = new VertexBuffer(2);
                buffer.Resize(2);
                buffer[0] = new DrawVertex{pos = new Point(0, 0)};
                buffer[1] = new DrawVertex{pos = new Point(1, 0)};

                VertexBuffer bufferToAppend = new VertexBuffer(3);
                bufferToAppend.Resize(3);
                bufferToAppend[0] = new DrawVertex{pos = new Point(2, 0)};
                bufferToAppend[1] = new DrawVertex{pos = new Point(3, 0)};
                bufferToAppend[2] = new DrawVertex{pos = new Point(4, 0)};

                buffer.Append(bufferToAppend);

                Assert.Equal(5, buffer.Count);
                Assert.Equal(2, buffer[2].pos.x);
                Assert.Equal(3, buffer[3].pos.x);
                Assert.Equal(4, buffer[4].pos.x);
            }
        }
    }
}