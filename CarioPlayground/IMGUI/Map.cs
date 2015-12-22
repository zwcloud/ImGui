using System;
namespace ImGui
{
    public delegate ITextFormat CTextFormat(
        string fontFamilyName,
        FontWeight fontWeight, FontStyle fontStyle, FontStretch fontStretch,
        float fontSize);

    public delegate ITextLayout CTextLayout(string text, ITextFormat textFormat, int maxWidth, int maxHeight);

    public abstract class Map
    {
        public CTextFormat CreateTextFormat;
        public CTextLayout CreateTextLayout;
    }
    
#if WINDOWS
    public class MapWindows : Map
    {
        public static Map MapFactory()
        {
            return new MapWindows
            {
                CreateTextFormat = CTextFormat,
                CreateTextLayout = CTextLayout,
            };
        }

        private static ITextFormat CTextFormat(
            string fontFamilyName,
            FontWeight fontWeight, FontStyle fontStyle, FontStretch fontStretch,
            float fontSize)
        {
            return new DWriteTextFormat(fontFamilyName, fontWeight, fontStyle, fontStretch, fontSize);
        }

        private static ITextLayout CTextLayout(string text, ITextFormat textFormat, int maxWidth, int maxHeight)
        {
            return new DWriteTextLayout(text, textFormat, maxWidth, maxHeight);
        }
    }
#elif LINUX
    public class MapLinux : Map
    {
        public static Map MapFactory()
        {
            return new MapLinux
            {
                CreateTextFormat = CTextFormat,
                CreateTextLayout = CTextLayout,
            };
        }

        private static ITextFormat CTextFormat(
            string fontFamilyName,
            FontWeight fontWeight, FontStyle fontStyle, FontStretch fontStretch,
            float fontSize)
        {
            return new PangoTextFormat(fontFamilyName, fontWeight, fontStyle, fontStretch, fontSize);
        }

        private static ITextLayout CTextLayout(string text, ITextFormat textFormat, int maxWidth, int maxHeight)
        {
            return new PangoTextLayout(text, textFormat, maxWidth, maxHeight);
        }
    }
#endif
}