using Xunit;

namespace ImGui.ControlTest
{
    public class Window
    {
        [Fact]
        public void CreateAWindow()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm(new Rect(320, 180, 800, 600));
            bool open = true;
            form.OnGUIAction = () =>
            {
                GUI.Begin("test window", ref open, new Point(100, 100), new Size(100, 100));
                GUI.End();
            };

            Application.Run(form);
        }
    }
}
