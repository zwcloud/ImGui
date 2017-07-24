using System.Collections.Generic;

namespace ImGui
{
    public static class Input
    {
        internal const double KeyRepeatDelay = 250; // When holding a key/button, time before it starts repeating, in ms (for buttons in Repeat mode, etc.).
        internal const double KeyRepeatRate = 200f; // When holding a key/button, rate at which it repeats, in ms.
        private static Mouse mouse;
        private static Keyboard keyboard;

        public static Mouse Mouse => mouse;
        public static Keyboard Keyboard => keyboard;

        static Input()
        {
            mouse = new Mouse();
            keyboard = new Keyboard();
        }

        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        public static Queue<char> ImeBuffer
        {
            get { return imeBuffer; }
            set { imeBuffer = value; }
        }

        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> imeBuffer = new Queue<char>();
    }
}
