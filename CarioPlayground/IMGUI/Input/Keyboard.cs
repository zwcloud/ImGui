using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IMGUI.Input
{
    public class Keyboard
    {
        static Keyboard()
        {
            keyStates = new InputState[256];
            lastKeyStates = new InputState[256];
            lastKeyPressedTime = new long[256];
            isRepeatingKey = new bool[256];
        }

        /// <summary>
        /// Last recorded key states
        /// </summary>
        /// <remarks>for detecting keystates' changes</remarks>
        private static InputState[] lastKeyStates;

        /// <summary>
        /// Key states of all keys
        /// </summary>
        private static InputState[] keyStates;

        /// <summary>
        /// Key state of CapsLock (readOnly)
        /// </summary>
        public static InputState CapsLock
        {
            get { return keyStates[(int)Key.CapsLock]; }
        }

        /// <summary>
        /// Key state of ScrollLock (readOnly)
        /// </summary>
        public static InputState ScrollLock
        {
            get { return keyStates[(int)Key.Scroll]; }
        }

        /// <summary>
        /// Key state of NumLock (readOnly)
        /// </summary>
        public static InputState NumLock
        {
            get { return keyStates[(int)Key.NumLock]; }
        }

        /// <summary>
        /// check if a single key is being pressing
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>true: pressing; false: released</returns>
        public static bool KeyDown(Key key)
        {
            return keyStates[(int)key] == InputState.Down;
        }

        /// <summary>
        /// Check if a single key is pressed
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>true: pressed; false: not pressed yet</returns>
        public static bool KeyPressed(Key key)
        {
            return lastKeyStates[(int)key] == InputState.Down && keyStates[(int)key] == InputState.Up;
        }

        private static long[] lastKeyPressedTime;
        private static bool[] isRepeatingKey;

        /// <summary>
        /// check if a single key is being pressing with interval time
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="isRepeat"></param>
        /// <returns>true: pressing; false: released</returns>
        /// <remarks>time unit is millisecond</remarks>
        public static bool KeyPressed(Key key, bool isRepeat)
        {
            bool isKeydownThisMoment = keyStates[(int)key] == InputState.Down;
            if (isKeydownThisMoment)
            {
                if (lastKeyPressedTime[(int)key] == 0)
                {
                    lastKeyPressedTime[(int)key] = Utility.MillisFrameBegin;
                    return true;
                }

                const float delay = 300;
                var t = lastKeyPressedTime[(int)key];
                if (!isRepeatingKey[(int)key])
                {
                    if (isRepeat && Utility.MillisFrameBegin - t > delay)
                    {
                        isRepeatingKey[(int)key] = true;
                        lastKeyPressedTime[(int)key] = Utility.MillisFrameBegin;
                        return true;
                    }
                }
                else
                {
                    const float interval = 50;
                    if (Utility.MillisFrameBegin - lastKeyPressedTime[(int)key] > interval)
                    {
                        lastKeyPressedTime[(int)key] = Utility.Millis;
                        return true;
                    }
                }
            }
            else
            {
                isRepeatingKey[(int)key] = false;
                lastKeyPressedTime[(int)key] = 0;
            }
            return false;
        }

        /// <summary>
        /// Refresh keyboard states
        /// </summary>
        /// <returns>true: successful; false: failed</returns>
        /// <remarks>The keyboard states will persist until next call of this method, 
        /// and last states will be recorded.</remarks>
        /// TODO replace this with SFML keyboard event
        internal static bool Refresh()
        {
            byte[] keys = new byte[256];
            if (!Native.GetKeyboardState(keys))
            {
                int err = Marshal.GetLastWin32Error();
                Debug.WriteLine("Error {0}: GetKeyboardState Filed", err);
                return false;
            }

            //Record the keyboard states
            var tmpKeyStates = lastKeyStates;
            lastKeyStates = keyStates;
            if (tmpKeyStates != null)
                keyStates = tmpKeyStates;

            //一般按键
            for (var i = 0; i < keys.Length; ++i)
            {
                keyStates[i] = ((keys[i] & (byte)0x80) == 0x80) ? InputState.Down : InputState.Up;
            }

            //Toggle 按键
            keyStates[(int)Key.CapsLock] = ((keys[(int)Key.CapsLock] & 0x01) == 1) ? InputState.On : InputState.Off;
            keyStates[(int)Key.Scroll] = ((keys[(int)Key.Scroll] & 0x01) == 1) ? InputState.On : InputState.Off;
            keyStates[(int)Key.NumLock] = ((keys[(int)Key.NumLock] & 0x01) == 1) ? InputState.On : InputState.Off;
            return true;
        }
    }
}