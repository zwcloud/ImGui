using System;
using ImGui;

namespace AndroidTemplate
{
    class MainForm : Form
    {
        TestUI testUI = new TestUI();

        public MainForm(Point position, Size size) : base(new Rect(position, size)) { }

        protected override void OnGUI()
        {
            testUI.OnGUI();
        }
    }
}