using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(400, 300, 800, 600))
        {
        }

        TestUI testUI = new TestUI();

        protected override void OnGUI()
        {
            GUILayout.Begin("Debug", new Point(60, 60), new Size(400, 400));
            GUILayout.End();
            //testUI.OnGUI();
        }
    }
}