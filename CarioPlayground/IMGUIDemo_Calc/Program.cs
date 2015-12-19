using System;
using System.Diagnostics;
using ImGui;

namespace IMGUIDemo
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new Form1(512, 512));
        }
    }
}
