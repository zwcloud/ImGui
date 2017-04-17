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
            Input.Mouse.LeftButtonPressed = Input.Mouse.LeftButtonState == InputState.Down && Input.Mouse.LeftButtonDownDuration < 0;
            Input.Mouse.LeftButtonReleased = Input.Mouse.LeftButtonState == InputState.Up && Input.Mouse.LeftButtonDownDuration >= 0;
            Input.Mouse.LeftButtonDownDuration = Input.Mouse.LeftButtonState == InputState.Down ? (Input.Mouse.LeftButtonDownDuration < 0 ? 0 : Input.Mouse.LeftButtonDownDuration + Application.DetlaTime) : -1;

            Input.Mouse.RightButtonPressed = Input.Mouse.RightButtonState == InputState.Down && Input.Mouse.RightButtonDownDuration < 0;
            Input.Mouse.RightButtonReleased = Input.Mouse.RightButtonState == InputState.Up && Input.Mouse.RightButtonDownDuration >= 0;
            Input.Mouse.RightButtonDownDuration = Input.Mouse.RightButtonState == InputState.Down ? (Input.Mouse.RightButtonDownDuration < 0 ? 0 : Input.Mouse.RightButtonDownDuration + Application.DetlaTime) : -1;
            
            if (Input.Mouse.LeftButtonPressed) ++Input.Mouse.LeftButtonPressedTimes;
            if (Input.Mouse.LeftButtonReleased) ++Input.Mouse.LeftButtonReleasedTimes;
            if (Input.Mouse.RightButtonPressed) ++Input.Mouse.RightButtonPressedTimes;
            if (Input.Mouse.RightButtonReleased) ++Input.Mouse.RightButtonReleasedTimes;

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
                Application.logger.Clear();
                Application.logger.Msg("fps:{0,5:0.0}, mouse pos: {1}, detlaTime: {2}ms", g.fps, GetMousePos().ToString(), Application.DetlaTime);
                Application.logger.Msg("Input");
                Application.logger.Msg("    LeftButtonState {0}", Input.Mouse.LeftButtonState);
                Application.logger.Msg("    LeftButtonDownDuration {0}ms", Input.Mouse.LeftButtonDownDuration);
                Application.logger.Msg("    LeftButtonPressed {0}, {1} times", Input.Mouse.LeftButtonPressed, Input.Mouse.LeftButtonPressedTimes);
                Application.logger.Msg("    LeftButtonReleased {0}, {1} times", Input.Mouse.LeftButtonReleased, Input.Mouse.LeftButtonReleasedTimes);

                Application.logger.Msg("    RightButtonState {0}", Input.Mouse.RightButtonState);
                Application.logger.Msg("    RightButtonDownDuration {0}ms", Input.Mouse.RightButtonDownDuration);
                Application.logger.Msg("    RightButtonPressed {0}, {1} times", Input.Mouse.RightButtonPressed, Input.Mouse.RightButtonPressedTimes);
                Application.logger.Msg("    RightButtonReleased {0}, {1} times", Input.Mouse.RightButtonReleased, Input.Mouse.RightButtonReleasedTimes);

                Application.logger.Msg("ActiveId: {0}, ActiveIdIsAlive: {1}", g.ActiveId, g.ActiveIdIsAlive);
                Application.logger.Msg("HoverId: {0}", g.HoverId);

                Application.logger.Msg("Cursor: {0}", Input.Mouse.Cursor);
            }
        }
    }
}
