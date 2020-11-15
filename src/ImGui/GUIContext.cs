using ImGui.Input;
using System.Collections.Generic;

namespace ImGui
{
    internal partial class GUIContext
    {
        public long Time;
        public long FrameCount = 0;

        // fps
        public long lastFrameCount = 0;
        public long lastFPSUpdateTime;
        public int fps;

        public WindowManager WindowManager { get; } = new WindowManager();

        public InputTextState InputTextState = new InputTextState();

        public Stack<Rect> ClipRectStack { get; } = new Stack<Rect>(new[] { Rect.Big });

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

        public int HoveredIdPreviousFrame { get; internal set; }

        public bool ActiveIdAllowOverlap { get; internal set; }
        public Vector ActiveIdClickOffset { get; internal set; }
        public bool HoverIdAllowOverlap { get; internal set; }
        public long DeltaTime { get; internal set; }

        public bool Initialized { get; internal set; }

        public long FrameCountEnded { get; internal set; } = -1;
        public long FrameCountRendered { get; internal set; } = -1;
        public int CaptureMouseNextFrame { get; internal set; }
        
        // Debug Tools
        internal bool DebugItemPickerActive { get; set; } = false;
        internal int DebugItemPickerBreakID { get; set; } = 0;

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
                rectClipped.Intersect(window.WindowContainer.Rect);

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
            if (g.HoverId != 0 && g.HoverId != id && !g.HoverIdAllowOverlap)
                return false;
            Window window = g.WindowManager.CurrentWindow;
            if (g.WindowManager.HoveredWindow != window &&
                (!flattenChilds || g.WindowManager.HoveredRootWindow != window.RootWindow))
                return false;
            if (g.ActiveId != 0 && g.ActiveId != id && !g.ActiveIdAllowOverlap)
                return false;
            if (!IsMouseHoveringRect(bb.Min, bb.Max)) 
                return false;
            if (!this.WindowManager.IsWindowContentHoverable(g.WindowManager.HoveredRootWindow))
                return false;

            // [DEBUG] Item Picker tool!
            // We perform the check here because SetHoveredID() is not frequently called (1~ time a frame), making
            // the cost of this tool near-zero.
            // TODO Consider how to get slightly better call-stack and support picking non-hovered
            // items.
            if (g.DebugItemPickerActive && g.HoveredIdPreviousFrame == id)
            {
                g.ForegroundDrawingContext.DrawRectangle(
                    null, new Rendering.Pen(Color.Argb(255, 255, 255, 0), 1), bb);
            }
            if (g.DebugItemPickerBreakID == id)
            {
                //System.Diagnostics.Debugger.Break();

                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(2, true);
                var frames = stackTrace.GetFrames();
                if (frames != null)
                {
                    System.Diagnostics.StackFrame targetFrame = null;
                    bool nextFrame = false;
                    foreach (var frame in frames)
                    {
                        var methodInfo = frame.GetMethod();
                        if (methodInfo.IsPublic && methodInfo.DeclaringType == typeof(GUILayout))
                        {
                            nextFrame = true;
                            continue;
                        }
                        if(nextFrame)
                        {
                            targetFrame = frame;
                            break;
                        }
                    }
                    if (targetFrame != null)
                    {
                        string fileName = targetFrame.GetFileName();
                        string methodName = targetFrame.GetMethod().Name;
                        int lineNumber = targetFrame.GetFileLineNumber();
                        System.Diagnostics.Debug.WriteLine($"{fileName}({lineNumber}): {methodName}");
                    }
                }
            }
            return true;
        }

        public bool IsMouseLeftButtonClicked(bool repeat)
        {
            GUIContext g = this;
            long t = Mouse.Instance.LeftButtonDownDuration;

            if (repeat && t > Keyboard.KeyRepeatDelay)
            {
                double delay = Keyboard.KeyRepeatDelay, rate = Keyboard.KeyRepeatRate;
                if (
                    ((t - delay) % rate) > rate * 0.5f
                    !=
                    ((t - delay - g.DeltaTime) % rate) > rate * 0.5f
                )
                {
                    return true;
                }
            }

            return false;
        }

        internal string DevOnly_GetNodeName(int id)
        {
            foreach (var window in WindowManager.Windows)
            {
                var node = window.RenderTree.Root.GetNodeById(id);
                if (node != null)
                {
                    return node.Name;
                }
            }

            return "<empty>";
        }

        internal void DebugStartItemPicker()
        {
            DebugItemPickerActive = true;
        }
    }
}