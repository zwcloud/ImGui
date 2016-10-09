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
                GUILayout.Label("IMGUI Demo project", "CaptionLabel");
                GUILayout.Label(" ", "_space", GUILayout.ExpandWidth(true));

                if (GUILayout.Button("🗕", "Minimize", GUILayout.Width(46), GUILayout.Height(29)))
                {
                    Form.current.Minimize();
                }
                if (Form.current.FormState == FormState.Normal)
                {
                    if (GUILayout.Button("🗖", "Maximize", GUILayout.Width(46), GUILayout.Height(29)))
                    {
                        Form.current.Maximize();
                    }
                }
                else
                {
                    if (GUILayout.Button("🗗", "Restore", GUILayout.Width(46), GUILayout.Height(29)))
                    {
                        Form.current.Normalize();
                    }
                }
                if (GUILayout.Button("🗙", "Close", GUILayout.Width(46), GUILayout.Height(29)))
                {
                    Form.current.RequestClose();
                }
            }
            GUILayout.EndHorizontal();

            //GUILayout.BeginVertical();
            //    GUILayout.Button("Hello mybutton", "ButtonTop");
            //    GUILayout.BeginHorizontal();
            //        GUILayout.Button("Hello mybutton", "ButtonMiddle1");
            //        GUILayout.Button("Hello mybutton", "ButtonMiddle2");
            //        GUILayout.Button("Hello mybutton", "ButtonMiddle3");
            //    GUILayout.EndHorizontal();
            //    GUILayout.Button("Hello mybutton", "ButtonDown");
            //GUILayout.EndVertical();

            //GUILayout.Button("dummy0", "dummy0");
            //GUILayout.Button("dummy1", "dummy1");
            //GUILayout.Button("dummy2", "dummy2");

            //GUILayout.BeginVertical();
            //{
            //    GUILayout.Button("dummy0", "dummy0");
            //    GUILayout.Button("dummy1", "dummy1");
            //    GUILayout.Button("dummy2", "dummy2");
            //}
            //GUILayout.EndVertical();

            //GUILayout.BeginHorizontal();
            //{
            //    GUILayout.Button("dummy0", "dummy0");
            //    GUILayout.Button("dummy1", "dummy1");
            //    GUILayout.Button("dummy2", "dummy2");
            //}
            //GUILayout.EndHorizontal();

            //GUILayout.BeginVertical();//A
            //{
            //    GUILayout.BeginHorizontal();//B
            //    {
            //        GUILayout.Button("dummy0", "dummy0");
            //    }
            //    GUILayout.EndHorizontal();
            //    GUILayout.BeginHorizontal();//C
            //    {
            //        GUILayout.Button("dummy1", "dummy1");
            //        GUILayout.BeginVertical();//D
            //        {
            //            GUILayout.Button("dummy2", "dummy2");
            //        }
            //        GUILayout.EndVertical();
            //    }
            //    GUILayout.EndHorizontal();
            //}
            //GUILayout.EndVertical();

            //GUILayout.BeginVertical();
            //{
            //    GUILayout.BeginHorizontal();
            //    {
            //        GUILayout.BeginVertical();
            //        {
            //            GUILayout.Button("dummy0", "dummy0");
            //        }
            //        GUILayout.EndVertical();
            //        GUILayout.BeginVertical();
            //        {
            //            GUILayout.Button("dummy1", "dummy1");
            //            GUILayout.BeginHorizontal();
            //            {
            //                GUILayout.Button("dummy2", "dummy2");
            //            }
            //            GUILayout.EndHorizontal();
            //        }
            //        GUILayout.EndVertical();
            //    }
            //    GUILayout.EndHorizontal();
            //}
            //GUILayout.EndVertical();

            //GUILayout.BeginVertical(Skin.current.Box);
            //{
            //    GUILayout.BeginHorizontal();
            //    {
            //        GUILayout.Button("dummy0", "dummy0");
            //        GUILayout.Button("dummy1", "dummy1");
            //        GUILayout.Button("dummy2", "dummy2");
            //        GUILayout.Button("dummy3", "dummy3");
            //        GUILayout.Button("dummy4", "dummy4");
            //        GUILayout.Button("dummy5", "dummy5");
            //        GUILayout.Button("dummy6", "dummy6");
            //        GUILayout.Button("dummy7", "dummy7");
            //        GUILayout.Button("dummy8", "dummy8");
            //        GUILayout.Button("dummy9", "dummy9");
            //    }
            //    GUILayout.EndHorizontal();
            //    GUILayout.BeginHorizontal();
            //    {
            //        GUILayout.Button("dummy11", "dummy11");
            //        GUILayout.BeginVertical(Skin.current.Box);
            //        {
            //            GUILayout.Button("dummy12", "dummy12");
            //            GUILayout.Button("dummy13", "dummy13");
            //            GUILayout.BeginHorizontal();
            //            {
            //                GUILayout.Button("dummy14", "dummy14");
            //                GUILayout.Button("dummy15", "dummy15");
            //            }
            //            GUILayout.EndHorizontal();
            //        }
            //        GUILayout.EndVertical();
            //    }
            //    GUILayout.EndHorizontal();
            //}
            //GUILayout.EndVertical();

            //GUILayout.BeginVertical(Skin.current.Box);
            //{
            //    GUILayout.Button("dummy1", "dummy1");
            //    GUILayout.Button("dummy2", "dummy2");
            //    GUILayout.BeginHorizontal();
            //    {
            //        GUILayout.Button("dummy3", "dummy3");
            //        GUILayout.Button("dummy4", "dummy4");
            //    }
            //    GUILayout.EndHorizontal();
            //}
            //GUILayout.EndVertical();


        }
    }

}
