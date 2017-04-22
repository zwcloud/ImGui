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

        protected override void OnGUI()
        {
            GUILayout.Begin("Debug1", ref DebugWindow1Open, new Point(173, 142), new Size(400, 400), 1, WindowFlags.Default);
            GUILayout.End();

            GUILayout.Begin("Debug2", ref DebugWindow2Open, new Point(43, 256), new Size(400, 300), 1, WindowFlags.Default);
            GUILayout.End();
        }
    }
}