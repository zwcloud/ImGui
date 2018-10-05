using ImGui.Common.Primitive;
using Xunit;

namespace ImGui.UnitTest
{
    public partial class GUIFacts
    {
        public class TheButtonMethod
        {
            [Fact]
            public void ShowOneFixedButton()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm();
                form.OnGUIAction = () =>
                {
                    if (GUI.Button(new Rect(5, 5, 100, 30), "Apply"))
                    {
                        Log.LogToVSOutput("clicked");
                    }
                };

                Application.Run(form);
            }

            [Fact]
            public void ShowTwoFixedButtons()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm();
                form.OnGUIAction = () =>
                {
                    if (GUI.Button(new Rect(5, 5, 100, 30), "Button1"))
                    {
                        Log.Msg("clicked Button1");
                    }
                    if (GUI.Button(new Rect(5, 50, 100, 40), "Button2"))
                    {
                        Log.Msg("clicked Button2");
                    }
                };

                Application.Run(form);
            }

            [Fact]
            public void ShowOneLayoutButton()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm();
                form.OnGUIAction = () =>
                {
                    if (GUILayout.Button("Apply"))
                    {
                        Log.Msg("clicked");
                    }
                };

                Application.Run(form);
            }

        }
    }
}