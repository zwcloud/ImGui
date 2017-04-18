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

            Application.Run(new Form1(() => {

                GUILayout.Begin("Debug", new Point(60, 60), new Size(400, 400));
                GUILayout.End();
            }));
        }
    }
}
