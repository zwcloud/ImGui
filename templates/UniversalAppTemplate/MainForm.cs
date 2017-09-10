using ImGui;
using ImGui.Common.Primitive;

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
            //if(GUILayout.BeginChild("Child", (200,400), true, WindowFlags.Default))
            //{
            //    GUILayout.Button("Button~1");
            //    GUILayout.Button("Button~2");
            //    GUILayout.Button("Button~3");
            //    GUILayout.Button("Button~4");
            //    GUILayout.EndChild();
            //}

            demo.OnGUI();
        }
    }
}

