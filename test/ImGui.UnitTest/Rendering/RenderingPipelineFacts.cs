using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class RenderingPipelineFacts
    {
        [Fact]
        public void RenderANode()
        {
            var node = new Node(1, new Rect(10, 20, 300, 60));
            var context = node.RenderOpenStatic();
            context.DrawLine(new Pen(Color.Red, 4), new Point(10, 20), new Point(100, 60));
            context.DrawLine(new Pen(Color.Blue, 4), new Point(60, 100), new Point(20, 10));
            context.DrawRectangle(new Brush(Color.Aqua), new Pen(Color.Black, 3),
                new Rect(new Point(30, 30), new Point(80, 80)));
            context.Close();

            ShowImage(node, 110, 110,
                $@"Rendering\images\{nameof(RenderingPipelineFacts)}\{nameof(RenderANode)}.png");
        }

        [Fact]
        public void RenderAStatedNode()
        {
            var node = new Node(1, new Rect(10, 20, 300, 60));
            node.RuleSet.StrokeColor = Color.Black;
            node.RuleSet.StrokeWidth = 4;
            node.RuleSet.FillColor = Color.Green;
            node.RuleSet.Set(GUIStyleName.FillColor, Color.Red, GUIState.Hover);

            {
                node.State = GUIState.Normal;
                var context = node.RenderOpen();
                context.DrawRectangle(new Rect(new Point(30, 30), new Point(80, 80)));
                context.Close();
                ShowImage(node, 110, 110,
                    $@"Rendering\images\{nameof(RenderingPipelineFacts)}\{nameof(RenderAStatedNode)}_Normal.png");
            }

            {
                node.State = GUIState.Hover;
                var context = node.RenderOpen();
                context.DrawRectangle(new Rect(new Point(30, 30), new Point(80, 80)));
                context.Close();
                ShowImage(node, 110, 110,
                    $@"Rendering\images\{nameof(RenderingPipelineFacts)}\{nameof(RenderAStatedNode)}_Hover.png");
            }

        }

        [Fact]
        public void RenderPathGeometry()
        {
            var node = new Node(1, new Rect(10, 20, 300, 60));

            var geometry = new PathGeometry();
            var figure = new PathFigure();
            figure.StartPoint = new Point(5, 90);
            figure.Segments.Add(new LineSegment(new Point(125,0),true));
            figure.Segments.Add(new LineSegment(new Point(245,90),true));
            figure.Segments.Add(new LineSegment(new Point(200,230),true));
            figure.Segments.Add(new LineSegment(new Point(52,230),true));
            figure.Segments.Add(new LineSegment(new Point(5,90),true));
            geometry.Figures.Add(figure);

            var context = node.RenderOpenStatic();
            context.DrawGeometry(new Brush(Color.BlueViolet), new Pen(Color.Black, 4), geometry);
            context.DrawRectangle(new Brush(Color.YellowGreen), new Pen(Color.Cornsilk, 4), new Rect(100, 100, 50, 50));
            context.Close();

            Util.DrawNodeToImage_NewPipeline(out var imageRawBytes, node, 300, 300);
            Util.CheckExpectedImage(imageRawBytes, 300, 300,
                $@"Rendering\images\{nameof(RenderingPipelineFacts)}\{nameof(RenderPathGeometry)}.png");
        }

        [Fact]
        public void RenderBoxModel()
        {
            var node = new Node(1, new Rect(20, 20, 200, 80));
            node.RuleSet.Border = (10, 20, 30, 40);
            node.RuleSet.BorderColor = (Color.Red, Color.DarkGreen, Color.DeepSkyBlue, Color.YellowGreen);

            var context = node.RenderOpen();
            context.DrawBoxModel();
            context.Close();

            ShowImage(node, 250, 250,
                $@"Rendering\images\{nameof(RenderingPipelineFacts)}\{nameof(RenderBoxModel)}.png");
        }

        [Fact]
        public void RenderRoundedRectangle()
        {
            var node = new Node(1, new Rect(20, 20, 200, 80));
            node.RuleSet.StrokeColor = Color.Black;
            node.RuleSet.StrokeWidth = 4;
            node.RuleSet.FillColor = Color.DeepSkyBlue;
            node.RuleSet.BorderRadius = (10, 10, 10, 10);

            var context = node.RenderOpen();
            context.DrawRoundedRectangle(new Rect(20, 20, 100, 60));
            context.Close();

            DrawAndCheck(node, 250, 250,
                $@"Rendering\images\{nameof(RenderingPipelineFacts)}\{nameof(RenderRoundedRectangle)}.png");
        }


        [Fact]
        public void RenderRoundBoxModel()
        {
            var node = new Node(1, new Rect(100, 100, 200, 200));
            node.RuleSet.Border = (top:20, right: 30, 0, left:40);
            node.RuleSet.BorderColor = (Color.Red, Color.DarkGreen, Color.DeepSkyBlue, Color.Black);
            node.RuleSet.BorderRadius = (TopLeft:50, 40, 0, 0);
            node.RuleSet.BackgroundColor = Color.AliceBlue;

            var context = node.RenderOpen();
            context.DrawBoxModel();
            context.Close();

            ShowImage(node, 300, 250,
                $@"Rendering\images\{nameof(RenderingPipelineFacts)}\{nameof(RenderRoundBoxModel)}.png");
        }

        private static void ShowImage(Node node, int width, int height, string path)
        {
            Util.DrawNodeToImage_NewPipeline(out var imageRawBytes, node, width, height);
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
            Util.SaveImage(image, path);
            Util.OpenImage(path);
        }

        private static void DrawAndCheck(Node node, int width, int height, string path)
        {
            Util.DrawNodeToImage_NewPipeline(out var imageRawBytes, node, width, height);
            Util.CheckExpectedImage(imageRawBytes, width, height, path);
        }
    }
}