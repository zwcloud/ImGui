using ImGui;

namespace AndroidTemplate
{
    internal class MainForm : Form
    {
        public MainForm(Point position, Size size) : base(new Rect(position, size)) { }

        public void OnGUI()
        {
            GUILayout.Button("123");
        }
    }
}
