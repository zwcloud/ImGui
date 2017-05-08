using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ImGui
{
    partial class Form
    {
        bool debugWindowOpen = true;

        internal void NewFrame()
        {
            current = this;
            GUIContext g = this.uiContext;

            if (!g.Initialized)
            {
                // Initialize on first frame
                g.Initialized = true;
            }

            // Time
            g.DeltaTime = Application.DeltaTime;
            g.Time += g.DeltaTime;
            g.FrameCount++;

            //fps
            var detlaTime = g.Time - g.lastFPSUpdateTime;
            g.lastFrameCount++;
            if (detlaTime > 1000)
            {
                g.fps = (int)g.lastFrameCount;
                g.lastFrameCount = 0;
                g.lastFPSUpdateTime = g.Time;
            }

            // Process input
            #region mouse position
            if (Input.Mouse.MousePos.X < 0 && Input.Mouse.MousePos.Y < 0)
                Input.Mouse.MousePos = new Point(-9999.0f, -9999.0f);
            if ((Input.Mouse.MousePos.X < 0 && Input.Mouse.MousePos.Y < 0) || (Input.Mouse.MousePosPrev.X < 0 && Input.Mouse.MousePosPrev.Y < 0))   // if mouse just appeared or disappeared (negative coordinate) we cancel out movement in MouseDelta
                Input.Mouse.MouseDelta = Vector.Zero;
            else
                Input.Mouse.MouseDelta = Input.Mouse.MousePos - Input.Mouse.MousePosPrev;
            Input.Mouse.MousePosPrev = Input.Mouse.MousePos;
            #endregion

            #region mouse left button
            Input.Mouse.LeftButtonPressed = Input.Mouse.LeftButtonState == InputState.Down && Input.Mouse.LeftButtonDownDuration < 0;
            Input.Mouse.LeftButtonReleased = Input.Mouse.LeftButtonState == InputState.Up && Input.Mouse.LeftButtonDownDuration >= 0;
            Input.Mouse.LeftButtonDownDurationPrev = Input.Mouse.LeftButtonDownDuration;
            Input.Mouse.LeftButtonDownDuration = Input.Mouse.LeftButtonState == InputState.Down ? (Input.Mouse.LeftButtonDownDuration < 0 ? 0 : Input.Mouse.LeftButtonDownDuration + g.DeltaTime) : -1;
            Input.Mouse.LeftButtonDoubleClicked = false;
            if (Input.Mouse.LeftButtonPressed)
            {
                if (g.Time - Input.Mouse.LeftButtonClickedTime < Mouse.DoubleClickIntervalTimeSpan)
                {
                    if ((Input.Mouse.MousePos - Input.Mouse.LeftButtonPressedPos).LengthSquared < Mouse.DoubleClickMaxDistance * Mouse.DoubleClickMaxDistance)
                    {
                        Input.Mouse.LeftButtonDoubleClicked = true;
                    }
                    Input.Mouse.LeftButtonClickedTime = -99999; // so the third click isn't turned into a double-click
                }
                else
                {
                    Input.Mouse.LeftButtonClickedTime = g.Time;
                }
                Input.Mouse.LeftButtonPressedPos = Input.Mouse.MousePos;
                Input.Mouse.DragMaxDiatanceSquared = 0;
            }
            else if(Input.Mouse.LeftButtonState == InputState.Down)
            {
                Input.Mouse.DragMaxDiatanceSquared = Math.Max(Input.Mouse.DragMaxDiatanceSquared, (Input.Mouse.MousePos - Input.Mouse.LeftButtonPressedPos).LengthSquared);
            }
            if (Input.Mouse.LeftButtonPressed) ++Input.Mouse.LeftButtonPressedTimes;
            if (Input.Mouse.LeftButtonReleased) ++Input.Mouse.LeftButtonReleasedTimes;
            if(Input.Mouse.LeftButtonDoubleClicked) ++Input.Mouse.LeftButtonDoubleClickedTimes;
            #endregion

            #region mouse right button
            Input.Mouse.RightButtonPressed = Input.Mouse.RightButtonState == InputState.Down && Input.Mouse.RightButtonDownDuration < 0;
            Input.Mouse.RightButtonReleased = Input.Mouse.RightButtonState == InputState.Up && Input.Mouse.RightButtonDownDuration >= 0;
            Input.Mouse.RightButtonDownDuration = Input.Mouse.RightButtonState == InputState.Down ? (Input.Mouse.RightButtonDownDuration < 0 ? 0 : Input.Mouse.RightButtonDownDuration + g.DeltaTime) : -1;
            
            if (Input.Mouse.RightButtonPressed) ++Input.Mouse.RightButtonPressedTimes;
            if (Input.Mouse.RightButtonReleased) ++Input.Mouse.RightButtonReleasedTimes;
            #endregion
            
            // Clear reference to active widget if the widget isn't alive anymore
            g.HoveredIdPreviousFrame = g.HoverId;
            g.HoverId = 0;
            g.HoverIdAllowOverlap = false;
            if (!g.ActiveIdIsAlive && g.ActiveIdPreviousFrame == g.ActiveId && g.ActiveId != 0)
                g.SetActiveID(0);
            g.ActiveIdPreviousFrame = g.ActiveId;
            g.ActiveIdIsAlive = false;
            g.ActiveIdIsJustActivated = false;

            // Handle user moving window (at the beginning of the frame to avoid input lag or sheering). Only valid for root windows.
            if (g.MovedWindowMoveId!=0 && g.MovedWindowMoveId == g.ActiveId)
            {
                g.KeepAliveID(g.MovedWindowMoveId);
                Debug.Assert(g.MovedWindow!=null && g.MovedWindow.RootWindow!=null);
                Debug.Assert(g.MovedWindow.RootWindow.MoveID == g.MovedWindowMoveId);
                if (Input.Mouse.LeftButtonState == InputState.Down)
                {
                    if (!(g.MovedWindow.Flags.HaveFlag(WindowFlags.NoMove)))
                    {
                        g.MovedWindow.PosFloat += Input.Mouse.MouseDelta;
                    }
                    g.FocusWindow(g.MovedWindow);
                }
                else
                {
                    g.SetActiveID(0);
                    g.MovedWindow = null;
                    g.MovedWindowMoveId = 0;
                }
            }
            else
            {
                g.MovedWindow = null;
                g.MovedWindowMoveId = 0;
            }

            // Find the window we are hovering. Child windows can extend beyond the limit of their parent so we need to derive HoveredRootWindow from HoveredWindow
            g.HoveredWindow = g.MovedWindow ?? g.FindHoveredWindow(Input.Mouse.MousePos, false);
            if (g.HoveredWindow != null && (g.HoveredWindow.Flags.HaveFlag(WindowFlags.ChildWindow)))
                g.HoveredRootWindow = g.HoveredWindow.RootWindow;
            else
                g.HoveredRootWindow = (g.MovedWindow != null) ? g.MovedWindow.RootWindow : g.FindHoveredWindow(Input.Mouse.MousePos, true);

            // Are we using inputs? Tell user so they can capture/discard the inputs away from the rest of their application.
            // When clicking outside of a window we assume the click is owned by the application and won't request capture. We need to track click ownership.
            int mouse_earliest_button_down = -1;
            bool mouse_any_down = false;
            {
                if (Input.Mouse.LeftButtonPressed)
                    Input.Mouse.LeftButtonMouseDownOwned = (g.HoveredWindow != null);
                mouse_any_down = mouse_any_down || (Input.Mouse.LeftButtonState == InputState.Down);
            }
            bool mouse_avail_to_imgui = (mouse_earliest_button_down == -1) || Input.Mouse.LeftButtonMouseDownOwned;
            if (g.CaptureMouseNextFrame != -1)
                Input.Mouse.WantCaptureMouse = (g.CaptureMouseNextFrame != 0);
            else
                Input.Mouse.WantCaptureMouse = (mouse_avail_to_imgui && (g.HoveredWindow != null || mouse_any_down)) || (g.ActiveId != 0);
            Input.Mouse.Cursor = Cursor.Default;
            //TODO Keyboard
            //g.OsImePosRequest = ImVec2(1.0f, 1.0f); // OS Input Method Editor showing on top-left of our window by default

            // If mouse was first clicked outside of ImGui bounds we also cancel out hovering.
            if (!mouse_avail_to_imgui)
                g.HoveredWindow = g.HoveredRootWindow = null;


            // Mark all windows as not visible
            for (int i = 0; i != g.Windows.Count; i++)
            {
                Window window = g.Windows[i];
                window.WasActive = window.Active;
                window.Active = false;
                window.Accessed = false;
            }

            // No window should be open at the beginning of the frame.
            // But in order to allow the user to call NewFrame() multiple times without calling Render(), we are doing an explicit clear.
            g.CurrentWindowStack.Clear();

            // Create implicit window - we will only render it if the user has added something to it.
            GUILayout.Begin("Debug", ref debugWindowOpen);
        }

        internal void EndFrame()
        {
            GUIContext g = Form.current.uiContext;
            Debug.Assert(g.Initialized);                       // Forgot to call ImGui::NewFrame()
            Debug.Assert(g.FrameCountEnded != g.FrameCount);   // ImGui::EndFrame() called multiple times, or forgot to call ImGui::NewFrame() again

            // Hide implicit "Debug" window if it hasn't been used
            Debug.Assert(g.CurrentWindowStack.Count == 1);    // Mismatched Begin()/End() calls
            if (g.CurrentWindow!=null && !g.CurrentWindow.Accessed)
                g.CurrentWindow.Active = false;
            GUILayout.End();

            // Click to focus window and start moving (after we're done with all our widgets)
            if (g.ActiveId == 0 && g.HoverId == 0 && Input.Mouse.LeftButtonPressed)
            {
                if (!(g.FocusedWindow!=null && !g.FocusedWindow.WasActive && g.FocusedWindow.Active)) // Unless we just made a popup appear
                {
                    if (g.HoveredRootWindow != null)
                    {
                        g.FocusWindow(g.HoveredWindow);
                        if (!(g.HoveredWindow.Flags.HaveFlag(WindowFlags.NoMove)))
                        {
                            g.MovedWindow = g.HoveredWindow;
                            g.MovedWindowMoveId = g.HoveredRootWindow.MoveID;
                            g.SetActiveID(g.MovedWindowMoveId, g.HoveredRootWindow);
                        }
                    }
                    else if (g.FocusedWindow != null/* && GetFrontMostModalRootWindow() == NULL*/)
                    {
                        // Clicking on void disable focus
                        g.FocusWindow(null);
                    }
                }
            }

            //// Sort the window list so that all child windows are after their parent
            //// We cannot do that on FocusWindow() because childs may not exist yet
            //g.WindowsSortBuffer.resize(0);
            //g.WindowsSortBuffer.reserve(g.Windows.Size);
            //for (int i = 0; i != g.Windows.Size; i++)
            //{
            //    ImGuiWindow* window = g.Windows[i];
            //    if (window->Active && (window->Flags & ImGuiWindowFlags_ChildWindow))       // if a child is active its parent will add it
            //        continue;
            //    AddWindowToSortedBuffer(g.WindowsSortBuffer, window);
            //}
            //IM_ASSERT(g.Windows.Size == g.WindowsSortBuffer.Size);  // we done something wrong
            //g.Windows.swap(g.WindowsSortBuffer);

            // Clear Input data for next frame
            Input.Mouse.MouseWheel = 0.0f;
            //memset(g.IO.InputCharacters, 0, sizeof(g.IO.InputCharacters));

            g.FrameCountEnded = g.FrameCount;
        }

        /// <summary>
        /// GUI Logic. This will be implemented by the user.
        /// </summary>
        protected abstract void OnGUI();

        /// <summary>
        /// GUI Loop
        /// </summary>
        internal void GUILoop()
        {
            NewFrame();

            OnGUI();

            Render();

            Log();
        }

        internal void Render()
        {
            var g = this.uiContext;

            Debug.Assert(g.Initialized);   // Forgot to call ImGui::NewFrame()

            if (g.FrameCountEnded != g.FrameCount)
                EndFrame();
            g.FrameCountRendered = g.FrameCount;

            this.renderer.Clear();
            foreach (var window in this.uiContext.Windows)
            {
                if(window.Active)
                {
                    this.renderer.RenderDrawList(window.DrawList, (int)this.ClientSize.Width, (int)this.ClientSize.Height);
                }
            }
            this.renderer.SwapBuffers();
        }

        internal void Log()
        {
            GUIContext g = this.uiContext;

            if (g.LogEnabled)
            {
                var l = Application.logger;
                l.Clear();
                l.Msg("fps:{0,5:0.0}, mouse pos: {1}, detlaTime: {2}ms", g.fps, Input.Mouse.MousePos, g.DeltaTime);
                l.Msg("Input");
                l.Msg("    LeftButtonState {0}", Input.Mouse.LeftButtonState);
                l.Msg("    LeftButtonDownDuration {0}ms", Input.Mouse.LeftButtonDownDuration);
                l.Msg("    LeftButtonPressed {0}, {1} times", Input.Mouse.LeftButtonPressed, Input.Mouse.LeftButtonPressedTimes);
                l.Msg("    LeftButtonReleased {0}, {1} times", Input.Mouse.LeftButtonReleased, Input.Mouse.LeftButtonReleasedTimes);
                l.Msg("    LeftButtonDoubleClicked {0}, {1} times", Input.Mouse.LeftButtonDoubleClicked, Input.Mouse.LeftButtonDoubleClickedTimes);

                l.Msg("ActiveId: {0}, ActiveIdIsAlive: {1}", g.ActiveId, g.ActiveIdIsAlive);
                l.Msg("HoverId: {0}", g.HoverId);

                l.Msg("Cursor: {0}", Input.Mouse.Cursor);

                l.Msg("Window:");
                l.Msg("    HoveredWindow: {0}", (g.HoveredWindow != null) ? g.HoveredWindow.ID.ToString() : "<none>");
                l.Msg("    Window List:");
                for (int i = 0; i < g.Windows.Count; i++)
                {
                    var window = g.Windows[i];
                    l.Msg("        [{0}]:{1}", i, window.ID);
                }
            }
        }
    }
}
