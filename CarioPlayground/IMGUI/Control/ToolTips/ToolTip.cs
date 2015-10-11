namespace IMGUI
{
    public class ToolTip : Form
    {
        private static ToolTip instance = new ToolTip();
        public static ToolTip Instance { get { return instance; } }

        public void Show(string text)
        {
            this.Text = text;
            this.Show();
        }

        //void Hide();

        string Text { get; set; }

        #region Overrides of Form

        protected override void OnGUI(GUI gui)
        {
            gui.Label(new Rect(0,0,(Size) Skin.current.ToolTip.ExtraStyles["FixedSize"]), Text, "ToolTipLabel");
        }

        #endregion
    }
}