using ImGui;
using ImGui.ControlTest;
using Xunit;

namespace ImGui.ControlTest
{
    public class Slider
    {
        public Slider()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitSysDependencies();
        }

        [Fact]
        public void SliderShouldWork()
        {
            var value = 0.5;
            var min = 0.0;
            var max = 1.0;

            Application.Run(new MainForm(() =>
            {
                value = GUI.Slider(new Rect(10, 10, 200, 30), "Slider", value, min, max);
                value = GUI.VSlider(new Rect(10, 50, 30, 200), "VSlider", value, min, max);
            }));
        }

        [Fact]
        public void LayoutSliderShouldWork()
        {
            var value0 = 0.5;
            var value1 = 0.5;
            var min = 0.0;
            var max = 1.0;

            Application.Run(new MainForm(() =>
            {
                value0 = GUILayout.Slider("Slider0", value0, min, max);
                value1 = GUILayout.VSlider("Slider1", value1, min, max);
            }));
        }
    }
}
