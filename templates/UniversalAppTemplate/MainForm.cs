using ImGui;
using System.Diagnostics;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 600))
        {
        }

        TestUI testUI = new TestUI();

        bool DebugWindowOpen = true;

        protected override void OnGUI()
        {   
            GUILayout.Begin("Debug", ref DebugWindowOpen, new Point(60, 60), new Size(400, 400), 1, WindowFlags.Default);
            //if(GUI.Button(new Rect(65, 70, new Size(34, 27)), "MyButton", "MyButton1"))
            //{
            //    Debug.WriteLine("Clicked");
            //}
            GUILayout.End();
            //testUI.OnGUI();
        }
    }
}