using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 600))
        {
        }

        bool open = true;

        string texta = "aaaaaa";
        string textb = "bbbbbbbbb";

        protected override void OnGUI()
        {
            GUILayout.Begin("UnnamedWindow", ref open);
            texta = GUILayout.Textbox("a", 200, texta);
            textb = GUILayout.Textbox("b", 200, textb);
            GUILayout.Button("TestB");
            GUILayout.End();
        }
    }
}