using System;

namespace ImGui
{
    class AndroidContextFactory : ContextFactory
    {
        public static ContextFactory MapFactory()
        {
            return new AndroidContextFactory
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
                text, fontFamily, fontSize,//TODO confirm fontSize matters
                stretch, style, weight,
                maxWidth, maxHeight, alignment);
        }

        private static IWindowContext CWindowContext()
        {
            return new AndroidWindowContext();
        }

        private static IInputContext CInputContext()
        {
            return new AndroidInputContext();
        }

        private static IRenderer CRenderer()
        {
            return new OpenGLESRenderer();
        }

        private static ITexture CTexture()
        {
            throw new NotImplementedException();
        }
    }
}