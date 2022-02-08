using System;
using ImGui;

namespace WebTemplateApp
{
    public class MainForm : ImGui.Form
    {
        public MainForm(Rect rect) : base(rect)
        {
        }
    }

    public class Program
    {
        public static void Main()
        {
            var form = new MainForm(new Rect(800, 400));
        }
    }
}
