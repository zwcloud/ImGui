using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IMGUI
{
    /// <summary>
    /// input
    /// </summary>
    public static class Input
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern private bool GetKeyboardState(byte[] lpKeyState);

        /// <summary>
        /// Key states of all keys
        /// </summary>
        private static KeyState[] KeyStates;

        /// <summary>
        /// Key state of CapsLock (ReadOnly)
        /// </summary>
        public static KeyState CapsLock
        {
            get { return KeyStates[(int)Key.CapsLock]; }
        }

        /// <summary>
        /// Key state of ScrollLock (ReadOnly)
        /// </summary>
        public static KeyState ScrollLock
        {
            get { return KeyStates[(int)Key.Scroll]; }
        }

        /// <summary>
        /// Key state of NumLock (ReadOnly)
        /// </summary>
        public static KeyState NumLock
        {
            get { return KeyStates[(int)Key.NumLock]; }
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static Input()
        {
            KeyStates = new KeyState[256];
        }

        /// <summary>
        /// check if a single key is being pressing
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>true: pressing; false: released</returns>
        public static bool KeyDown(Key key)
        {
            return KeyStates[(int)key] == KeyState.Down;                
        }

        public static bool GetKeyStates()
        {
            byte[] keys = new byte[256];
            if (!GetKeyboardState(keys))
            {
                int err = Marshal.GetLastWin32Error();
                Debug.WriteLine("Error {0}: GetKeyboardState Filed", err);
                return false;
            }

            //一般按键
            for (var i = 0; i < keys.Length; ++i)
            {
                var key = (int)keys[i];
                KeyStates[key] = ((key & 0x80) == 1)?KeyState.Down:KeyState.Up;
            }

            //Toggle 按键
            KeyStates[(int)Key.CapsLock] = ((keys[(int)Key.CapsLock] & 0x01) == 1) ? KeyState.On : KeyState.Off;
            KeyStates[(int)Key.Scroll] = ((keys[(int)Key.Scroll] & 0x01) == 1) ? KeyState.On : KeyState.Off;
            KeyStates[(int)Key.NumLock] = ((keys[(int)Key.NumLock] & 0x01) == 1) ? KeyState.On : KeyState.Off;

            return true;
        }

        public static void Refresh()
        {

        }
    }
}
