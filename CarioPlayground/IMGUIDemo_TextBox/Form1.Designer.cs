using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IMGUI;

namespace IMGUIDemo_TextBox
{
    partial class Form1
    {
        protected override void OnGUI(GUI gui)
        {
            gui.TextBox(new Rect(10, 10, 110, 30), "Hello world!", "Textbox0");
            gui.TextBox(new Rect(10, 110, 110, 130), "Hello world!", "Textbox1");
        }
    }

}
