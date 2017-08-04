using System;
using ImGui.Common.Primitive;

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
        public Func<Point, Size, WindowTypes, IWindow> CreateWindow;
        public Action<Cursor> ChangeCursor;
        public Func<IRenderer> CreateRenderer;
        public Func<ITexture> CreateTexture;
    }
}