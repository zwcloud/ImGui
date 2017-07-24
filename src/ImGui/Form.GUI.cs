using System;
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
            WindowManager w = g.WindowManager;

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

            #region Input
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

            w.NewFrame(g);

            // Create implicit window - we will only render it if the user has added something to it.
            GUI.Begin("Debug", ref debugWindowOpen);
        }

        internal void EndFrame()
        {
            GUIContext g = Form.current.uiContext;
            WindowManager w = g.WindowManager;
            Debug.Assert(g.Initialized);                       // Forgot to call ImGui::NewFrame()
            Debug.Assert(g.FrameCountEnded != g.FrameCount);   // ImGui::EndFrame() called multiple times, or forgot to call ImGui::NewFrame() again

            // Hide implicit "Debug" window if it hasn't been used
            Debug.Assert(w.WindowStack.Count == 1);    // Mismatched Begin()/End() calls
            if (w.CurrentWindow!=null && !w.CurrentWindow.Accessed)
                w.CurrentWindow.Active = false;
            GUI.End();

            w.EndFrame(g);

            // Clear Input data for next frame
            Input.Mouse.MouseWheel = 0;
            Input.ImeBuffer.Clear();

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
            GUIContext g = this.uiContext;
            WindowManager w = g.WindowManager;

            Debug.Assert(g.Initialized);   // Forgot to call NewFrame()

            if (g.FrameCountEnded != g.FrameCount)
                EndFrame();
            g.FrameCountRendered = g.FrameCount;

            this.renderer.Clear();
            foreach (var window in w.Windows)
            {
                if(window.Active)
                {
                    this.renderer.RenderDrawList(window.DrawList, (int)this.ClientSize.Width, (int)this.ClientSize.Height);
                }
            }

            if (OverlayDrawList.DrawBuffer.CommandBuffer.Count != 0)
            {
                this.renderer.RenderDrawList(OverlayDrawList, (int)this.ClientSize.Width, (int)this.ClientSize.Height);
            }

            this.renderer.SwapBuffers();
        }

        internal void Log()
        {
            GUIContext g = this.uiContext;

            if (g.LogEnabled)
            {
                var l = Application.logger;
                WindowManager w = g.WindowManager;
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

                l.Msg("Window:");
                l.Msg("    HoveredWindow: {0}", (w.HoveredWindow != null) ? w.HoveredWindow.ID.ToString() : "<none>");
                l.Msg("    Window List:");
                for (int i = 0; i < w.Windows.Count; i++)
                {
                    var window = w.Windows[i];
                    l.Msg("        [{0}]:{1}", i, window.ID);
                }
            }
        }
    }
}
