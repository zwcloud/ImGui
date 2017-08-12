using System;
using ImGui;
using ImGui.Common.Primitive;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        protected override void OnGUI()
        {
            GUILayout.BeginHorizontal("H~~~1");
            GUILayout.Button("1");
            GUILayout.Button("2");
            GUILayout.Button("3");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal("H~~~2");
            GUILayout.PushHStretchFactor(3);
            GUILayout.Button("3");
            GUILayout.PopHStretchFactor();
            GUILayout.PushHStretchFactor(2);
            GUILayout.Button("2");
            GUILayout.PopHStretchFactor();
            GUILayout.PushHStretchFactor(1);
            GUILayout.Button("1");
            GUILayout.PopHStretchFactor();
            GUILayout.EndHorizontal();

            //GUILayout.Space("!!~~1", 100);
            //GUILayout.BeginHorizontal("H~~~");
            //GUILayout.Space("!!~~2", 10);
            //GUILayout.PushHorizontalStretchFactor(2);
            //GUILayout.Button("A f:2");
            //GUILayout.PopHorizontalStretchFactor();
            //GUILayout.PushHorizontalStretchFactor(1);
            //GUILayout.Button("B f:1");
            //GUILayout.Button("C f:1");
            //GUILayout.PopHorizontalStretchFactor();
            //GUILayout.EndHorizontal();
        }
    }
}
