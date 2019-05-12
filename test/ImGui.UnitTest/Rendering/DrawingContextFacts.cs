using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class DrawingContextFacts
    {
        [Fact]
        public void PopulateDrawingVisual()
        {
            void Populate(DrawingContext drawingContext)
            {
                drawingContext.DrawLine(new Pen(Color.Red, 4), new Point(10, 20), new Point(100, 60));
                drawingContext.DrawLine(new Pen(Color.Blue, 4), new Point(60, 100), new Point(20, 10));
                drawingContext.DrawRectangle(new Brush(Color.Aqua), new Pen(Color.Black, 3),
                    new Rect(new Point(30, 30), new Point(80, 80)));
            }

            //write records into visual's content
            var visual = new DrawingVisual(1);
            var context = visual.RenderOpen();
            Populate(context);
            context.Close();

            //write records into ContentChecker
            var checker = new ContentChecker();
            Populate(checker);

            //read records from visual to checker and compare
            checker.StartCheck();
            visual.RenderContent(new RenderContext(checker, null));
        }


        [Fact]
        public void PopulateNode()
        {
            void Populate(DrawingContext drawingContext)
            {
                var geometry = new PathGeometry();
                var figure = new PathFigure();
                figure.StartPoint = new Point(5, 90);
                figure.Segments.Add(new LineSegment(new Point(125,0),true));
                figure.Segments.Add(new LineSegment(new Point(245,90),true));
                figure.Segments.Add(new LineSegment(new Point(200,230),true));
                figure.Segments.Add(new LineSegment(new Point(52,230),true));
                figure.Segments.Add(new LineSegment(new Point(5,90),true));
                geometry.Figures.Add(figure);
                drawingContext.DrawGeometry(new Brush(Color.BlueViolet), new Pen(Color.Black, 4), geometry);
                drawingContext.DrawRectangle(new Brush(Color.YellowGreen), new Pen(Color.Cornsilk, 4), new Rect(100, 100, 50, 50));
            }

            var node = new Node(1, new Rect(10, 20, 300, 60));
            var context = node.RenderOpenStatic();
            Populate(context);
            context.Close();

            //write records into ContentChecker
            var checker = new ContentChecker();
            Populate(checker);

            //read records from visual to checker and compare
            checker.StartCheck();
            node.RenderContent(new RenderContext(checker, null));
        }
    }
}
