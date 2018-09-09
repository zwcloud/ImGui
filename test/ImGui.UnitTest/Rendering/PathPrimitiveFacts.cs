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
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(Point.Zero);
                primitive.PathLineTo(new Point(0, 10));
                primitive.PathLineTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 0));

                Assert.Equal(4, primitive.Path.Count);
                Assert.IsType<LineToCommand>(primitive.Path[0]);

                {
                    var cmd = (LineToCommand)primitive.Path[0];
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
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(Point.Zero);
                primitive.PathArcToFast(new Point(10, 0), 10, 3, 6);

                {
                    var cmd1 = (LineToCommand)primitive.Path[1];
                    var firstArcPoint = cmd1.Point;
                    Assert.Equal(10, firstArcPoint.x, precision: 2);
                    Assert.Equal(10, firstArcPoint.y, precision: 2);
                }

                {
                    var lastCmd = (LineToCommand)primitive.Path[primitive.Path.Count - 1];
                    var lastArcPoint = lastCmd.Point;
                    Assert.Equal(0, lastArcPoint.x, precision: 2);
                    Assert.Equal(0, lastArcPoint.y, precision: 2);
                }
            }

            [Fact]
            public void Draw()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(Point.Zero);
                primitive.PathArcToFast(new Point(10, 0), 10, 6, 9);
                primitive.PathStroke(1, Color.Black);

                Util.DrawPathPrimitive(primitive);
            }
        }
    }
}