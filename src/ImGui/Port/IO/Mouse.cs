namespace ImGui
{
    /// <summary>
    /// input
    /// </summary>
    public class Mouse
    {
        #region Settings

        /// <summary>
        /// Time for a double-click, in seconds.
        /// </summary>
        internal const long DoubleClickIntervalTimeSpan = 300;

        /// <summary>
        /// Distance threshold to stay in to validate a double-click, in pixels.
        /// </summary>
        internal const double DoubleClickMaxDistance = 6.0;


        #endregion

        #region Left button

        /// <summary>
        /// Left button state
        /// </summary>
        private InputState leftButtonState = InputState.Up;

        /// <summary>
        /// Last recorded left mouse button state
        /// </summary>
        /// <remarks>for detecting left mouse button state' changes</remarks>
        public InputState LastLeftButtonState { get; internal set; } = InputState.Up;

        /// <summary>
        /// Button state of left mouse button(readonly)
        /// </summary>
        public InputState LeftButtonState
        {
            get { return leftButtonState; }
            set { leftButtonState = value; }
        }

        public long LeftButtonDownDurationPrev { get; internal set; } = -1;
        public long LeftButtonDownDuration { get; internal set; } = -1;

        /// <summary>
        /// Is left mouse button released?(readonly)
        /// </summary>
        public bool LeftButtonReleased { get; internal set; } = false;

        /// <summary>
        /// Is left mouse button clicked?(readonly)
        /// </summary>
        public bool LeftButtonPressed { get; internal set; } = false;

        public int LeftButtonPressedTimes = 0;
        public int RightButtonPressedTimes = 0;
        public int LeftButtonReleasedTimes = 0;
        public int RightButtonReleasedTimes = 0;

        #endregion

        #region Right button
        /// <summary>
        /// Last recorded right mouse button state
        /// </summary>
        public InputState LastRightButtonState { get; internal set; } = InputState.Up;

        /// <summary>
        /// Right button state
        /// </summary>
        InputState rightButtonState = InputState.Up;

        /// <summary>
        /// Button state of the right mouse button(readonly)
        /// </summary>
        public InputState RightButtonState
        {
            get { return rightButtonState; }
            set { rightButtonState = value; }
        }

        public long RightButtonDownDuration { get; internal set; } = -1;

        /// <summary>
        /// Is right mouse button released?(readonly)
        /// </summary>
        public bool RightButtonReleased { get; internal set; } = false;

        /// <summary>
        /// Check if the right mouse button is pressed(readonly)
        /// </summary>
        public bool RightButtonPressed { get; internal set; } = false;
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
        public Point LastMousePos
        {
            get { return lastMousePos; }
        }

        /// <summary>
        /// Mouse position in screen (readonly)
        /// </summary>
        public Point MousePos
        {
            get { return mousePos; }
            set
            {
                lastMousePos = mousePos;
                mousePos = value;
            }
        }

        /// <summary>
        /// Is mouse moving?
        /// </summary>
        public bool MouseMoving
        {
            get { return mousePos != lastMousePos; }
        }

        public short MouseWheel { get; set; }

        #endregion

        #region Cursor
        public Cursor Cursor
        {
            get { return Application.inputContext.MouseCursor; }
            set { Application.inputContext.MouseCursor = value; }
        }

        public bool LeftButtonDoubleClicked { get; internal set; }
        public long LeftButtonClickedTime { get; internal set; }
        public Point LeftButtonPressedPos { get; internal set; }
        public double DragMaxDiatance { get; internal set; }
        public double DragMaxDiatanceSquared { get; internal set; }
        public int LeftButtonDoubleClickedTimes { get; internal set; }
        public Point MousePosPrev { get; internal set; }
        public Vector MouseDelta { get; internal set; }
        public bool LeftButtonMouseDownOwned { get; internal set; } = false;
        public bool WantCaptureMouse { get; internal set; }
        #endregion
    }
}
