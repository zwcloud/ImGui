using Xunit;

namespace ImGui.ControlTest
{
    public class Image
    {
        [Fact]
        public void ShowOneFixedImage()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                GUI.Image(new Rect(0, 0, 100, 100), @"assets\images\logo.png");
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowDynamicFixedImage()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            var paths = new[] { @"assets\images\logo.png", @"assets\images\button.png" };
            var index = 0;
            form.OnGUIAction = () =>
            {
                GUI.Image(new Rect(0, 0, 100, 100), paths[index]);
                if (GUI.Button(new Rect(0, 120, 100, 30), "Change Image"))
                {
                    index = (index + 1) % 2;
                }
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowOneLayoutedImage()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                GUILayout.Image(@"assets\images\logo.png");
            };

            Application.Run(form);
        }
    }
}
