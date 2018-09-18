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

                Node node = new Node(1, "container");
                node.AttachLayoutGroup(true, GUILayout.Width(300).Height(40));
                node.UseBoxModel = true;

                GUIStyle style = GUIStyle.Basic;
                style.Save();
                style.PushBorder(1);
                style.PushBorderColor(Color.Black);
                style.PushPadding((1, 2, 1, 2));
                style.PushBgColor(Color.Silver);

                node.Layout();
                node.Draw(primitiveRenderer, meshList);

                style.Restore();

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