using System.Collections.Generic;
using ImGui.Common.Primitive;
using ImGui.GraphicsImplementation;
using ImGui.Input;
using ImGui.OSImplentation.Windows;
using ImGui.Rendering;
using ImGui.Core;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public partial class NodeFacts
    {
        public class TheDrawMethod
        {
            [Fact]
            public void DrawANode()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();

                Node node = new Node(1);

                var primitive = new PathPrimitive();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 100));
                primitive.PathLineTo(new Point(100, 100));
                primitive.PathLineTo(new Point(100, 10));
                primitive.PathClose();
                primitive.PathFill(Color.Black);

                node.Primitive = primitive;

                node.Draw(primitiveRenderer);

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

                        //rebuild mesh buffer
                        MeshBuffer.Clear();
                        MeshBuffer.Init();
                        MeshBuffer.Build();

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height);
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
            public void UpdateANode()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();

                Node node = new Node(1);

                var primitive = new PathPrimitive();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 100));
                primitive.PathLineTo(new Point(100, 100));
                primitive.PathLineTo(new Point(100, 10));
                primitive.PathClose();
                var fillCmd = primitive.PathFill(Color.Black);

                node.Primitive = primitive;

                node.Draw(primitiveRenderer);

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
                        if (Keyboard.Instance.KeyDown(Key.D1))
                        {
                            fillCmd.Color = Color.Red;
                        }
                        if (Keyboard.Instance.KeyDown(Key.D2))
                        {
                            fillCmd.Color = Color.Green;
                        }
                        if (Keyboard.Instance.KeyDown(Key.D3))
                        {
                            fillCmd.Color = Color.Blue;
                        }

                        if (Keyboard.Instance.KeyDown(Key.Escape))
                        {
                            Application.Quit();
                        }

                        //update nodes
                        if (node.ActiveInTree)//this is actually always true
                        {
                            node.Draw(primitiveRenderer);
                        }

                        //rebuild mesh buffer
                        MeshBuffer.Clear();
                        MeshBuffer.Init();
                        MeshBuffer.Build();

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height);
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
            public void UpdateTwoNode()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
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
                    node0FillCmd = primitive.PathFill(Color.Black);
                    node.Primitive = primitive;

                    node.Draw(primitiveRenderer);
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
                    node1FillCmd = primitive.PathFill(Color.Black);

                    node.Primitive = primitive;

                    node.Draw(primitiveRenderer);
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
                        if (Keyboard.Instance.KeyDown(Key.NumPad1))
                        {
                            node0FillCmd.Color = Color.Red;
                            node1FillCmd.Color = Color.Blue;
                        }
                        if (Keyboard.Instance.KeyDown(Key.NumPad2))
                        {
                            node0FillCmd.Color = Color.Green;
                            node1FillCmd.Color = Color.Orange;
                        }

                        if (Keyboard.Instance.KeyDown(Key.Escape))
                        {
                            Application.Quit();
                        }

                        //update nodes
                        foreach (var node in nodes)
                        {
                            if (node.ActiveInTree)
                            {
                                node.Draw(primitiveRenderer);
                            }
                        }

                        //rebuild mesh buffer
                        MeshBuffer.Clear();
                        MeshBuffer.Init();
                        MeshBuffer.Build();

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height);
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
            public void ShowHideANode()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                var nodes = new List<Node>();
                {
                    Node node = new Node(1);
                    nodes.Add(node);
                    var primitive = new PathPrimitive();
                    primitive.PathMoveTo(new Point(10, 10));
                    primitive.PathLineTo(new Point(10, 100));
                    primitive.PathLineTo(new Point(100, 100));
                    primitive.PathLineTo(new Point(100, 10));
                    primitive.PathClose();
                    primitive.PathFill(Color.Red);

                    node.Primitive = primitive;

                    node.Draw(primitiveRenderer);
                }
                var theNode = nodes[0];

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
                        if (Keyboard.Instance.KeyPressed(Key.Space))
                        {
                            theNode.ActiveSelf = !theNode.ActiveSelf;
                            Log.Msg("Key.Space Pressed. theNode becomes " + (theNode.ActiveSelf ? "visible" : "invisible"));
                        }

                        if (Keyboard.Instance.KeyDown(Key.Escape))
                        {
                            Application.Quit();
                        }

                        //update nodes
                        foreach (var node in nodes)
                        {
                            if (node.ActiveInTree)
                            {
                                node.Draw(primitiveRenderer);
                            }
                        }

                        //rebuild mesh buffer
                        MeshBuffer.Clear();
                        MeshBuffer.Init();
                        MeshBuffer.Build();

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height);
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

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                Node node = new Node(1);
                var primitive = new TextPrimitive();
                primitive.Text = "AAA";
                node.Primitive = primitive;
                node.Draw(primitiveRenderer);
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
                            node.Draw(primitiveRenderer);
                        }

                        //rebuild mesh buffer
                        MeshBuffer.Clear();
                        MeshBuffer.Init();
                        MeshBuffer.Build();

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height);
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

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                var nodes = new List<Node>();
                {
                    Node node = new Node(1);
                    nodes.Add(node);
                    var primitive = new TextPrimitive();
                    primitive.Text = "AAA";
                    node.Primitive = primitive;
                    node.Draw(primitiveRenderer);
                    node.Rect.X = 1;
                    node.Rect.Y = 1;
                }
                {
                    Node node = new Node(1);
                    nodes.Add(node);
                    var primitive = new TextPrimitive();
                    primitive.Text = "B";
                    node.Primitive = primitive;
                    node.Draw(primitiveRenderer);
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
                                node.Draw(primitiveRenderer);
                            }
                        }

                        //rebuild mesh buffer
                        MeshBuffer.Clear();
                        MeshBuffer.Init();
                        MeshBuffer.Build();

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height);
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

                var primitiveRenderer = new BuiltinPrimitiveRenderer();
                Node node = new Node(1);
                var primitive = new TextPrimitive();
                primitive.Text = "AAA";
                node.Primitive = primitive;
                node.Draw(primitiveRenderer);
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
                            node.Draw(primitiveRenderer);
                        }

                        //rebuild mesh buffer
                        MeshBuffer.Clear();
                        MeshBuffer.Init();
                        MeshBuffer.Build();

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height);
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

                        box0.ForEach(n=>n.Draw(primitiveRenderer));

                        //rebuild mesh buffer
                        MeshBuffer.Clear();
                        MeshBuffer.Init();
                        MeshBuffer.Build();

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height);

                        box1.ForEach(n=>n.Draw(primitiveRenderer));

                        //rebuild mesh buffer
                        MeshBuffer.Clear();
                        MeshBuffer.Init();
                        MeshBuffer.Build();

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height);

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
