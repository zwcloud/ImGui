using System;
using ImGui.Common;
using ImGui.Common.Primitive;

namespace ImGui.Layout
{
    internal class LayoutEntry
    {
        public int Id { get; set; }

        public Rect Rect;//border-box
        public double ContentWidth { get; set; }//exact content width, pre-calculated from content and style
        public double ContentHeight { get; set; }//exact content height, pre-calculated from content and style
        public double MinWidth { get; set; } = 1;//minimum width of content-box
        public double MaxWidth { get; set; } = 9999;//maximum width of content-box
        public double MinHeight { get; set; } = 1;//minimum height of content-box
        public double MaxHeight { get; set; } = 9999;//maximum height of content-box
        public int HorizontalStretchFactor { get; set; }//horizontal stretch factor
        public int VerticalStretchFactor { get; set; }//vertical stretch factor

        public bool HorizontallyStretched => !this.IsFixedWidth && this.HorizontalStretchFactor > 0;
        public bool VerticallyStretched => !this.IsFixedHeight && this.VerticalStretchFactor > 0;

        public bool IsFixedWidth => MathEx.AmostEqual(this.MinWidth, this.MaxWidth);
        public bool IsFixedHeight => MathEx.AmostEqual(this.MinHeight, this.MaxHeight);

        public GUIStyle Style;

        public LayoutGroup Parent;

        public LayoutEntry(GUIStyle style, params LayoutOption[] options)
        {
            this.Style = style ?? GUIStyle.Default;
            if (options != null)
            {
                ApplyOptions(options);
            }
        }

        protected void ApplyOptions(LayoutOption[] options)
        {
            if (options == null)
            {
                return;
            }
            foreach (var option in options)
            {
                switch (option.type)
                {
                    case LayoutOptionType.FixedWidth:
                        double horizontalSpace = this.Style.PaddingHorizontal + this.Style.BorderHorizontal;
                        if ((double)option.Value < horizontalSpace)
                        {
                            throw new InvalidOperationException(
                                string.Format("The specified width is too small. It must bigger than the horizontal padding and border size ({0}).", horizontalSpace));
                        }
                        this.MinWidth = this.MaxWidth = (double)option.Value;
                        this.HorizontalStretchFactor = 0;
                        break;
                    case LayoutOptionType.FixedHeight:
                        double verticalSpace = this.Style.PaddingVertical + this.Style.BorderVertical;
                        if ((double)option.Value < verticalSpace)
                        {
                            throw new InvalidOperationException(
                                string.Format("The specified height is too small. It must bigger than the vertical padding and border size ({0}).", verticalSpace));
                        }
                        this.MinHeight = this.MaxHeight = (double)option.Value;
                        this.VerticalStretchFactor = 0;
                        break;
                    case LayoutOptionType.StretchWidth:
                        this.HorizontalStretchFactor = (int)option.Value;
                        break;
                    case LayoutOptionType.StretchHeight:
                        this.VerticalStretchFactor = (int)option.Value;
                        break;
                    case LayoutOptionType.MinWidth:
                    case LayoutOptionType.MaxWidth:
                    case LayoutOptionType.MinHeight:
                    case LayoutOptionType.MaxHeight:
                    case LayoutOptionType.AlignStart:
                    case LayoutOptionType.AlignMiddle:
                    case LayoutOptionType.AlignEnd:
                    case LayoutOptionType.AlignJustify:
                    case LayoutOptionType.EqualSize:
                    case LayoutOptionType.Spacing:
                        //TODO handle min/max width/height
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public virtual void CalcWidth(double unitPartWidth = -1d)
        {
            if (this.HorizontallyStretched)
            {
                if (unitPartWidth > 0)
                {
                    this.Rect.Width = unitPartWidth * this.HorizontalStretchFactor;
                    this.ContentWidth = this.Rect.Width - this.Style.PaddingHorizontal - this.Style.BorderHorizontal;
                }
                else
                {
                    throw new ArgumentException("The unit part width is invalid", nameof(unitPartWidth));
                }
            }
            else if (this.IsFixedWidth)
            {
                this.Rect.Width = this.MinWidth;
                this.ContentWidth = this.Rect.Width - this.Style.PaddingHorizontal - this.Style.BorderHorizontal;
            }
            else
            {
                this.Rect.Width = this.ContentWidth + this.Style.PaddingHorizontal + this.Style.BorderHorizontal;
            }
        }

        public virtual void CalcHeight(double unitPartHeight = -1d)
        {
            if (this.VerticallyStretched)
            {
                if (unitPartHeight > 0)
                {
                    this.Rect.Height = unitPartHeight * this.VerticalStretchFactor;
                    this.ContentHeight = this.Rect.Height - this.Style.PaddingVertical - this.Style.BorderVertical;
                }
                else
                {
                    throw new ArgumentException("The unit part height is invalid", nameof(unitPartHeight));
                }
            }
            else if (this.IsFixedHeight)
            {
                this.Rect.Height = this.MinHeight;
                this.ContentHeight = this.Rect.Height - this.Style.PaddingVertical - this.Style.BorderVertical;
            }
            else
            {
                this.Rect.Height = this.ContentHeight + this.Style.PaddingVertical + this.Style.BorderVertical;
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
