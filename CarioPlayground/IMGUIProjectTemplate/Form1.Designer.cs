using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IMGUI;

namespace IMGUIProjectTemplate
{
    partial class Form1
    {
        protected override void OnGUI(GUI gui)
        {
            gui.Label(new Rect(36, 36, 110, 30), "Hello World!", "Label0");
        }
    }

}
