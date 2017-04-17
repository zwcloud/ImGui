using System;
using Xunit;
using ImGui;

namespace ControlTest
{
    public class SliderTest
    {
        public SliderTest()
        {
            Application.InitSysDependencies();
        }

        [Fact]
        public void SliderShouldWork()
        {
            var value = 0.5;
            var min = 0.0;
            var max = 1.0;

            Application.Run(new Form1(() => {
                value = GUI.Slider(new Rect(10, 10, 200, 30), value, min, max, "Slider0");
                value = GUI.VSlider(new Rect(10, 50, 30, 200), value, min, max, "Slider1");
            }));
        }

        [Fact]
        public void LayoutSliderShouldWork()
        {
            var value0 = 0.5;
            var value1 = 0.5;
            var min = 0.0;
            var max = 1.0;

            Application.Run(new Form1(() => {
                value0 = GUILayout.Slider("Slider0", value0, min, max, "Slider0");
                value1 = GUILayout.VSlider("Slider1", value0, min, max, "Slider1");
            }));
        }
    }
}
