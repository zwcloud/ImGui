using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 800))
        {
        }

        bool openA = true;
        bool openB = true;

        string texta = "aaaaaa";
        string textb = "bbbbbbbbb";

        protected override void OnGUI()
        {
            GUILayout.Begin("window A", ref openA);

            GUILayout.Label("Textboxes:");
            GUILayout.BeginChild("textboxes", new Size(200, 300), true, WindowFlags.Default);
            for (int i = 0; i < 12; i++)
            {
                GUILayout.Textbox("textBoxA"+i, 100, "TextBox A." + i);
            }
            GUILayout.EndChild();
            GUILayout.End();
        }
    }
}