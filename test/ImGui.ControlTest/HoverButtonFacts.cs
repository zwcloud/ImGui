using Xunit;

namespace ImGui.ControlTest
{
    public class HoverButton
    {
        [Fact]
        public void FixedHoverButton()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                if (GUI.Button(new Rect(0, 0, 100, 30), "Active when hovered"))
                {
                    Log.Msg("Active");
                }
            };

            Application.Run(form);
        }

        [Fact]
        public void LayoutedHoverButton()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                if (GUILayout.HoverButton("Active when hovered"))
                {
                    Log.Msg("Active");
                }
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowTwoLayoutedButton()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                if (GUILayout.Button("Apply"))
                {
                    Log.Msg("clicked Apply");
                }
                if (GUILayout.Button("Revert"))
                {
                    Log.Msg("clicked Revert");
                }
            };

            Application.Run(form);
        }
    }
}
