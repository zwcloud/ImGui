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
            form.OnGUIAction = () =>
            {
                enabled = GUI.Toggle(new Rect(0, 0, 100, 30), "enabled", enabled);
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowOneLayoutedToggle()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            var enabled = true;
            form.OnGUIAction = () =>
            {
                enabled = GUILayout.Toggle("enabled", enabled);
            };

            Application.Run(form);
        }
    }
}
