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
            var textPad = new TextPad();
            Application.Run(new MainForm(), textPad.OnGUI);
        }
    }
}
