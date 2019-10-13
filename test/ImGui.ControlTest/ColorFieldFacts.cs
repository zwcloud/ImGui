using Xunit;

namespace ImGui.ControlTest
{
    public class ColorField
    {
        [Fact]
        public void ShowColorFields()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            Color col1 = new Color(1.0f, 0.0f, 0.2f);
            Color col2 = new Color(0.4f, 0.7f, 0.0f, 0.5f);
            form.OnGUIAction = () =>
            {
                col1 = GUILayout.ColorField("color 1", col1);
                col2 = GUILayout.ColorField("color 2", col2);
            };

            Application.Run(form);
        }
    }
}
