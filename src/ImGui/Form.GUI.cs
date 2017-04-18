using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    partial class Form
    {
        internal void NewFrame()
        {
            current = this;
            GUIContext g = this.uiContext;

            // Process input

            #region Mouse left button
            Input.Mouse.LeftButtonPressed = Input.Mouse.LeftButtonState == InputState.Down && Input.Mouse.LeftButtonDownDuration < 0;
            Input.Mouse.LeftButtonReleased = Input.Mouse.LeftButtonState == InputState.Up && Input.Mouse.LeftButtonDownDuration >= 0;
            Input.Mouse.LeftButtonDownDuration = Input.Mouse.LeftButtonState == InputState.Down ? (Input.Mouse.LeftButtonDownDuration < 0 ? 0 : Input.Mouse.LeftButtonDownDuration + Application.DetlaTime) : -1;
            Input.Mouse.LeftButtonDoubleClicked = false;
            if (Input.Mouse.LeftButtonPressed)
            {
                if(Application.Time - Input.Mouse.LeftButtonClickedTime < Mouse.DoubleClickIntervalTimeSpan)
                {
                    if ((Input.Mouse.MousePos - Input.Mouse.LeftButtonPressedPos).LengthSquared < Input.Mouse.DoubleClickMaxDistance * Input.Mouse.DoubleClickMaxDistance)
                    {
                        Input.Mouse.LeftButtonDoubleClicked = true;
                    }
                    Input.Mouse.LeftButtonClickedTime = long.MinValue; // so the third click isn't turned into a double-click
                }
                else
                {
                    Input.Mouse.LeftButtonClickedTime = Application.Time;
                }
                Input.Mouse.LeftButtonPressedPos = Input.Mouse.MousePos;
                Input.Mouse.DoubleClickMaxDistance = 0;
            }
            else if(Input.Mouse.LeftButtonState == InputState.Down)
            {
                Input.Mouse.DragMaxDiatanceSquared = Math.Max(Input.Mouse.DragMaxDiatanceSquared, (Input.Mouse.MousePos - Input.Mouse.LeftButtonPressedPos).LengthSquared);
            }
            if (Input.Mouse.LeftButtonPressed) ++Input.Mouse.LeftButtonPressedTimes;
            if (Input.Mouse.LeftButtonReleased) ++Input.Mouse.LeftButtonReleasedTimes;
            if(Input.Mouse.LeftButtonDoubleClicked) ++Input.Mouse.LeftButtonDoubleClickedTimes;
            #endregion

            #region Mouse right button
            Input.Mouse.RightButtonPressed = Input.Mouse.RightButtonState == InputState.Down && Input.Mouse.RightButtonDownDuration < 0;
            Input.Mouse.RightButtonReleased = Input.Mouse.RightButtonState == InputState.Up && Input.Mouse.RightButtonDownDuration >= 0;
            Input.Mouse.RightButtonDownDuration = Input.Mouse.RightButtonState == InputState.Down ? (Input.Mouse.RightButtonDownDuration < 0 ? 0 : Input.Mouse.RightButtonDownDuration + Application.DetlaTime) : -1;
            
            if (Input.Mouse.RightButtonPressed) ++Input.Mouse.RightButtonPressedTimes;
            if (Input.Mouse.RightButtonReleased) ++Input.Mouse.RightButtonReleasedTimes;
            #endregion

            // Calculate fps
            g.elapsedFrameCount++;
            var detlaTime = Application.Time - g.lastFPSUpdateTime;
            if (detlaTime > 1000)
            {
                g.fps = g.elapsedFrameCount;
                g.elapsedFrameCount = 0;
                g.lastFPSUpdateTime = Application.Time;
            }

            // Process control hover/active IDs
            g.HoverIdPreviousFrame = g.HoverId;
            g.HoverId = GUIContext.None;
            if (!g.ActiveIdIsAlive && g.ActiveIdPreviousFrame == g.ActiveId && g.ActiveId != GUIContext.None)
                g.ActiveId = GUIContext.None;
            g.ActiveIdPreviousFrame = g.ActiveId;
            g.ActiveIdIsAlive = false;
            g.ActiveIdIsJustActivated = false;

            // Find the window we are hovering. Child windows can extend beyond the limit of their parent so we need to derive HoveredRootWindow from HoveredWindow
            g.HoveredWindow = (g.MovedWindow!=null) ? g.MovedWindow : g.FindHoveredWindow(Input.Mouse.MousePos, false);
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
            this.renderer.Clear();
            this.renderer.RenderDrawList(this.DrawList, (int)this.ClientSize.Width, (int)this.ClientSize.Height);
            this.renderer.SwapBuffers();
        }

        internal void Log()
        {
            GUIContext g = this.uiContext;

            if (g.LogEnabled)
            {
                var l = Application.logger;
                l.Clear();
                l.Msg("fps:{0,5:0.0}, mouse pos: {1}, detlaTime: {2}ms", g.fps, GetMousePos().ToString(), Application.DetlaTime);
                l.Msg("Input");
                l.Msg("    LeftButtonState {0}", Input.Mouse.LeftButtonState);
                l.Msg("    LeftButtonDownDuration {0}ms", Input.Mouse.LeftButtonDownDuration);
                l.Msg("    LeftButtonPressed {0}, {1} times", Input.Mouse.LeftButtonPressed, Input.Mouse.LeftButtonPressedTimes);
                l.Msg("    LeftButtonReleased {0}, {1} times", Input.Mouse.LeftButtonReleased, Input.Mouse.LeftButtonReleasedTimes);
                l.Msg("    LeftButtonDoubleClicked {0}", Input.Mouse.LeftButtonDoubleClicked);
                l.Msg("    LeftButtonDoubleClickedTimes {0}", Input.Mouse.LeftButtonDoubleClickedTimes);


                l.Msg("ActiveId: {0}, ActiveIdIsAlive: {1}", g.ActiveId, g.ActiveIdIsAlive);
                l.Msg("HoverId: {0}", g.HoverId);

                l.Msg("Cursor: {0}", Input.Mouse.Cursor);
            }
        }
    }
}
