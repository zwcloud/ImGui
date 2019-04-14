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
                var primitive = new PathGeometry();
                primitive.PathMoveTo(Point.Zero);

                Assert.Single(primitive.Path);
                Assert.IsType<MoveToCommand>(primitive.Path[0]);
                var cmd = (MoveToCommand) primitive.Path[0];
                Assert.Equal(Point.Zero, cmd.Point);
            }
        }

        public class PathLineTo
        {
            [Fact]
            public void Works()
            {
                var primitive = new PathGeometry();
                primitive.PathMoveTo(Point.Zero);
                primitive.PathLineTo(new Point(0, 10));
                primitive.PathLineTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 0));

                Assert.Equal(4, primitive.Path.Count);
                {
                    Assert.IsType<MoveToCommand>(primitive.Path[0]);
                    var cmd = (MoveToCommand)primitive.Path[0];
                    Assert.Equal(Point.Zero, cmd.Point);
                }
                {
                    var cmd = (LineToCommand)primitive.Path[1];
                    Assert.Equal(new Point(0, 10), cmd.Point);
                }
                {
                    var cmd = (LineToCommand)primitive.Path[2];
                    Assert.Equal(new Point(10, 10), cmd.Point);
                }
                {
                    var cmd = (LineToCommand)primitive.Path[3];
                    Assert.Equal(new Point(10, 0), cmd.Point);
                }
            }
        }

        public class PathArcToFast
        {
            [Fact]
            public void Works()
            {
                var primitive = new PathGeometry();
                primitive.PathMoveTo(Point.Zero);
                primitive.PathArcFast(new Point(10, 0), 10, 3, 6);

                var cmd = (ArcCommand) primitive.Path[1];
                var center = cmd.Center;
                var amin = cmd.Amin;
                var amax = cmd.Amax;
                Assert.Equal(10, center.x, precision: 2);
                Assert.Equal(0, center.y, precision: 2);
                Assert.Equal(3, amin);
                Assert.Equal(6, amax);
            }
        }
    }
}