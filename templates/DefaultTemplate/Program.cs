using System;
using ImGui;

namespace DefaultTemplate
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Init();
            Application.Run(new Form1());
        }
    }
}
