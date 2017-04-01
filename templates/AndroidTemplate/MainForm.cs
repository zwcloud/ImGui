using ImGui;
using System;

namespace AndroidTemplate
{
    public class MainForm : Form
    {
        public MainForm(IntPtr nativeWindow, Point position, Size size) : base(nativeWindow, position, size)
        {
        }
        

        protected override void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("auto-sized", "l1");
            GUILayout.Button("Top Button", "b1");
            GUILayout.BeginHorizontal();
            GUILayout.Button("Middle Button##1", "b2");
            GUILayout.Button("Middle Button##2", "b3");
            GUILayout.Button("Middle Button##3", "b4");
            GUILayout.EndHorizontal();
            GUILayout.Button("Bottom Button", "b5");
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            {
                GUILayout.Label("fixed-size", "l2");
                GUILayout.BeginHorizontal();
                GUILayout.Button("Width:200", "b6", GUILayout.Width(200));
                GUILayout.Button("Width:100", "b7", GUILayout.Width(100));
                GUILayout.Button("Width:300", "b8", GUILayout.Width(300));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            {
                GUILayout.Label("stretched: evenly spaced", "l3");
                GUILayout.BeginHorizontal();
                GUILayout.Button("Button0", "b9", GUILayout.ExpandWidth(true));
                GUILayout.Button("Button1", "b10", GUILayout.ExpandWidth(true));
                GUILayout.Button("Button2", "b11", GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            {
                GUILayout.Label("stretched: spaced with factors", "l4");
                GUILayout.BeginHorizontal(GUILayout.Width(500));
                GUILayout.Button("Stretch:1", "b12", GUILayout.StretchWidth(1));
                GUILayout.Button("Stretch:2", "b13", GUILayout.StretchWidth(2));
                GUILayout.Button("Stretch:3", "b14", GUILayout.StretchWidth(3));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}