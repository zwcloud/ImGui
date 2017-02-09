//#define INSPECT_STATE
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
        /// Left button state
        /// </summary>
        private static InputState leftButtonState = InputState.Up;

        /// <summary>
        /// Last recorded left mouse button state
        /// </summary>
        /// <remarks>for detecting left mouse button state' changes</remarks>
        public static InputState LastLeftButtonState { get; internal set; } = InputState.Up;

        /// <summary>
        /// Button state of left mouse button(readonly)
        /// </summary>
        public static InputState LeftButtonState
        {
            get { return leftButtonState; }
            internal set { leftButtonState = value; }
        }

        public static long LeftButtonDownDuration { get; internal set; } = -1;

        /// <summary>
        /// Is left mouse button released?(readonly)
        /// </summary>
        public static bool LeftButtonReleased { get; internal set; } = false;

        /// <summary>
        /// Is left mouse button clicked?(readonly)
        /// </summary>
        public static bool LeftButtonPressed { get; internal set; } = false;

        #endregion

        #region Right button
        /// <summary>
        /// Last recorded right mouse button state
        /// </summary>
        public static InputState LastRightButtonState { get; internal set; } = InputState.Up;

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
            internal set { rightButtonState = value; }
        }

        public static long RightButtonDownDuration { get; internal set; } = -1;

        /// <summary>
        /// Is right mouse button released?(readonly)
        /// </summary>
        public static bool RightButtonReleased { get; internal set; } = false;

        /// <summary>
        /// Check if the right mouse button is pressed(readonly)
        /// </summary>
        public static bool RightButtonPressed { get; internal set; } = false;
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
            internal set
            {
                lastMousePos = mousePos;
                mousePos = value;
            }
        }

        /// <summary>
        /// Is mouse moving?
        /// </summary>
        public static bool MouseMoving
        {
            get { return mousePos != lastMousePos; }
        }

        public static float MouseWheel { get; internal set; }

        #endregion
        
    }


}
