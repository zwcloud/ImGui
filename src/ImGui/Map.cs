using System;

namespace ImGui
{
    delegate ITextContext CTextContext(
        string text, string fontFamily, int fontSize,
        FontStretch stretch, FontStyle style, FontWeight weight,
        int maxWidth, int maxHeight,
        TextAlignment alignment);

    abstract class PlatformContext
    {
        public CTextContext CreateTextContext;
        public Func<IWindowContext> CreateWindowContext;
        public Func<IInputContext> CreateInputContext;
        public Func<IRenderer> CreateRenderer;
        public Func<ITexture> CreateTexture;
    }
}