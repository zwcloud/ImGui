using Xunit;

namespace ImGui.UnitTest
{
    public class WindowFacts
    {
        public class TheRunMethod
        {
            [Fact]
            public void TheWindowShouldBeDrawn()
            {
                Application.Init();

                var form = new MainForm();
                bool open = true;
                form.OnGUIAction = () =>
                {
                    GUI.Begin("test window", ref open);
                    GUI.End();
                };

                Application.Run(form);
            }
        }
    }
}