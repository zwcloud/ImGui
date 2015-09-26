using System;
using ZWCloud.DWriteCairo;

namespace IMGUI
{
    internal class DWriteTextLayoutProxy : ITextLayout
    {
        public ZWCloud.DWriteCairo.TextLayout layout;
        private string text;

        public DWriteTextLayoutProxy(string text, ITextFormat textFormat, float maxWidth, float maxHeight)
        {
            layout = ZWCloud.DWriteCairo.DWriteCairo.CreateTextLayout(text, ((DWriteTextFormatProxy) textFormat).TextFormat, maxWidth, maxHeight);
        }

        private ZWCloud.DWriteCairo.TextLayout Layout
        {
            get { return layout; }
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException();
                }
                if(layout != null)
                {
                    layout.Dispose();
                }

                layout = value;
            }
        }

        public int MaxWidth
        {
            get { return (int) Layout.Width; }
            set { Layout.Width = value; }
        }

        public int MaxHeight
        {
            get { return (int) Layout.Height; }
            set { Layout.Height = value; }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight) Layout.FontWeight; }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)Layout.FontStyle; }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch)Layout.FontStretch; }
        }

        public string Text
        {
            get
            {
                if(text == null)
                {
                    return string.Empty;
                }
                return text;
            }
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException();
                }
                text = value;
                Layout = ZWCloud.DWriteCairo.DWriteCairo.CreateTextLayout(
                    text,
                    ((DWriteTextFormatProxy) TextFormat).TextFormat,
                    MaxWidth, MaxHeight);
            }
        }

        public ITextFormat TextFormat { get; set; }

        public void Show(Cairo.Context context)
        {
            ZWCloud.DWriteCairo.DWriteCairo.ShowLayout(context, Layout);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            layout.Dispose();
        }

        #endregion
    }
}