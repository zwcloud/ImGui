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
            var calculation = new Calculation();
            Application.Run(new Form1(), calculation.OnGUI);
        }
    }
}
