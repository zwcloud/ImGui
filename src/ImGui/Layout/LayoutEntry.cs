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
        public int Id { get; private set; }

        /// <summary>
        /// border-box, the layout result
        /// </summary>
        public Rect Rect;

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
        public double MinWidth { get; set; } = 1;

        /// <summary>
        /// maximum width of border-box
        /// </summary>
        public double MaxWidth { get; set; } = 9999;

        /// <summary>
        /// minimum height of border-box
        /// </summary>
        public double MinHeight { get; set; } = 1;

        /// <summary>
        /// maximum height of border-box
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

        public LayoutEntry()
        {

        }

        public LayoutEntry(int id, Size contentSize)
        {
            this.Id = id;
            this.ContentWidth = contentSize.Width;
            this.ContentHeight = contentSize.Height;

            ApplyStyle();
        }

        public void Init(int id, Size contentSize)
        {
            this.Id = id;
            this.ContentWidth = contentSize.Width;
            this.ContentHeight = contentSize.Height;

            ApplyStyle();
        }

        protected virtual void ApplyStyle()
        {
            var style = Form.current.uiContext.StyleStack.Style;

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
                }
                else
                {
                    throw new ArgumentException("The unit part width is invalid", nameof(unitPartWidth));
                }
            }
            else if (this.IsFixedWidth)
            {
                this.Rect.Width = this.MinWidth;
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

        public double GetDefaultWidth()
        {
            if(this.IsFixedWidth)
            {
                throw new Exception("Cannot get default width of a fixed size entry.");
            }
            return this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
        }

        public double GetDefaultHeight()
        {
            if (this.IsFixedHeight)
            {
                throw new Exception("Cannot get default width of a fixed height entry.");
            }
            return this.ContentHeight + this.PaddingVertical + this.BorderVertical;
        }

        public LayoutEntry Clone()
        {
            return (LayoutEntry)MemberwiseClone();
        }
    }
}
