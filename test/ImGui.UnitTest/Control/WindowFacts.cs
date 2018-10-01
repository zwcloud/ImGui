using ImGui.Common.Primitive;
using Xunit;

namespace ImGui.UnitTest
{
    public partial class GUIFacts
    {
        public class TheBeginEndMethods
        {
            [Fact]
            public void TheWindowShouldBeDrawn()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm(new Rect(320, 180, 800,600));
                bool open = true;
                form.OnGUIAction = () =>
                {
                    //GUI.Begin("test window", ref open);
                    //GUI.End();
                };

                Application.Run(form);
            }
        }
    }
}