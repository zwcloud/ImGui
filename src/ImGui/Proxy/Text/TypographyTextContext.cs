using System;
using Typography.TextPrint;

namespace ImGui
{
    /// <summary>
    /// Text context based on Typography
    /// </summary>
    /// <remarks>TypographyTextContext is an pure C# implementation of <see cref="ITextContext"/>.</remarks>
    class TypographyTextContext : ITextContext
    {
        private static TextPrinter thePrinter = new TextPrinter();//use a single and unique text-printer

        public TypographyTextContext(string text, string fontFamily, float fontSizeInDip,
            FontStretch stretch, FontStyle style, FontWeight weight,
            int maxWidth, int maxHeight,
            TextAlignment alignment)
        {
            thePrinter.FontFilename = fontFamily;

            this.FontSize = (int)fontSizeInDip;
            this.Alignment = alignment;
            this.MaxWidth = maxWidth;
            this.MaxHeight = maxHeight;
            this.Text = text;
        }

        #region Implementation of ITextContext

        //TODO Implement those properties when Typography is ready.

        public int FontSize
        {
            get;
            set;
        }

        public TextAlignment Alignment
        {
            get;
            set;
        }

        public int MaxWidth
        {
            get;
            set;
        }

        public int MaxHeight
        {
            get;
            set;
        }

        public Rect Rect
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public void Build(Point offset, TextMesh textMesh)
        {
            Profile.Start("TypographyTextContext.Build");
            thePrinter.FontSizeInPoints = this.FontSize;            
            thePrinter.Draw(textMesh, this.Text.ToCharArray(), (float)offset.X, (float)offset.Y);//TODO remove ToCharArray
            Profile.End();
        }

        public uint XyToIndex(float pointX, float pointY, out bool isInside)
        {
            throw new NotImplementedException();
        }

        public void IndexToXY(uint textPosition, bool isTrailingHit, out float pointX, out float pointY, out float height)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            // nothing: no native res is used.
        }

        #endregion
    }

}
