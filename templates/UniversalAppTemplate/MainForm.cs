using ImGui;
using ImGui.Common.Primitive;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        static double f = 0.0f;

        private bool showAnotherWindow = true;

        bool showDemoWindow = false;
        TestUI demoUI = new TestUI();

        protected override void OnGUI()
        {
            // 1. Show a simple window
            // Tip: if we don't call GUI.Begin()/GUI.End() the widgets appears in a window automatically called "Debug"
            {
                GUILayout.Label("Hello, world!");
                f = GUILayout.Slider("float", f, 0, 1);
                //TODO color control
                if (GUILayout.Button("Show Demo Window")) showDemoWindow = !showDemoWindow;
                if (GUILayout.Button("Show Another Window")) showAnotherWindow = !showAnotherWindow;
                //var fps = Form.current.uiContext.fps;
                //GUILayout.Label(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000.0f / fps, fps)); //removed, will be added again when textmesh cache is solved
            }

            // 2. Show another simple window, this time using an explicit Begin/End pair
            if (showAnotherWindow)
            {
                GUI.Begin("Another Window", ref showAnotherWindow, (70, 450), (400, 100));
                GUILayout.Label("Hello");
                GUI.End();
            }

            // 3. Show the ImGui demo window. Most of the sample code is in demoUI.ShowTestWindow()
            if (showDemoWindow)
            {
                demoUI.ShowTestWindow(ref showDemoWindow);
            }

        }
    }
}
