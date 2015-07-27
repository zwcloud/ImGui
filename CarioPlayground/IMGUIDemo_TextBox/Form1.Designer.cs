using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMGUI;

namespace IMGUIDemo
{
    partial class Form1
    {
        protected override void OnGUI(GUI gui)
        {
            gui.TextBox(new Rect(10, 10, 110, 30), "text1 啊啊", "Textbox0");

        }
    }

}
