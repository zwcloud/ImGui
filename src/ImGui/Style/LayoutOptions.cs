using System;

namespace ImGui
{
    public struct LayoutOptions
    {
        //TODO stateful options
        //TODO implement min/max width/height
        //TODO enable per-entry or per-group alignment adjustment
        internal double? MinWidth;
        internal double? MaxWidth;
        internal double? MinHeight;
        internal double? MaxHeight;
        internal int? HorizontalStretchFactor;
        internal int? VerticalStretchFactor;

        internal Color? fontColor;
        internal double? fontSize;
        internal string fontFamily;
        internal TextAlignment? textAlignment;

        public LayoutOptions Width(double width)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
            this.MinWidth = this.MaxWidth = width;
            return this;
        }

        public LayoutOptions Height(double height)
        {
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
            this.MinHeight = this.MaxHeight = height;
            return this;
        }

        public LayoutOptions ExpandWidth(bool expand)
        {
            this.HorizontalStretchFactor = expand ? 1 : 0;
            return this;
        }

        public LayoutOptions ExpandHeight(bool expand)
        {
            this.VerticalStretchFactor = expand ? 1 : 0;
            return this;
        }

        public LayoutOptions StretchWidth(int factor)
        {
            if (factor <= 0) throw new ArgumentOutOfRangeException(nameof(factor), "The stretch factor must be positive.");
            this.HorizontalStretchFactor = factor;
            return this;
        }

        public LayoutOptions StretchHeight(int factor)
        {
            if (factor <= 0) throw new ArgumentOutOfRangeException(nameof(factor), "The stretch factor must be positive.");
            this.VerticalStretchFactor = factor;
            return this;
        }

        public LayoutOptions FontFamily(string family)
        {
            this.fontFamily = family;
            return this;
        }

        public LayoutOptions FontSize(double size)
        {
            this.fontSize = size;
            return this;
        }

        public LayoutOptions FontColor(Color color)
        {
            this.fontColor = color;
            return this;
        }

        public LayoutOptions TextAlignment(TextAlignment textAlignment)
        {
            this.textAlignment = textAlignment;
            return this;
        }

    }
}
