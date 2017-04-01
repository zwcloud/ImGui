using System;
using ImGui;

namespace EmptyTemplate
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
