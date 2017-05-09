using ImGui;
using Xunit;

namespace ControlTest
{
    public class TextBoxTest
    {
        public TextBoxTest()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitSysDependencies();
        }

        [Fact]
        public void SliderShouldWork()
        {
            var text = "Hello ImGui!";

            Application.Run(new Form1(() => {
                text = GUI.Textbox(new Rect(10, 10, 200, 30), "My Name", text);
            }));
        }
    }

}
