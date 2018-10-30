using ImGui;
using ImGui.Common.Primitive;
using ImGui.UnitTest;
using Xunit;

namespace ControlTest
{
    public class SliderTest
    {
        public SliderTest()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitSysDependencies();
        }

        [Fact]
        public void SliderShouldWork()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitSysDependencies();

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
