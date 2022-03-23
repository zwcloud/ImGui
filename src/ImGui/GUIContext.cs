using System;
using ImGui.Input;

namespace ImGui
{
    internal class GUIContext
    {
        public long Time;
        public long FrameCount = 0;
        
        public ImGuiConfigFlags ConfigFlagsCurrFrame;// ConfigFlags at the time of NewFrame()
        public ImGuiConfigFlags ConfigFlagsLastFrame;
        public int ViewportFrontMostStampCount;// Every time the front-most window changes, we stamp its viewport with an incrementing counter
        public Form MouseLastHoveredViewport;
        public bool ActiveIdNoClearOnFocusLoss = false;

        // fps
        public long lastFrameCount = 0;
        public long lastFPSUpdateTime;
        public int fps;

        public WindowManager WindowManager { get; } = new WindowManager();

        public InputTextState InputTextState = new InputTextState();

        // HoverId and ActiveId

        public int HoverId { get; set; } = 0;

        public int ActiveId { get; private set; } = 0;

        public int ActiveIdIsAlive { get; set; } = 0;

        public int ActiveIdPreviousFrame { get; set; } = 0;

        public bool ActiveIdPreviousFrameIsAlive { get; set; } = false;

        public bool ActiveIdIsJustActivated { get; set; } = false;

        // Store the last non-zero ActiveId
        public int LastActiveId { get; set; } = 0;

        public int HoveredIdPreviousFrame { get; internal set; } = 0;

        public bool ActiveIdAllowOverlap { get; internal set; }
        public Vector ActiveIdClickOffset { get; internal set; }
        public bool HoverIdAllowOverlap { get; internal set; }
        public long DeltaTime { get; internal set; }

        public bool Initialized { get; internal set; }

        public long FrameCountEnded { get; internal set; } = -1;
        public long FrameCountRendered { get; internal set; } = -1;
        public long FrameCountPlatformEnded { get; internal set; } = -1;
        
        // Debug Tools
        internal bool DebugItemPickerActive { get; set; } = false;
        internal int DebugItemPickerBreakID { get; set; } = 0;

        public void SetActiveID(int id, Window window = null)
        {
            var g = this;
            g.ActiveIdIsJustActivated = (g.ActiveId != id);
            if (g.ActiveIdIsJustActivated)
            {
                if (id != 0)
                {
                    g.LastActiveId = id;
                }
            }
            g.ActiveId = id;
            g.ActiveIdAllowOverlap = false;
            g.WindowManager.ActiveIdWindow = window;
            g.ActiveIdNoClearOnFocusLoss = false;
            if (id != 0)
            {
                g.ActiveIdIsAlive = id;
            }
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
            {
                g.ActiveIdIsAlive = id;
            }
            if (g.ActiveIdPreviousFrame == id)
            {
                g.ActiveIdPreviousFrameIsAlive = true;
            }
        }
        
        public void ClearActiveID()
        {
            SetActiveID(0, null); // g.ActiveId = 0;
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

        public bool IsHovered(Rect bb, int id, bool flattenChildren = false)
        {
            GUIContext g = this;
            if (g.HoverId != 0 && g.HoverId != id && !g.HoverIdAllowOverlap)
                return false;
            Window window = g.WindowManager.CurrentWindow;
            if (g.WindowManager.HoveredWindow != window &&
                (!flattenChildren || g.WindowManager.HoveredRootWindow != window.RootWindow))
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
                Form.current.ForegroundDrawingContext.DrawRectangle(
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

    [Flags]
    internal enum ImGuiConfigFlags
    {
        None                   = 0,
        ViewportsEnable        = 1 << 10,  // Viewport enable flags (require both ImGuiBackendFlags_PlatformHasViewports + ImGuiBackendFlags_RendererHasViewports set by the respective backends)
    }

    enum ImGuiBackendFlags
    {
        None                  = 0,
        HasGamepad            = 1 << 0,   // Backend Platform supports gamepad and currently has one connected.
        HasMouseCursors       = 1 << 1,   // Backend Platform supports honoring GetMouseCursor() value to change the OS cursor shape.
        HasSetMousePos        = 1 << 2,   // Backend Platform supports io.WantSetMousePos requests to reposition the OS mouse position (only used if ImGuiConfigFlags_NavEnableSetMousePos is set).
        RendererHasVtxOffset  = 1 << 3,   // Backend Renderer supports ImDrawCmd::VtxOffset. This enables output of large meshes (64K+ vertices) while still using 16-bit indices.

        // [BETA] Viewports
        PlatformHasViewports  = 1 << 10,  // Backend Platform supports multiple viewports.
        HasMouseHoveredViewport=1 << 11,  // Backend Platform supports setting io.MouseHoveredViewport to the viewport directly under the mouse _IGNORING_ viewports with the ImGuiViewportFlags_NoInputs flag and _REGARDLESS_ of whether another viewport is focused and may be capturing the mouse. This information is _NOT EASY_ to provide correctly with most high-level engines! Don't set this without studying _carefully_ how the backends handle ImGuiViewportFlags_NoInputs!
        RendererHasViewports  = 1 << 12   // Backend Renderer supports multiple viewports.
    }
}