using System;
using System.Diagnostics;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.Input;

namespace ImGui
{
    public partial class GUILayout
    {
    }

    public partial class GUI
    {
        public static bool BeginWindow(string name, ref bool open, Point position, Size size, double backgroundAlpha = 1, WindowFlags flags = WindowFlags.VerticalScrollbar)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            WindowManager w = g.WindowManager;
            Debug.Assert(name != null);                        // Window name required
            Debug.Assert(g.Initialized);                       // Forgot to call NewFrame()
            Debug.Assert(g.FrameCountEnded != g.FrameCount);   // Called Render() or EndFrame() and haven't called NewFrame() again yet

            // Find or create
            Window window = w.FindWindowByName(name) ?? w.CreateWindow(name, position, size, flags);

            // Check if this is the first call of Begin
            long currentFrame = g.FrameCount;
            bool firstBeginOfTheFrame = (window.LastActiveFrame != currentFrame);
            if (firstBeginOfTheFrame)
            {
                window.Flags = flags;
            }
            else
            {
                flags = window.Flags;
            }

            // Add to stack
            Window parentWindow = w.WindowStack.Count != 0 ? w.WindowStack[w.WindowStack.Count - 1] : null;
            w.WindowStack.Add(window);
            w.CurrentWindow = window;
            Debug.Assert(parentWindow != null || !flags.HaveFlag(WindowFlags.ChildWindow));

            // Update known root window (if we are a child window, otherwise window == window->RootWindow)
            int rootIdx;
            for (rootIdx = w.WindowStack.Count - 1; rootIdx > 0; rootIdx--)
            {
                if (!(w.WindowStack[rootIdx].Flags.HaveFlag(WindowFlags.ChildWindow)))
                    break;
            }
            window.RootWindow = w.WindowStack[rootIdx];

            // When reusing window again multiple times a frame, just append content (don't need to setup again)
            if (firstBeginOfTheFrame)
            {
                window.FirstUpdate(name, position, size, ref open, backgroundAlpha, flags, currentFrame, parentWindow);
            }

            // Inner clipping rectangle
            {
                // We set this up after processing the resize grip so that our clip rectangle doesn't lag by a frame
                // Note that if our window is collapsed we will end up with a null clipping rectangle which is the correct behavior.
                Rect titleBarRect = window.TitleBarRect;
                const float borderSize = 0;
                var paddingHorizontal = window.Style.PaddingHorizontal;
                // Force round to ensure that e.g. (int)(max.x-min.x) in user's render code produce correct result.
                Rect clipRect = new Rect(
                    new Point(Math.Floor(0.5f + titleBarRect.Min.X + Math.Max(borderSize, Math.Floor(paddingHorizontal * 0.5f))),
                              Math.Floor(0.5f + titleBarRect.Max.Y + borderSize)),
                    new Point(Math.Floor(0.5f + window.Position.X + window.Size.Width - Math.Max(borderSize, Math.Floor(paddingHorizontal * 0.5f))),
                              Math.Floor(0.5f + window.Position.Y + window.Size.Height - borderSize)));
                
                //TODO handle clip rect
            }

            // Clear 'accessed' flag last thing
            if (firstBeginOfTheFrame)
                window.Accessed = false;
            window.BeginCount++;

            // Child window can be out of sight and have "negative" clip windows.
            // Mark them as collapsed so commands are skipped earlier (we can't manually collapse because they have no title bar).
            if (flags.HaveFlag(WindowFlags.ChildWindow))
            {
                Debug.Assert(flags.HaveFlag(WindowFlags.NoTitleBar));
                window.Collapsed = parentWindow != null && parentWindow.Collapsed;
            }

            // Return false if we don't intend to display anything to allow user to perform an early out optimization
            window.SkipItems = window.Collapsed || !window.Active;
            return !window.SkipItems;
        }

        public static void EndWindow()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;

            // Pop
            w.WindowStack.RemoveAt(w.WindowStack.Count - 1);
            w.CurrentWindow = (w.WindowStack.Count == 0) ? null : w.WindowStack[w.WindowStack.Count - 1];
        }

        public static bool Begin(string name, ref bool open)
        {
            return Begin(name, ref open, Point.Zero, new Size(400, 300));//TODO stack newly added window automatically
        }

        public static bool Begin(string name, ref bool open, Point position, Size size)
        {
            return Begin(name, ref open, position, size, 1, WindowFlags.ShowBorders);
        }

        public static bool Begin(string name, ref bool open, Point position, Size size, double bg_alpha = 1, WindowFlags flags = WindowFlags.VerticalScrollbar)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            WindowManager w = g.WindowManager;
            Debug.Assert(name != null);                        // Window name required
            Debug.Assert(g.Initialized);                       // Forgot to call NewFrame()
            Debug.Assert(g.FrameCountEnded != g.FrameCount);   // Called Render() or EndFrame() and haven't called NewFrame() again yet

            if (flags.HaveFlag(WindowFlags.NoInputs))
            {
                flags |= WindowFlags.NoMove | WindowFlags.NoResize;
            }

            // Find or create
            Window window = w.FindWindowByName(name) ?? w.CreateWindow(name, position, size, flags);

            // Check if this is the first call of Begin
            long current_frame = g.FrameCount;
            bool first_begin_of_the_frame = (window.LastActiveFrame != current_frame);
            if (first_begin_of_the_frame)
            {
                window.Flags = flags;
            }
            else
            {
                flags = window.Flags;
            }

            // Add to stack
            Window parent_window = w.WindowStack.Count != 0 ? w.WindowStack[w.WindowStack.Count - 1] : null;
            w.WindowStack.Add(window);
            w.CurrentWindow = window;
            Debug.Assert(parent_window != null || !flags.HaveFlag(WindowFlags.ChildWindow));

            // Update known root window (if we are a child window, otherwise window == window->RootWindow)
            int root_idx;
            for (root_idx = w.WindowStack.Count - 1; root_idx > 0; root_idx--)
            {
                if (!(w.WindowStack[root_idx].Flags.HaveFlag(WindowFlags.ChildWindow)))
                    break;
            }
            window.RootWindow = w.WindowStack[root_idx];

            // When reusing window again multiple times a frame, just append content (don't need to setup again)
            if (first_begin_of_the_frame)
            {
                window.Setup(name, position, size, ref open, bg_alpha, flags, current_frame, parent_window);
            }

            // Inner clipping rectangle
            {
                // We set this up after processing the resize grip so that our clip rectangle doesn't lag by a frame
                // Note that if our window is collapsed we will end up with a null clipping rectangle which is the correct behavior.
                Rect title_bar_rect = window.TitleBarRect;
                const float border_size = 0;
                var paddingHorizontal = window.Style.PaddingHorizontal;
                // Force round to ensure that e.g. (int)(max.x-min.x) in user's render code produce correct result.
                Rect clip_rect = new Rect(
                    new Point(Math.Floor(0.5f + title_bar_rect.Min.X + Math.Max(border_size, Math.Floor(paddingHorizontal * 0.5f))),
                              Math.Floor(0.5f + title_bar_rect.Max.Y + border_size)),
                    new Point(Math.Floor(0.5f + window.Position.X + window.Size.Width - Math.Max(border_size, Math.Floor(paddingHorizontal * 0.5f))),
                              Math.Floor(0.5f + window.Position.Y + window.Size.Height - border_size)));
                window.DrawList.PushClipRect(clip_rect, true);
                window.ClipRect = clip_rect;
                //window.DrawList.AddRect(window.ClipRect.TopLeft, window.ClipRect.BottomRight, Color.Red);//test only
            }

            // Clear 'accessed' flag last thing
            if (first_begin_of_the_frame)
                window.Accessed = false;
            window.BeginCount++;

            // Child window can be out of sight and have "negative" clip windows.
            // Mark them as collapsed so commands are skipped earlier (we can't manually collapse because they have no title bar).
            if (flags.HaveFlag(WindowFlags.ChildWindow))
            {
                Debug.Assert(flags.HaveFlag(WindowFlags.NoTitleBar));
                window.Collapsed = parent_window != null && parent_window.Collapsed;
            }

            window.StackLayout.Begin();

            // Return false if we don't intend to display anything to allow user to perform an early out optimization
            window.SkipItems = window.Collapsed || !window.Active;
            return !window.SkipItems;
        }


        public static void End()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;

            window.DrawList.PopClipRect();

            window.StackLayout.Layout();

            // Pop
            w.WindowStack.RemoveAt(w.WindowStack.Count - 1);
            w.CurrentWindow = (w.WindowStack.Count == 0) ? null : w.WindowStack[w.WindowStack.Count - 1];
        }
    }
}
