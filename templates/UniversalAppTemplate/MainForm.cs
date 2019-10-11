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
        protected override void OnGUI()
        {
            text = GUILayout.TextBox("t1", new Size(300, 500), text);
        }
    }
}

