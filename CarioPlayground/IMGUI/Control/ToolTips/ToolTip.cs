using System;
using System.Diagnostics;

namespace IMGUI
{
    public class ToolTip : WinForm
    {
        private enum ToopTipState
        {
            Initial,
            Hiden,
            Shown
        }

        private ToopTipState state = ToopTipState.Initial;

        private static ToolTip instance;
        public static ToolTip Instance { get { return instance; } }

        private readonly Stopwatch stopwatch = new Stopwatch();

        internal static void Init()
        {
            instance = new ToolTip();
        }

        public ToolTip()
        {
            Size = new Size(200, 40);
            Position = new Point(400, 400);
        }

        private string TipText { get; set; }
        private int PersistTime { get; set; }
        private int ReshowTime { get; set; }

        void DoRequest()
        {
            switch (state)
            {
                case ToopTipState.Initial:
                    this.Show();
                    stopwatch.Start();
                    state = ToopTipState.Shown;
                    break;
                case ToopTipState.Hiden:
                    bool isReshowTimeTimeOut = stopwatch.ElapsedMilliseconds >= ReshowTime;
                    if(isReshowTimeTimeOut)
                    {
                        this.Show();
                        stopwatch.Restart();
                        state = ToopTipState.Shown;
                    }
                    break;
                case ToopTipState.Shown:
                    bool isTimeOut = stopwatch.ElapsedMilliseconds >= PersistTime;
                    if(isTimeOut)
                    {
                        this.Hide();
                        stopwatch.Restart();
                        state = ToopTipState.Hiden;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Show(string text, float time = 1.0f, float reshowTime = 0.2f)
        {
            this.TipText = text;
            this.PersistTime = (int) (time*1000);
            this.ReshowTime = (int) (reshowTime*1000);
            DoRequest();
        }

        public new void Hide()
        {
            base.Hide();
        }
        
        protected override void OnGUI(GUI gui)
        {
            gui.Label(new Rect(0, 0, (Size)Skin.current.ToolTip.ExtraStyles["FixedSize"]), TipText, "ToolTipLabel");
        }
    }
}