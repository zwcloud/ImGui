using Xunit;

namespace ImGui.ControlTest
{
    public class CollapsingHeader
    {
        [Fact]
        public void ShowOneCollapsingHeader()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            bool open = true;
            form.OnGUIAction = () =>
            {
                if (GUILayout.CollapsingHeader("Item List", ref open))
                {
                    GUILayout.Label("Item 0");
                    GUILayout.Label("Item 1");
                    GUILayout.Label("Item 2");
                    GUILayout.Button("Item 3");
                }
            };

            Application.Run(form);
        }
    }
}
