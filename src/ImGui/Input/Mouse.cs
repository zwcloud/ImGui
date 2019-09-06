namespace ImGui.Input
{
    /// <summary>
    /// Mouse status and operations
    /// </summary>
    public class Mouse
    {
        public static Mouse Instance { get; }

        static Mouse()
        {
            Instance = new Mouse();
        }

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
        /// Button state of left mouse button (readonly)
        /// </summary>
        public KeyState LeftButtonState { get; set; } = KeyState.Up;

        /// <summary>
        /// Is left mouse button released? (readonly)
        /// </summary>
        public bool LeftButtonReleased { get; internal set; } = false;

        /// <summary>
        /// Is left mouse button pressed? (readonly)
        /// </summary>
        public bool LeftButtonPressed { get; internal set; } = false;

        /// <summary>
        /// Is left mouse button double-clicked? (readonly)
        /// </summary>
        public bool LeftButtonDoubleClicked { get; internal set; } = false;

        /// <summary>
        /// How long are left button being pressed until now? unit: ms
        /// </summary>
        public long LeftButtonDownDuration { get; internal set; } = -1;

        #region internal
        internal long LeftButtonDownDurationPrev { get; set; } = -1;
        internal int LeftButtonPressedTimes = 0;
        internal int LeftButtonReleasedTimes = 0;
        internal long LeftButtonClickedTime { get; set; }
        internal Point LeftButtonPressedPos { get; set; }
        internal double DragMaxDiatanceSquared { get; set; }
        internal int LeftButtonDoubleClickedTimes { get; set; }
        #endregion

        #endregion

        #region Right button

        /// <summary>
        /// Button state of the right mouse button(readonly)
        /// </summary>
        public KeyState RightButtonState { get; internal set; } = KeyState.Up;

        /// <summary>
        /// Is right mouse button released? (readonly)
        /// </summary>
        public bool RightButtonReleased { get; internal set; } = false;

        /// <summary>
        /// Check if the right mouse button is pressed (readonly)
        /// </summary>
        public bool RightButtonPressed { get; internal set; } = false;

        /// <summary>
        /// Check if the right mouse button is double-clicked (readonly)
        /// </summary>
        public bool RightButtonDoubleClicked { get; internal set; } = false;

        /// <summary>
        /// How long are right button being pressed until now? unit: ms
        /// </summary>
        public long RightButtonDownDuration { get; internal set; } = -1;

        #region internal
        internal int RightButtonPressedTimes = 0;
        internal int RightButtonReleasedTimes = 0;
        #endregion

        #endregion

        #region Position

        private static Point _position;

        /// <summary>
        /// mouse position at last frame (readonly)
        /// </summary>
        public Point LastPosition { get; internal set; }

        /// <summary>
        /// mouse position (readonly)
        /// </summary>
        public Point Position
        {
            get => _position;
            set
            {
                this.LastPosition = _position;
                _position = value;
            }
        }

        /// <summary>
        /// Is mouse moving?
        /// </summary>
        public bool Moving => this.Position != this.LastPosition;

        /// <summary>
        /// Offset of mouse compared to last frame
        /// </summary>
        public Vector MouseDelta { get; internal set; }

        /// <summary>
        /// mouse wheel scrolling value
        /// </summary>
        public short MouseWheel { get; internal set; }

        #endregion

        #region Cursor

        public Cursor Cursor { get; set; } = Cursor.Default;

        #endregion
    }
}
