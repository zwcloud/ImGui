using System;
using System.Threading.Tasks;

namespace ImGui
{
    public class Keyboard
    {
        public static Func<string, Task<string>> ShowCallback;
        public static Task<string> Show(string text)
        {
            return ShowCallback?.Invoke(text);
        }

        public Keyboard()
        {
            keyStates = new InputState[(int)Key.KeyCount];
            lastKeyStates = new InputState[(int)Key.KeyCount];
            lastKeyPressedTime = new long[(int)Key.KeyCount];
            isRepeatingKey = new bool[(int)Key.KeyCount];
        }

        /// <summary>
        /// Last recorded key states
        /// </summary>
        /// <remarks>for detecting keystates' changes</remarks>
        internal InputState[] lastKeyStates;

        /// <summary>
        /// Key states of all keys
        /// </summary>
        internal InputState[] keyStates;
        
        /// <summary>
        /// check if a single key is being pressing
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>true: pressing; false: released</returns>
        public bool KeyDown(Key key)
        {
            return keyStates[(int)key] == InputState.Down;
        }

        /// <summary>
        /// Check if a single key is pressed
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>true: pressed; false: not pressed yet</returns>
        public bool KeyPressed(Key key)
        {
            return lastKeyStates[(int)key] == InputState.Down && keyStates[(int)key] == InputState.Up;
        }

        public bool KeyOn(Key key)
        {
            return keyStates[(int)key] == InputState.On;
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
        public bool KeyPressed(Key key, bool isRepeat)
        {
            bool isKeydownThisMoment = keyStates[(int)key] == InputState.Down;
            if (isKeydownThisMoment)
            {
                if (lastKeyPressedTime[(int)key] == 0)
                {
                    lastKeyPressedTime[(int)key] = Application.Time;
                    return true;
                }

                const float delay = 300;
                var t = lastKeyPressedTime[(int)key];
                if (!isRepeatingKey[(int)key])
                {
                    if (isRepeat && Application.Time - t > delay)
                    {
                        isRepeatingKey[(int)key] = true;
                        lastKeyPressedTime[(int)key] = Application.Time;
                        return true;
                    }
                }
                else
                {
                    const float interval = 50;
                    if (Application.Time - lastKeyPressedTime[(int)key] > interval)
                    {
                        lastKeyPressedTime[(int) key] = Application.Time;
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
    }
}