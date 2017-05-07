using System;
using Xunit;
using ImGui;

namespace ControlTest
{
    public class WindowTest
    {
        public WindowTest()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitSysDependencies();
        }

        [Fact]
        public void WindowShouldWork()
        {
            var value = 0.5;
            var min = 0.0;
            var max = 1.0;
            bool open = true;

            Application.Run(new Form1(() => {

                GUILayout.Begin("Debug", ref open, new Point(60, 60), new Size(400, 400), 1, WindowFlags.Default);
                GUILayout.End();
            }));
        }
    }
}
