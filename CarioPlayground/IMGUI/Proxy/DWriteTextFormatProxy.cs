using System;

namespace ImGui
{
    internal class DWriteTextFormatProxy : ITextFormat
    {
        public readonly ZWCloud.DWriteCairo.TextFormat textFormat;

        public ZWCloud.DWriteCairo.TextFormat TextFormat
        {
            get { return textFormat; }
        }

        public DWriteTextFormatProxy(
            string fontFamilyName,
            FontWeight fontWeight, FontStyle fontStyle, FontStretch fontStretch,
            float fontSize)
        {
            textFormat = ZWCloud.DWriteCairo.DWriteCairo.CreateTextFormat(fontFamilyName,
                (ZWCloud.DWriteCairo.FontWeight) fontWeight, (ZWCloud.DWriteCairo.FontStyle) fontStyle,
                (ZWCloud.DWriteCairo.FontStretch) fontStretch,
                fontSize);
        }

        public string FontFamily
        {
            get { return TextFormat.FontFamilyName; }
        }

        public int FontSize
        {
            get { return (int) TextFormat.FontSize; }
            set { throw new NotImplementedException(); }
        }

        public TextAlignment Alignment
        {
            get { return (TextAlignment) TextFormat.TextAlignment; }
            set { TextFormat.TextAlignment = (ZWCloud.DWriteCairo.TextAlignment) value; }
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            textFormat.Dispose();
        }

        #endregion
    }
}