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

            Application.Run(form, () =>
            {
                GUILayout.ProgressBar("Progress",
                    System.DateTime.Now.Millisecond / 1000.0f, new Size(100, 20));
            });
        }
    }
}
