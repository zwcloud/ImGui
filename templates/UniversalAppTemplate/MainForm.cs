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
            GUILayout.Button("1");//width = content width + (padding width) + (border width)
            GUILayout.Button("2");
            GUILayout.Button("3");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal("H~~~2");
            GUILayout.PushHStretchFactor(3);
            GUILayout.PushWidth(200);
            GUILayout.Button("3");//width fixed to 200, content-box width = 200 - (padding width) - (border width)
            GUILayout.PopWidth();
            GUILayout.PopHStretchFactor();
            GUILayout.PushHStretchFactor(2);//width is stretched, width = 2 * (contatiner content-box width)/(container children count)
            GUILayout.Button("2");
            GUILayout.PopHStretchFactor();
            GUILayout.PushVStretchFactor(1);
            GUILayout.Button("1");
            GUILayout.PopVStretchFactor();
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
