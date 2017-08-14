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

        protected override void OnGUI()
        {
            Label("Three default sized button.");
            BeginHorizontal("H~~~1");
            Button("1");
            Button("2");
            Button("3");
            EndHorizontal();
            Label("Three fixed sized(100) button.");
            BeginHorizontal("H~~~2");
            PushFixedWidth(100);
            Button("1");
            Button("2");
            Button("3");
            PopStyleVar(2);
            EndHorizontal();
            Label("Three stretched sized button with 1/2/3 stretch factor.");
            BeginHorizontal("H~~~3");
            PushHStretchFactor(1);
            Button("1");
            PopStyleVar();
            PushHStretchFactor(2);
            Button("2");
            PopStyleVar();
            PushHStretchFactor(3);
            Textbox("3", 60, "3");
            PopStyleVar();
            EndHorizontal();
        }
    }
}

