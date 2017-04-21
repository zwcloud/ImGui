using System;
using ImGui;

namespace AndroidTemplate
{
    class MainForm : Form
    {
        //TestUI testUI = new TestUI();

        public MainForm(Point position, Size size) : base(new Rect(position, size)) { }

        bool DebugWindow1Open = true;
        bool DebugWindow2Open = true;

        protected override void OnGUI()
        {
            GUILayout.Begin("Debug1", ref DebugWindow1Open, new Point(60, 60), new Size(400, 400), 1, WindowFlags.Default);
            GUILayout.End();

            GUILayout.Begin("Debug2", ref DebugWindow2Open, new Point(100, 60), new Size(400, 300), 1, WindowFlags.Default);
            GUILayout.End();
        }
    }
}