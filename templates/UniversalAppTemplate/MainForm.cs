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
            GUILayout.Label("11111");
            if (GUILayout.BeginChild("Sub1", 0, GUILayout.Height(300).ExpandWidth(true)))
            {
                GUILayout.Label("2222#1");
                GUILayout.Label("2222#2");
                GUILayout.Label("2222#3");
                GUILayout.Label("2222#4");
                GUILayout.Label("2222#5");
                GUILayout.Label("2222#6");
                GUILayout.Label("2222#7");

                GUILayout.EndChild();
            }

            if (GUILayout.TreeNode("single", ref open))
            {
                GUILayout.Label("2222");
                GUILayout.Label("333");
                GUILayout.Label("44");
            }
            GUILayout.TreePop();
        }
    }
}

