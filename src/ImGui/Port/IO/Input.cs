namespace ImGui
{
    public static class Input
    {
        private static Mouse mouse;
        private static Keyboard keyboard;

        public static Mouse Mouse { get => mouse; }
        public static Keyboard Keyboard { get => keyboard; }

        static Input()
        {
            mouse = new Mouse();
            keyboard = new Keyboard();
        }
    }
}
