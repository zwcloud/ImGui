using System;
using Xunit;
using ImGui;

namespace Test
{
    public class Form1 : Form
    {
        private Action onGUI;

        public Form1(Action onGUI) : base(new Point(400, 300), new Size(800, 600)) { this.onGUI = onGUI; }

        protected override void OnGUI()
        {
            if (this.onGUI != null)
                this.onGUI();
        }
    }

    public class LayoutEngineTest
    {
        public LayoutEngineTest()
        {
            Application.Init();
        }

        [Fact]
        public void ShouldHandleExistenceCorrectly()
        {
            var a = false;
            Application.Run(new Form1(() => {
                if (GUILayout.Button("Button", "Button"))
                {
                    Console.WriteLine("Clicked\n");
                    a ^= true;
                }
                if (a)
                {
                    GUILayout.Label("Thanks for clicking me!");
                }
            }));
        }
    }
}
