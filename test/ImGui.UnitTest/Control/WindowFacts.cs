using System;
using ImGui.Common.Primitive;
using Xunit;

namespace ImGui.UnitTest
{
    public class WindowFacts
    {
        public class Form1 : Form
        {
            private Action onGUI;

            public Form1(Action onGUI) : base(new Point(400, 300), new Size(800, 600)) { this.onGUI = onGUI; Form.current = this; }

            protected override void OnGUI()
            {
                this.onGUI?.Invoke();
            }
        }
        

        public class General
        {
            public General()
            {
                Application.InitSysDependencies();
            }

            [Fact]
            public void TheWindowShouldBeDrawn()
            {
                Application.Run(new Form1(() => {

                }));
            }
        }
    }
}