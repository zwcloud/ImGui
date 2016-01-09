using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ImGui;

namespace ImGuiIDemo_Layout
{
    partial class Form1
    {
        protected override void OnGUI(GUI gui)
        {
            var w = this.Size.Width;
            var h = this.Size.Height;
            gui.BeginV();
            {
                //gui.TitleBar(new Rect(0, 0, w, h*0.08), null, "Cloud Editor", "ApplicationTitle");
                gui.Space(new Rect(0, 0, w, h * 0.02));
                gui.BeginH();
                {
                    gui.Button(new Rect(w * 0.1, h * 0.1), "dummy0", "dummy0");
                    gui.Button(new Rect(w * 0.1, h * 0.1), "dummy1", "dummy1");
                    gui.Button(new Rect(w * 0.1, h * 0.1), "dummy2", "dummy2");
                    gui.Button(new Rect(w * 0.1, h * 0.1), "dummy3", "dummy3");
                    gui.Button(new Rect(w * 0.1, h * 0.1), "dummy4", "dummy4");
                    gui.Button(new Rect(w * 0.1, h * 0.1), "dummy5", "dummy5");
                    gui.Button(new Rect(w * 0.1, h * 0.1), "dummy6", "dummy6");
                    gui.Button(new Rect(w * 0.1, h * 0.1), "dummy7", "dummy7");
                    gui.Button(new Rect(w * 0.1, h * 0.1), "dummy8", "dummy8");
                    gui.Button(new Rect(w * 0.1, h * 0.1), "dummy9", "dummy9");
                }
                gui.EndH();
                gui.BeginH();
                {
                    gui.Button(new Rect(w * 0.2, h * 0.8), "dummy11", "dummy11");
                    gui.BeginV();
                    {
                        gui.Button(new Rect(w * 0.8, h * 0.4), "dummy12", "dummy12");
                        gui.Button(new Rect(w * 0.8, h * 0.2), "dummy13", "dummy13");
                        gui.BeginH();
                        {
                            gui.Button(new Rect(w * 0.8 * 0.5, h * 0.2), "dummy14", "dummy14");
                            gui.Button(new Rect(w * 0.8 * 0.5, h * 0.2), "dummy15", "dummy15");
                        }
                        gui.EndH();
                    }
                    gui.EndV();
                }
                gui.EndH();
            }
            gui.EndV();
        }
    }

}
