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
            GUILayout.BeginHorizontal("HGroup~1");
            GUILayout.Button("Button 1");
            GUILayout.Button("Button 2");
            GUILayout.Button("Button 3");
            GUILayout.EndHorizontal();
            //GUILayout.Button("123");
            //GUI.Begin("test window", ref open);
            //GUI.End();
        }
    }
}

