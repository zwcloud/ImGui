using System;
using ImGui.Common.Primitive;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class NodeFacts
    {
        public class TheLayoutMethod
        {
            [Fact]
            public void LayoutANodeWithTwoChildren() // Add rect; Add rect then remove rect
            {
                Node a = new Node();
                a.Id = 1;
                a.Rect = new Rect(0, 0, 300, 400);
                a.AttachLayoutGroup(true);
                
                Node b = new Node();
                b.Id = 2;
                b.Rect = new Rect(0, 0, 100, 100);
                b.AttachLayoutEntry(new Size(100, 100));
                
                Node c = new Node();
                c.Id = 3;
                c.Rect = new Rect(0, 0, 100, 200);
                c.AttachLayoutEntry(new Size(100, 200));

                a.Children.Add(b);
                a.Children.Add(c);

                a.Layout();

                DrawNode(a);
            }

            private void DrawNode(Node node, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
            {
                using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, (int)node.Rect.Width, (int)node.Rect.Height))
                using (Cairo.Context context = new Cairo.Context(surface))
                {
                    Draw(context, node);

                    string outputPath = "D:\\my\\ImGui.UnitTest.Output";
                    if (!System.IO.Directory.Exists(outputPath))
                    {
                        System.IO.Directory.CreateDirectory(outputPath);
                    }

                    string filePath = outputPath + "\\" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff_") + surface.GetHashCode() + memberName + ".png";
                    surface.WriteToPng(filePath);
                    Util.OpenImage(filePath);
                }
            }

            private static void Draw(Cairo.Context context, Node node)
            {
                foreach (var entry in node.Children)
                {
                    if (entry.HorizontallyStretched || entry.VerticallyStretched)
                    {
                        context.FillRectangle(entry.Rect, CairoEx.ColorLightBlue);
                    }
                    else if (entry.IsFixedWidth || entry.IsFixedHeight)
                    {
                        context.FillRectangle(entry.Rect, CairoEx.ColorOrange);
                    }
                    else
                    {
                        context.FillRectangle(entry.Rect, CairoEx.ColorPink);
                    }
                    context.StrokeRectangle(entry.Rect, CairoEx.ColorBlack);
                    var innerGroup = entry;
                    if (innerGroup.Children != null)
                    {
                        context.Save();
                        Draw(context, innerGroup);
                        context.Restore();
                    }
                }
            }
        }

    }
}
