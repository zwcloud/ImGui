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
            BeginHorizontal("H~~~1");
            PushHStretchFactor(1);
            PushFixedWidth(200);//pushed 2 style modifiers
            Button("1");
            PopStyleVar(2);
            Space("space", 100);
            PushBorder((1, 2, 3, 4));
            Button("2");
            PopStyleVar(4);
            PopStyleVar();
            FlexibleSpace("spring", 2);
            PushHStretchFactor(2);
            Button("3");
            PopStyleVar();
            EndHorizontal();
        }
    }
}

