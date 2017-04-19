using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ImGui
{
    partial class Form
    {
        internal void NewFrame()
        {
            current = this;
            GUIContext g = this.uiContext;

            // Time
            g.DeltaTime = Application.DeltaTime;
            g.Time += g.DeltaTime;
            g.FrameCount++;
            var detlaTime = g.Time - g.lastFPSUpdateTime;
            if (detlaTime > 1000)
            {
                g.fps = (int)g.FrameCount;
                g.FrameCount = 0;
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
            g.HoveredIdAllowOverlap = false;
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
            if (g.HoveredWindow != null)
                g.HoveredRootWindow = g.HoveredWindow.RootWindow;
            else
                g.HoveredRootWindow = (g.MovedWindow != null) ? g.MovedWindow.RootWindow : g.FindHoveredWindow(Input.Mouse.MousePos, true);

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

            this.renderer.Clear();
            foreach (var window in this.uiContext.Windows)
            {
                this.renderer.RenderDrawList(window.DrawList, (int)this.ClientSize.Width, (int)this.ClientSize.Height);
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
            }
        }
    }
}
