using ImGui.Common.Primitive;
using Xunit;

namespace ImGui.UnitTest
{
    public partial class GUIFacts
    {
        public class TheButtonMethod
        {
            [Fact]
            public void ShowAButton()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm();
                form.OnGUIAction = () =>
                {
                    if (GUI.Button(new Rect(5, 5, 100, 30), "Apply"))
                    {
                        Log.Msg("clicked");
                    }
                };

                Application.Run(form);
            }
        }
    }
}