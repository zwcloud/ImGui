using System;
using ImGui;
using ImGui.OSImplementation.Shared;
using ImGui.OSImplementation.Windows;
using ImGui.Rendering;

namespace MultipleNativeWindowTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.InitSysDependencies();
            Win32Window windowA = new Win32Window();
            windowA.Init((100, 100), (300, 400), WindowTypes.Regular);
            windowA.Show();
                
            Win32Window windowB = new Win32Window();
            windowB.Init((500, 100), (400, 500), WindowTypes.Regular);
            windowB.Show();

            Win32OpenGLRenderer renderer = new Win32OpenGLRenderer(windowA);
            var mesh = new QuadMesh(new Rect(100, 100, 200, 200), Color.Blue);

            while (!windowA.Closed)
            {
                windowA.MainLoop(() =>
                {
                    renderer.SetRenderingWindow(windowA);
                    renderer.Clear(Color.Rgb(200, 100, 100));
                    Win32OpenGLRenderer.DrawMesh(OpenGLMaterial.shapeMaterial, mesh, (int)windowA.ClientSize.Width, (int)windowA.ClientSize.Height);
                    renderer.SwapBuffers();

                    renderer.SetRenderingWindow(windowB);
                    renderer.Clear(Color.Rgb(100, 200, 100));
                    Win32OpenGLRenderer.DrawMesh(OpenGLMaterial.shapeMaterial, mesh, (int)windowB.ClientSize.Width, (int)windowB.ClientSize.Height);
                    renderer.SwapBuffers();

                });
            }
        }
    }
}
