namespace IMGUI
{
    [System.ComponentModel.DesignerCategory("")]
    public class ToolTip : BasicForm
    {
        private static ToolTip instance = new ToolTip();
        public static ToolTip Instance { get { return instance; } }

        static ToolTip()
        {
        }

        private string TipText { get; set; }
        public void Show(string text)
        {
            this.TipText = text;
            if(!this.Visible)
            {
                this.Show();
            }
        }

        public new void Hide()
        {
            if (this.Visible)
            {
                base.Hide();
            }
        }
        
        protected override void OnGUI(GUI gui)
        {
            gui.Label(new Rect(0, 0, (Size)Skin.current.ToolTip.ExtraStyles["FixedSize"]), TipText, "ToolTipLabel");
        }
    }
}