using ImGui;
using ImGui.Common.Primitive;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        private bool open;
        protected override void OnGUI()
        {
            //GUILayout.Button("123");
            GUI.Begin("test window", ref open);
            GUI.End();
        }
    }
}

