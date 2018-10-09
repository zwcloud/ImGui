using ImGui;
using ImGui.Common.Primitive;

namespace AndroidTemplate
{
    class MainForm : Form
    {
        public MainForm(Point position, Size size) : base(new Rect(position, size)) { }

        protected override void OnGUI()
        {
            GUILayout.Button("123");
        }
    }
}
