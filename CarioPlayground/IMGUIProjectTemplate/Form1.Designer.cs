using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ImGui;

namespace ImGuiProjectTemplate
{
    partial class Form1
    {
        protected override void OnGUI(GUI gui)
        {
            gui.Button(new Rect(0, 36, 300, 30), "Hello World!", "Button0");
        }
    }

}
