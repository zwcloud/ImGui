using Xunit;

namespace ImGui.ControlTest
{
    public class Separator
    {
        [Fact]
        public void LayoutSeparator()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            bool open = true;
            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                GUI.Begin("Test", ref open, Point.Zero, new Size(300, 300));
                GUILayout.Button("Button 0");
                GUILayout.Button("Button 1");
                GUILayout.Separator("separator1");
                GUILayout.Button("Button 2");
                GUILayout.Button("Button 3");
                GUILayout.Button("Button 4");
                GUILayout.Separator("separator2");
                GUILayout.Button("Button 5");
                GUI.End();
            };

            Application.Run(form);
        }
    }
}
