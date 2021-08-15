using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ImGui;

namespace MultiFormTest
{
    public class Form1 : Form
    {
        public Form1() : base(new Rect(400, 300, 800, 600))
        {
        }
    }

    public class ApplicationFacts
    {
        public class CanOpenMultipleWindows
        {
            public CanOpenMultipleWindows()
            {
                Application.InitSysDependencies();
            }

            [Fact]
            public void OpenTwo()
            {
                Form1 form = new Form1();
                Application.Run(form, () =>
                {
                    GUI.Begin("New window", new Rect(10, 20, 300, 200), WindowFlags.Popup);
                    GUILayout.Label("== content in New window ==");
                    GUI.End();
                });
            }

        }
    }
}
