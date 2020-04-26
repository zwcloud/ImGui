using System;
using ImGui.Input;
using ImGui.OSAbstraction;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.OSAbstraction.Window;

namespace ImGui.OSImplementation.Android
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

        private static ITextContext CTextContext(string text, string fontFamily, double fontSize, TextAlignment alignment)
        {
            return new TypographyTextContext(text, fontFamily, fontSize, alignment);
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
            return new OpenGLESTexture();
        }
    }
}