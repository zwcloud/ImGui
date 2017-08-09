using ImGui;
using ImGui.Common.Primitive;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        bool showDemoWindow = false;
        TestUI demoUI = new TestUI();

        private bool showAnotherWindow = true;

        protected override void OnGUI()
        {
            showDemoWindow = GUILayout.ToggleButton("Show Demo Window", showDemoWindow);
            if(showDemoWindow)
            {
                GUI.Begin("ImGui Demo", ref showDemoWindow, (650, 20), (550, 680));
                demoUI.OnGUI();
                GUI.End();
            }

            if(showAnotherWindow)
            {
                GUI.Begin("Another Window", ref showAnotherWindow, (70, 450), (400, 100));
                GUILayout.Label("Hello");
                GUI.End();
            }

        }
    }
}
