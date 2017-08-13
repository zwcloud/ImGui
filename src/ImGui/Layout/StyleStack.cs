using System;
using System.Collections.Generic;

namespace ImGui
{
    struct StyleModifier
    {
        public Action<GUIStyle> callback;
    }

    internal class StyleStack
    {
        Stack<GUIStyle> Stack { get; } = new Stack<GUIStyle>();
        Stack<Action<GUIStyle>> ModifierStack { get; } = new Stack<Action<GUIStyle>>();
        public GUIStyle Style
        {
            get
            {
                if(this.Stack.Count > 0)
                {
                    return this.Stack.Peek();
                }
                else
                {
                    return GUISkin.Instance[GUIControlName.Button];
                }
            }
        }

        public void Push(GUIStyle style)
        {
            this.Stack.Push(style);
        }

        public void Pop()
        {
            this.Stack.Pop();
        }

        #region positon, size

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

        public int HorizontalStretchFactor => this.Style.HorizontalStretchFactor;
        public int VerticalStretchFactor => this.Style.VerticalStretchFactor;

        public void PushStretchFactor(bool isVertical, int factor)
        {
            if (isVertical)
            {
                var oldVerticalStretchFactor = this.Style.VerticalStretchFactor;
                this.Style.VerticalStretchFactor = factor;
                Action<GUIStyle> callback = (s) =>
                {
                    s.VerticalStretchFactor = oldVerticalStretchFactor;
                };
                ModifierStack.Push(callback);
            }
            else
            {
                var oldHorizontalStretchFactor = this.Style.HorizontalStretchFactor;
                this.Style.HorizontalStretchFactor = factor;
                Action<GUIStyle> callback = (s) =>
                {
                    s.HorizontalStretchFactor = oldHorizontalStretchFactor;
                };
                ModifierStack.Push(callback);
            }
        }

        public void PopStretchFactor(bool isVertical)
        {
            var restore = ModifierStack.Pop();
            restore(this.Style);
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
            borderStack.Push(border);
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
            paddingStack.Push(padding);
            this.Padding = padding;
        }

        public void PopPadding()
        {
            paddingStack.Pop();
            this.Padding = paddingStack.Count == 0 ? (-1, -1, -1, -1) : paddingStack.Peek();
        }

        #endregion

        #endregion

        #region image, color

        #endregion

    }
}
