using ImGui;
using Xunit;

namespace ImGui.ControlTest
{
    public class TextBoxTest
    {
        public TextBoxTest()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitSysDependencies();
        }

        [Fact]
        public void TestBoxWorksCorrectly()
        {
            var text = "Hello ImGui!你好";

            Application.Run(new MainForm(() =>
            {
                text = GUILayout.TextBox("Name", new Size(200, 30), text);
            }));
        }
    }

}
