namespace IMGUI
{
    public sealed class Skin
    {
		public Font Font { get; private set; }
        public Style Button { get; private set; }

        internal static Skin _current;

        static Skin()
        {
            _current = new Skin();
        }

        public Skin()
        {
            Font = new Font();
            Button = new Style();
        }

    }
}
