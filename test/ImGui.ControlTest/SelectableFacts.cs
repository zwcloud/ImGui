using Xunit;

namespace ImGui.ControlTest
{
    public class Selectable
    {
        [Fact]
        public void ShowOneFixedSelectable()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            var enabled = true;
            form.OnGUIAction = () =>
            {
                enabled = GUI.Selectable(new Rect(0, 0, 100, 30), "select this", enabled);
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowOneLayoutedSelectable()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            var enabled = true;
            form.OnGUIAction = () =>
            {
                enabled = GUILayout.Selectable("select this", enabled);
            };

            Application.Run(form);
        }
    }
}
