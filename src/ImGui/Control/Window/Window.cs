using System;
using System.Diagnostics;

namespace ImGui
{
    public partial class GUI
    {
        public static bool Begin(string name, ref bool open)
        {
            return Begin(name, ref open, Point.Zero, new Size(400, 300));//TODO stack newly added window automatically
        }
        
        public static bool Begin(string name, ref bool open, Rect rect, double bg_alpha = 1)
        {
            return Begin(name, ref open, rect.Location, rect.Size, bg_alpha);
        }

        public static bool Begin(string name, ref bool open, Point position, Size size)
        {
            return Begin(name, ref open, position, size, 1, WindowFlags.ShowBorders);
        }

        public static void Begin(string title, Point position, Size size, WindowFlags flags = WindowFlags.ShowBorders)
        {
            var open = true;
            Begin(title, ref open, position, size, 1, flags);
        }

        public static bool Begin(string name, ref bool open, Point position, Size size, double bg_alpha = 1, WindowFlags flags = WindowFlags.VerticalScrollbar)
        {
            if (bg_alpha < 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(bg_alpha), nameof(bg_alpha) + " cannot be negative.");
            }

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
                window.FirstUpdate(name, size, ref open, bg_alpha, flags, current_frame, parent_window);
                ImGui.Development.Metrics.ActiveWindows++;
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
                window.Position = position;
            }

            // Return false if we don't intend to display anything to allow user to perform an early out optimization
            window.SkipItems = window.Collapsed || !window.Active;

            window.RenderTree.CurrentContainer = window.ClientAreaNode;

            return !window.SkipItems;
        }


        public static void End()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;

            window.RenderTree.CurrentContainer = window.ClientAreaNode;
            window.ClientAreaNode.OnGUI();
            window.Layout();

            // Pop
            w.WindowStack.RemoveAt(w.WindowStack.Count - 1);
            w.CurrentWindow = (w.WindowStack.Count == 0) ? null : w.WindowStack[w.WindowStack.Count - 1];
        }
    }
}
