using ImGui.Common.Primitive;
using Xunit;

namespace ImGui.UnitTest
{
    public class LabelFacts
    {
        public class Label
        {
            [Fact]
            public void ShowOneFixedLabel()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm();
                form.OnGUIAction = () =>
                {
                    GUI.Label(new Rect(0, 0, 100, 30), "Some Text");
                };

                Application.Run(form);
            }

            [Fact]
            public void ShowDynamicLabel()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm();
                var text = "before#1";
                form.OnGUIAction = () =>
                {
                    GUI.Label(new Rect(0, 0, 100, 30), text);
                    if (GUI.Button(new Rect(0, 40, 100, 30), "++"))
                    {
                        text = text == "before#1" ? "after#0" : "before#1";
                    }
                };

                Application.Run(form);
            }
        }
    }
}