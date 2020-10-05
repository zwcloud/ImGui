using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Input;

namespace ImGui
{
    internal class WindowManager
    {
        public readonly List<Window> Windows = new List<Window>();

        public readonly List<Window> WindowStack = new List<Window>();

        public Window CurrentWindow;
        public Window PopupWindow;
        public Window HoveredWindow { get; internal set; }
        public Window HoveredRootWindow { get; internal set; }

        public Window MovedWindow { get; internal set; }
        public int MovedWindowMoveId { get; internal set; }

        public Window FocusedWindow { get; private set; }

        public Window ActiveIdWindow { get; internal set; }

        public bool IsWindowContentHoverable(Window window)
        {
            // An active popup disable hovering on other windows (apart from its own children)
            Window focused_window = this.FocusedWindow;
            if (focused_window != null)
            {
                Window focused_root_window = focused_window.RootWindow;
                if (focused_root_window != null)
                {
                    if (focused_root_window.Flags.HaveFlag(WindowFlags.Popup) && focused_root_window.WasActive && focused_root_window != window.RootWindow)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Window FindWindowByName(string name)
        {
            for (int i = 0; i < this.Windows.Count; i++)
            {
                if (this.Windows[i].ID == name.GetHashCode())
                {
                    return this.Windows[i];
                }
            }
            return null;
        }

        public Window FindHoveredWindow(Point pos, bool excluding_childs)
        {
            for (int i = this.Windows.Count - 1; i >= 0; i--)
            {
                Window window = this.Windows[i];
                if (!window.Active)
                    continue;
                if (excluding_childs && window.Flags.HaveFlag(WindowFlags.ChildWindow))
                    continue;

                if (window.Rect.Contains(pos))
                    return window;
            }
            return null;
        }

        /// <summary>
        /// Moving window to front of display (which happens to be back of our sorted list)
        /// </summary>
        /// <param name="window"></param>
        public void FocusWindow(Window window)
        {
            var g = Form.current.uiContext;

            // Always mark the window we passed as focused. This is used for keyboard interactions such as tabbing.
            this.FocusedWindow = window;

            // Passing null allow to disable keyboard focus
            if (window == null) return;

            // And move its root window to the top of the pile
            if (window.RootWindow != null)
            {
                window = window.RootWindow;
            }

            // Steal focus on active widgets
            if (window.Flags.HaveFlag(WindowFlags.Popup))
            {
                if (g.ActiveId != 0 && (this.ActiveIdWindow != null) && this.ActiveIdWindow.RootWindow != window)
                {
                    g.SetActiveID(0);
                }
            }

            // Bring to front
            if ((window.Flags.HaveFlag(WindowFlags.NoBringToFrontOnFocus) || this.Windows[this.Windows.Count - 1] == window))
            {
                return;
            }
            for (int i = 0; i < this.Windows.Count; i++)
            {
                if (this.Windows[i] == window)
                {
                    this.Windows.RemoveAt(i);
                    break;
                }
            }
            this.Windows.Add(window);
        }

        internal Window CreateWindow(string name, Point position, Size size, WindowFlags flags)
        {
            var window = new Window(name, position, size, flags);
            this.Windows.Add(window);
            return window;
        }

        public void NewFrame(GUIContext g)
        {
            // Handle user moving window (at the beginning of the frame to avoid input lag or sheering). Only valid for root windows.
            if (this.MovedWindowMoveId != 0 && this.MovedWindowMoveId == g.ActiveId)
            {
                g.KeepAliveID(this.MovedWindowMoveId);
                Debug.Assert(this.MovedWindow != null && this.MovedWindow.RootWindow != null);
                Debug.Assert(this.MovedWindow.RootWindow.MoveID == this.MovedWindowMoveId);
                if (Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    if (!this.MovedWindow.Flags.HaveFlag(WindowFlags.NoMove))
                    {
                        this.MovedWindow.Position += Mouse.Instance.MouseDelta;
                    }
                    this.FocusWindow(this.MovedWindow);
                }
                else
                {
                    g.SetActiveID(0);
                    this.MovedWindow = null;
                    this.MovedWindowMoveId = 0;
                }
            }
            else
            {
                this.MovedWindow = null;
                this.MovedWindowMoveId = 0;
            }

            // Find the window we are hovering. Child windows can extend beyond the limit of their parent so we need to derive HoveredRootWindow from HoveredWindow
            this.HoveredWindow = this.MovedWindow ?? this.FindHoveredWindow(Mouse.Instance.Position, false);
            if (this.HoveredWindow != null && (this.HoveredWindow.Flags.HaveFlag(WindowFlags.ChildWindow)))
                this.HoveredRootWindow = this.HoveredWindow.RootWindow;
            else
                this.HoveredRootWindow = (this.MovedWindow != null) ? this.MovedWindow.RootWindow : this.FindHoveredWindow(Mouse.Instance.Position, true);


            // Scale & Scrolling
            if (this.HoveredWindow!=null && Mouse.Instance.MouseWheel != 0.0 && !this.HoveredWindow.Collapsed)
            {
                Window window = this.HoveredWindow;
                if (Keyboard.Instance.KeyDown(Key.LeftControl))
                {
                    //Scale
                    //TODO
                }
                else
                {
                    // Scroll
                    if (!(window.Flags.HaveFlag(WindowFlags.NoScrollWithMouse)))
                    {
                        var newScrollY = window.ClientAreaNode.ScrollOffset.Y - Math.Sign(Mouse.Instance.MouseWheel) * 20/*scroll step*/;
                        float window_rounding = (float)window.WindowContainer.RuleSet.Get<double>(StylePropertyName.WindowRounding);
                        double resize_corner_size = Math.Max(window.WindowContainer.RuleSet.FontSize * 1.35, window_rounding + 1.0 + window.WindowContainer.RuleSet.FontSize * 0.2);
                        var contentSize = window.ContentRect.Size;
                        var vH = window.Rect.Height - window.TitleBarHeight - window.WindowContainer.RuleSet.BorderVertical - window.WindowContainer.RuleSet.PaddingVertical;
                        var cH = contentSize.Height;
                        if(cH > vH)
                        {
                            newScrollY = MathEx.Clamp(newScrollY, 0, cH - vH);
                            window.SetWindowScrollY(newScrollY);
                        }
                    }
                }
            }

            // Mark all windows as not visible
            for (int i = 0; i != this.Windows.Count; i++)
            {
                Window window = this.Windows[i];
                window.WasActive = window.Active;
                window.Active = false;
                window.Accessed = false;

                //disable all nodes in the window
                window.ClientAreaNode.Foreach(n => { n.ActiveSelf = false; return true; });
                window.AbsoluteVisualList.ForEach(n => n.ActiveSelf = false);
            }

            // Clear temp data
            for (int i = 0; i != this.Windows.Count; i++)
            {
                Window window = this.Windows[i];
                window.TempData.Clear();
            }

            // No window should be open at the beginning of the frame.
            // But in order to allow the user to call NewFrame() multiple times without calling Render(), we are doing an explicit clear.
            this.WindowStack.Clear();
        }

        private List<Window> windowSwapBuffer = new List<Window>();
        private List<Window> childWindows = new List<Window>();
        public void EndFrame(GUIContext g)
        {
            // Click to focus window and start moving (after we're done with all our widgets)
            if (g.ActiveId == 0 && g.HoverId == 0 && Mouse.Instance.LeftButtonPressed)
            {
                if (this.HoveredRootWindow != null)
                {
                    this.FocusWindow(this.HoveredWindow);
                    if (!(this.HoveredWindow.Flags.HaveFlag(WindowFlags.NoMove)))
                    {
                        this.MovedWindow = this.HoveredWindow;
                        this.MovedWindowMoveId = this.HoveredRootWindow.MoveID;
                        g.SetActiveID(this.MovedWindowMoveId, this.HoveredRootWindow);
                    }
                }
                else if (this.FocusedWindow != null)
                {
                    // Clicking on void disable focus
                    this.FocusWindow(null);
                }
            }

            // Sort windows so child windows always rendered after its parent TODO optimize this
            windowSwapBuffer.Clear();
            childWindows.Clear();
            for (int i = 0; i < Windows.Count; i++)
            {
                var window = Windows[i];
                if (window.Active && window.Flags.HaveFlag(WindowFlags.ChildWindow))
                {
                    childWindows.Add(window);
                }
                else
                {
                    windowSwapBuffer.Add(window);
                }
            }

            foreach (var childWindow in childWindows)
            {
                var parentWindowIndex = windowSwapBuffer.FindIndex(win => win == childWindow.RootWindow);
                if (parentWindowIndex < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                windowSwapBuffer.Insert(parentWindowIndex + 1, childWindow);
            }

            Debug.Assert(windowSwapBuffer.Count == Windows.Count);

            Windows.Clear();
            Windows.AddRange(windowSwapBuffer);
        }
    }
}
