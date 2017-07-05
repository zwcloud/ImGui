using System;
using ImGui;

namespace HostEditor
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
