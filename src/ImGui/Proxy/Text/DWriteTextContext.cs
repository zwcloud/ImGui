using System;
using System.Diagnostics;

namespace ImGui
{
    internal class DWriteTextContext : ITextContext
    {
        private readonly DWriteSharp.TextFormat textFormat;
        private DWriteSharp.TextLayout textLayout;
        private string text;

        public DWriteTextContext(string text, string fontFamily, float fontSizeInDip,
            FontStretch stretch, FontStyle style, FontWeight weight,
            int maxWidth, int maxHeight,
            TextAlignment alignment)
        {
            textFormat = DWriteSharp.DWrite.CreateTextFormat(fontFamily,
                (DWriteSharp.FontWeight) weight, (DWriteSharp.FontStyle) style,
                (DWriteSharp.FontStretch) stretch,
                fontSizeInDip);
            this.Alignment = alignment;

            this.text = text;
            textLayout = DWriteSharp.DWrite.CreateTextLayout(text,
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
            set { textFormat.TextAlignment = (DWriteSharp.TextAlignment) value; }
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
                textLayout = DWriteSharp.DWrite.CreateTextLayout(
                    text,
                    textFormat,
                    tempMaxWidth, tempMaxHeight);
            }
        }

        public void Build(Point offset, PointAdder pointAdder, BezierAdder bezierAdder, PathCloser pathCloser, FigureBeginner figureBeginner, FigureEnder figureEnder)
        {
            DWriteSharp.DWrite.RenderLayoutToMesh(textLayout, (float)offset.X, (float)offset.Y, pointAdder.Invoke, bezierAdder.Invoke, pathCloser.Invoke,
                figureBeginner.Invoke, figureEnder.Invoke);
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