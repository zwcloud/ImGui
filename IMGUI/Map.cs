using System;
namespace ImGui
{
    delegate ITextContext CTextContext(
        string text, string fontFamily, int fontSize,
        FontStretch stretch, FontStyle style, FontWeight weight,
        int maxWidth, int maxHeight,
        TextAlignment alignment);

    delegate IWindowContext CWindowContext();

    delegate IInputContext CInputContext();

    delegate IRenderer CRenderer();

    delegate ITexture CTexture();

    abstract class Map
    {
        public CTextContext CreateTextContext;
        public CWindowContext CreateWindowContext;
        public CInputContext CreateInputContext;
        public CRenderer CreateRenderer;
        public CTexture CreateTexture;
    }

    class MapWindows : Map
    {
        public static Map MapFactory()
        {
            return new MapWindows
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
            return new DWriteTextContext(
                text, fontFamily, fontSizeInDip,
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

    class MapLinux : Map
    {
        public static Map MapFactory()
        {
            return new MapLinux
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
            throw new NotImplementedException();

            //return new PangoTextContext(
            //    text, fontFamily, fontSize,
            //    stretch, style, weight,
            //    maxWidth, maxHeight,
            //    alignment);
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