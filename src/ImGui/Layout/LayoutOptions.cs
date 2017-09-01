using System;

namespace ImGui
{
    public struct LayoutOptions
    {
        //TODO implement min/max width/height
        //TODO enable per-entry or per-group alignment adjustment
        internal double? MinWidth;
        internal double? MaxWidth;
        internal double? MinHeight;
        internal double? MaxHeight;
        internal int? HorizontalStretchFactor;
        internal int? VerticalStretchFactor;

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
    }
}
