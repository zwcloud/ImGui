using System;

namespace ImGui.ControlTest
{
    public class MainForm : Form
    {
        public Action OnGUIAction { get; set; }

        public MainForm(int width, int height) : base(new Rect(320, 180, width, height))
        {
        }

        public MainForm(Rect rect) : base(rect)
        {
        }

        public MainForm() : base(new Rect(320, 180, 300, 400))
        {
        }

        public MainForm(Action onGUI) : base(new Rect(320, 180, 300, 400))
        {
            this.OnGUIAction = onGUI;
        }
        
        public MainForm(int width, int height, Action onGUI) : base(new Rect(320, 180, width, height))
        {
            this.OnGUIAction = onGUI;
        }

        protected override void OnGUI()
        {
            this.OnGUIAction?.Invoke();
        }
    }
}
