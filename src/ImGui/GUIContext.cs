using ImGui.Input;
using System.Collections.Generic;

namespace ImGui
{
    internal class GUIContext
    {
        public long Time;
        public long FrameCount = 0;

        // fps
        public long lastFrameCount = 0;
        public long lastFPSUpdateTime;
        public int fps;

        public WindowManager WindowManager { get; } = new WindowManager();

        public InputTextState InputTextState = new InputTextState();

        private int hoverId;
        private int activeId;
        private bool activeIdIsAlive;
        private bool activeIdIsJustActivated;

        private int hoverIdPreviousFrame;
        private int activeIdPreviousFrame;

        //-----

        public const int None = 0;

        public int HoverId
        {
            get => this.hoverId;
            set => this.hoverId = value;
        }

        public int ActiveId
        {
            get => this.activeId;
            private set => this.activeId = value;
        }

        public bool ActiveIdIsAlive
        {
            get => this.activeIdIsAlive;
            set => this.activeIdIsAlive = value;
        }

        public int ActiveIdPreviousFrame
        {
            get => this.activeIdPreviousFrame;
            set => this.activeIdPreviousFrame = value;
        }

        public bool ActiveIdIsJustActivated
        {
            get => this.activeIdIsJustActivated;
            set => this.activeIdIsJustActivated = value;
        }

        public int HoverIdPreviousFrame
        {
            get => this.hoverIdPreviousFrame;
            set => this.hoverIdPreviousFrame = value;
        }


        public bool ActiveIdAllowOverlap { get; internal set; }
        public Vector ActiveIdClickOffset { get; internal set; }
        public bool HoverIdAllowOverlap { get; internal set; }
        public long DeltaTime { get; internal set; }
        public int HoveredIdPreviousFrame { get; internal set; }

        public bool Initialized { get; internal set; }

        public long FrameCountEnded { get; internal set; } = -1;
        public long FrameCountRendered { get; internal set; } = -1;
        public int CaptureMouseNextFrame { get; internal set; }

        public void SetActiveID(int id, Window window = null)
        {
            var g = this;
            g.ActiveId = id;
            g.ActiveIdAllowOverlap = false;
            g.ActiveIdIsJustActivated = true;
            g.WindowManager.ActiveIdWindow = window;
        }

        public void SetHoverID(int id)
        {
            var g = this;
            g.HoverId = id;
            g.HoverIdAllowOverlap = false;
        }

        public void KeepAliveID(int id)
        {
            var g = this;
            if (g.ActiveId == id)
                g.ActiveIdIsAlive = true;
        }


        public bool IsMouseHoveringRect(Rect rect, bool clip = true)
        {
            var g = this;
            Window window = g.WindowManager.CurrentWindow;

            // Clip
            Rect rectClipped = rect;
            if (clip)
                rectClipped.Intersect(window.ClipRect);

            return rectClipped.Contains(Mouse.Instance.Position);
        }

        public bool IsMouseHoveringRect(Point rMin, Point rMax, bool clip = true)
        {
            Rect rect = new Rect(rMin, rMax);
            return IsMouseHoveringRect(rect, clip);
        }

        public bool IsHovered(Rect bb, int id, bool flattenChilds = false)
        {
            GUIContext g = this;
            if (g.HoverId == 0 || g.HoverId == id || g.HoverIdAllowOverlap)
            {
                Window window = g.WindowManager.CurrentWindow;
                if (g.WindowManager.HoveredWindow == window || (flattenChilds && g.WindowManager.HoveredRootWindow == window.RootWindow))
                    if ((g.ActiveId == 0 || g.ActiveId == id || g.ActiveIdAllowOverlap) && IsMouseHoveringRect(bb.Min, bb.Max))
                        if (this.WindowManager.IsWindowContentHoverable(g.WindowManager.HoveredRootWindow))
                            return true;
            }
            return false;
        }

        public bool IsMouseLeftButtonClicked(bool repeat)
        {
            GUIContext g = this;
            long t = Mouse.Instance.LeftButtonDownDuration;

            if (repeat && t > Keyboard.KeyRepeatDelay)
            {
                double delay = Keyboard.KeyRepeatDelay, rate = Keyboard.KeyRepeatRate;
                if (
                    ((t - delay)%rate) > rate * 0.5f
                    !=
                    ((t - delay - g.DeltaTime)% rate) > rate * 0.5f
                    )
                    return true;
            }

            return false;
        }
    }
}