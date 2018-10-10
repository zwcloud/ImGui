using System;
using System.Threading.Tasks;

namespace ImGui.Input
{
    /// <summary>
    /// Keyboard status and operations
    /// </summary>
    public class Keyboard
    {
        public static Keyboard Instance { get; }

        static Keyboard()
        {
            Instance = new Keyboard();
        }

        #region Settings

        /// <summary>
        /// When holding a key/button, time before it starts repeating, in ms (for buttons in Repeat mode, etc.).
        /// </summary>
        internal const double KeyRepeatDelay = 250;

        /// <summary>
        /// When holding a key/button, rate at which it repeats, in ms.
        /// </summary>
        internal const double KeyRepeatRate = 200f;

        #endregion

        // HACK for android
        public static Func<string, Task<string>> ShowCallback { get; set; }
        public static Task<string> Show(string text)
        {
            return ShowCallback?.Invoke(text);
        }

        public Keyboard()
        {
            this.keyStates = new KeyState[(int)Key.KeyCount];
            this.lastKeyStates = new KeyState[(int)Key.KeyCount];
            this.lastKeyPressedTime = new long[(int)Key.KeyCount];
            this.isRepeatingKey = new bool[(int)Key.KeyCount];
        }

        /// <summary>
        /// Last recorded key states
        /// </summary>
        /// <remarks>for detecting keystates' changes</remarks>
        internal KeyState[] lastKeyStates;

        /// <summary>
        /// Key states of all keys
        /// </summary>
        internal KeyState[] keyStates;
        
        /// <summary>
        /// check if a single key is being pressing
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>true: pressing; false: released</returns>
        public bool KeyDown(Key key)
        {
            return this.keyStates[(int)key] == KeyState.Down;
        }

        /// <summary>
        /// Check if a single key is pressed
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>true: pressed; false: not pressed yet</returns>
        public bool KeyPressed(Key key)
        {
            return this.lastKeyStates[(int)key] == KeyState.Down && this.keyStates[(int)key] == KeyState.Up;
        }

        public bool KeyOn(Key key)
        {
            return this.keyStates[(int)key] == KeyState.On;
        }

        private readonly long[] lastKeyPressedTime;
        private readonly bool[] isRepeatingKey;

        /// <summary>
        /// check if a single key is being pressing with interval time
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="isRepeat"></param>
        /// <returns>true: pressing; false: released</returns>
        /// <remarks>time unit is millisecond</remarks>
        public bool KeyPressed(Key key, bool isRepeat)
        {
            bool isKeydownThisMoment = this.keyStates[(int)key] == KeyState.Down;
            if (isKeydownThisMoment)
            {
                if (this.lastKeyPressedTime[(int)key] == 0)
                {
                    this.lastKeyPressedTime[(int)key] = Time.time;
                    return true;
                }

                const float delay = 300;
                var t = this.lastKeyPressedTime[(int)key];
                if (!this.isRepeatingKey[(int)key])
                {
                    if (isRepeat && Time.time - t > delay)
                    {
                        this.isRepeatingKey[(int)key] = true;
                        this.lastKeyPressedTime[(int)key] = Time.time;
                        return true;
                    }
                }
                else
                {
                    const float interval = 50;
                    if (Time.time - this.lastKeyPressedTime[(int)key] > interval)
                    {
                        this.lastKeyPressedTime[(int) key] = Time.time;
                        return true;
                    }
                }
            }
            else
            {
                this.isRepeatingKey[(int)key] = false;
                this.lastKeyPressedTime[(int)key] = 0;
            }
            return false;
        }

        public void OnFrameBegin()
        {
        }

        public void OnFrameEnd()
        {
            //save current key states to last key states
            for (var i = 0; i < keyStates.Length; i++)
            {
                lastKeyStates[i] = keyStates[i];
            }
        }


    }
}