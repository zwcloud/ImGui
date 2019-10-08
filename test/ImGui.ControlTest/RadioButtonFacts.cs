using Xunit;

namespace ImGui.ControlTest
{
    public class RadioButton
    {
        [Fact]
        public void ShowOneLayoutedRadioButton()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            int active_id = 1;
            form.OnGUIAction = () =>
            {
                GUILayout.RadioButton("Radio 0", ref active_id, 0);
                GUILayout.RadioButton("Radio 1", ref active_id, 1);
                GUILayout.RadioButton("Radio 2", ref active_id, 2);
            };

            Application.Run(form);
        }
    }
}
