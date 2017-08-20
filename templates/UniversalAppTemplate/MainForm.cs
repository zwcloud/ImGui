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

        Demo demo = new Demo();

        bool open1, open2, open3;

        protected override void OnGUI()
        {
            if(TreeNode("MyTree1", ref open1))
            {
                Button("Item1_1");
                if (TreeNode("MyTree2", ref open2))
                {
                    Button("Item2_1");
                    if (TreeNode("MyTree3", ref open3))
                    {
                        Button("Item3_1");
                        Button("Item3_2");
                        Button("Item3_3");
                    }
                    TreePop();
                    Button("Item2_2");
                }
                TreePop();
            }
            TreePop();

            demo.OnGUI();
        }
    }
}

