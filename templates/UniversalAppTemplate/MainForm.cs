using ImGui;
using ImGui.Common.Primitive;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 800))
        {
        }

        bool showDemoWindow = false;
        TestUI demoUI = new TestUI();

        private bool showAnotherWindow = true;
        private bool openGroupA = true, openGroupB = false;

        protected override void OnGUI()
        {
            showDemoWindow = GUILayout.ToggleButton("Show Demo Window", showDemoWindow);
            if(showDemoWindow)
            {
                GUI.Begin("ImGui Demo", ref showDemoWindow);
                demoUI.OnGUI();
                GUI.End();
            }

            if(showAnotherWindow)
            {
                GUI.Begin("Another Window", ref showAnotherWindow);
                if (GUILayout.CollapsingHeader("MyGroupA", ref openGroupA))
                {
                    GUILayout.BeginVertical("GroupA");
                    GUILayout.Label("AAAA");
                    GUILayout.Label("BBBB");
                    GUILayout.Label("CCCC");
                    GUILayout.Label("DDDD");
                    GUILayout.EndVertical();
                }

                if (GUILayout.CollapsingHeader("MyGroupB", ref openGroupB))
                {
                    GUILayout.BeginHorizontal("GroupB");
                    GUILayout.Label("BAAAA啊");
                    GUILayout.Label("BBBBB");
                    GUILayout.Label("BCCCC");
                    GUILayout.Label("BDDDD");
                    GUILayout.EndHorizontal();
                }
                GUI.End();
            }

        }
    }
}
