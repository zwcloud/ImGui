using ImGui;

namespace EmptyTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(400, 300, 250, 450))
        {
        }

        protected override void OnGUI()
        {
            GUI.Label(new Rect(0, 0, 100, 30), "Hello ImGui", "helloLabel");
        }
    }
}