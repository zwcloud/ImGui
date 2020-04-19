﻿using System;
using System.Diagnostics;
using System.Threading;
using ImGui.Input;

namespace ImGui
{
    partial class Form
    {
        private bool debugWindowOpen = true;

        public Color BackgroundColor { get; set; } = Color.Argb(255, 114, 144, 154);

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
            g.DeltaTime = Time.deltaTime;
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

            #region mouse position
            if (Mouse.Instance.Position.X < 0 && Mouse.Instance.Position.Y < 0)
                Mouse.Instance.Position = new Point(-9999.0f, -9999.0f);
            if ((Mouse.Instance.Position.X < 0 && Mouse.Instance.Position.Y < 0) || (Mouse.Instance.LastPosition.X < 0 && Mouse.Instance.LastPosition.Y < 0))   // if mouse just appeared or disappeared (negative coordinate) we cancel out movement in MouseDelta
                Mouse.Instance.MouseDelta = Vector.Zero;
            else
                Mouse.Instance.MouseDelta = Mouse.Instance.Position - Mouse.Instance.LastPosition;
            Mouse.Instance.LastPosition = Mouse.Instance.Position;
            #endregion

            #region mouse left button
            Mouse.Instance.LeftButtonPressed = Mouse.Instance.LeftButtonState == KeyState.Down && Mouse.Instance.LeftButtonDownDuration < 0;
            Mouse.Instance.LeftButtonReleased = Mouse.Instance.LeftButtonState == KeyState.Up && Mouse.Instance.LeftButtonDownDuration >= 0;
            Mouse.Instance.LeftButtonDownDurationPrev = Mouse.Instance.LeftButtonDownDuration;
            Mouse.Instance.LeftButtonDownDuration = Mouse.Instance.LeftButtonState == KeyState.Down ? (Mouse.Instance.LeftButtonDownDuration < 0 ? 0 : Mouse.Instance.LeftButtonDownDuration + g.DeltaTime) : -1;
            Mouse.Instance.LeftButtonDoubleClicked = false;
            if (Mouse.Instance.LeftButtonPressed)
            {
                if (g.Time - Mouse.Instance.LeftButtonClickedTime < Mouse.DoubleClickIntervalTimeSpan)
                {
                    if ((Mouse.Instance.Position - Mouse.Instance.LeftButtonPressedPos).LengthSquared < Mouse.DoubleClickMaxDistance * Mouse.DoubleClickMaxDistance)
                    {
                        Mouse.Instance.LeftButtonDoubleClicked = true;
                    }
                    Mouse.Instance.LeftButtonClickedTime = -99999; // so the third click isn't turned into a double-click
                }
                else
                {
                    Mouse.Instance.LeftButtonClickedTime = g.Time;
                }
                Mouse.Instance.LeftButtonPressedPos = Mouse.Instance.Position;
                Mouse.Instance.DragMaxDiatanceSquared = 0;
            }
            else if (Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                Mouse.Instance.DragMaxDiatanceSquared = Math.Max(Mouse.Instance.DragMaxDiatanceSquared, (Mouse.Instance.Position - Mouse.Instance.LeftButtonPressedPos).LengthSquared);
            }
            if (Mouse.Instance.LeftButtonPressed) ++Mouse.Instance.LeftButtonPressedTimes;
            if (Mouse.Instance.LeftButtonReleased) ++Mouse.Instance.LeftButtonReleasedTimes;
            if (Mouse.Instance.LeftButtonDoubleClicked) ++Mouse.Instance.LeftButtonDoubleClickedTimes;
            #endregion

            #region mouse right button
            Mouse.Instance.RightButtonPressed = Mouse.Instance.RightButtonState == KeyState.Down && Mouse.Instance.RightButtonDownDuration < 0;
            Mouse.Instance.RightButtonReleased = Mouse.Instance.RightButtonState == KeyState.Up && Mouse.Instance.RightButtonDownDuration >= 0;
            Mouse.Instance.RightButtonDownDuration = Mouse.Instance.RightButtonState == KeyState.Down ? (Mouse.Instance.RightButtonDownDuration < 0 ? 0 : Mouse.Instance.RightButtonDownDuration + g.DeltaTime) : -1;

            if (Mouse.Instance.RightButtonPressed) ++Mouse.Instance.RightButtonPressedTimes;
            if (Mouse.Instance.RightButtonReleased) ++Mouse.Instance.RightButtonReleasedTimes;
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

            this.BackgroundDrawingContext = backgroundNode.RenderOpen();
            this.ForegroundDrawingContext = foregroundNode.RenderOpen();

            // Create implicit debug window - we will only render it if the user has added something to it.
            GUI.Begin("Debug", ref this.debugWindowOpen, Application.InitialDebugWindowRect, 0.8);
        }

        internal void EndFrame()
        {
            GUIContext g = Form.current.uiContext;
            WindowManager w = g.WindowManager;
            Debug.Assert(g.Initialized);                       // Forgot to call ImGui::NewFrame()
            Debug.Assert(g.FrameCountEnded != g.FrameCount);   // ImGui::EndFrame() called multiple times, or forgot to call ImGui::NewFrame() again

            // Hide implicit "Debug" window if it hasn't been used
            Debug.Assert(w.WindowStack.Count == 1);    // Mismatched Begin()/End() calls
            if (w.CurrentWindow != null && !w.CurrentWindow.Accessed)
            {
                w.CurrentWindow.Active = false;
            }
            GUI.End();//end of the implicit "Debug" window

            this.BackgroundDrawingContext.Close();
            this.ForegroundDrawingContext.Close();
            w.EndFrame(g);

            // Clear Input data for next frame
            Mouse.Instance.MouseWheel = 0;
            Ime.ImeBuffer.Clear();

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
            this.NewFrame();

            this.OnGUI();

            this.Render();

            this.Log();

            if (ImGui.Log.Enabled && ImGui.Log.LogStatus)
            {
                //Status logging is time consuming, so we lower down the fps by sleeping here.
                Thread.Sleep(40);
            }
        }

        internal void Render()
        {
            GUIContext g = this.uiContext;
            WindowManager w = g.WindowManager;

            Debug.Assert(g.Initialized);   // Make sure that NewFrame() is called.

            if (g.FrameCountEnded != g.FrameCount)
            {
                this.EndFrame();
            }
            g.FrameCountRendered = g.FrameCount;

            this.renderer.Clear(this.BackgroundColor);
            RenderBackground();
            foreach (var window in w.Windows)
            {
                if (!window.Active) continue;
                window.Render(this.renderer, ClientSize);
            }
            RenderForeground();
            this.renderer.SwapBuffers();
        }


        internal void Log()
        {
            GUIContext g = this.uiContext;

            this.nativeWindow.Title =
                $"fps:{g.fps,5:0.0}, mouse pos: {Mouse.Instance.Position}, deltaTime: {g.DeltaTime}ms";

            var l = Application.Logger;
            if (ImGui.Log.Enabled && ImGui.Log.LogStatus)
            {
                WindowManager w = g.WindowManager;
                l.Clear();
                l.Msg("fps:{0,5:0.0}, mouse pos: {1}, deltaTime: {2}ms", g.fps, Mouse.Instance.Position, g.DeltaTime);
                l.Msg("Input");
                l.Msg("    LeftButtonState {0}", Mouse.Instance.LeftButtonState);
                l.Msg("    LeftButtonDownDuration {0}ms", Mouse.Instance.LeftButtonDownDuration);
                l.Msg("    LeftButtonPressed {0}, {1} times", Mouse.Instance.LeftButtonPressed, Mouse.Instance.LeftButtonPressedTimes);
                l.Msg("    LeftButtonReleased {0}, {1} times", Mouse.Instance.LeftButtonReleased, Mouse.Instance.LeftButtonReleasedTimes);
                l.Msg("    LeftButtonDoubleClicked {0}, {1} times", Mouse.Instance.LeftButtonDoubleClicked, Mouse.Instance.LeftButtonDoubleClickedTimes);

                l.Msg("ActiveId: {0}, ActiveIdIsAlive: {1}", g.ActiveId, g.ActiveIdIsAlive);
                l.Msg("HoverId: {0}", g.HoverId);

                l.Msg("Window:");
                l.Msg("    HoveredWindow: {0}", (w.HoveredWindow != null) ? w.HoveredWindow.ID.ToString() : "<none>");
                l.Msg("    MovedWindow: {0}", (w.MovedWindow != null) ? w.MovedWindow.ID.ToString() : "<none>");
                l.Msg("    Window List:");
                for (int i = 0; i < w.Windows.Count; i++)
                {
                    var window = w.Windows[i];
                    l.Msg($"        [{i}]:{window.Name}, active: {window.Active}, rect: {window.Rect}");
                }
            }
        }
    }
}
