using System;
using ImGui.Common.Primitive;
using ImGui.Input;
using ImGui.OSAbstraction;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.OSAbstraction.Window;

namespace ImGui.OSImplentation.Android
{
    internal class AndroidContext : PlatformContext
    {
        public static PlatformContext MapFactory()
        {
            return new AndroidContext
            {
                CreateTextContext = CTextContext,
                CreateWindow = CWindow,
                ChangeCursor = DoChangeCursor,
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

        private static IWindow CWindow(Point point, Size size, WindowTypes windowType)
        {
            AndroidWindow window = new AndroidWindow();
            window.Init();
            window.Size = size;
            return window;
        }

        private static void DoChangeCursor(Cursor cursor)
        {
            //nothing to do
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