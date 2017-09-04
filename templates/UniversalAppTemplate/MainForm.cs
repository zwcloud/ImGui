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

        LayoutOptions smallRed = new LayoutOptions().FontColor(Color.Red).FontSize(20);

        protected override void OnGUI()
        {
            GUILayout.InputText("MyText", "text");
            //using (GUILayout.HScope("V1"))
            //{
            //    GUILayout.Button("MyButton1", this.smallRed);
            //    GUILayout.Button("MyButton2", new LayoutOptions().FontColor(Color.Blue).FontSize(40));
            //}
            //demo.OnGUI();
        }
    }
}

