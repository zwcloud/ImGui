using System;

namespace ImGui
{
    class LinuxContext : PlatformContext
    {
        public static PlatformContext MapFactory()
        {
            return new LinuxContext
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
            var fontSizeInDip = Utility.PointToDip(fontSize);
            return new TypographyTextContext(
                text, fontFamily, fontSizeInDip,
                stretch, style, weight,
                maxWidth, maxHeight, alignment);
        }
        
        private static IWindowContext CWindowContext()
        {
            return new LinuxWindowContext();
        }

        private static IInputContext CInputContext()
        {
            return new LinuxInputContext();
        }

        private static IRenderer CRenderer()
        {
            return new LinuxOpenGLRenderer();
        }

        private static ITexture CTexture()
        {
            throw new NotImplementedException();
        }

    }
}