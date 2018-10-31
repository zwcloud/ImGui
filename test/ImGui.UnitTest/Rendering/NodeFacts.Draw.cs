using System;
using ImGui.Common.Primitive;
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
        static NodeFacts()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitSysDependencies();
        }

        public class Draw
        {
            [Fact]
            public void DrawANode()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 100));
                primitive.PathLineTo(new Point(100, 100));
                primitive.PathLineTo(new Point(100, 10));
                primitive.PathClose();
                primitive.PathFill(Color.Black);

                Node node = new Node(1);
                node.Primitive = primitive;
                
                Util.DrawNodeToImage(out var imageRawBytes, node, 110, 110);
                Util.CheckExpectedImage(imageRawBytes, 110, 110, @"Rendering\images\NodeFacts.Draw.DrawANode.png");
            }

            [Fact]
            public void UpdateANode()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 100));
                primitive.PathLineTo(new Point(100, 100));
                primitive.PathLineTo(new Point(100, 10));
                primitive.PathClose();
                var fillCmd = primitive.PathFill(Color.Black);

                Node node = new Node(1);
                node.Primitive = primitive;

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
                var primitive = new TextPrimitive("Before");
                node.Primitive = primitive;

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
                    var primitive = new PathPrimitive();
                    primitive.PathMoveTo(new Point(10, 10));
                    primitive.PathLineTo(new Point(10, 100));
                    primitive.PathLineTo(new Point(100, 100));
                    primitive.PathLineTo(new Point(100, 10));
                    primitive.PathClose();
                    node0FillCmd = primitive.PathFill(Color.Green);
                    node.Primitive = primitive;
                }
                {
                    Node node = new Node(1);
                    nodes.Add(node);
                    var primitive = new PathPrimitive();
                    primitive.PathMoveTo(new Point(110, 10));
                    primitive.PathLineTo(new Point(110, 100));
                    primitive.PathLineTo(new Point(200, 100));
                    primitive.PathLineTo(new Point(200, 10));
                    primitive.PathClose();
                    node1FillCmd = primitive.PathFill(Color.Orange);

                    node.Primitive = primitive;
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
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 100));
                primitive.PathLineTo(new Point(100, 100));
                primitive.PathLineTo(new Point(100, 10));
                primitive.PathClose();
                primitive.PathFill(Color.Red);
                node.Primitive = primitive;

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

                var primitiveRenderer = new BuiltinPrimitiveRenderer();

                Node node = new Node(1);
                node.Primitive = new PathPrimitive();

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

                        var d = node.Primitive as PathPrimitive;
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
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                Node node = new Node(1);
                var primitive = new TextPrimitive("AAA");
                node.Primitive = primitive;
                node.Draw(primitiveRenderer, meshList);
                node.Rect.X = 1;
                node.Rect.Y = 1;

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(300, 400), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                window.Show();

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

                        //update the node
                        if (node.ActiveInTree)
                        {
                            node.Draw(primitiveRenderer, meshList);
                        }

                        //rebuild mesh buffer
                        meshBuffer.Clear();
                        meshBuffer.Init();
                        meshBuffer.Build(meshList);

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height, (shapeMesh: meshBuffer.ShapeMesh, imageMesh: meshBuffer.ImageMesh, meshBuffer.TextMesh));
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
            public void DrawTwoTextNode()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                var nodes = new List<Node>();
                {
                    Node node = new Node(1);
                    nodes.Add(node);
                    var primitive = new TextPrimitive("AAA");
                    node.Primitive = primitive;
                    node.Draw(primitiveRenderer, meshList);
                    node.Rect.X = 1;
                    node.Rect.Y = 1;
                }
                {
                    Node node = new Node(1);
                    nodes.Add(node);
                    var primitive = new TextPrimitive("B");
                    node.Primitive = primitive;
                    node.Draw(primitiveRenderer, meshList);
                    node.Rect.X = 1;
                    node.Rect.Y = 40;
                }

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(300, 400), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                window.Show();

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

                        //update nodes
                        foreach (var node in nodes)
                        {
                            if (node.ActiveInTree)
                            {
                                node.Draw(primitiveRenderer, meshList);
                            }
                        }

                        //rebuild mesh buffer
                        meshBuffer.Clear();
                        meshBuffer.Init();
                        meshBuffer.Build(meshList);

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height, (shapeMesh: meshBuffer.ShapeMesh, imageMesh: meshBuffer.ImageMesh, meshBuffer.TextMesh));
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
            public void DrawOneTextNodeAtPosition()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                Node node = new Node(1);
                var primitive = new TextPrimitive("AAA");
                node.Primitive = primitive;
                node.Draw(primitiveRenderer, meshList);
                node.Rect.X = 100;
                node.Rect.Y = 30;

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(300, 400), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                window.Show();

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

                        //update the node
                        if (node.ActiveInTree)
                        {
                            node.Draw(primitiveRenderer, meshList);
                        }

                        //rebuild mesh buffer
                        meshBuffer.Clear();
                        meshBuffer.Init();
                        meshBuffer.Build(meshList);

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height, (shapeMesh: meshBuffer.ShapeMesh, imageMesh: meshBuffer.ImageMesh, meshBuffer.TextMesh));
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
            public void DrawOneImageNode()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(500, 500), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                Node node = new Node(1, "imageNode", new Rect(10, 10, 300, 200));
                node.Primitive = new ImagePrimitive(@"assets\images\logo.png");
                node.Draw(primitiveRenderer, meshList);

                window.Show();

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

                        //update the node
                        if (node.ActiveInTree)
                        {
                            node.Draw(primitiveRenderer, meshList);
                        }

                        //rebuild mesh buffer
                        meshBuffer.Clear();
                        meshBuffer.Init();
                        meshBuffer.Build(meshList);

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height, (shapeMesh: meshBuffer.ShapeMesh, imageMesh: meshBuffer.ImageMesh, meshBuffer.TextMesh));
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
            public void DrawOverlappedRectangles()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();

                MeshBuffer meshBuffer0 = new MeshBuffer();
                MeshList meshList0 = new MeshList();
                var box0 = new List<Node>();
                {
                    {
                        Node node = new Node(0);
                        box0.Add(node);
                        var primitive = new PathPrimitive();
                        primitive.PathRect(new Point(10, 10), new Point(100, 100));
                        primitive.PathFill(Color.Orange);
                        node.Primitive = primitive;
                    }

                    {
                        Node node = new Node(1);
                        box0.Add(node);
                        var primitive = new PathPrimitive();
                        primitive.PathRect(new Point(9, 9), new Point(101, 101));
                        primitive.PathStroke(2, Color.Black);
                        node.Primitive = primitive;
                    }
                }
                var box1 = new List<Node>();
                MeshBuffer meshBuffer1 = new MeshBuffer();
                MeshList meshList1 = new MeshList();
                {
                    {
                        Node node = new Node(3);
                        box1.Add(node);
                        var primitive = new PathPrimitive();
                        primitive.PathRect(new Point(50, 50), new Point(140, 140));
                        primitive.PathFill(Color.LightBlue);
                        node.Primitive = primitive;
                    }
                    {
                        Node node = new Node(4);
                        box1.Add(node);
                        var primitive = new PathPrimitive();
                        primitive.PathRect(new Point(49, 49), new Point(141, 141));
                        primitive.PathStroke(2, Color.Red);
                        node.Primitive = primitive;
                    }

                }

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(400, 400), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                window.Show();

                bool switched = false;

                void DrawBox(List<Node> boxNodes, MeshList meshList, MeshBuffer meshBuffer)
                {
                    boxNodes.ForEach(n => n.Draw(primitiveRenderer, meshList));

                    //rebuild mesh buffer
                    meshBuffer.Clear();
                    meshBuffer.Init();
                    meshBuffer.Build(meshList);

                    //draw mesh buffer to screen
                    renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height, (shapeMesh: meshBuffer.ShapeMesh, imageMesh: meshBuffer.ImageMesh, meshBuffer.TextMesh));
                }

                while (true)
                {
                    Time.OnFrameBegin();
                    Keyboard.Instance.OnFrameBegin();

                    window.MainLoop(() =>
                    {
                        renderer.Clear(Color.FrameBg);

                        if (Keyboard.Instance.KeyDown(Key.Escape))
                        {
                            Application.Quit();
                        }

                        if (Keyboard.Instance.KeyPressed(Key.Space))
                        {
                            switched = !switched;
                        }

                        if (switched)
                        {
                            DrawBox(box0, meshList0, meshBuffer0);
                            DrawBox(box1, meshList1, meshBuffer1);
                        }
                        else
                        {
                            DrawBox(box1, meshList1, meshBuffer1);
                            DrawBox(box0, meshList0, meshBuffer0);
                        }

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
            public void DrawBoxModelText()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                Node node = new Node(1, "textNode", new Rect(10, 10, 300, 40));
                StyleRuleSetBuilder ruleSetBuilder = new StyleRuleSetBuilder(node.RuleSet);
                ruleSetBuilder
                    .Border((5, 10, 5, 10))
                    .BorderColor(Color.HotPink)
                    .BackgroundColor(Color.Azure)
                    .Padding((4, 2, 4, 2));
                node.UseBoxModel = true;
                var primitive = new TextPrimitive("AAA");
                node.Primitive = primitive;
                node.Draw(primitiveRenderer, meshList);


                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(800, 600), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                window.Show();

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

                        //update the node
                        if (node.ActiveInTree)
                        {
                            node.Draw(primitiveRenderer, meshList);
                        }

                        //rebuild mesh buffer
                        meshBuffer.Clear();
                        meshBuffer.Init();
                        meshBuffer.Build(meshList);

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height, (shapeMesh: meshBuffer.ShapeMesh, imageMesh: meshBuffer.ImageMesh, meshBuffer.TextMesh));
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
            public void DrawBoxModelImage()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(800, 600), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                Node node = new Node(1, "imageNode", new Rect(10, 10, 300, 200));
                StyleRuleSetBuilder ruleSetBuilder = new StyleRuleSetBuilder(node.RuleSet);
                ruleSetBuilder
                    .Border((5, 10, 5, 10))
                    .BorderColor(Color.HotPink)
                    .Padding((4, 2, 4, 2));
                node.UseBoxModel = true;
                node.Primitive = new ImagePrimitive(@"assets\images\logo.png");
                node.Draw(primitiveRenderer, meshList);

                window.Show();

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

                        //rebuild mesh buffer
                        meshBuffer.Clear();
                        meshBuffer.Init();
                        meshBuffer.Build(meshList);

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height, (shapeMesh: meshBuffer.ShapeMesh, imageMesh: meshBuffer.ImageMesh, meshBuffer.TextMesh));
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
        }
    }
}
