using ImGui;
using System.Diagnostics;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 600))
        {
        }

        TestUI testUI = new TestUI();
        
        bool DebugWindow1Open = true;
        bool DebugWindow2Open = true;

        protected override void OnGUI()
        {
            //GUILayout.Begin("Debug1", ref DebugWindow1Open, new Point(0, 0), new Size(400, 400), 1, WindowFlags.Default);
            //
            //var clientRect = GUILayout.GetWindowClientRect();
            //GUILayout.BeginHorizontal("example_h2", GUIStyle.Default, GUILayout.Width(clientRect.Width), GUILayout.Height(clientRect.Height));
            //{
            //    GUILayout.Button("Left", GUILayout.StretchWidth(1));
            //    GUILayout.BeginVertical("example_v1", GUIStyle.Default,
            //        GUILayout.StretchWidth(1), GUILayout.Width(clientRect.Width), GUILayout.Height(clientRect.Height));
            //    {
            //        GUILayout.Button("Middle");
            //        GUILayout.Button("Center");
            //        GUILayout.Button("Bottom");
            //    }
            //    GUILayout.EndVertical();
            //    GUILayout.Button("Right", GUILayout.StretchWidth(1));
            //}
            //GUILayout.EndHorizontal();
            //
            //GUILayout.End();

            GUILayout.Begin("Debug2", ref DebugWindow2Open, new Point(43, 256), new Size(400, 300), 1, WindowFlags.Default);
            testUI.OnGUI();
            GUILayout.End();
        }
    }
}