using ImGui;
using ImGui.Common.Primitive;

namespace AndroidTemplate
{
    class MainForm : Form
    {
        public MainForm(Point position, Size size) : base(new Rect(position, size)) { }

        Demo demo = new Demo();

        protected override void OnGUI()
        {
            demo.OnGUI();
        }
    }
}
