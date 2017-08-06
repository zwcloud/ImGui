using System.Collections.Generic;

namespace ImGui
{
    /// <summary>
    /// Input APIs
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// Mouse
        /// </summary>
        public static Mouse Mouse { get; }

        /// <summary>
        /// Keyboard
        /// </summary>
        public static Keyboard Keyboard { get; }

        static Input()
        {
            Mouse = new Mouse();
            Keyboard = new Keyboard();
        }

        /// <summary>
        /// Character buffer for IME
        /// </summary>
        internal static Queue<char> ImeBuffer { get; set; } = new Queue<char>(16);
    }
}
