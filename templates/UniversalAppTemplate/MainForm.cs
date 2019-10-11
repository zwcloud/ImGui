using System;
using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        string text = @"
#include <iostream>
using namespace std;
int main() 
{
    cout << ""Hello, World!"";
    return 0;
}
";
        bool open = false;
        protected override void OnGUI()
        {
            if (GUILayout.TreeNode("single", ref open))
            {
                GUILayout.Label("11111");
                GUILayout.Label("2222");
                GUILayout.Label("333");
                GUILayout.Label("44");
            }
            GUILayout.TreePop();
        }
    }
}

