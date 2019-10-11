using Xunit;

namespace ImGui.ControlTest
{
    public class ListBox
    {
        [Fact]
        public void ShowOneLayoutedListBox()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            var items = new []{ 1, 22, 333, 4444, 55555 };
            var selectedIndex = 0;
            form.OnGUIAction = () =>
            {
                selectedIndex = GUILayout.ListBox("select this", items, selectedIndex);
            };

            Application.Run(form);
        }
    }
}
