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
            var context = node.RenderOpen();
            context.DrawLine(new Pen(Color.Red, 4), new Point(10, 20), new Point(100, 60));
            context.Close();

            int width = 110, height = 110;
            Util.DrawNodeToImage_NewPipeline(out var imageRawBytes, node, width, height);
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
            var path = $@"Rendering\images\{nameof(RenderingPipelineFacts)}\{nameof(RenderANode)}.png";
            Util.SaveImage(image, path);
            Util.OpenImage(path);
        }
    }
}