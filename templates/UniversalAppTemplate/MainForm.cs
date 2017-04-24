using ImGui;
using System.Diagnostics;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 600))
        {
        }

        //TestUI testUI = new TestUI();
        
        bool DebugWindow1Open = true;
        bool DebugWindow2Open = true;

        static float middleWeight = 0.3f;
        static float centerWeight = 0.3f;

        protected override void OnGUI()
        {
            GUILayout.Begin("Debug1", ref DebugWindow1Open, new Point(0, 0), new Size(400, 400), 1, WindowFlags.Default);

            var clientRect = GUILayout.GetWindowClientRect();
            GUILayout.BeginH("example_h2", GUIStyle.Default, GUILayout.Width(clientRect.Width), GUILayout.Height(clientRect.Height));
            {
                GUILayout.Button("Left", GUILayout.StretchWidth(1));
                GUILayout.BeginV("example_v1", GUIStyle.Default,
                    GUILayout.StretchWidth(1), GUILayout.Width(clientRect.Width), GUILayout.Height(clientRect.Height));
                {
                    GUILayout.Button("Middle");
                    GUILayout.Button("Center");
                    GUILayout.Button("Bottom");
                }
                GUILayout.EndV();
                GUILayout.Button("Right", GUILayout.StretchWidth(1));
            }
            GUILayout.EndH();

            GUILayout.End();

            GUILayout.Begin("Debug2", ref DebugWindow2Open, new Point(43, 256), new Size(400, 300), 1, WindowFlags.Default);
            GUILayout.Button("按钮4");
            GUILayout.Button("按钮5", GUILayout.ExpandWidth(true));
            GUILayout.Button("按钮6");
            GUILayout.End();
        }
    }
}