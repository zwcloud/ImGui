using System;
using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        Demo demo = new Demo();

        protected override void OnGUI()
        {
            //GUILayout.Label("Hello ImGui,");
            //demo.OnGUI();
            GUILayout.BeginVertical("H", GUILayout.Width(100).ExpandHeight(true));
            GUILayout.Button("A", GUILayout.Width(60).Height(60));
            GUILayout.Button("B", GUILayout.Width(60).Height(60));
            GUILayout.Button("C", GUILayout.Width(60).Height(60));
            GUILayout.EndVertical();
        }
    }
}

