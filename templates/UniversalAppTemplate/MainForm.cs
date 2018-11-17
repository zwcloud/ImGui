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
        private bool clicked;
        protected override void OnGUI()
        {
            if (GUILayout.Button("Button"))
            {
                clicked = !clicked;
            }
            if (clicked)
            {
                GUILayout.Button("123");
                GUILayout.Label("345");
            }

            //GUILayout.Button("123");
            //GUI.Begin("test window", ref open);
            //GUI.End();
        }
    }
}

