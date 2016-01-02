using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ImGui;

namespace ImGuiDemo_TextBox
{
    partial class Form1
    {
        protected override void OnGUI(GUI gui)
        {
            gui.Label(new Rect(0, 0, this.Size.Width, 40), "IMGUI Demo project - TextBox", "CaptionLabel");
            gui.TextBox(new Rect(10, 50, 110, 30), "Hello world!", "Textbox0");
            gui.TextBox(new Rect(10, 150, 110, 130), "Hello world!", "Textbox1");
        }
    }

}
