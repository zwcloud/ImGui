using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        private readonly Demo demo = new Demo();

        protected override void OnGUI()
        {
            //demo.OnGUI();
            if (GUILayout.TreeNode("NodeA"))
            {
                if (GUILayout.TreeNode("NodeB"))
                {
                    GUILayout.Label("SomeLabel");
                }
                GUILayout.TreePop();
            }
            GUILayout.TreePop();
        }
    }
}

