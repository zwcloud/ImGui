using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 800))
        {
        }

        bool openDemo = true;
        TestUI demoUI = new TestUI();

        bool openA = true;

        protected override void OnGUI()
        {
            //GUILayout.Begin("ImGui Demo", ref openDemo);
            //demoUI.OnGUI();
            //GUILayout.End();

            GUILayout.Begin("window A", ref openA);
            GUILayout.BeginVertical("GroupA");
            GUILayout.Label("AAAA");
            GUILayout.Label("BBBB");
            GUILayout.Label("CCCC");
            GUILayout.Label("DDDD");
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal("GroupB");
            GUILayout.Label("BAAAA");
            GUILayout.Label("BBBBB");
            GUILayout.Label("BCCCC");
            GUILayout.Label("BDDDD");
            GUILayout.EndHorizontal();
            GUILayout.End();
        }
    }
}