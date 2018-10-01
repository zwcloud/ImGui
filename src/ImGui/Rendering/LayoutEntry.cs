using System;
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

        public double BorderTop => this.node.RuleSet.BorderTop;
        public double BorderRight => this.node.RuleSet.BorderRight;
        public double BorderBottom => this.node.RuleSet.BorderBottom;
        public double BorderLeft => this.node.RuleSet.BorderLeft;
        public double BorderHorizontal => BorderLeft + BorderRight;
        public double BorderVertical => BorderTop + BorderBottom;

        public double PaddingTop => this.node.RuleSet.PaddingTop;
        public double PaddingRight => this.node.RuleSet.PaddingRight;
        public double PaddingBottom => this.node.RuleSet.PaddingBottom;
        public double PaddingLeft => this.node.RuleSet.PaddingLeft;
        public double PaddingHorizontal => PaddingLeft + PaddingRight;
        public double PaddingVertical => PaddingTop + PaddingBottom;

        public LayoutEntry(Node node, Size contentSize)
        {
            this.node = node;

            this.ContentWidth = contentSize.Width;
            this.ContentHeight = contentSize.Height;

            if (this.node.RuleSet.IsFixedWidth)
            {
                double horizontalSpace = this.PaddingHorizontal + this.BorderHorizontal;
                var fixedWidth = this.node.RuleSet.MinWidth;
                if(fixedWidth < horizontalSpace)
                {
                    throw new LayoutException(
                        $"Specified width is too small. It must bigger than the horizontal padding and border size ({horizontalSpace}).");
                }
                this.node.RuleSet.HorizontalStretchFactor = 0;
            }

            if (this.node.RuleSet.IsFixedHeight)
            {
                double verticalSpace = this.PaddingVertical + this.BorderVertical;
                var fixedHeight = this.node.RuleSet.MinHeight;
                if (fixedHeight < verticalSpace)
                {
                    throw new LayoutException(
                        $"Specified height is too small. It must bigger than the vertical padding and border size ({verticalSpace}).");
                }
                this.node.RuleSet.VerticalStretchFactor = 0;
            }
        }

        public virtual void CalcWidth(double unitPartWidth = -1d)
        {
            if (this.node.RuleSet.HorizontallyStretched)
            {
                if (unitPartWidth > 0)
                {
                    this.node.Width = unitPartWidth * this.node.RuleSet.HorizontalStretchFactor;
                }
                else
                {
                    throw new ArgumentException("The unit part width is invalid", nameof(unitPartWidth));
                }
            }
            else if (this.node.RuleSet.IsFixedWidth)
            {
                this.node.Width = this.node.RuleSet.MinWidth;
            }
            else
            {
                this.node.Width = this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
            }
        }

        public virtual void CalcHeight(double unitPartHeight = -1d)
        {
            if (this.node.RuleSet.VerticallyStretched)
            {
                if (unitPartHeight > 0)
                {
                    this.node.Height = unitPartHeight * this.node.RuleSet.VerticalStretchFactor;
                    this.ContentHeight = this.node.Height - this.PaddingVertical - this.BorderVertical;
                }
                else
                {
                    throw new ArgumentException("The unit part height is invalid", nameof(unitPartHeight));
                }
            }
            else if (this.node.RuleSet.IsFixedHeight)
            {
                this.node.Height = this.node.RuleSet.MinHeight;
                this.ContentHeight = this.node.Height - this.PaddingVertical - this.BorderVertical;
            }
            else
            {
                this.node.Height = this.ContentHeight + this.PaddingVertical + this.BorderVertical;
            }
        }

        public virtual void SetX(double x)
        {
            this.node.X = x;
        }

        public virtual void SetY(double y)
        {
            this.node.Y = y;
        }

        public double GetDefaultWidth()
        {
            if(this.node.RuleSet.IsFixedWidth)
            {
                throw new LayoutException("Cannot get default width of a fixed size entry.");
            }
            return this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
        }

        public double GetDefaultHeight()
        {
            if (this.node.RuleSet.IsFixedHeight)
            {
                throw new LayoutException("Cannot get default width of a fixed height entry.");
            }
            return this.ContentHeight + this.PaddingVertical + this.BorderVertical;
        }
    }

}
