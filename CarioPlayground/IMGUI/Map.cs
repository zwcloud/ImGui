using System;
namespace ImGui
{
    public delegate ITextContext CTextContext(
        string text, string fontFamily, int fontSize,
        FontStretch stretch, FontStyle style, FontWeight weight,
        int maxWidth, int maxHeight,
        TextAlignment alignment);

    public abstract class Map
    {
        public CTextContext CreateTextContext;
    }
    
    public class MapWindows : Map
    {
        public static Map MapFactory()
        {
            return new MapWindows
            {
                CreateTextContext = CTextContext
            };
        }

        private static ITextContext CTextContext(
            string text, string fontFamily, int fontSize,
            FontStretch stretch, FontStyle style, FontWeight weight,
            int maxWidth, int maxHeight,
            TextAlignment alignment)
        {
            return new DWriteTextContext(
                text, fontFamily, fontSize,
                stretch, style, weight,
                maxWidth, maxHeight, alignment);
        }

    }

    public class MapLinux : Map
    {
        public static Map MapFactory()
        {
            return new MapLinux
            {
                CreateTextContext = CTextContext,
            };
        }

        private static ITextContext CTextContext(
            string text, string fontFamily, int fontSize,
            FontStretch stretch, FontStyle style, FontWeight weight,
            int maxWidth, int maxHeight,
            TextAlignment alignment)
        {
            return new PangoTextContext(
                text, fontFamily, fontSize,
                stretch, style, weight,
                maxWidth, maxHeight,
                alignment);
        }
    }
}