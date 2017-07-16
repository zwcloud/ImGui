using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 600))
        {
        }

        bool openA = true;
        bool openB = true;

        string texta = "aaaaaa";
        string textb = "bbbbbbbbb";

        protected override void OnGUI()
        {
            GUILayout.Begin("window A", ref openA);
            for (int i = 0; i < 12; i++)
            {
                GUILayout.Textbox("textBoxA"+i, 100, "TextBox A." + i);
            }
            GUILayout.End();
        }
    }
}