using Xunit;

namespace ImGui.ControlTest
{
    public class TreeNode
    {
        [Fact]
        public void ShowATreeNode()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            var open = true;
            form.OnGUIAction = () =>
            {
                if (GUILayout.TreeNode("single", ref open))
                {
                    GUILayout.Label("11111");
                    GUILayout.Label("2222");
                    GUILayout.Label("333");
                    GUILayout.Label("44");
                    GUILayout.TreePop();
                }
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowAThreeLevelTreeNode()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            bool open1 = true, open2 = true, open3 = false;
            bool selected1 = false, selected2 = false;
            form.OnGUIAction = () =>
            {
                if (GUILayout.TreeNode("root", ref open1))
                {
                    GUILayout.Label("11111");
                    if (GUILayout.TreeNode("child", ref open2))
                    {
                        GUILayout.Label("44444");
                        if (GUILayout.TreeNode("grandchild", ref open3))
                        {
                            GUILayout.Button("555555");
                            GUILayout.TreePop();
                        }
                        GUILayout.TreePop();
                    }
                    selected1 = GUILayout.Selectable("2222", selected1);
                    GUILayout.Label("33333");
                    selected2 = GUILayout.Selectable(".NET Core", selected2);
                    GUILayout.TreePop();
                }
            };

            Application.Run(form);
        }
    }
}
