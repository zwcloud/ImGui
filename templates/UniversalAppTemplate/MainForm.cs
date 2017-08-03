using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 800))
        {
        }

        private bool open = true;
        private string text = "123";

        bool openDemo = true;
        TestUI demoUI = new TestUI();

        bool openA = true;

        protected override void OnGUI()
        {
            //GUI.Begin("Demo", ref open, new Point(60, 60), new Size(400, 400), 1, WindowFlags.Default);
            //text = GUILayout.Textbox("MyTextBox", new Size(400, 600), text);
            //GUI.End();
            //GUI.Begin("ImGui Demo", ref openDemo);
            //demoUI.OnGUI();
            //GUI.End();

            GUI.Begin("window A", ref openA);
            GUILayout.BeginVertical("GroupA");
            GUILayout.Label("AAAA");
            GUILayout.Label("BBBB");
            GUILayout.Label("CCCC");
            GUILayout.Label("DDDD");
            GUILayout.EndVertical();
            
            GUILayout.BeginHorizontal("GroupB");
            GUILayout.Label("BAAAA啊");
            GUILayout.Label("BBBBB");
            GUILayout.Label("BCCCC");
            GUILayout.Label("BDDDD");
            GUILayout.EndHorizontal();
            GUI.End();
        }
    }
}