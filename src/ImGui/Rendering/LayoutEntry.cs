using System;
using ImGui.Common;
using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class LayoutEntry
    {
        protected Node node;

        /// <summary>
        /// exact content width, externally pre-calculated from content and style
        /// </summary>
        public double ContentWidth { get; set; }

        /// <summary>
        /// exact content height, externally pre-calculated from content and style
        /// </summary>
        public double ContentHeight { get; set; }

        /// <summary>
        /// minimum width of border-box
        /// </summary>
        public double MinWidth { get; set; }

        /// <summary>
        /// maximum width of border-box
        /// </summary>
        public double MaxWidth { get; set; }

        /// <summary>
        /// minimum height of border-box
        /// </summary>
        public double MinHeight { get; set; }

        /// <summary>
        /// maximum height of border-box
        /// </summary>
        public double MaxHeight { get; set; }

        /// <summary>
        /// horizontal stretch factor
        /// </summary>
        public int HorizontalStretchFactor { get; set; }

        /// <summary>
        /// vertical stretch factor
        /// </summary>
        public int VerticalStretchFactor { get; set; }

        /// <summary>
        /// Is this entry or group horizontally stretched?
        /// </summary>
        public bool HorizontallyStretched => !this.IsFixedWidth && this.HorizontalStretchFactor > 0;

        /// <summary>
        /// Is this entry or group vertically stretched?
        /// </summary>
        public bool VerticallyStretched => !this.IsFixedHeight && this.VerticalStretchFactor > 0;

        /// <summary>
        /// Is this entry or group has a fixed width?
        /// </summary>
        public bool IsFixedWidth => MathEx.AmostEqual(this.MinWidth, this.MaxWidth);

        /// <summary>
        /// Is this entry or group has a fixed height?
        /// </summary>
        public bool IsFixedHeight => MathEx.AmostEqual(this.MinHeight, this.MaxHeight);

        public bool IsDefaultWidth => !IsFixedWidth && !HorizontallyStretched;

        public bool IsDefaultHeight => !IsFixedWidth && !VerticallyStretched;

        public bool IsStretchedWidth => HorizontallyStretched;

        public bool IsStretchedHeight => VerticallyStretched;

        public (double, double, double, double) Border;

        public (double, double, double, double) Padding;

        public double BorderTop => Border.Item1;
        public double BorderRight => Border.Item2;
        public double BorderBottom => Border.Item3;
        public double BorderLeft => Border.Item4;
        public double BorderHorizontal => BorderLeft + BorderRight;
        public double BorderVertical => BorderTop + BorderBottom;

        public double PaddingTop => Padding.Item1;
        public double PaddingRight => Padding.Item2;
        public double PaddingBottom => Padding.Item3;
        public double PaddingLeft => Padding.Item4;
        public double PaddingHorizontal => PaddingLeft + PaddingRight;
        public double PaddingVertical => PaddingTop + PaddingBottom;
        
        public LayoutEntry(Node node, Size contentSize, LayoutOptions? options)
        {
            this.node = node;
            this.ContentWidth = 0;
            this.ContentHeight = 0;
            this.node.Rect = Rect.Zero;
            this.MinWidth = 1;
            this.MaxWidth = 9999;
            this.MinHeight = 1;
            this.MaxHeight = 9999;
            this.HorizontalStretchFactor = 0;
            this.VerticalStretchFactor = 0;
            this.Border = (0, 0, 0, 0);
            this.Padding = (0, 0, 0, 0);

            this.ContentWidth = contentSize.Width;
            this.ContentHeight = contentSize.Height;

            this.ApplyStyle();
            if (options.HasValue)
            {
                this.ApplyOptions(options.Value);
            }
        }

        protected void ApplyStyle()
        {
            var style = GUIStyle.Basic;

            this.MinWidth = MathEx.Clamp(style.MinWidth, 1, 9999);
            this.MaxWidth = MathEx.Clamp(style.MaxWidth, 1, 9999);
            this.MinHeight = MathEx.Clamp(style.MinHeight, 1, 9999);
            this.MaxHeight = MathEx.Clamp(style.MaxHeight, 1, 9999);
            if (this.MinWidth > this.MaxWidth)
            {
                this.MaxWidth = this.MinWidth;
            }
            if (this.MinHeight > this.MaxHeight)
            {
                this.MaxHeight = this.MinHeight;
            }

            this.Border = style.Border;
            this.Padding = style.Padding;

            if (IsFixedWidth)
            {
                double horizontalSpace = this.PaddingHorizontal + this.BorderHorizontal;
                var fixedWidth = MinWidth;
                if(fixedWidth < horizontalSpace)
                {
                    throw new LayoutException(
                        $"Specified width is too small. It must bigger than the horizontal padding and border size ({horizontalSpace}).");
                }
                this.HorizontalStretchFactor = 0;
            }
            else
            {
                this.HorizontalStretchFactor = style.HorizontalStretchFactor;
            }

            if (IsFixedHeight)
            {
                double verticalSpace = this.PaddingVertical + this.BorderVertical;
                var fixedHeight = MinHeight;
                if (fixedHeight < verticalSpace)
                {
                    throw new LayoutException(
                        $"Specified height is too small. It must bigger than the vertical padding and border size ({verticalSpace}).");
                }
                this.VerticalStretchFactor = 0;
            }
            else
            {
                this.VerticalStretchFactor = style.VerticalStretchFactor;
            }
        }

        protected void ApplyOptions(LayoutOptions options)
        {
            var style = GUIStyle.Basic;

            if(options.MinWidth.HasValue && options.MaxWidth.HasValue)
            {
                var value = options.MinWidth.Value;
                if (value < style.PaddingHorizontal + style.BorderHorizontal)
                {
                    throw new LayoutException(
                        $"The specified width is too small. It must bigger than the horizontal padding and border size ({style.PaddingHorizontal + style.BorderHorizontal}).");
                }
                this.MinWidth = this.MaxWidth = value;
                this.HorizontalStretchFactor = 0;
            }
            if (options.MinHeight.HasValue && options.MaxHeight.HasValue)
            {
                var value = options.MinHeight.Value;
                if (value < style.PaddingVertical + style.BorderVertical)
                {
                    throw new LayoutException(
                        $"The specified height is too small. It must bigger than the vertical padding and border size ({style.PaddingVertical + style.BorderVertical}).");
                }
                this.MinHeight = this.MaxHeight = value;
                this.VerticalStretchFactor = 0;
            }
            if(options.HorizontalStretchFactor.HasValue)
            {
                this.HorizontalStretchFactor = options.HorizontalStretchFactor.Value;
            }
            if (options.VerticalStretchFactor.HasValue)
            {
                this.VerticalStretchFactor = options.VerticalStretchFactor.Value;
            }
        }

        public virtual void CalcWidth(double unitPartWidth = -1d)
        {
            if (this.HorizontallyStretched)
            {
                if (unitPartWidth > 0)
                {
                    this.node.Rect.Width = unitPartWidth * this.HorizontalStretchFactor;
                }
                else
                {
                    throw new ArgumentException("The unit part width is invalid", nameof(unitPartWidth));
                }
            }
            else if (this.IsFixedWidth)
            {
                this.node.Rect.Width = this.MinWidth;
            }
            else
            {
                this.node.Rect.Width = this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
            }
        }

        public virtual void CalcHeight(double unitPartHeight = -1d)
        {
            if (this.VerticallyStretched)
            {
                if (unitPartHeight > 0)
                {
                    this.node.Rect.Height = unitPartHeight * this.VerticalStretchFactor;
                    this.ContentHeight = this.node.Rect.Height - this.PaddingVertical - this.BorderVertical;
                }
                else
                {
                    throw new ArgumentException("The unit part height is invalid", nameof(unitPartHeight));
                }
            }
            else if (this.IsFixedHeight)
            {
                this.node.Rect.Height = this.MinHeight;
                this.ContentHeight = this.node.Rect.Height - this.PaddingVertical - this.BorderVertical;
            }
            else
            {
                this.node.Rect.Height = this.ContentHeight + this.PaddingVertical + this.BorderVertical;
            }
        }

        public virtual void SetX(double x)
        {
            this.node.Rect.X = x;
        }

        public virtual void SetY(double y)
        {
            this.node.Rect.Y = y;
        }

        public double GetDefaultWidth()
        {
            if(this.IsFixedWidth)
            {
                throw new LayoutException("Cannot get default width of a fixed size entry.");
            }
            return this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
        }

        public double GetDefaultHeight()
        {
            if (this.IsFixedHeight)
            {
                throw new LayoutException("Cannot get default width of a fixed height entry.");
            }
            return this.ContentHeight + this.PaddingVertical + this.BorderVertical;
        }
    }

}
