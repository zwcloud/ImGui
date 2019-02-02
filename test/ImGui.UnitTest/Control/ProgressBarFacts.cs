using ImGui.Common.Primitive;
using Xunit;

namespace ImGui.UnitTest
{
    public class ProgressBarFacts
    {
        public class ProgressBar
        {
            [Fact]
            public void ShowOneProgressBar()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm();
                bool open = true;
                form.OnGUIAction = () =>
                {
                    GUILayout.ProgressBar("Progress", 0.4, new Size(300, 40));
                };

                Application.Run(form);
            }
        }
    }
}