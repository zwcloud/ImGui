using System;
using System.Diagnostics;
using ImGui.Common;
using ImGui.Common.Primitive;

namespace ImGui.Layout
{
    [DebuggerDisplay("Entry {Id}, Rect={Rect}")]
    internal class LayoutEntry
    {
        /// <summary>
        /// identifier number of this entry/group
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// border-box, the layout result
        /// </summary>
        public Rect Rect;

        /// <summary>
        /// exact content width, pre-calculated from content and style
        /// </summary>
        public double ContentWidth { get; set; }

        /// <summary>
        /// exact content height, pre-calculated from content and style
        /// </summary>
        public double ContentHeight { get; set; }

        /// <summary>
        /// minimum width of content-box
        /// </summary>
        public double MinWidth { get; set; } = 1;

        /// <summary>
        /// maximum width of content-box
        /// </summary>
        public double MaxWidth { get; set; } = 9999;

        /// <summary>
        /// minimum height of content-box
        /// </summary>
        public double MinHeight { get; set; } = 1;

        /// <summary>
        /// maximum height of content-box
        /// </summary>
        public double MaxHeight { get; set; } = 9999;

        private int h = 0;
        /// <summary>
        /// horizontal stretch factor
        /// </summary>
        public int HorizontalStretchFactor { get { return h; }
            set { h = value; } }

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

        public (double, double, double, double) Border = (0, 0, 0, 0);

        public (double, double, double, double) Padding = (0, 0, 0, 0);

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

        public LayoutEntry(int id, GUIStyle style, Size contentSize)
        {
            this.Id = id;
            this.ContentWidth = contentSize.Width;
            this.ContentHeight = contentSize.Height;

            ApplyStyle(style);
        }

        protected virtual void ApplyStyle(GUIStyle style)
        {
            if(style == null)
            {
                style = GUIStyle.Default;
            }

            this.MinWidth = MathEx.Clamp(this.MinWidth, 1, 9999);
            this.MaxWidth = MathEx.Clamp(this.MaxWidth, 1, 9999);
            this.MinHeight = MathEx.Clamp(this.MinHeight, 1, 9999);
            this.MaxHeight = MathEx.Clamp(this.MaxHeight, 1, 9999);
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
                    throw new InvalidOperationException(
                        string.Format("Specified width is too small. It must bigger than the horizontal padding and border size ({0}).", horizontalSpace));
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
                    throw new InvalidOperationException(
                        string.Format("Specified height is too small. It must bigger than the vertical padding and border size ({0}).", verticalSpace));
                }
                this.VerticalStretchFactor = 0;
            }
            else
            {
                this.VerticalStretchFactor = style.VerticalStretchFactor;
            }
        }

        public virtual void CalcWidth(double unitPartWidth = -1d)
        {
            if (this.HorizontallyStretched)
            {
                if (unitPartWidth > 0)
                {
                    this.Rect.Width = unitPartWidth * this.HorizontalStretchFactor;
                    this.ContentWidth = this.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal;
                }
                else
                {
                    throw new ArgumentException("The unit part width is invalid", nameof(unitPartWidth));
                }
            }
            else if (this.IsFixedWidth)
            {
                this.Rect.Width = this.MinWidth;
                this.ContentWidth = this.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal;
            }
            else
            {
                this.Rect.Width = this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
            }
        }

        public virtual void CalcHeight(double unitPartHeight = -1d)
        {
            if (this.VerticallyStretched)
            {
                if (unitPartHeight > 0)
                {
                    this.Rect.Height = unitPartHeight * this.VerticalStretchFactor;
                    this.ContentHeight = this.Rect.Height - this.PaddingVertical - this.BorderVertical;
                }
                else
                {
                    throw new ArgumentException("The unit part height is invalid", nameof(unitPartHeight));
                }
            }
            else if (this.IsFixedHeight)
            {
                this.Rect.Height = this.MinHeight;
                this.ContentHeight = this.Rect.Height - this.PaddingVertical - this.BorderVertical;
            }
            else
            {
                this.Rect.Height = this.ContentHeight + this.PaddingVertical + this.BorderVertical;
            }
        }

        public virtual void SetX(double x)
        {
            this.Rect.X = x;
        }

        public virtual void SetY(double y)
        {
            this.Rect.Y = y;
        }

        public LayoutEntry Clone()
        {
            return (LayoutEntry)MemberwiseClone();
        }
    }
}
