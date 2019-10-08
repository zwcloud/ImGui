using Xunit;

namespace ImGui.ControlTest
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
        public void ShowDynamicFixedLabel()
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

        [Fact]
        public void ShowOneLayoutedLabel()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                GUILayout.Label("Some Text");
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowThreeLayoutedLabel()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                GUILayout.Label("Some Text##0");
                GUILayout.Label("Some Text##1");
                GUILayout.Label("Some Text##2");
            };

            Application.Run(form);
        }
    }
}
