using ImGui.Common.Primitive;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class PathPrimitiveFacts
    {
        public class PathMoveTo
        {
            [Fact]
            public void Works()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(Point.Zero);

                Assert.Equal(primitive.Path[0].Points[0], Point.Zero);
            }
        }

        public class PathLineTo
        {
            [Fact]
            public void Works()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(Point.Zero);
                primitive.PathLineTo(new Point(0, 10));
                primitive.PathLineTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 0));

                Assert.Equal(4, primitive.Path.Count);
                Assert.Equal(Point.Zero, primitive.Path[0].Points[0]);
                Assert.Equal(new Point(0, 10), primitive.Path[1].Points[0]);
                Assert.Equal(new Point(10, 10), primitive.Path[2].Points[0]);
                Assert.Equal(new Point(10, 0), primitive.Path[3].Points[0]);
            }
        }

        public class PathArcToFast
        {
            [Fact]
            public void Works()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(Point.Zero);
                primitive.PathArcToFast(new Point(10, 0), 10, 3, 6);

                Assert.Equal(Point.Zero, primitive.Path[0].Points[0]);

                var firstArcPoint = primitive.Path[1].Points[0];
                Assert.Equal(10, firstArcPoint.x, precision: 2);
                Assert.Equal(10, firstArcPoint.y, precision: 2);

                var lastPathData = primitive.Path[primitive.Path.Count - 1];
                var lastArcPoint = lastPathData.Points[lastPathData.Points.Length - 1];
                Assert.Equal(0, lastArcPoint.x, precision: 2);
                Assert.Equal(0, lastArcPoint.y, precision: 2);
            }

            [Fact]
            public void Draw()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(Point.Zero);
                primitive.PathArcToFast(new Point(10, 0), 10, 6, 9);

                Util.DrawPathPrimitive(primitive);

            }
        }
    }
}