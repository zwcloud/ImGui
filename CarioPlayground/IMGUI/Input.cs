using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Win32;

namespace IMGUI
{
    /// <summary>
    /// input
    /// </summary>
    public static class Input
    {
        #region Keyboard
        /// <summary>
        /// Last recorded key states
        /// </summary>
        /// <remarks>for detecting keystates' changes</remarks>
        private static InputState[] LastKeyStates;

        /// <summary>
        /// Key states of all keys
        /// </summary>
        private static InputState[] KeyStates;

        /// <summary>
        /// Key state of CapsLock (readOnly)
        /// </summary>
        public static InputState CapsLock
        {
            get { return KeyStates[(int)Key.CapsLock]; }
        }

        /// <summary>
        /// Key state of ScrollLock (readOnly)
        /// </summary>
        public static InputState ScrollLock
        {
            get { return KeyStates[(int)Key.Scroll]; }
        }

        /// <summary>
        /// Key state of NumLock (readOnly)
        /// </summary>
        public static InputState NumLock
        {
            get { return KeyStates[(int)Key.NumLock]; }
        }

        /// <summary>
        /// check if a single key is being pressing
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>true: pressing; false: released</returns>
        public static bool KeyDown(Key key)
        {
            return KeyStates[(int)key] == InputState.Down;
        }

        /// <summary>
        /// Check if a single key is pressed
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>true: pressed; false: not pressed yet</returns>
        public static bool KeyPressed(Key key)
        {
            return LastKeyStates[(int)key] == InputState.Down && KeyStates[(int)key] == InputState.Up;
        }
        #endregion

        #region Mouse
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
        /// Button state of left mouse button(readonly)
        /// </summary>
        public static InputState LeftButtonState
        {
            get { return leftButtonState; }
        }

        /// <summary>
        /// Check if the left mouse button is clicked(readonly)
        /// </summary>
        public static bool LeftButtonClicked
        {
            get
            {
                return lastLeftButtonState == InputState.Down
                    && leftButtonState == InputState.Up;
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
        static Point mousePos;

        /// <summary>
        /// Mouse position (readonly)
        /// </summary>
        public static Point MousePos
        {
            get { return mousePos; }
        }
        #endregion
        #endregion

        /// <summary>
        /// Static constructor
        /// </summary>
        static Input()
        {
            KeyStates = new InputState[256];
            LastKeyStates = new InputState[256];
        }

        /// <summary>
        /// Refresh input states
        /// </summary>
        /// <param name="clientPosX">x position of the client area</param>
        /// <param name="clientPosY">y position of the client area</param>
        /// <param name="clientRect">rect of the client area(top,left are both zero)</param>
        /// <returns>true: successful; false: failed</returns>
        /// <remarks>The input states will persist until next call of this method, 
        /// and last input states will be recorded.</remarks>
        public static bool Refresh(int clientPosX, int clientPosY, RECT clientRect)
        {
            /*
             * TODO check if the window has focus
             */

            /*
             * Keyboard
             */
            byte[] keys = new byte[256];
            if (!Native.GetKeyboardState(keys))
            {
                int err = Marshal.GetLastWin32Error();
                Debug.WriteLine("Error {0}: GetKeyboardState Filed", err);
                return false;
            }

            //Record the keyboard states
            var tmpKeyStates = LastKeyStates;
            LastKeyStates = KeyStates;
            if(tmpKeyStates != null)
                KeyStates = tmpKeyStates;

            //一般按键
            for (var i = 0; i < keys.Length; ++i)
            {
                KeyStates[i] = ((keys[i] & (byte)0x80) == 0x80) ? InputState.Down : InputState.Up;
            }

            //Toggle 按键
            KeyStates[(int)Key.CapsLock] = ((keys[(int)Key.CapsLock] & 0x01) == 1) ? InputState.On : InputState.Off;
            KeyStates[(int)Key.Scroll] = ((keys[(int)Key.Scroll] & 0x01) == 1) ? InputState.On : InputState.Off;
            KeyStates[(int)Key.NumLock] = ((keys[(int)Key.NumLock] & 0x01) == 1) ? InputState.On : InputState.Off;

            /*
             * Mouse
             */
            //Buttons's states
            lastLeftButtonState = leftButtonState;
            leftButtonState = ((Native.GetAsyncKeyState((ushort)Button.Left) & (ushort)0x8000) == (ushort)0x8000) ? InputState.Down : InputState.Up;
            lastRightButtonState = rightButtonState;
            rightButtonState = ((Native.GetAsyncKeyState((ushort)Button.Right) & (ushort)0x8000) == (ushort)0x8000) ? InputState.Down : InputState.Up;
            //Debug.WriteLine("Mouse Left {0}, Right {1}", leftButtonState.ToString(), rightButtonState.ToString());
            //Position
            var clientWidth = clientRect.Right - clientRect.Left;
            var clientHeight = clientRect.Bottom - clientRect.Top;
            POINT cursorPosPoint = new POINT(0, 0);
            Native.GetCursorPos(out cursorPosPoint);//Position in screen
            
            float screenX = cursorPosPoint.X;
            float screenY = cursorPosPoint.Y;
            mousePos.X = (int)screenX - clientPosX;
            mousePos.Y = (int)screenY - clientPosY;
            if (mousePos.X < 0)
                mousePos.X = 0;
            else if (mousePos.X > clientWidth)
                mousePos.X = clientWidth;
            if (mousePos.Y < 0)
                mousePos.Y = 0;
            else if (mousePos.Y > clientHeight)
                mousePos.Y = clientHeight;
            //Now mousePos is the position in the client area

            return true;
        }


    }
}
