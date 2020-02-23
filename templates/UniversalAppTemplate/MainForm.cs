using ImGui;

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
            //GUILayout.Label("Hello ImGui,");
            //demo.OnGUI();
            GUILayout.BeginVertical("V", GUILayout.ExpandWidth(true).ExpandHeight(true));
                GUILayout.BeginHorizontal("H", GUILayout.Width(100).ExpandHeight(true));
                    GUILayout.Button("A", GUILayout.Width(120).Height(120));
                    GUILayout.Button("B", GUILayout.Width(120).Height(120));
                    GUILayout.Button("C", GUILayout.Width(120).Height(120));
                GUILayout.EndHorizontal();
                GUILayout.Button("D", GUILayout.Width(120).Height(120));
            GUILayout.EndVertical();
        }
    }
}

