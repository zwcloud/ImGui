using System;
using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        private bool open;
        private bool clicked;
        double value0 = 0.5;
        double value1 = 0.5;
        double min = 0.0;
        double max = 1.0;

        protected override void OnGUI()
        {
            //value0 = GUI.Slider(new Rect(10, 10, 200, 30), "Slider", value0, min, max);
            //value0 = GUI.VSlider(new Rect(10, 50, 30, 200), "VSlider", value0, min, max);
            value0 = GUILayout.Slider("Slider0", value0, min, max);
            value1 = GUILayout.VSlider("Slider1", value1, min, max);
            //if (GUI.HoverButton(new Rect(0, 0, 100, 30), "Active when hovered"))
            //{
            //    Console.WriteLine("Active");
            //}
            //GUILayout.BeginHorizontal("group 1");
            //GUILayout.Button("Button1");
            //GUILayout.Button("Button2");
            //GUILayout.Button("Button3");
            //GUILayout.EndHorizontal();
            //
            //if (GUILayout.Button("Button"))
            //{
            //    clicked = !clicked;
            //}
            //if (clicked)
            //{
            //    GUILayout.Button("123");
            //    GUILayout.Label("345");
            //}
            //GUI.Button(new Rect(100, 10, 200,40), "123");

            //GUILayout.Button("123");
            //GUI.Begin("test window", ref open);
            //GUI.End();
        }
    }
}

