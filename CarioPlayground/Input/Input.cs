using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Win32;
using Cairo;

namespace IMGUI
{
    /// <summary>
    /// input
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// Key states of all keys
        /// </summary>
        private static InputState[] KeyStates;

        /// <summary>
        /// Key state of CapsLock (ReadOnly)
        /// </summary>
        public static InputState CapsLock
        {
            get { return KeyStates[(int)Key.CapsLock]; }
        }

        /// <summary>
        /// Key state of ScrollLock (ReadOnly)
        /// </summary>
        public static InputState ScrollLock
        {
            get { return KeyStates[(int)Key.Scroll]; }
        }

        /// <summary>
        /// Key state of NumLock (ReadOnly)
        /// </summary>
        public static InputState NumLock
        {
            get { return KeyStates[(int)Key.NumLock]; }
        }

        static InputState lastLeftButtonState = InputState.Up;
        static InputState leftButtonState = InputState.Up;
        /// <summary>
        /// Button state of left mouse button
        /// </summary>
        public static InputState LeftButtonState
        {
            get { return leftButtonState; }
        }

        /// <summary>
        /// Check if the left mouse button is clicked
        /// </summary>
        public static bool LeftButtonClicked
        {
            get
            {
                if (lastLeftButtonState == InputState.Down
                    && leftButtonState == InputState.Up)
                    return true;
                else
                    return false;
            }
        }

        static InputState lastRightButtonState = InputState.Up;
        static InputState rightButtonState = InputState.Up;
        /// <summary>
        /// Button state of the right mouse button
        /// </summary>
        public static InputState RightButtonState
        {
            get { return rightButtonState; }
        }

        /// <summary>
        /// Check if the right mouse button is clicked
        /// </summary>
        public static bool RightButtonClicked
        {
            get
            {
                if (lastRightButtonState == InputState.Down
                    && rightButtonState == InputState.Up)
                    return true;
                else
                    return false;
            }
        }

        static Point mousePos;
        public static Point MousePos
        {
            get { return mousePos; }
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static Input()
        {
            KeyStates = new InputState[256];
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

        public static bool Refresh(int clientPosX, int clientPosY, RECT clientRect)
        {
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

            //一般按键
            for (var i = 0; i < keys.Length; ++i)
            {
                var key = (int)keys[i];
                KeyStates[key] = ((key & 0x80) == 1)?InputState.Down:InputState.Up;
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
            leftButtonState = ((Native.GetAsyncKeyState((ushort)Button.Left) & (ushort)0x8000) == 1) ? InputState.Down : InputState.Up;
            lastRightButtonState = rightButtonState;
            rightButtonState = ((Native.GetAsyncKeyState((ushort)Button.Left) & (ushort)0x8000) == 1) ? InputState.Down : InputState.Up;
            //Position
            var clientWidth = clientRect.Right - clientRect.Left;
            var clientHeight = clientRect.Bottom - clientRect.Top;
            POINT cursorPosPoint = new POINT(0, 0);
            Native.GetCursorPos(out cursorPosPoint);
            
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

            return true;
        }


    }
}
