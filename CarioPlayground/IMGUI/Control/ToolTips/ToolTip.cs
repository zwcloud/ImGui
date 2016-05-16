using System;
using System.Diagnostics;

namespace ImGui
{
#if f
    public sealed class ToolTip : BorderlessForm
    {
        internal enum ToopTipState
        {
            Inactive,
            Active
        }

        internal ToopTipState state = ToopTipState.Inactive;

        private readonly Stopwatch stopwatch = new Stopwatch();
        private int persistTime = 3000;
        private int reshowTime = 200;
        internal bool requested = false;
        private string tipText = "(empty)";
        private System.Timers.Timer timer;

        public ToolTip()
            : base(100, 30)
        {
            timer = new System.Timers.Timer(PersistTime);
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Hide();
        }

        internal string TipText
        {
            get { return tipText; }
            set { tipText = value; }
        }

        internal int PersistTime
        {
            get { return persistTime; }
            set { persistTime = value; }
        }

        internal int ReshowTime
        {
            get { return reshowTime; }
            set { reshowTime = value; }
        }

        internal void Update()
        {
            switch (state)
            {
                case ToopTipState.Inactive:
                    this.Hide();
                    //Debug.WriteLine("Hide");
                    if (requested)
                    {
                        //Debug.WriteLine("requested");
                        stopwatch.Restart();
                        state = ToopTipState.Active;
                    }
                    break;
                case ToopTipState.Active:
                    this.Show();
                    GUILoop();
                    //Debug.WriteLine("Show");
                    if (stopwatch.ElapsedMilliseconds >= PersistTime)
                    {
                        stopwatch.Stop();
                        state = ToopTipState.Inactive;
                        //Debug.WriteLine("Timeout");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            requested = false;
        }

        public override void Hide()
        {
            base.Hide();
            timer.Stop();
        }

        public override void Show()
        {
            this.Position = Input.Mouse.MousePos;
            base.Show();
            timer.Start();
        }

        protected override void OnGUI(GUI gui)
        {
            gui.Label(new Rect((Size)Skin.current.ToolTip.ExtraStyles["FixedSize"]), TipText, "ToolTipLabel");
        }
    }
#endif

}