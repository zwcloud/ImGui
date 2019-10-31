using ImGui.OSAbstraction.Text;
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
        public void PopulateDrawingVisualWithGlyphRun()
        {
            void Populate(DrawingContext drawingContext)
            {
                drawingContext.DrawGlyphRun(new Brush(Color.Black),
                    new GlyphRun(Point.Zero, "啊rABC", GUIStyle.Default.FontFamily, 24));
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
        public void PopulateDrawingVisualWithFormattedText()
        {
            void Populate(DrawingContext drawingContext)
            {
                drawingContext.DrawText(new Brush(Color.Black),
                    new FormattedText(Point.Zero, "啊rABC", GUIStyle.Default.FontFamily, 24));
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
            var geometry = new PathGeometry();
            var figure = new PathFigure();
            figure.StartPoint = new Point(5, 90);
            figure.Segments.Add(new LineSegment(new Point(125,0),true));
            figure.Segments.Add(new LineSegment(new Point(245,90),true));
            figure.Segments.Add(new LineSegment(new Point(200,230),true));
            figure.Segments.Add(new LineSegment(new Point(52,230),true));
            figure.Segments.Add(new LineSegment(new Point(5,90),true));
            geometry.Figures.Add(figure);
            GlyphRun glyphRun = new GlyphRun(Point.Zero, "123", GUIStyle.Default.FontFamily, 20);

            void Populate(DrawingContext drawingContext)
            {
                drawingContext.DrawGeometry(new Brush(Color.BlueViolet), new Pen(Color.Black, 4), geometry);
                drawingContext.DrawRectangle(new Brush(Color.YellowGreen), new Pen(Color.Cornsilk, 4), new Rect(100, 100, 50, 50));
                drawingContext.DrawGlyphRun(new Brush(Color.Red), glyphRun);
            }

            var node = new Node(1, new Rect(10, 20, 300, 60));
            var context = node.RenderOpen();
            Populate(context);
            context.Close();

            //write records into ContentChecker
            var checker = new ContentChecker();
            Populate(checker);

            //read records from visual to checker and compare
            checker.StartCheck();
            node.RenderContent(new RenderContext(checker, null));
        }

        [Fact]
        public void PopulateNodeWithStyle()
        {
            void Populate(DrawingContext drawingContext, Node n)
            {
                drawingContext.DrawRectangle(n.RuleSet, new Rect(new Point(30, 30), new Point(80, 80)));
            }

            var node = new Node(1, new Rect(10, 20, 300, 60));
            node.RuleSet.StrokeColor = Color.Black;
            node.RuleSet.StrokeWidth = 4;
            node.RuleSet.FillColor = Color.Green;
            node.RuleSet.Set(StylePropertyName.FillColor, Color.Red, GUIState.Hover);

            {
                node.State = GUIState.Normal;
                var context = node.RenderOpen();
                Populate(context, node);
                context.Close();

                //write records into ContentChecker
                var checker = new ContentChecker();
                Populate(checker, node);

                //read records from visual to checker and compare
                checker.StartCheck();
                node.RenderContent(new RenderContext(checker, null));
            }

            {
                node.State = GUIState.Hover;
                var context = node.RenderOpen();
                Populate(context, node);
                context.Close();

                //write records into ContentChecker
                var checker = new ContentChecker();
                Populate(checker, node);

                //read records from visual to checker and compare
                checker.StartCheck();
                node.RenderContent(new RenderContext(checker, null));
            }

        }
    }
}
