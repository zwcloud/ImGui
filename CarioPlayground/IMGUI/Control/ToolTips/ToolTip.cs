using System.Windows.Forms;

namespace IMGUI
{
    [System.ComponentModel.DesignerCategory("")]
    public class ToolTip : Form
    {
        private static ToolTip instance = new ToolTip();
        public static ToolTip Instance { get { return instance; } }

        static ToolTip()
        {
            Instance.FormBorderStyle = FormBorderStyle.None;
            Instance.ShowInTaskbar = false;
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

        #region Overrides of Form
        const int WS_EX_NOACTIVATE = 0x08000000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams ret = base.CreateParams;
                ret.ExStyle |= WS_EX_NOACTIVATE;
                return ret;
            }
        } 

        protected override void OnGUI(GUI gui)
        {
            gui.Label(new Rect(0, 0, (Size)Skin.current.ToolTip.ExtraStyles["FixedSize"]), TipText, "ToolTipLabel");
        }

        #endregion
    }
}