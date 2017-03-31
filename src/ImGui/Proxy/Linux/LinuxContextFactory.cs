using System;

namespace ImGui
{
    class LinuxContextFactory : ContextFactory
    {
        public static ContextFactory MapFactory()
        {
            throw new NotImplementedException();
            //return new LinuxContextFactory
            //{
            //    CreateTextContext = CTextContext,
            //    CreateWindowContext = CWindowContext,
            //    CreateInputContext = CInputContext,
            //    CreateRenderer = CRenderer,
            //    CreateTexture = CTexture,
            //};
        }

        private static ITextContext CTextContext(
            string text, string fontFamily, int fontSize,
            FontStretch stretch, FontStyle style, FontWeight weight,
            int maxWidth, int maxHeight,
            TextAlignment alignment)
        {
            throw new NotImplementedException();
        }
        
        private static IWindowContext CWindowContext()
        {
            throw new NotImplementedException();
        }

        private static IInputContext CInputContext()
        {
            throw new NotImplementedException();
        }

        private static IRenderer CRenderer()
        {
            throw new NotImplementedException();
        }

        private static ITexture CTexture()
        {
            throw new NotImplementedException();
        }

    }
}