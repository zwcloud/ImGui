using System;

namespace ImGui.Rendering
{
    internal partial class Node
    {
        public double BorderTop => this.RuleSet.BorderTop;
        public double BorderRight => this.RuleSet.BorderRight;
        public double BorderBottom => this.RuleSet.BorderBottom;
        public double BorderLeft => this.RuleSet.BorderLeft;
        public double BorderHorizontal => BorderLeft + BorderRight;
        public double BorderVertical => BorderTop + BorderBottom;

        public double PaddingTop => this.RuleSet.PaddingTop;
        public double PaddingRight => this.RuleSet.PaddingRight;
        public double PaddingBottom => this.RuleSet.PaddingBottom;
        public double PaddingLeft => this.RuleSet.PaddingLeft;
        public double PaddingHorizontal => PaddingLeft + PaddingRight;
        public double PaddingVertical => PaddingTop + PaddingBottom;

        public void CheckRuleSetForLayout_Entry()
        {
            //TODO recheck this.RuleSet when layout-related rules are added or changed

            if (this.RuleSet.IsFixedWidth)
            {
                double horizontalSpace = this.PaddingHorizontal + this.BorderHorizontal;
                var fixedWidth = this.RuleSet.MinWidth;
                if(fixedWidth < horizontalSpace)
                {
                    throw new LayoutException(
                        $"Specified width is too small. It must bigger than the horizontal padding and border size ({horizontalSpace}).");
                }
                this.RuleSet.HorizontalStretchFactor = 0;
            }

            if (this.RuleSet.IsFixedHeight)
            {
                double verticalSpace = this.PaddingVertical + this.BorderVertical;
                var fixedHeight = this.RuleSet.MinHeight;
                if (fixedHeight < verticalSpace)
                {
                    throw new LayoutException(
                        $"Specified height is too small. It must bigger than the vertical padding and border size ({verticalSpace}).");
                }
                this.RuleSet.VerticalStretchFactor = 0;
            }
        }

        public void CalcWidth_Entry(double unitPartWidth = -1d)
        {
            if (this.RuleSet.HorizontallyStretched)
            {
                if (unitPartWidth > 0)
                {
                    this.Width = unitPartWidth * this.RuleSet.HorizontalStretchFactor;
                }
                else
                {
                    throw new ArgumentException("The unit part width is invalid", nameof(unitPartWidth));
                }
            }
            else if (this.RuleSet.IsFixedWidth)
            {
                this.Width = this.RuleSet.MinWidth;
                this.ContentWidth = Width - PaddingHorizontal - BorderHorizontal;
                if (ContentWidth < NaturalWidth)
                {
                    ContentWidth = NaturalWidth;
                    Width = NaturalWidth + PaddingHorizontal + BorderHorizontal;
                }
            }
            else
            {
                this.Width = this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
            }
        }

        public void CalcHeight_Entry(double unitPartHeight = -1d)
        {
            if (this.RuleSet.VerticallyStretched)
            {
                if (unitPartHeight > 0)
                {
                    this.Height = unitPartHeight * this.RuleSet.VerticalStretchFactor;
                    this.ContentHeight = this.Height - this.PaddingVertical - this.BorderVertical;
                }
                else
                {
                    throw new ArgumentException("The unit part height is invalid", nameof(unitPartHeight));
                }
            }
            else if (this.RuleSet.IsFixedHeight)
            {
                this.Height = this.RuleSet.MinHeight;
                this.ContentHeight = this.Height - this.PaddingVertical - this.BorderVertical;
                if (ContentHeight < this.NaturalHeight)
                {
                    ContentHeight = this.NaturalHeight;
                    Height = this.NaturalHeight + PaddingVertical + BorderVertical;
                }
            }
            else
            {
                this.Height = this.ContentHeight + this.PaddingVertical + this.BorderVertical;
            }
        }

        public void SetX_Entry(double x)
        {
            this.X = x;
            this.ContentRect.X = x + this.PaddingLeft + this.BorderLeft;
        }

        public void SetY_Entry(double y)
        {
            this.Y = y;
            this.ContentRect.Y = y + this.PaddingTop + this.BorderTop;
        }

        public double GetDefaultWidth()
        {
            return this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
        }

        public double GetDefaultHeight()
        {
            return this.ContentHeight + this.PaddingVertical + this.BorderVertical;
        }
    }

}
