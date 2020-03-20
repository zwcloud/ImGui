using Xunit;
using ImGui.Rendering;

namespace ImGui.UnitTest.Rendering
{
    public class PathGeometryBuilderFacts
    {
        public class ToGeometry
        {
            [Fact]
            public void ExportEmptyGeometry()
            {
                {
                    PathGeometryBuilder g = new PathGeometryBuilder();

                    g.BeginPath();
                    var geometry = g.ToGeometry();

                    var pathGeometry = geometry as PathGeometry;
                    Assert.NotNull(pathGeometry);
                    Assert.Empty(pathGeometry.Figures);
                }
                {
                    PathGeometryBuilder g = new PathGeometryBuilder();

                    g.BeginPath();
                    g.ClosePath();
                    var geometry = g.ToGeometry();

                    var pathGeometry = geometry as PathGeometry;
                    Assert.NotNull(pathGeometry);
                    Assert.Empty(pathGeometry.Figures);
                }
                {
                    PathGeometryBuilder g = new PathGeometryBuilder();

                    g.BeginPath();
                    g.ClosePath();
                    g.Stroke();
                    var geometry = g.ToGeometry();

                    var pathGeometry = geometry as PathGeometry;
                    Assert.NotNull(pathGeometry);
                    Assert.Single(pathGeometry.Figures);
                    Assert.Empty(pathGeometry.Figures[0].Segments);
                }
            }

            [Fact]
            public void ExportGeometryCase1()
            {
                PathGeometryBuilder g = new PathGeometryBuilder();

                g.BeginPath();
                g.MoveTo(50, 140);
                g.LineTo(150, 60);
                g.LineTo(250, 140);
                g.ClosePath();
                g.Stroke();
                var geometry = g.ToGeometry();

                var pathGeometry = geometry as PathGeometry;
                Assert.NotNull(pathGeometry);
                Assert.Single(pathGeometry.Figures);

                var figure = pathGeometry.Figures[0];
                Assert.NotNull(figure);
                Assert.Equal(figure.StartPoint, new Point(50, 140));
                Assert.True(figure.IsClosed);
                Assert.False(figure.IsFilled);

                var segments = figure.Segments;
                Assert.Equal(3, segments.Count);

                Assert.IsType<LineSegment>(segments[0]);
                var segment0 = segments[0] as LineSegment;
                Assert.True(segment0.IsStroked);
                Assert.Equal(segment0.Point, new Point(150, 60));

                Assert.IsType<LineSegment>(segments[1]);
                var segment1 = segments[1] as LineSegment;
                Assert.True(segment1.IsStroked);
                Assert.Equal(segment1.Point, new Point(250, 140));

                Assert.IsType<LineSegment>(segments[2]);
                var segment2 = segments[2] as LineSegment;
                Assert.True(segment2.IsStroked);
                Assert.Equal(segment2.Point, new Point(50, 140));
            }

            [Fact]
            public void ExportGeometryCase2()
            {
                PathGeometryBuilder g = new PathGeometryBuilder();

                g.BeginPath();
                g.Rect(10, 10, 100, 200, false);
                g.Stroke();
                var geometry = g.ToGeometry();

                var pathGeometry = geometry as PathGeometry;
                Assert.NotNull(pathGeometry);
                Assert.Single(pathGeometry.Figures);

                var figure = pathGeometry.Figures[0];
                Assert.NotNull(figure);
                Assert.Equal(figure.StartPoint, new Point(10, 10));
                Assert.False(figure.IsClosed);
                Assert.False(figure.IsFilled);

                var segments = figure.Segments;
                Assert.Single(segments);

                Assert.IsType<PolyLineSegment>(segments[0]);
                var segment = segments[0] as PolyLineSegment;
                Assert.True(segment.IsStroked);
                Assert.Equal(segment.Points, new[] {
                    new Point(110, 10), new Point(110, 210),
                    new Point(10, 210), new Point(10, 10) });
            }

            [Fact]
            public void ExportGeometryMultipleFigures()
            {
                PathGeometryBuilder g = new PathGeometryBuilder();

                g.BeginPath();
                g.MoveTo(50, 140);
                g.LineTo(150, 60);
                g.LineTo(250, 140);
                g.ClosePath();
                g.Stroke();

                g.BeginPath();
                g.Rect(10, 10, 100, 200, false);
                g.Stroke();

                var geometry = g.ToGeometry();

                var pathGeometry = geometry as PathGeometry;
                Assert.NotNull(pathGeometry);
                Assert.Equal(2, pathGeometry.Figures.Count);
                Assert.NotNull(pathGeometry.Figures[0]);
                Assert.NotNull(pathGeometry.Figures[1]);
            }
        }
    }
}