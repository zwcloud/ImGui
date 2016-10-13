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
        protected override void OnGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(50));
            {
                GUILayout.Label("IMGUI Demo project");
                GUILayout.Label(" ", GUILayout.ExpandWidth(true));

                if (GUILayout.Button("🗕", GUILayout.Width(46), GUILayout.Height(29)))
                {
                    Form.current.Minimize();
                }
                if (Form.current.FormState == FormState.Normal)
                {
                    if (GUILayout.Button("🗖", GUILayout.Width(46), GUILayout.Height(29)))
                    {
                        Form.current.Maximize();
                    }
                }
                else
                {
                    if (GUILayout.Button("🗗", GUILayout.Width(46), GUILayout.Height(29)))
                    {
                        Form.current.Normalize();
                    }
                }
                if (GUILayout.Button("🗙", GUILayout.Width(46), GUILayout.Height(29)))
                {
                    Form.current.RequestClose();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical(Skin.current.Box);
                GUILayout.Label("auto-sized");
                GUILayout.Button("Top Button");
                GUILayout.BeginHorizontal();
                    GUILayout.Button("Middle Button##1");
                    GUILayout.Button("Middle Button##2");
                    GUILayout.Button("Middle Button##3");
                GUILayout.EndHorizontal();
                GUILayout.Button("Bottom Button");
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.BeginVertical(Skin.current.Box);
            {
                GUILayout.Label("fixed-size");
                GUILayout.BeginHorizontal();
                    GUILayout.Button("Width:200", GUILayout.Width(200));
                    GUILayout.Button("Width:100", GUILayout.Width(100));
                    GUILayout.Button("Width:300", GUILayout.Width(300));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.BeginVertical(Skin.current.Box);
            {
                GUILayout.Label("stretched: evenly spaced");
                GUILayout.BeginHorizontal();
                    GUILayout.Button("Button0", GUILayout.ExpandWidth(true));
                    GUILayout.Button("Button1", GUILayout.ExpandWidth(true));
                    GUILayout.Button("Button2", GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.BeginVertical(Skin.current.Box);
            {
                GUILayout.Label("stretched: spaced with factors");
                GUILayout.BeginHorizontal(GUILayout.Width(500));
                GUILayout.Button("Stretch:1", GUILayout.StretchWidth(1));
                GUILayout.Button("Stretch:2", GUILayout.StretchWidth(2));
                GUILayout.Button("Stretch:3", GUILayout.StretchWidth(3));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }

}
