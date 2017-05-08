namespace ImGui
{
    class WindowsContext : PlatformContext
    {
        public static PlatformContext MapFactory()
        {
            return new WindowsContext
            {
                CreateTextContext = CTextContext,
                CreateWindowContext = CWindowContext,
                CreateInputContext = CInputContext,
                CreateRenderer = CRenderer,
                CreateTexture = CTexture,
            };
        }

        private static ITextContext CTextContext(
            string text, string fontFamily, int fontSize,
            FontStretch stretch, FontStyle style, FontWeight weight,
            int maxWidth, int maxHeight,
            TextAlignment alignment)
        {
            return new TypographyTextContext(
                text, fontFamily, fontSize,
                stretch, style, weight,
                maxWidth, maxHeight, alignment);
        }

        private static IWindowContext CWindowContext()
        {
            return new Win32WindowContext();
        }

        private static IInputContext CInputContext()
        {
            return new Win32InputContext();
        }

        private static IRenderer CRenderer()
        {
            return new Win32OpenGLRenderer();
        }

        private static ITexture CTexture()
        {
            return new OpenGLTexture();
        }
    }
}