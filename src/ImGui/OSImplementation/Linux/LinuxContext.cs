using System;
using ImGui.Input;
using ImGui.OSAbstraction;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.OSAbstraction.Window;

namespace ImGui.OSImplementation.Linux
{
    internal class LinuxContext : PlatformContext
    {
        public static PlatformContext MapFactory()
        {
            return new LinuxContext
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

        private static IWindow CWindow(Point position, Size size, WindowTypes windowType)
        {
            LinuxWindow window = new LinuxWindow();
            window.Init(position, size, windowType);
            return window;
        }

        private static void DoChangeCursor(Cursor cursor)
        {
            throw new NotImplementedException();
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