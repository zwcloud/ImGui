using Xunit;

namespace ImGui.ControlTest
{
    public class Box
    {
        [Fact]
        public void ShowOneFixedBox()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            Application.Run(form, () =>
            {
                GUI.Box(new Rect(0, 0, 100, 30), "Some Text");
            });
        }

        [Fact]
        public void ShowDynamicFixedBox()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            var text = "before##1";

            Application.Run(form, () =>
            {
                GUI.Box(new Rect(0, 0, 100, 30), text);
                if (GUI.Button(new Rect(0, 40, 120, 30), "Change Text"))
                {
                    text = text == "before##1" ? "after##0" : "before##1";
                }
            });
        }

        [Fact]
        public void ShowOneLayoutedBox()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            Application.Run(form, () =>
            {
                GUILayout.Box("Some Text");
            });
        }

        [Fact]
        public void ShowThreeLayoutedBox()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            Application.Run(form, () =>
            {
                GUILayout.Box("Some Text##0");
                GUILayout.Box("Some Text##1");
                GUILayout.Box("Some Text##2");
            });
        }
    }
}
