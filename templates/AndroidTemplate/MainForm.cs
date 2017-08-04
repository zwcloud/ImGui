using System;
using ImGui;
using ImGui.Common.Primitive;

namespace AndroidTemplate
{
    class MainForm : Form
    {
        //TestUI testUI = new TestUI();

        public MainForm(Point position, Size size) : base(new Rect(position, size)) { }

        private bool open = true;
        private string text = "123XD";

        protected override void OnGUI()
        {
            GUI.Begin("Demo", ref open, new Point(60, 60), new Size(400, 400), 1, WindowFlags.Default);
            text = GUILayout.Textbox("MyTextBox", new Size(400, 600), text);
            GUI.End();
        }
    }
}