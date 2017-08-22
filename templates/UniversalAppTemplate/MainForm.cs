using ImGui;
using ImGui.Common.Primitive;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        Demo demo = new Demo();

        protected override void OnGUI()
        {
            demo.OnGUI();
        }
    }
}

