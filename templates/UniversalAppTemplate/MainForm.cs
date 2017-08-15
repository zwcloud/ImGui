using ImGui;
using ImGui.Common.Primitive;

using static ImGui.GUILayout;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        bool myWindowOpen = true;

        protected override void OnGUI()
        {
            GUI.Begin("MyWindow", ref myWindowOpen);
            Label("Three button of default size.");
            BeginHorizontal("H~~~1");
            Button("1");
            Button("2");
            Button("3");
            EndHorizontal();
            Label("Three fixed-width (100 pixels) buttons.");
            BeginHorizontal("H~~~2");
            PushFixedWidth(100);
            Button("1");
            Button("2");
            Button("3");
            PopStyleVar(2);
            EndHorizontal();
            Label("Three stretched sized buttons with 1/2/3 stretch factor.");
            BeginHorizontal("H~~~3");
            PushHStretchFactor(1);
            Button("1");
            PopStyleVar();
            PushHStretchFactor(2);
            Button("2");
            PopStyleVar();
            PushHStretchFactor(3);
            Button("3");
            PopStyleVar();
            EndHorizontal();
            GUI.End();
        }
    }
}

