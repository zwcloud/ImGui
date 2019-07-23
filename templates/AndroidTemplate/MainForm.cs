using ImGui;

namespace AndroidTemplate
{
    internal class MainForm : Form
    {
        public MainForm(Point position, Size size) : base(new Rect(position, size)) { }

        protected override void OnGUI()
        {
            GUILayout.Button("123");
        }
    }
}
