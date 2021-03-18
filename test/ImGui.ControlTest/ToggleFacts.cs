using Xunit;

namespace ImGui.ControlTest
{
    public class Toggle
    {
        [Fact]
        public void ShowOneFixedToggle()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            var enabled = true;

            Application.Run(form, () =>
            {
                enabled = GUI.Toggle(new Rect(0, 0, 100, 30), "enabled", enabled);
            });
        }

        [Fact]
        public void ShowOneLayoutedToggle()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            var enabled = true;

            Application.Run(form, () =>
            {
                enabled = GUILayout.Toggle("enabled", enabled);
            });
        }
    }
}
