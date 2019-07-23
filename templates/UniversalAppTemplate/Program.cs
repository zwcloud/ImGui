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
            Application.Run(new MainForm());
        }
    }
}
