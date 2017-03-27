using System;
using System.Diagnostics;
using ImGui;

namespace Calculator
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
