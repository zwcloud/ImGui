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
        
        [Fact]
        public void TestBoxLayoutCorrectly()
        {
            string str0 = "Hello, world!";
            int i0 = 123;
            float f0 = 0.001f;
            Application.Run(new MainForm(() =>
            {
                str0 = GUILayout.InputText("input text", str0);
                i0 = GUILayout.InputInt("input int", i0);
                f0 = GUILayout.InputFloat("input float", f0);
            }));
        }
        
        [Fact]
        public void EmptyTestBoxSizeIsCorrect()
        {
            string str0 = "";
            Application.Run(new MainForm(() =>
            {
                str0 = GUILayout.InputText("default", str0);
            }));
        }
    }

}
