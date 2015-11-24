using System;

namespace IMGUI
{
    internal class DWriteTextLayoutProxy : ITextLayout
    {
        private ZWCloud.DWriteCairo.TextLayout layout;
        private string text;
        private Cairo.Path path;
        private bool dirty;
        private readonly ITextFormat textFormat;
        private Rect rect;

        public DWriteTextLayoutProxy(string text, ITextFormat textFormat, int maxWidth, int maxHeight)
        {
            this.textFormat = textFormat;
            this.text = text;
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
            get { return Layout.MaxWidth; }
            set
            {
                if (Layout.MaxWidth == value)
                {
                    return;
                }
                Layout.MaxWidth = value;
                dirty = true;
            }
        }

        public int MaxHeight
        {
            get { return Layout.MaxHeight; }
            set
            {
                if (Layout.MaxHeight == value)
                {
                    return;
                }
                Layout.MaxHeight = value;
                dirty = true;
            }
        }

        public Rect Rect
        {
            get
            {
                float left, top, width, height;
                Layout.GetRect(out left, out top, out width, out height);
                return new Rect(left, top, width, height);
            }
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
                if(text == value)
                {
                    return;
                }

                text = value;
                Layout = ZWCloud.DWriteCairo.DWriteCairo.CreateTextLayout(
                    text,
                    ((DWriteTextFormatProxy) TextFormat).TextFormat,
                    MaxWidth, MaxHeight);
                dirty = true;
            }
        }

        public ITextFormat TextFormat
        {
            get { return textFormat; }
        }

        public Cairo.Path Path
        {
            get
            {
                if(path == null)
                {
                    throw new InvalidOperationException(
                        "Path is not available beacuse it is not build. Call BuildPath to build it.");
                }
                return path;
            }
        }
        
        public void BuildPath(Cairo.Context context)
        {
            if (path == null || dirty)
            {
                if (path !=null && dirty)
                {
                    Path.Dispose();
                }
                path = ZWCloud.DWriteCairo.DWriteCairo.RenderLayoutToCairoPath(context, Layout);
            }
        }

        public uint XyToIndex(float pointX, float pointY, out bool isInside)
        {
            return layout.XyToIndex(pointX, pointY, out isInside);
        }

        public void IndexToXY(uint textPosition, bool isTrailingHit, out float pointX, out float pointY, out float height)
        {
            layout.IndexToXY(textPosition, isTrailingHit, out pointX, out pointY, out height);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            layout.Dispose();
        }

        #endregion
    }
}