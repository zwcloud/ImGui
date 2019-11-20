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

        [Fact]
        public void ShowTwoCollapsingHeader()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            bool open1 = false, open2 = false;
            form.OnGUIAction = () =>
            {
                if (GUILayout.CollapsingHeader("Header 1", ref open1))
                {
                    GUILayout.Label("Item A");
                    GUILayout.Label("Item B");
                }
                if (GUILayout.CollapsingHeader("Header 2", ref open2))
                {
                    GUILayout.Label("Item C");
                    GUILayout.Label("Item D");
                }
            };

            Application.Run(form);
        }
    }
}
