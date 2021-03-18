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

            Application.Run(form, () =>
            {
                if (GUI.HoverButton(new Rect(0, 0, 100, 30), "Active when hovered"))
                {
                    Log.Msg("Active");
                }
            });
        }

        [Fact]
        public void LayoutedHoverButton()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();

            Application.Run(form, () =>
            {
                if (GUILayout.HoverButton("Active when hovered"))
                {
                    Log.Msg("Active");
                }
            });
        }

        [Fact]
        public void ShowTwoLayoutedButton()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();

            Application.Run(form, () =>
            {
                if (GUILayout.Button("Apply"))
                {
                    Log.Msg("clicked Apply");
                }
                if (GUILayout.Button("Revert"))
                {
                    Log.Msg("clicked Revert");
                }
            });
        }
    }
}
