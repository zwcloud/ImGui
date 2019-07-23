using System;
using ImGui;

namespace HostEditor
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
