using ImGui.Common.Primitive;
using ImGui.Core;
using ImGui.GraphicsImplementation;
using ImGui.Input;
using ImGui.OSImplentation.Windows;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public partial class NodeFacts
    {
        public class NodeContainer
        {
            [Fact]
            public void DrawAndLayoutEmptyContainer()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(400, 400), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                window.Show();

                bool DrawNode(Node n, MeshList list)
                {
                    if (!n.ActiveInTree)
                    {
                        return false;
                    }

                    n.Draw(primitiveRenderer, meshList);
                    return true;
                }

                Node node = null;
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

                        if (node == null)
                        {
                            node = new Node(1, "container");
                            node.AttachLayoutGroup(true, GUILayout.Width(300).Height(40));
                            node.UseBoxModel = true;
                            StyleRuleSetBuilder b = new StyleRuleSetBuilder(node);
                            b.Border(1)
                                .BorderColor(Color.Black)
                                .Padding((top: 1, right: 2, bottom: 1, left: 2))
                                .BackgroundColor(Color.Silver);
                        }

                        {
                            DrawNode(node, meshList);
                            node.Foreach(n=>DrawNode(n, meshList));
                            node.Layout();
                        }

                        //rebuild mesh buffer
                        meshBuffer.Clear();
                        meshBuffer.Init();
                        meshBuffer.Build(meshList);

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height, meshBuffer);
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
            public void DrawAndLayoutContainerWithElements()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(400, 400), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                window.Show();

                bool DrawNode(Node n, MeshList list)
                {
                    if (!n.ActiveInTree)
                    {
                        return false;
                    }

                    n.Draw(primitiveRenderer, meshList);
                    return true;
                }

                Node container = null;
                Node icon;
                Node title;
                Node closeButton;

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

                        if (container == null)
                        {
                            container = new Node(1, "container");
                            container.AttachLayoutGroup(false, GUILayout.Width(300).Height(40));
                            container.UseBoxModel = true;
                            StyleRuleSetBuilder b = new StyleRuleSetBuilder(container);
                            b.Border(1)
                                .BorderColor(Color.Black)
                                .Padding((top: 4, right: 3, bottom: 4, left: 3))
                                .BackgroundColor(Color.Silver)
                                .AlignmentVertical(Alignment.Center)
                                .AlignmentHorizontal(Alignment.Center);

                            icon = new Node(2, "icon");
                            icon.AttachLayoutEntry(new Size(20, 20), GUILayout.Width(20).Height(20));
                            icon.UseBoxModel = false;
                            icon.Primitive = new ImagePrimitive(@"assets\images\logo.png");

                            title = new Node(3, "title");
                            title.AttachLayoutEntry(Size.Zero, GUILayout.Height(20));
                            title.UseBoxModel = false;
                            title.Primitive = new TextPrimitive("title");

                            closeButton = new Node(4, "close button");
                            closeButton.AttachLayoutEntry(new Size(20, 20), GUILayout.Width(20).Height(20));
                            closeButton.UseBoxModel = false;
                            PathPrimitive path = new PathPrimitive();
                            path.PathRect(new Rect(0, 0, 20, 20));
                            closeButton.Primitive = path;

                            container.AppendChild(icon);
                            container.AppendChild(title);
                            container.AppendChild(closeButton);
                        }

                        {
                            DrawNode(container, meshList);
                            container.Foreach(n=>DrawNode(n, meshList));
                            container.Layout();
                        }

                        //rebuild mesh buffer
                        meshBuffer.Clear();
                        meshBuffer.Init();
                        meshBuffer.Build(meshList);

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height, meshBuffer);
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
            public void DrawAWindow()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var primitiveRenderer = new BuiltinPrimitiveRenderer();

                MeshBuffer meshBuffer = new MeshBuffer();
                MeshList meshList = new MeshList();

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(800, 600), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                window.Show();

                bool DrawNode(Node n, MeshList list)
                {
                    if (!n.ActiveInTree)
                    {
                        return false;
                    }

                    n.Draw(primitiveRenderer, list);
                    return true;
                }

                //window
                var windowContainer = new Node("#window");
                windowContainer.AttachLayoutGroup(true, GUILayout.Width(400));
                windowContainer.UseBoxModel = true;
                windowContainer.RuleSet.Border = (1, 1, 1, 1);
                windowContainer.RuleSet.BackgroundColor = Color.White;

                //title bar
                {
                    var titleBarContainer = new Node(1, "#titleBar");
                    titleBarContainer.AttachLayoutGroup(false, GUILayout.ExpandWidth(true).Height(40));
                    titleBarContainer.UseBoxModel = true;
                    StyleRuleSetBuilder b = new StyleRuleSetBuilder(titleBarContainer);
                    b.Padding((top: 8, right: 8, bottom: 8, left: 8))
                        .FontColor(Color.Black)
                        .FontSize(12)
                        .BackgroundColor(Color.White)
                        .AlignmentVertical(Alignment.Center)
                        .AlignmentHorizontal(Alignment.Start);

                    var icon = new Node(2, "#icon");
                    icon.AttachLayoutEntry(new Size(20, 20), GUILayout.Width(20).Height(20));
                    icon.UseBoxModel = false;
                    icon.Primitive = new ImagePrimitive(@"assets\images\logo.png");

                    var title = new Node(3, "#title");
                    title.AttachLayoutEntry(Size.Zero, GUILayout.Height(20));
                    title.UseBoxModel = false;
                    title.Primitive = new TextPrimitive("The Window Title");

                    var closeButton = new Node(4, "#close button");
                    closeButton.AttachLayoutEntry(new Size(20, 20), GUILayout.Width(20).Height(20));
                    closeButton.UseBoxModel = false;
                    PathPrimitive path = new PathPrimitive();
                    path.PathRect(new Rect(0, 0, 20, 20));
                    closeButton.Primitive = path;

                    titleBarContainer.AppendChild(icon);
                    titleBarContainer.AppendChild(title);
                    titleBarContainer.AppendChild(closeButton);
                    windowContainer.AppendChild(titleBarContainer);
                }
                
                Node clientArea;
                //client area background
                {
                    clientArea = new Node("#ClientArea_Background");
                    clientArea.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).Height(200));
                    windowContainer.AppendChild(clientArea);
                }

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
                        
                        if (Keyboard.Instance.KeyDown(Key.Space))
                        {
                            clientArea.ActiveSelf = !clientArea.ActiveSelf;
                        }

                        {
                            DrawNode(windowContainer, meshList);
                            windowContainer.Foreach(n=>DrawNode(n, meshList));
                            windowContainer.Layout();
                        }

                        //rebuild mesh buffer
                        meshBuffer.Clear();
                        meshBuffer.Init();
                        meshBuffer.Build(meshList);

                        //draw mesh buffer to screen
                        renderer.Clear(Color.FrameBg);
                        renderer.DrawMeshes((int)window.ClientSize.Width, (int)window.ClientSize.Height, meshBuffer);
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