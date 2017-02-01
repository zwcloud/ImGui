using System;

namespace ImGui
{
    internal struct InputInfo
    {
        private Point mousePosition;
        private Vector delta;
        private MouseButton button;
        private Modifiers modifiers;
        private float pressure;
        private int clickCount;
        private uint character;
        private uint keycode;

        /// <summary>
        /// Mouse position on screen
        /// </summary>
        public Point MousePosition
        {
            get { return mousePosition; }
            set { mousePosition = value; }
        }

        /// <summary>
        /// mouse position delta
        /// </summary>
        public Vector Delta
        {
            get { return delta; }
            set { delta = value; }
        }

        /// <summary>
        /// mouse button
        /// </summary>
        public MouseButton Button
        {
            get { return button; }
            set { button = value; }
        }

        /// <summary>
        /// modifier key
        /// </summary>
        public Modifiers Modifiers
        {
            get { return modifiers; }
            set { modifiers = value; }
        }

        /// <summary>
        /// stylus pressure
        /// </summary>
        public float Pressure
        {
            get { return pressure; }
            set { pressure = value; }
        }

        /// <summary>
        /// mouse clicked count
        /// </summary>
        public int ClickCount
        {
            get { return clickCount; }
            set { clickCount = value; }
        }

        /// <summary>
        /// keyboard character
        /// </summary>
        public uint Character
        {
            get { return character; }
            set { character = value; }
        }

        /// <summary>
        /// scan code
        /// </summary>
        public uint Keycode
        {
            get { return keycode; }
            set { keycode = value; }
        }
    }

    [Flags]
    internal enum Modifiers
    {
        Shift = 1 << 0,
        Control = 1 << 1,
        Alt = 1 << 2,
        Command = 1 << 3,
        Numeric = 1 << 4,
        CapsLock = 1 << 5
    }

    [Flags]
    internal enum MouseButton
    {
        LeftButton = 0,
        RightButton = 1,
        MiddleButton = 2
    }
}