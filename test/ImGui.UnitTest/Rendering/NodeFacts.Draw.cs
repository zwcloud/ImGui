using System;
using ImGui.GraphicsImplementation;
using ImGui.Input;
using ImGui.OSImplentation.Windows;
using ImGui.Rendering;
using System.Collections.Generic;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public partial class NodeFacts
    {
        public class Draw : IClassFixture<ApplicationFixture>
        {
            [Fact]
            public void DrawANode()
            {
                var primitive = new PathGeometry();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 100));
                primitive.PathLineTo(new Point(100, 100));
                primitive.PathLineTo(new Point(100, 10));
                primitive.PathClose();
                primitive.PathFill(Color.Black);

                Node node = new Node(1);
                node.Geometry = primitive;

                Util.DrawNodeToImage(out var imageRawBytes, node, 110, 110);
                Util.CheckExpectedImage(imageRawBytes, 110, 110, @"Rendering\images\NodeFacts.Draw.DrawANode.png");
            }

            [Fact]
            public void UpdateANode()
            {
                var primitive = new PathGeometry();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 100));
                primitive.PathLineTo(new Point(100, 100));
                primitive.PathLineTo(new Point(100, 10));
                primitive.PathClose();
                var fillCmd = primitive.PathFill(Color.Black);

                Node node = new Node(1);
                node.Geometry = primitive;

                {
                    fillCmd.Color = Color.Red;
                    Util.DrawNodeToImage(out var imageRawBytes, node, 110, 110);
                    Util.CheckExpectedImage(imageRawBytes, 110, 110, @"Rendering\images\NodeFacts.Draw.UpdateANode.Black.png");
                }
                {
                    fillCmd.Color = Color.Green;
                    Util.DrawNodeToImage(out var imageRawBytes, node, 110, 110);
                    Util.CheckExpectedImage(imageRawBytes, 110, 110, @"Rendering\images\NodeFacts.Draw.UpdateANode.Green.png");
                }
                {
                    fillCmd.Color = Color.Blue;
                    Util.DrawNodeToImage(out var imageRawBytes, node, 110, 110);
                    Util.CheckExpectedImage(imageRawBytes, 110, 110, @"Rendering\images\NodeFacts.Draw.UpdateANode.Blue.png");
                }

            }

            [Fact]
            public void UpdateATextNode()
            {
                Node node = new Node(1);
                var primitive = new TextGeometry("Before");
                node.Geometry = primitive;

                {
                    primitive.Text = "Before";
                    Util.DrawNodeToImage(out var imageRawBytes, node, 100, 30);
                    Util.CheckExpectedImage(imageRawBytes, 100, 30, @"Rendering\images\NodeFacts.Draw.UpdateATextNode.Before.png");
                }

                {
                    primitive.Text = "After";
                    Util.DrawNodeToImage(out var imageRawBytes, node, 100, 30);
                    Util.CheckExpectedImage(imageRawBytes, 100, 30, @"Rendering\images\NodeFacts.Draw.UpdateATextNode.After.png");
                }
            }

            [Fact]
            public void UpdateTwoNode()
            {
                var nodes = new List<Node>();
                FillCommand node0FillCmd, node1FillCmd;
                {
                    Node node = new Node(0);
                    nodes.Add(node);
                    var primitive = new PathGeometry();
                    primitive.PathMoveTo(new Point(10, 10));
                    primitive.PathLineTo(new Point(10, 100));
                    primitive.PathLineTo(new Point(100, 100));
                    primitive.PathLineTo(new Point(100, 10));
                    primitive.PathClose();
                    node0FillCmd = primitive.PathFill(Color.Green);
                    node.Geometry = primitive;
                }
                {
                    Node node = new Node(1);
                    nodes.Add(node);
                    var primitive = new PathGeometry();
                    primitive.PathMoveTo(new Point(110, 10));
                    primitive.PathLineTo(new Point(110, 100));
                    primitive.PathLineTo(new Point(200, 100));
                    primitive.PathLineTo(new Point(200, 10));
                    primitive.PathClose();
                    node1FillCmd = primitive.PathFill(Color.Orange);

                    node.Geometry = primitive;
                }

                {
                    node0FillCmd.Color = Color.Red;
                    node1FillCmd.Color = Color.Blue;
                    Util.DrawNodesToImage(out var imageRawBytes, nodes, 110, 110);
                    Util.CheckExpectedImage(imageRawBytes, 110, 110, @"Rendering\images\NodeFacts.Draw.UpdateTwoNode.Before.png");
                }

                {
                    node0FillCmd.Color = Color.Green;
                    node1FillCmd.Color = Color.Orange;
                    Util.DrawNodesToImage(out var imageRawBytes, nodes, 110, 110);
                    Util.CheckExpectedImage(imageRawBytes, 110, 110, @"Rendering\images\NodeFacts.Draw.UpdateTwoNode.After.png");
                }
            }

            [Fact]
            public void ShowHideANode()
            {
                Node node = new Node(1);
                var primitive = new PathGeometry();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 100));
                primitive.PathLineTo(new Point(100, 100));
                primitive.PathLineTo(new Point(100, 10));
                primitive.PathClose();
                primitive.PathFill(Color.Red);
                node.Geometry = primitive;

                {
                    node.ActiveSelf = true;
                    Util.DrawNodeToImage(out var imageRawBytes, node, 110, 110);
                    Util.CheckExpectedImage(imageRawBytes, 110, 110, @"Rendering\images\NodeFacts.Draw.ShowHideANode.Show.png");
                }

                {
                    node.ActiveSelf = false;
                    Util.DrawNodeToImage(out var imageRawBytes, node, 110, 110);
                    Util.CheckExpectedImage(imageRawBytes, 110, 110, @"Rendering\images\NodeFacts.Draw.ShowHideANode.Hide.png");
                }
            }

            [Fact]
            public void ShowAnimateNode()
            {
                //FIXME make this test automatable

                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();

                var primitiveRenderer = new BuiltinGeometryRenderer();

                Node node = new Node(1);
                node.Geometry = new PathGeometry();

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(800, 600), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                window.Show();

                var A = new Point(200, 200);
                var B = new Point(600, 200);
                var C = new Point(400, 400);

                while (true)
                {
                    Time.OnFrameBegin();
                    Keyboard.Instance.OnFrameBegin();

                    window.MainLoop(() =>
                    {
                        if (Keyboard.Instance.KeyDown(Key.Escape))
                        {
                            Application.Quit();
                        }

                        var normal = (Time.time % 1000) / 1000f * 2 - 1;
                        var rad = normal * Math.PI;
                        var A_ = A + 50 * new Vector(Math.Cos(rad) - Math.Sin(rad), Math.Sin(rad) + Math.Cos(rad));
                        rad += Math.PI * 0.333;
                        var B_ = B + 30 * new Vector(Math.Cos(rad) - Math.Sin(rad), Math.Sin(rad) + Math.Cos(rad));
                        rad += Math.PI * 0.666;
                        var C_ = C + 70 * new Vector(Math.Cos(rad) - Math.Sin(rad), Math.Sin(rad) + Math.Cos(rad));

                        var d = node.Geometry as PathGeometry;
                        d.PathClear();
                        d.PathMoveTo(A_);
                        d.PathLineTo(B_);
                        d.PathLineTo(C_);
                        d.PathStroke(2, Color.Blue);

                        //update nodes
                        if (node.ActiveInTree)//this is actually always true
                        {
                            node.Draw(primitiveRenderer, meshList);
                        }

                        //rebuild mesh buffer
                        meshBuffer.Clear();
                        meshBuffer.Init();
                        meshBuffer.Build(meshList);

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height,
                            (shapeMesh: meshBuffer.ShapeMesh, imageMesh: meshBuffer.ImageMesh, meshBuffer.TextMesh));
                        renderer.SwapBuffers();
                    });

                    if (Application.RequestQuit)
                    {
                        break;
                    }

                    Keyboard.Instance.OnFrameEnd();
                    Time.OnFrameEnd();
                }
            }

            [Fact]
            public void DrawOneTextNode()
            {
                Node node = new Node(1);
                var primitive = new TextGeometry("ImGUI立即");
                node.Geometry = primitive;
                node.Rect.X = 1;
                node.Rect.Y = 1;

                Util.DrawNodeToImage(out var imageRawBytes, node, 100, 30);
                Util.CheckExpectedImage(imageRawBytes, 100, 30, @"Rendering\images\NodeFacts.Draw.DrawOneTextNode.png");
            }

            [Fact]
            public void DrawTwoTextNode()
            {
                var nodes = new List<Node>();
                {
                    Node node = new Node(1);
                    nodes.Add(node);
                    var primitive = new TextGeometry("AAA");
                    node.Geometry = primitive;
                    node.Rect.X = 1;
                    node.Rect.Y = 1;
                }
                {
                    Node node = new Node(1);
                    nodes.Add(node);
                    var primitive = new TextGeometry("B");
                    node.Geometry = primitive;
                    node.Rect.X = 1;
                    node.Rect.Y = 40;
                }

                Util.DrawNodesToImage(out var imageRawBytes, nodes, 100, 70);
                Util.CheckExpectedImage(imageRawBytes, 100, 70, @"Rendering\images\NodeFacts.Draw.DrawTwoTextNode.png");
            }

            [Fact]
            public void DrawOneTextNodeAtPosition()
            {
                Node node = new Node(1);
                var primitive = new TextGeometry("AAA");
                node.Geometry = primitive;
                node.Rect.X = 50;
                node.Rect.Y = 30;

                Util.DrawNodeToImage(out var imageRawBytes, node, 150, 60);
                Util.CheckExpectedImage(imageRawBytes, 150, 60, @"Rendering\images\NodeFacts.Draw.DrawOneTextNodeAtPosition.png");
            }

            [Fact]
            public void DrawOneImageNode()
            {
                Node node = new Node(1, "imageNode", new Rect(10, 10, 300, 200));
                node.Geometry = new ImageGeometry(@"assets\images\logo.png");

                Util.DrawNodeToImage(out var imageRawBytes, node, 350, 250);
                Util.CheckExpectedImage(imageRawBytes, 350, 250, @"Rendering\images\NodeFacts.Draw.DrawOneImageNode.png");
            }

            [Fact]
            public void DrawOverlappedRectangles()
            {
                var box0 = new List<Node>();
                {
                    {
                        Node node = new Node(0);
                        box0.Add(node);
                        var primitive = new PathGeometry();
                        primitive.PathRect(new Point(10, 10), new Point(100, 100));
                        primitive.PathFill(Color.Orange);
                        node.Geometry = primitive;
                    }
                    {
                        Node node = new Node(1);
                        box0.Add(node);
                        var primitive = new PathGeometry();
                        primitive.PathRect(new Point(9, 9), new Point(101, 101));
                        primitive.PathStroke(2, Color.Black);
                        node.Geometry = primitive;
                    }
                }
                var box1 = new List<Node>();
                {
                    {
                        Node node = new Node(3);
                        box1.Add(node);
                        var primitive = new PathGeometry();
                        primitive.PathRect(new Point(50, 50), new Point(140, 140));
                        primitive.PathFill(Color.LightBlue);
                        node.Geometry = primitive;
                    }
                    {
                        Node node = new Node(4);
                        box1.Add(node);
                        var primitive = new PathGeometry();
                        primitive.PathRect(new Point(49, 49), new Point(141, 141));
                        primitive.PathStroke(2, Color.Red);
                        node.Geometry = primitive;
                    }
                }

                {
                    var box0Foreground = new List<Node>();
                    box0Foreground.AddRange(box0);
                    box0Foreground.AddRange(box1);
                    Util.DrawNodesToImage(out var imageRawBytes, box0Foreground, 150, 150);
                    Util.CheckExpectedImage(imageRawBytes, 150, 150, @"Rendering\images\NodeFacts.Draw.DrawOverlappedRectangles.Before.png");
                }
                {
                    var box1Foreground = new List<Node>();
                    box1Foreground.AddRange(box1);
                    box1Foreground.AddRange(box0);
                    Util.DrawNodesToImage(out var imageRawBytes, box1Foreground, 150, 150);
                    Util.CheckExpectedImage(imageRawBytes, 150, 150, @"Rendering\images\NodeFacts.Draw.DrawOverlappedRectangles.After.png");
                }
            }

            [Fact]
            public void DrawBoxModelText()
            {
                Node node = new Node(1, "textNode", new Rect(10, 10, 90, 40));
                StyleRuleSetBuilder ruleSetBuilder = new StyleRuleSetBuilder(node.RuleSet);
                ruleSetBuilder
                    .Border((5, 10, 5, 10))
                    .BorderColor(Color.HotPink)
                    .BackgroundColor(Color.Azure)
                    .Padding((4, 2, 4, 2));
                node.UseBoxModel = true;
                var primitive = new TextGeometry("AAA");
                node.Geometry = primitive;

                Util.DrawNodeToImage(out var imageRawBytes, node, 120, 60);
                Util.CheckExpectedImage(imageRawBytes, 120, 60,
                    @"Rendering\images\NodeFacts.Draw.DrawBoxModelText.png");
            }

            [Fact]
            public void DrawBoxModelImage()
            {
                Node node = new Node(1, "imageNode", new Rect(10, 10, 300, 200));
                StyleRuleSetBuilder ruleSetBuilder = new StyleRuleSetBuilder(node.RuleSet);
                ruleSetBuilder
                    .Border((5, 10, 5, 10))
                    .BorderColor(Color.HotPink)
                    .Padding((4, 2, 4, 2));
                node.UseBoxModel = true;
                node.Geometry = new ImageGeometry(@"assets\images\logo.png");

                Util.DrawNodeToImage(out var imageRawBytes, node, 400, 300);
                Util.CheckExpectedImage(imageRawBytes, 400, 300,
                    @"Rendering\images\NodeFacts.Draw.DrawBoxModelImage.png");
            }
        }
    }
}
