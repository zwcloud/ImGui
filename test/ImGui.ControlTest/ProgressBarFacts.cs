using Xunit;

namespace ImGui.ControlTest
{
    public class ProgressBar
    {
        [Fact]
        public void ShowOneProgressBar()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            bool open = true;
            form.OnGUIAction = () =>
            {
                GUILayout.ProgressBar("Progress", System.DateTime.Now.Millisecond / 1000.0f, new Size(100, 20));
            };

            Application.Run(form);
        }
    }
}
