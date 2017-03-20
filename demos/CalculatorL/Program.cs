using ImGui;
using System;

namespace Calculator
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Init();
            Application.Run(new Form1());
        }
    }
}
