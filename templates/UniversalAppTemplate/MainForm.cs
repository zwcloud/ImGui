using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        Demo demo = new Demo();

        bool open = true;
        protected override void OnGUI()
        {
            GUILayout.Label("Debug Window Text");
            //demo.OnGUI();
            GUI.Begin("Test", ref open, new Point(200, 200), new Size(200,300), 0.5);
            GUILayout.Label("Test Window Text");
            GUI.End();
        }
    }
}

