using System;
using ImGui;

namespace UniversalAppTemplate
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Init();
            Application.Run(new MainForm(), OnGUI);
        }

        private static readonly Demo demo = new Demo();

        static string[] text = {
            "A", "BB", "CCC"
        };

        private static void OnGUI()
        {
            demo.OnGUI();
            GUI.Begin("Combobox demo", new Point(10, 10), new Size(400, 400));
            GUI.ComboBox(new Rect(10, 10, 200, 20), text);
            GUI.End();
        }
    }
}
