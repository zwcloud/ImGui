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
            gui.BeginV();
                gui.Label(new Rect(0, 0, this.Size.Width, 40), "Demo project - Layout", "CaptionLabel");
                gui.BeginH();
                    gui.Button(new Rect(65, 30), "Button 0", "btn0");
                    gui.Button(new Rect(65, 30), "Button 1", "btn1");
                    gui.BeginV();
                        gui.Button(new Rect(65, 30), "Button 2", "btn2");
                        gui.BeginH();
                            gui.Button(new Rect(65, 30), "Button 3", "btn3");
                            gui.Button(new Rect(65, 30), "Button 4", "btn4");
                        gui.EndH();
                        gui.Button(new Rect(65, 30), "Button 5", "btn5");
                        gui.BeginV();
                        {
                            gui.Button(new Rect(120, 30), "点我", "buttonX0");
                            gui.TextBox(new Rect(120, 60), "输入一点东西", "TextBox0");
                            gui.Toggle(new Rect(120, 30), "好了吗?", false, "Toggle1");
                            gui.Radio(new Rect(120, 30), "项目1", "g0", false, "radio0");
                            gui.Radio(new Rect(120, 30), "项目2", "g0", false, "radio1");
                            gui.Radio(new Rect(120, 30), "项目3", "g0", false, "radio2");
                            gui.Button(new Rect(120, 30), "点我啊", "buttonX1");
                        }
                        gui.EndV();
                        gui.Button(new Rect(70, 30), "Button 6", "btn6");
                        gui.Button(new Rect(65, 30), "Button 7", "btn7");
                        gui.Button(new Rect(65, 30), "Button 8", "btn8");
                    gui.EndV();
                    gui.Button(new Rect(65, 30), "Button 9", "btn9");
                gui.EndH();
            gui.EndV();

            gui.Button(new Rect(100, 200, 180, 23), "I'm not a layouted button.", "btnx");

        }
    }

}
