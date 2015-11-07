//#define INSPECT_STATE
using System;
using System.Collections;
using System.Collections.Generic;

//TODO decouple Input from windows => write a stand-alone and cross-platform Input library
namespace IMGUI.Input
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
            
        }

        internal static class MouseCommand
        {
            public const string MouseDown = "_MouseDown";
            public const string MouseUp = "_MouseUp";
            public const string Fetch = "_Fetch";
        }

        private static string[] states =
        {
            MouseState.Idle, MouseCommand.MouseDown, MouseState.Pressed,
            MouseState.Pressed, MouseCommand.Fetch, MouseState.PressFetched,
            MouseState.Pressed, MouseCommand.MouseUp, MouseState.Released,
            MouseState.PressFetched, MouseCommand.MouseUp, MouseState.Released,
            MouseState.Released, MouseCommand.Fetch, MouseState.Idle,
            MouseState.Released, MouseCommand.MouseDown, MouseState.Pressed
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

        public static bool MouseMoving
        {
            get { return mousePos != lastMousePos; }
        }

        #endregion

        #region Drag

        private static IEnumerator<bool> ClickChecker
        {
            get { return clickChecker; }
            set { clickChecker = value; }
        }

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

        private static IEnumerator<bool> clickChecker = ClickStateMachine.Instance.GetEnumerator();
        class ClickStateMachine : IEnumerable<bool>
        {
            enum ClickState
            {
                One,
                Two,
                Three
            }

            private static ClickStateMachine instance;
            public static ClickStateMachine Instance
            {
                get
                {
                    if(instance == null)
                        instance = new ClickStateMachine();
                    return instance;
                }
            }

            private ClickState state;

            public IEnumerator<bool> GetEnumerator()
            {
                while (true)
                {
                    switch(state)
                    {
                        case ClickState.One:
                            if(LastLeftButtonState == InputState.Up && LeftButtonState == InputState.Down)
                            {
                                state = ClickState.Two;
                            }
                            yield return false;
                            break;
                        case ClickState.Two:
                            if(MouseMoving)
                            {
                                state = ClickState.One;
                                yield return false;
                            }
                            if(LeftButtonState == InputState.Up)
                            {
                                state = ClickState.One;
                                yield return false;
                            }
                            state = ClickState.Three;
                            yield return true;
                            break;
                        case ClickState.Three:
                            state = ClickState.One;
                            yield return false;
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
