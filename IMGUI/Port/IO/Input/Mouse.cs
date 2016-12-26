﻿//#define INSPECT_STATE
using System;
using System.Collections;
using System.Collections.Generic;

namespace ImGui.Input
{
    /// <summary>
    /// input
    /// </summary>
    internal static class Mouse
    {
        #region State machine define
        internal static class MouseState
        {
            public const string Idle = "Idle";
            public const string Pressed = "Pressed";
            public const string PressFetched = "PressFetched";
            public const string Released = "Released";
            public const string Suspended = "Suspended";
            public const string Resumed = "_Resumed";//Instant state
        }

        internal static class MouseCommand
        {
            public const string MouseDown = "_MouseDown";
            public const string MouseUp = "_MouseUp";
            public const string Fetch = "_Fetch";
            public const string Resume = "_Resume";
            public const string Suspend = "_Suspend";
        }

        private static string[] states =
        {
            MouseState.Idle, MouseCommand.MouseDown, MouseState.Pressed,
            MouseState.Pressed, MouseCommand.Fetch, MouseState.PressFetched,
            MouseState.Pressed, MouseCommand.MouseUp, MouseState.Released,
            MouseState.PressFetched, MouseCommand.MouseUp, MouseState.Released,
            MouseState.Released, MouseCommand.Fetch, MouseState.Idle,
            MouseState.Released, MouseCommand.MouseDown, MouseState.Pressed,
            
            MouseState.Idle, MouseCommand.Suspend, MouseState.Suspended,
            MouseState.Pressed, MouseCommand.Suspend, MouseState.Suspended,
            MouseState.PressFetched, MouseCommand.Suspend, MouseState.Suspended,
            MouseState.Released, MouseCommand.Suspend, MouseState.Suspended,
            MouseState.Suspended, MouseCommand.Resume, MouseState.Resumed,
        };
        #endregion

        /// <summary>
        /// Double click interval time span
        /// </summary>
        /// <remarks>
        /// if the interval between two mouse click is longer than this value,
        /// the two clicking action is not considered as a double-click action.
        /// </remarks>
        internal const float DoubleClickIntervalTimeSpan = 0.2f;

        #region Left button
        /// <summary>
        /// Last recorded left mouse button state
        /// </summary>
        /// <remarks>for detecting left mouse button state' changes</remarks>
        private static InputState lastLeftButtonState = InputState.Up;

        /// <summary>
        /// Left button state
        /// </summary>
        private static InputState leftButtonState = InputState.Up;

        /// <summary>
        /// Last recorded left mouse button state
        /// </summary>
        /// <remarks>for detecting left mouse button state' changes</remarks>
        public static InputState LastLeftButtonState
        {
            get { return lastLeftButtonState; }
        }

        /// <summary>
        /// Button state of left mouse button(readonly)
        /// </summary>
        public static InputState LeftButtonState
        {
            get { return leftButtonState; }
        }

        private static bool leftButtonReleased = false;
        /// <summary>
        /// Is left mouse button released?(readonly)
        /// </summary>
        public static bool LeftButtonReleased
        {
            get
            {
                return leftButtonReleased;
            }
        }

        public static bool leftButtonPressed = false;
        /// <summary>
        /// Is left mouse button clicked?(readonly)
        /// </summary>
        public static bool LeftButtonPressed
        {
            get
            {
                return leftButtonPressed;
            }
        }

        #endregion

        #region Right button
        /// <summary>
        /// Last recorded right mouse button state
        /// </summary>
        /// <remarks>for detecting right mouse button state' changes</remarks>
        static InputState lastRightButtonState = InputState.Up;

        /// <summary>
        /// Right button state
        /// </summary>
        static InputState rightButtonState = InputState.Up;

        /// <summary>
        /// Button state of the right mouse button(readonly)
        /// </summary>
        public static InputState RightButtonState
        {
            get { return rightButtonState; }
        }

        /// <summary>
        /// Check if the right mouse button is clicked(readonly)
        /// </summary>
        public static bool RightButtonClicked
        {
            get
            {
                return lastRightButtonState == InputState.Down
                    && rightButtonState == InputState.Up;
            }
        }
        #endregion

        #region Position
        /// <summary>
        /// Mouse position
        /// </summary>
        static Point lastMousePos;

        /// <summary>
        /// Mouse position
        /// </summary>
        static Point mousePos;

        /// <summary>
        /// Last mouse position in screen (readonly)
        /// </summary>
        public static Point LastMousePos
        {
            get { return lastMousePos; }
        }

        /// <summary>
        /// Mouse position in screen (readonly)
        /// </summary>
        public static Point MousePos
        {
            get { return mousePos; }
        }

        /// <summary>
        /// Is mouse's position changed compared to last frame
        /// </summary>
        public static bool MouseMoving
        {
            get { return mousePos != lastMousePos; }
        }

        #endregion
        
        internal static readonly StateMachine stateMachine = new StateMachine(MouseState.Idle, states);

        static Mouse()
        {
        }

        /// <summary>
        /// Refresh mouse states
        /// </summary>
        /// <returns>true: successful; false: failed</returns>
        /// <remarks>The mouse states will persist until next call of this method, 
        /// and last states will be recorded.</remarks>
        public static bool Refresh()
        {
            Input.Mouse.stateMachine.MoveNext(Input.Mouse.MouseCommand.Fetch);//Fetch unused state

            //Buttons's states
            lastLeftButtonState = leftButtonState;
            leftButtonState = Application.inputContext.IsMouseLeftButtonDown ? InputState.Down : InputState.Up;
            lastRightButtonState = rightButtonState;
            rightButtonState = Application.inputContext.IsMouseRightButtonDown ? InputState.Down : InputState.Up;
            //Debug.WriteLine("Mouse Left {0}, Right {1}", leftButtonState.ToString(), rightButtonState.ToString());

            if (lastLeftButtonState == InputState.Down && leftButtonState == InputState.Up)
            {
                leftButtonReleased = true;
            }
            else
            {
                leftButtonReleased = false;
            }

            if (lastLeftButtonState == InputState.Up && leftButtonState == InputState.Down)
            {
                leftButtonPressed = true;
            }
            else
            {
                leftButtonPressed = false;
            }

            //Position
            lastMousePos = mousePos;
            var pos = Application.inputContext.MousePosition;
            mousePos = new Point(pos.X, pos.Y);

#if INSPECT_STATE
            var A = stateMachine.CurrentState;
#endif
            if (leftButtonState == InputState.Down)
            {
                stateMachine.MoveNext(MouseCommand.MouseDown);
            }
            if (leftButtonState == InputState.Up)
            {
                stateMachine.MoveNext(MouseCommand.MouseUp);
            }
#if INSPECT_STATE
            var B = stateMachine.CurrentState;
            System.Diagnostics.Debug.WriteLineIf(A != B, string.Format("Mouse {0}=>{1}", A, B));
#endif

            return true;
        }
        
        private static string suspendedState;
        public static void Suspend()
        {
            suspendedState = stateMachine.CurrentState;
            stateMachine.MoveNext(MouseCommand.Suspend);
        }

        public static void Resume()
        {
            if(stateMachine.MoveNext(MouseCommand.Resume))
            {
                System.Diagnostics.Debug.Assert(suspendedState != null);
                stateMachine.CurrentState = suspendedState;
                suspendedState = null;
            }
        }
    }


}
