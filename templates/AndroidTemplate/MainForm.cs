using ImGui;

namespace AndroidTemplate
{
    internal class MainForm : Form
    {
        public MainForm(Point position, Size size) : base(new Rect(position, size)) { }
        
        private static readonly Demo demo = new Demo();

        static string[] text = {
            "A", "BB", "CCC"
        };

        public void OnGUI()
        {
            demo.OnGUI();
            GUI.Begin("Combobox demo", new Point(10, 10), new Size(400, 400));
            GUI.ComboBox(new Rect(10, 10, 200, 20), text);
            GUI.End();
        }
    }
}
