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

        private static void ShowImage(Node node, int width, int height, string path)
        {
            Util.DrawNodeToImage_NewPipeline(out var imageRawBytes, node, width, height);
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
            Util.SaveImage(image, path);
            Util.OpenImage(path);
        }
    }
}