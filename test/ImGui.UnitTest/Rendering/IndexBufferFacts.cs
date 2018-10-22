using Xunit;

namespace ImGui.UnitTest.DrawList
{
    public class IndexBufferFacts
    {
        public class TheAppendMethod
        {
            [Fact]
            public void AppendOneToAnother()
            {
                IndexBuffer buffer = new IndexBuffer(2);
                buffer.Resize(2);
                buffer[0] = new DrawIndex{Index = 0};
                buffer[1] = new DrawIndex{Index = 1};

                IndexBuffer bufferToAppend = new IndexBuffer(3);
                bufferToAppend.Resize(3);
                bufferToAppend[0] = new DrawIndex{Index = 2};
                bufferToAppend[1] = new DrawIndex{Index = 3};
                bufferToAppend[2] = new DrawIndex{Index = 4};

                buffer.Append(bufferToAppend);

                Assert.Equal(5, buffer.Count);
                Assert.Equal(2, buffer[2].Index);
                Assert.Equal(3, buffer[3].Index);
                Assert.Equal(4, buffer[4].Index);
            }
        }
    }
}