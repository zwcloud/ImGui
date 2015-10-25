using System;
using System.Diagnostics;
using IMGUI;

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
