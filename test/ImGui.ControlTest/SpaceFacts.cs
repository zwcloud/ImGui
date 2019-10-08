using Xunit;

namespace ImGui.ControlTest
{
    public class Space
    {
        [Fact]
        public void FixedSpace()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            bool open = true;
            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                GUI.Begin("Test", ref open, Point.Zero, new Size(100, 300));

                GUILayout.Button("button0");
                GUILayout.Space("space0", 10);
                GUILayout.Button("button1");
                GUILayout.Button("button2");
                GUILayout.Space("space1", 30);
                GUILayout.Button("button3");

                GUI.End();
            };

            Application.Run(form);
        }

        [Fact]
        public void FlexibleSpace()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            bool open = true;
            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                GUI.Begin("Test", ref open, Point.Zero, new Size(300, 300));

                GUILayout.BeginHorizontal("h0");
                {
                    GUILayout.Button("0", GUILayout.Width(20));
                    GUILayout.FlexibleSpace("space0", 1);
                    GUILayout.Button("1", GUILayout.Width(20));
                    GUILayout.FlexibleSpace("space1", 2);
                    GUILayout.Button("2", GUILayout.Width(20));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("h2");
                {
                    GUILayout.Button("0", GUILayout.Width(20));
                    GUILayout.FlexibleSpace("space0", 2);
                    GUILayout.Button("1", GUILayout.Width(20));
                    GUILayout.FlexibleSpace("space1", 1);
                    GUILayout.Button("2", GUILayout.Width(20));
                }
                GUILayout.EndHorizontal();
                GUI.End();
            };

            Application.Run(form);
        }
    }
}
