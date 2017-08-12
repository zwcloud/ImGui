using System.Collections.Generic;

namespace ImGui.Layout
{
    internal partial class StackLayout
    {
        #region min/max width/height

        Stack<(double, double)> widthStack = new Stack<(double, double)>();
        private (double, double) width { get; set; } = (1, 9999);
        public double MinWidth => this.width.Item1;
        public double MaxWidth => this.width.Item2;

        public void PushWidth((double, double) width)
        {
            widthStack.Push(width);
            this.width = width;
        }

        public void PopWidth()
        {
            widthStack.Pop();
            this.width = widthStack.Count == 0 ? (-1, -1) : widthStack.Peek();
        }


        Stack<(double, double)> heightStack = new Stack<(double, double)>();
        private (double, double) height { get; set; } = (1, 9999);
        public double MinHeight => this.height.Item1;
        public double MaxHeight => this.height.Item2;

        public void PushHeight((double, double) height)
        {
            heightStack.Push(height);
            this.height = height;
        }

        public void PopHeight()
        {
            heightStack.Pop();
            this.height = heightStack.Count == 0 ? (-1, -1) : heightStack.Peek();
        }

        #endregion

        #region stretch factor
        Stack<int> horizontalStretchFactorStack = new Stack<int>();
        Stack<int> verticalStretchFactorStack = new Stack<int>();

        public int HorizontalStretchFactor { get; set; } = 0;
        public int VerticalStretchFactor { get; set; } = 0;

        public void PushStretchFactor(bool isVertical, int factor)
        {
            if (isVertical)
            {
                this.verticalStretchFactorStack.Push(factor);
                this.VerticalStretchFactor = factor;
            }
            else
            {
                this.horizontalStretchFactorStack.Push(factor);
                this.HorizontalStretchFactor = factor;
            }
        }

        public void PopStretchFactor(bool isVertical)
        {
            if (isVertical)
            {
                verticalStretchFactorStack.Pop();
                this.VerticalStretchFactor = verticalStretchFactorStack.Count == 0 ? -1 : verticalStretchFactorStack.Peek();
            }
            else
            {
                horizontalStretchFactorStack.Pop();
                this.HorizontalStretchFactor = horizontalStretchFactorStack.Count == 0 ? -1 : horizontalStretchFactorStack.Peek();
            }
        }

        #endregion

        #region cell spacing

        Stack<double> cellSpacingHorizontalStack = new Stack<double>();
        Stack<double> cellSpacingVerticalStack = new Stack<double>();
        public double CellSpacingHorizontal { get; set; } = 0;
        public double CellSpacingVertical { get; set; } = 0;

        public void PushCellSpacing(bool isVertical, double spacing)
        {
            if(isVertical)
            {
                cellSpacingVerticalStack.Push(spacing);
                CellSpacingVertical = spacing;
            }
            else
            {
                cellSpacingHorizontalStack.Push(spacing);
                CellSpacingHorizontal = spacing;
            }
        }

        public void PopCellSpacing(bool isVertical)
        {
            if(isVertical)
            {
                cellSpacingVerticalStack.Pop();
                this.CellSpacingVertical = cellSpacingVerticalStack.Count == 0 ? -1 : cellSpacingVerticalStack.Peek();
            }
            else
            {
                cellSpacingHorizontalStack.Pop();
                this.CellSpacingHorizontal = cellSpacingHorizontalStack.Count == 0 ? -1 : cellSpacingHorizontalStack.Peek();
            }
        }

        #endregion

        #region alignment

        Stack<Alignment> alignmentHorizontalStack = new Stack<Alignment>();
        Stack<Alignment> alignmentVerticalStack = new Stack<Alignment>();
        public Alignment AlignmentHorizontal { get; set; } = Alignment.Start;
        public Alignment AlignmentVertical { get; set; } = Alignment.Start;

        public void PushAlignment(bool isVertical, Alignment spacing)
        {
            if (isVertical)
            {
                alignmentVerticalStack.Push(spacing);
                AlignmentVertical = spacing;
            }
            else
            {
                alignmentHorizontalStack.Push(spacing);
                AlignmentHorizontal = spacing;
            }
        }

        public void PopAlignment(bool isVertical)
        {
            if (isVertical)
            {
                alignmentVerticalStack.Pop();
                this.AlignmentVertical = alignmentVerticalStack.Count == 0 ? Alignment.Undefined : alignmentVerticalStack.Peek();
            }
            else
            {
                alignmentHorizontalStack.Pop();
                this.AlignmentHorizontal = alignmentHorizontalStack.Count == 0 ? Alignment.Undefined : alignmentHorizontalStack.Peek();
            }
        }

        #endregion

        #region box model
        Stack<(double, double, double, double)> borderStack = new Stack<(double, double, double, double)>();
        public (double, double, double, double) Border { get; set; } = (0, 0, 0, 0);

        public void PushBorder((double, double, double, double) border)
        {
            this.Border = border;
        }

        public void PopBorder()
        {
            borderStack.Pop();
            this.Border = borderStack.Count == 0 ? (-1, -1, -1, -1) : borderStack.Peek();
        }

        Stack<(double, double, double, double)> paddingStack = new Stack<(double, double, double, double)>();
        public (double, double, double, double) Padding { get; set; } = (0, 0, 0, 0);

        public void PushPadding((double, double, double, double) padding)
        {
            this.Padding = padding;
        }

        public void PopPadding()
        {
            paddingStack.Pop();
            this.Padding = paddingStack.Count == 0 ? (-1, -1, -1, -1) : paddingStack.Peek();
        }

        #endregion

    }
}
