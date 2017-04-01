using System;
using ImGui;

namespace UniversalAppTemplate
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Init();
            Application.Run(new MainForm());
        }
    }
}
