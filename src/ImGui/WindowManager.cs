using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Input;

namespace ImGui
{
    internal class NextWindowData
    {
        public Point? NextWindowPosition { get; set; }

        public void Clear()
        {
            NextWindowPosition = null;
        }
    }

    internal class WindowManager
    {
        public Form MainForm { get; set; }
        public readonly List<Form> Viewports = new List<Form>(8);
        public Form CurrentViewport { get; set; }
        public Form MouseViewport { get; set; }

        public readonly List<Window> Windows = new List<Window>();

        public readonly List<Window> WindowStack = new List<Window>();

        public Window CurrentWindow;
        public Window HoveredWindow { get; internal set; }
        public Window HoveredRootWindow { get; internal set; }

        public Window MovingWindow { get; internal set; }
        public int MovingWindowMoveId { get; internal set; }

        public Window FocusedWindow { get; private set; }

        public Window ActiveIdWindow { get; internal set; }

        public NextWindowData NextWindowData { get; set; } = new NextWindowData();

        public Form FindViewportById(int id)
        {
            if (id == MainForm.ID)
            {
                return MainForm;
            }

            foreach (var form in Viewports)
            {
                if (form.ID == id)
                {
                    return form;
                }
            }
            return null;
        }

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
            var g = Application.ImGuiContext;

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
            
            window.LastFrameJustFocused = g.FrameCount;

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
            var window = new Window(MainForm, name, position, size, flags);
            this.Windows.Add(window);
            return window;
        }

        public void NewFrame(GUIContext g)
        {
            // Handle user moving window (at the beginning of the frame to avoid input lag or sheering). Only valid for root windows.
            if (MovingWindow != null)
            {
                g.KeepAliveID(g.ActiveId);
                Debug.Assert(MovingWindow?.RootWindow != null);
                var movingWindow = MovingWindow.RootWindow;
                if (Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    var delta = Mouse.Instance.MouseDelta;
                    if (!MathEx.AmostZero(delta.X) && !MathEx.AmostZero(delta.Y))
                    {
                        movingWindow.Position += Mouse.Instance.MouseDelta;
                        movingWindow.Layout();
                    }
                    this.FocusWindow(MovingWindow);
                }
                else
                {
                    g.SetActiveID(0, null);
                    this.MovingWindow = null;
                }
            }
            else
            {
                if (ActiveIdWindow != null && ActiveIdWindow.MoveId == g.ActiveId)
                {
                    g.KeepAliveID(g.ActiveId);
                    if (Mouse.Instance.LeftButtonState != KeyState.Down)
                    {
                        g.SetActiveID(0, null);
                    }
                }
            }

            // Find the window we are hovering. Child windows can extend beyond the limit of their parent so we need to derive HoveredRootWindow from HoveredWindow
            this.HoveredWindow = this.MovingWindow ?? this.FindHoveredWindow(Mouse.Instance.Position, false);
            if (this.HoveredWindow != null && (this.HoveredWindow.Flags.HaveFlag(WindowFlags.ChildWindow)))
                this.HoveredRootWindow = this.HoveredWindow.RootWindow;
            else
                this.HoveredRootWindow = (this.MovingWindow != null) ? this.MovingWindow.RootWindow : this.FindHoveredWindow(Mouse.Instance.Position, true);


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
        private List<Window> popupWindows = new List<Window>();
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
                        this.MovingWindow = this.HoveredWindow;
                        this.MovingWindowMoveId = this.HoveredRootWindow.MoveId;
                        g.SetActiveID(this.MovingWindowMoveId, this.HoveredRootWindow);
                    }
                }
                else if (this.FocusedWindow != null)
                {
                    // Clicking on void disable focus
                    this.FocusWindow(null);
                }
            }

            //TODO refactor to don't rely on flag to identify window type, subclass Window instead
            // Sort windows so
            // 1. child windows always rendered after its parent
            // 2. popup windows always rendered after regular window
            windowSwapBuffer.Clear();
            childWindows.Clear();
            popupWindows.Clear();
            List<Window> windowsToRemove = new List<Window>();
            for (int i = 0; i < Windows.Count; i++)
            {
                var window = Windows[i];

                if (!window.Active)
                {
                    windowsToRemove.Add(window);
                    continue;
                }

                if (window.Active && window.Flags.HaveFlag(WindowFlags.ChildWindow))
                {
                    childWindows.Add(window);
                }
                else if (window.Flags.HaveFlag(WindowFlags.Popup))
                {
                    popupWindows.Add(window);
                }
                else
                {
                    windowSwapBuffer.Add(window);
                }
            }

            foreach (var window in windowsToRemove)
            {
                Windows.Remove(window);
            }

            foreach (var childWindow in childWindows)
            {
                var parentWindowIndex = windowSwapBuffer.FindIndex(win => win == childWindow.RootWindow);
                if (parentWindowIndex < 0)
                {
                    throw new IndexOutOfRangeException(
                        $"Cannot find the parent window of child window<{childWindow.Name}>");
                }
                windowSwapBuffer.Insert(parentWindowIndex + 1, childWindow);
            }
            windowSwapBuffer.AddRange(popupWindows);

            Debug.Assert(windowSwapBuffer.Count == Windows.Count);

            Windows.Clear();
            Windows.AddRange(windowSwapBuffer);
        }
    }
}
