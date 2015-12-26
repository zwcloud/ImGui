using System;

namespace ImGui
{
    internal class DWriteTextContext : ITextContext
    {
        private readonly ZWCloud.DWriteCairo.TextFormat textFormat;
        private ZWCloud.DWriteCairo.TextLayout textLayout;
        private bool dirty;
        private Cairo.Path path;
        private string text;

        public DWriteTextContext(string text, string fontFamily, int fontSize,
            FontStretch stretch, FontStyle style, FontWeight weight,
            int maxWidth, int maxHeight,
            TextAlignment alignment)
        {
            textFormat = ZWCloud.DWriteCairo.DWriteCairo.CreateTextFormat(fontFamily,
                (ZWCloud.DWriteCairo.FontWeight) weight, (ZWCloud.DWriteCairo.FontStyle) style,
                (ZWCloud.DWriteCairo.FontStretch) stretch,
                fontSize);
            this.Alignment = alignment;

            this.text = text;
            textLayout = ZWCloud.DWriteCairo.DWriteCairo.CreateTextLayout(text,
                textFormat, maxWidth, maxHeight);
        }

        public string FontFamily
        {
            get { return textFormat.FontFamilyName; }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight) textLayout.FontWeight; }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle) textLayout.FontStyle; }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch) textLayout.FontStretch; }
        }

        public int FontSize
        {
            get { return (int) textFormat.FontSize; }
        }

        public TextAlignment Alignment
        {
            get { return (TextAlignment) textFormat.TextAlignment; }
            set { textFormat.TextAlignment = (ZWCloud.DWriteCairo.TextAlignment) value; }
        }

        public int MaxWidth
        {
            get { return textLayout.MaxWidth; }
            set
            {
                if(textLayout.MaxWidth == value)
                {
                    return;
                }
                textLayout.MaxWidth = value;
                dirty = true;
            }
        }

        public int MaxHeight
        {
            get { return textLayout.MaxHeight; }
            set
            {
                if(textLayout.MaxHeight == value)
                {
                    return;
                }
                textLayout.MaxHeight = value;
                dirty = true;
            }
        }

        public Rect Rect
        {
            get
            {
                float left, top, width, height;
                textLayout.GetRect(out left, out top, out width, out height);
                return new Rect(left, top, width, height);
            }
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
                var tempMaxWidth = MaxWidth;
                var tempMaxHeight = MaxHeight;
                textLayout.Dispose();
                textLayout = ZWCloud.DWriteCairo.DWriteCairo.CreateTextLayout(
                    text,
                    textFormat,
                    tempMaxWidth, tempMaxHeight);
                dirty = true;
            }
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
            if(path == null || dirty)
            {
                if(path != null && dirty)
                {
                    Path.Dispose();
                }
                path = ZWCloud.DWriteCairo.DWriteCairo.RenderLayoutToCairoPath(context, textLayout);
            }
        }

        public uint XyToIndex(float pointX, float pointY, out bool isInside)
        {
            return textLayout.XyToIndex(pointX, pointY, out isInside);
        }

        public void IndexToXY(uint textPosition, bool isTrailingHit, out float pointX, out float pointY,
            out float height)
        {
            textLayout.IndexToXY(textPosition, isTrailingHit, out pointX, out pointY, out height);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            textFormat.Dispose();
            textLayout.Dispose();
        }

        #endregion
    }
}