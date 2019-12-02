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

        bool open1 = false, open2 = false;
        protected override void OnGUI()
        {
            if (GUILayout.CollapsingHeader("Header 1", ref open1))
            {
                GUILayout.Label("Item A");
                GUILayout.Label("Item B");
            }
            if (GUILayout.CollapsingHeader("Header 2", ref open2))
            {
                GUILayout.Label("Item C");
                GUILayout.Label("Item D");
            }
        }
    }
}

