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
                        }

                        {
                            GUIStyle style = GUIStyle.Basic;
                            style.Save();
                            style.PushBorder(1);
                            style.PushBorderColor(Color.Black);
                            style.PushPadding((1, 2, 1, 2));
                            style.PushBgColor(Color.Silver);

                            node.Draw(primitiveRenderer, meshList);

                            style.Restore();
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
                            container.AttachLayoutGroup(true, GUILayout.Width(300).Height(40));
                            container.UseBoxModel = true;

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
                            GUIStyle style = GUIStyle.Basic;
                            style.Save();
                            style.PushBorder(1);
                            style.PushBorderColor(Color.Black);
                            style.PushPadding((1, 2, 1, 2));
                            style.PushBgColor(Color.Silver);

                            container.Draw(primitiveRenderer, meshList);

                            style.Restore();
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
        }
    }
}