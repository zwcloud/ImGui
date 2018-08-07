using System;
using System.Collections.Generic;
using System.Reflection;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.GraphicsImplementation;
using ImGui.Input;
using ImGui.OSImplentation.Windows;
using ImGui.Rendering;
using ImGui.Core;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class NodeFacts
    {
        public class TheLayoutMethod
        {
            [Fact]
            public void ShowANodeWithTwoChildren() // Add rect; Add rect then remove rect
            {
                Node a = new Node(1);
                a.Rect = new Rect(0, 0, 300, 400);
                a.AttachLayoutGroup(true);

                Node b = new Node(2);
                b.Rect = new Rect(0, 0, 100, 100);
                b.AttachLayoutEntry(new Size(100, 100));

                Node c = new Node(3);
                c.Rect = new Rect(0, 0, 100, 200);
                c.AttachLayoutEntry(new Size(100, 200));

                a.Add(b);
                a.Add(c);

                a.Layout();

                Util.DrawNode(a);
            }

            [Fact]
            public void ShowAHorizontalGroupOf3ItemsWithDifferentStretchFactors()
            {
                Node group = new Node(0); group.AttachLayoutGroup(false, GUILayout.Width(600));
                Node item1 = new Node(1); item1.AttachLayoutEntry(new Size(20, 10), GUILayout.StretchWidth(1).Height(50));
                Node item2 = new Node(2); item2.AttachLayoutEntry(new Size(20, 10), GUILayout.StretchWidth(2).Height(60));
                Node item3 = new Node(3); item3.AttachLayoutEntry(new Size(20, 10), GUILayout.StretchWidth(1).Height(30));
                group.Add(item1);
                group.Add(item2);
                group.Add(item3);

                group.Layout();

                Util.DrawNode(group);
            }

            [Fact]
            void ShowAThreeLayerGroup()
            {
                // layer 1
                Node group1 = new Node(1); group1.AttachLayoutGroup(true, GUILayout.Width(400).Height(400));

                // layer 2
                Node group2 = new Node(2); group2.AttachLayoutGroup(false, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group3 = new Node(3); group3.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group4 = new Node(4); group4.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));

                // layer3
                Node group5 =  new Node(5); group5.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group6 =  new Node(6); group6.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group7 =  new Node(7); group7.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group8 =  new Node(8); group8.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group9 =  new Node(9); group9.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group10 = new Node(10); group10.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group11 = new Node(11); group11.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group12 = new Node(12); group12.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group13 = new Node(13); group13.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));

                group1.Add(group2);
                group1.Add(group3);
                group1.Add(group4);

                group2.Add(group5);
                group2.Add(group6);
                group2.Add(group7);
                group3.Add(group8);
                group3.Add(group9);
                group3.Add(group10);
                group4.Add(group11);
                group4.Add(group12);
                group4.Add(group13);

                group1.Layout();

                Util.DrawNode(group1);
            }

            [Fact]
            void LayoutDefaultSizedNodeInDefaultSizedGroup()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(100, 100));

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(100, 200));

                a.Add(b);
                a.Add(c);

                a.Layout();

                Util.DrawNode(a);
            }

            [Fact]
            void LayoutDefaultSizedNodeInFixedSizedGroup()
            {
                Node a = new Node(1);
                a.AttachLayoutGroup(true, GUILayout.Width(200).Height(200));

                Assert.True(a.IsFixedWidth);
                Assert.True(a.IsFixedHeight);

                Node b = new Node(2);
                b.AttachLayoutEntry(new Size(100, 100));

                Node c = new Node(3);
                c.AttachLayoutEntry(new Size(100, 200));

                a.Add(b);
                a.Add(c);

                a.Layout();

                Util.DrawNode(a);
            }


        }

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

                node.Primitive = primitive;
                node.IsFill = true;
                node.Brush = new Brush();

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

                node.Primitive = primitive;
                node.IsFill = true;
                node.Brush = new Brush();

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
                            node.Brush.FillColor = Color.Red;
                        }
                        if (Keyboard.Instance.KeyDown(Key.D2))
                        {
                            node.Brush.FillColor = Color.Green;
                        }
                        if (Keyboard.Instance.KeyDown(Key.D3))
                        {
                            node.Brush.FillColor = Color.Blue;
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
                {
                    Node node = new Node(1);
                    nodes.Add(node);
                    var primitive = new PathPrimitive();
                    primitive.PathMoveTo(new Point(10, 10));
                    primitive.PathLineTo(new Point(10, 100));
                    primitive.PathLineTo(new Point(100, 100));
                    primitive.PathLineTo(new Point(100, 10));
                    primitive.PathClose();

                    node.Primitive = primitive;
                    node.IsFill = true;
                    node.Brush = new Brush();

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

                    node.Primitive = primitive;
                    node.IsFill = true;
                    node.Brush = new Brush();

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
                            nodes[0].Brush.FillColor = Color.Red;
                            nodes[1].Brush.FillColor = Color.Blue;
                        }
                        if (Keyboard.Instance.KeyDown(Key.NumPad2))
                        {
                            nodes[0].Brush.FillColor = Color.Green;
                            nodes[1].Brush.FillColor = Color.Orange;
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

                    node.Primitive = primitive;
                    node.IsFill = true;
                    node.Brush = new Brush();
                    node.Brush.FillColor = Color.Red;

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
        }
    }
}
