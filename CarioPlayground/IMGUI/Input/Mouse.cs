//#define INSPECT_STATE
using System;
using System.Collections;
using System.Collections.Generic;

namespace ImGui.Input
{
    /// <summary>
    /// input
    /// </summary>
    public static class Mouse
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

        private static bool leftButtonClicked = false;
        /// <summary>
        /// Is left mouse button clicked?(readonly)
        /// </summary>
        public static bool LeftButtonClicked
        {
            get
            {
                return leftButtonClicked;
            }
            private set { leftButtonClicked = value; }
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

        #region Drag


        public static bool MouseDraging { get; private set; }

        public static IEnumerator<bool> DragChecker
        {
            get { return dragChecker; }
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
            leftButtonState = SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Left) ? InputState.Down : InputState.Up;
            lastRightButtonState = rightButtonState;
            rightButtonState = SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Right) ? InputState.Down : InputState.Up;
            //Debug.WriteLine("Mouse Left {0}, Right {1}", leftButtonState.ToString(), rightButtonState.ToString());
            //Position
            lastMousePos = mousePos;
            var pos = SFML.Window.Mouse.GetPosition();
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
            
            DragChecker.MoveNext();
            MouseDraging = DragChecker.Current;

            return true;
        }

        /// <summary>
        /// Get mouse position relative to a form
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Point GetMousePos(BaseForm form)
        {
            var tmp = SFML.Window.Mouse.GetPosition((SFML.Window.Window) form.InternalForm);
            return new Point(tmp.X, tmp.Y);
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
        
        private static IEnumerator<bool> dragChecker = DragStateMachine.Instance.GetEnumerator();

        class DragStateMachine : IEnumerable<bool>
        {
            enum DragState
            {
                One,
                Two,
                Three
            }

            private static DragStateMachine instance;
            public static DragStateMachine Instance
            {
                get
                {
                    if(instance == null)
                        instance = new DragStateMachine();
                    return instance;
                }
            }

            private DragState state;

            public IEnumerator<bool> GetEnumerator()
            {
                while(true)
                {
                    switch(state)
                    {
                        case DragState.One:
                            if(LastLeftButtonState == InputState.Up && LeftButtonState == InputState.Down)
                            {
                                state = DragState.Two;
                            }
                            yield return false;
                            break;
                        case DragState.Two:
                            if(LeftButtonState == InputState.Up)
                            {
                                yield return false;
                            }
                            if(!MouseMoving)
                            {
                                yield return false;
                            }
                            state = DragState.Three;
                            yield return true;
                            break;
                        case DragState.Three:
                            if(LeftButtonState == InputState.Up)
                            {
                                state = DragState.One;
                                yield return false;
                            }
                            else
                            {
                                yield return true;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }


}
