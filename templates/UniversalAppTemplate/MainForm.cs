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
            GUILayout.Begin("window A", ref openA, Point.Zero, new Size(400, 300), 1, WindowFlags.ShowBorders);
GUILayout.Textbox("textBoxA", 100, "TextBox A.");
GUILayout.End();

GUILayout.Begin("window B", ref openB);
GUILayout.Textbox("textBoxB", 120, "TextBox B.");
GUILayout.End();
        }
    }
}