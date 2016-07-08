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
            var w = this.Size.Width;
            var h = this.Size.Height;
            gui.BeginV();
            {
                gui.TitleBar(new Rect(0, 0, w, h*0.08), null, "ImGui Project 1", "ApplicationTitle");
                gui.Button(new Rect(0, 36, 300, 30), "Hello World!", "Button0");
            }
            gui.EndV();
        }
    }

}
