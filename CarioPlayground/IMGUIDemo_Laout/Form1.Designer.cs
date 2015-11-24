using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IMGUI;

namespace IMGUIDemo_Layout
{
    partial class Form1
    {
        protected override void OnGUI(GUI gui)
        {
#if true
            gui.BeginH();
                gui.LayoutButton("Button 0", "btn0");
                gui.LayoutButton("Button 1", "btn1");
                gui.BeginV();
                    gui.LayoutButton("Button 2", "btn2");
                    gui.BeginH();
                        gui.LayoutButton("Button 3", "btn3");
                        gui.LayoutButton("Button 4", "btn4");
                        gui.LayoutButton("Button 5", "btn5");
                    gui.EndH();
                    gui.LayoutButton("Button 6", "btn6");
                    gui.LayoutButton("Button 7", "btn7");
                gui.EndV();
                gui.LayoutButton("Button 8", "btn8");
                gui.LayoutButton("Button 9", "btn9");
            gui.EndH();
#else
            gui.Label(new Rect(0, 0, this.Size.Width, 40), "Demo project - Layout", "CaptionLabel");
            gui.BeginHorizontal(new Rect(this.Size.Width/2 - 200/2, 41, 200, 80));
            {
                gui.Button(new Rect(20, 30), "h0", "btn_h0");
                gui.Button(new Rect(30, 30), "h1", "btn_h1");
                gui.Button(new Rect(40, 30), "h2", "btn_h2");
                gui.Button(new Rect(50, 30), "h3", "btn_h3");
                gui.Button(new Rect(40, 30), "h4", "btn_h4");
                gui.Button(new Rect(30, 30), "h5", "btn_h5");
                gui.Button(new Rect(20, 30), "h6", "btn_h6");
            }
            gui.EndHorizontal();

            gui.BeginVertical(new Rect(0, 80, 80, 200));
            {
                gui.Button(new Rect(20, 20), "v0", "btn_v0");
                gui.Button(new Rect(20, 30), "v1", "btn_v1");
                gui.Button(new Rect(20, 40), "v2", "btn_v2");
                gui.Button(new Rect(20, 50), "v3", "btn_v3");
                gui.Button(new Rect(20, 40), "v4", "btn_v4");
                gui.Button(new Rect(20, 30), "v5", "btn_v5");
                gui.Button(new Rect(20, 20), "v6", "btn_v6");
            }
            gui.EndVertical();

            gui.BeginVertical(new Rect(this.Size.Width / 2 - 200 / 2, 80, 80, 200));
            {
                gui.Button(new Rect(120, 30), "点我", "buttonX0");
                gui.TextBox(new Rect(120, 60), "输入一点东西", "TextBox0");
                gui.Toggle(new Rect(120, 30), "好了吗?", false, "Toggle1");
                gui.Radio(new Rect(120, 30), "项目1", "g0", false, "radio0");
                gui.Radio(new Rect(120, 30), "项目2", "g0", false, "radio1");
                gui.Radio(new Rect(120, 30), "项目3", "g0", false, "radio2");
                gui.Button(new Rect(120, 30), "点我啊", "buttonX1");
            }
            gui.EndVertical();
#endif   
        }
    }

}
