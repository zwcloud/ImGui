using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 600))
        {
        }

        bool open = true;

        protected override void OnGUI()
        {
            GUILayout.Begin("UnnamedWindow", ref open);
            GUILayout.Textbox("a", new Size(200, 40), "aaaaaa");
            GUILayout.Textbox("b", new Size(200, 40), "bbbbbbbbb");
            GUILayout.Button("TestB");
            GUILayout.End();
        }
    }
}