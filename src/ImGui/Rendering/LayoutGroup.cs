using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.Rendering
{
    internal partial class Node
    {
        public double CellSpacingHorizontal { get; set; } = 0;
        public double CellSpacingVertical { get; set; } = 0;
        public Alignment AlignmentHorizontal { get; set; } = Alignment.Start;
        public Alignment AlignmentVertical { get; set; } = Alignment.Start;

        public bool IsVertical { get; private set; }

        protected void Group_Reset()
        {
            Entry_Reset();
            if (this.Children == null)
            {
                this.Children = new List<Node>();
            }
            else
            {
                this.Children.Clear();
            }

            this.CellSpacingHorizontal = 0;
            this.CellSpacingVertical = 0;
            this.AlignmentHorizontal = Alignment.Start;
            this.AlignmentVertical = Alignment.Start;
        }

        public void Group_Init(int id, bool isVertical, LayoutOptions? options)
        {
            this.Group_Reset();

            this.Id = id;
            //NOTE content size is always a calculated value

            this.IsVertical = isVertical;

            this.Group_ApplyStyle();
            if(options.HasValue)
            {
                this.ApplyOptions(options.Value);
            }
        }

        protected void Group_ApplyStyle()
        {
            Entry_ApplyStyle();

            var style = GUIStyle.Basic;

            var csh = style.CellSpacingHorizontal;
            if(csh >= 0)
            {
                this.CellSpacingHorizontal = csh;
            }
            var csv = style.CellSpacingVertical;
            if (csv >= 0)
            {
                this.CellSpacingVertical = csv;
            }
            this.AlignmentHorizontal = style.AlignmentHorizontal;
            this.AlignmentVertical = style.AlignmentVertical;
        }

        /// <summary>
        /// Append child entry to this group
        /// </summary>
        /// <param name="item"></param>
        public void Add(Node item)
        {
            if (this.Children == null)
            {
                this.Children = new List<Node>();
            }

            if (this.IsFixedWidth)
            {
                Debug.Assert(!this.HorizontallyStretched);
                if (this.IsVertical && item.HorizontalStretchFactor > 1)
                {
                    item.HorizontalStretchFactor = 1;
                }
            }
            else if (this.HorizontallyStretched)
            {
                if (this.IsVertical && item.HorizontalStretchFactor > 1)
                {
                    item.HorizontalStretchFactor = 1;
                }
            }
            else
            {
                item.HorizontalStretchFactor = 0;
            }

            if (this.IsFixedHeight)
            {
                Debug.Assert(!this.VerticallyStretched);
                if (!this.IsVertical && item.VerticalStretchFactor > 1)
                {
                    item.VerticalStretchFactor = 1;
                }
            }
            else if (this.VerticallyStretched)
            {
                if (!this.IsVertical && item.VerticalStretchFactor > 1)
                {
                    item.VerticalStretchFactor = 1;
                }
            }
            else
            {
                item.VerticalStretchFactor = 0;
            }

            item.Parent = this;
            this.Children.Add(item);
        }

        public void Group_CalcWidth(double unitPartWidth = -1)
        {
            if (this.HorizontallyStretched)//stretched width
            {
                // calculate the width
                this.Rect.Width = unitPartWidth * this.HorizontalStretchFactor;
                if (this.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal < 1)
                {
                    throw new LayoutException(
                        $"The width of Group<{this.Id}> is too small to hold any children.");
                }
                this.ContentWidth = this.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal;

                // calculate the width of children
                this.CalcChildrenWidth();
            }
            else if (this.IsFixedWidth)//fixed width
            {
                // calculate the width
                this.Rect.Width = this.MinWidth;

                if (this.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal < 1)
                {
                    throw new LayoutException($"The width of Group<{this.Id}> is too small to hold any children.");
                    return;
                }
                this.ContentWidth = this.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal;

                // calculate the width of children
                this.CalcChildrenWidth();
            }
            else // default width
            {
                if (this.IsVertical) //vertical group
                {
                    var temp = 0d;
                    // get the max width of children
                    foreach (var entry in this.Children)
                    {
                        entry.CalcWidth();
                        temp = Math.Max(temp, entry.Rect.Width);
                    }
                    this.ContentWidth = temp;
                }
                else
                {
                    var temp = 0d;
                    foreach (var entry in this.Children)
                    {
                        entry.CalcWidth();
                        temp += entry.Rect.Width + this.CellSpacingHorizontal;
                    }
                    temp -= this.CellSpacingHorizontal;
                    this.ContentWidth = temp < 0 ? 0 : temp;
                }
                this.Rect.Width = this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
            }
        }

        private void CalcChildrenWidth()
        {
            if (this.IsVertical) //vertical group
            {
                foreach (var entry in this.Children)
                {
                    if (entry.HorizontallyStretched)
                    {
                        entry.CalcWidth(this.ContentWidth);
                        //the unitPartWidth
                        //(actually every entry will have this width, because entry.HorizontalStretchFactor is always 1 in this case. See `LayoutGroup.Add`.)
                        //for stretched children is the content-box width of the group
                    }
                    else
                    {
                        entry.CalcWidth();
                    }
                }
            }
            else //horizontal group
            {
                // calculate the unitPartWidth for stretched children
                // calculate the width of fixed-size children

                var childCount = this.Children.Count;
                var cellSpacingWidth = this.CellSpacingHorizontal * (childCount - 1);
                if(cellSpacingWidth >= this.ContentWidth)
                {
                    throw new LayoutException(
                        $"Group<{this.Id}> doesn't have enough width for horizontal-cell-spacing<{this.CellSpacingHorizontal}> with {childCount} children.");
                }

                var widthWithoutCellSpacing = this.ContentWidth - cellSpacingWidth;

                double minWidthOfEntries = 0;
                double minStretchedWidth = 0;
                foreach (var entry in this.Children)
                {
                    if (entry.HorizontallyStretched)
                    {
                        var defaultWidth = entry.GetDefaultWidth();
                        minWidthOfEntries += defaultWidth;
                        minStretchedWidth += defaultWidth;
                    }
                    else if(entry.IsFixedWidth)
                    {
                        minWidthOfEntries += entry.MinWidth;
                    }
                    else
                    {
                        minWidthOfEntries += entry.GetDefaultWidth();
                    }
                }

                if(minWidthOfEntries > widthWithoutCellSpacing)//overflow
                {
                    var factor = 0;
                    foreach (var entry in this.Children)
                    {
                        if (entry.HorizontallyStretched)
                        {
                            factor += entry.HorizontalStretchFactor;
                        }
                    }
                    var unit = minStretchedWidth / factor;
                    // change all HorizontallyStretched entries to fixed width
                    foreach (var entry in this.Children)
                    {
                        if (entry.HorizontallyStretched)
                        {
                            entry.MinWidth = entry.MaxWidth = unit * entry.HorizontalStretchFactor;
                            entry.HorizontalStretchFactor = 0;
                        }
                        entry.CalcWidth();
                    }
                }
                else
                {
                    var factor = 0;
                    foreach (var entry in this.Children)
                    {
                        if (entry.HorizontallyStretched)
                        {
                            factor += entry.HorizontalStretchFactor;
                        }
                        else
                        {
                            entry.CalcWidth();
                        }
                    }

                    if (factor > 0)
                    {
                        var stretchedWidth = widthWithoutCellSpacing - minWidthOfEntries + minStretchedWidth;
                        var unit = stretchedWidth / factor;
                        // calculate the width of stretched children
                        foreach (var entry in this.Children)
                        {
                            if (entry.HorizontallyStretched)
                            {
                                entry.CalcWidth(unit);
                            }
                        }
                    }
                }

            }
        }

        public void Group_CalcHeight(double unitPartHeight = -1)
        {
            if (this.VerticallyStretched)
            {
                // calculate the height
                this.Rect.Height = unitPartHeight * this.VerticalStretchFactor;
                if (this.Rect.Height - this.PaddingVertical - this.BorderVertical < 1)
                {
                    throw new LayoutException($"The height of Group<{this.Id}> is too small to hold any children.");
                }
                this.ContentHeight = this.Rect.Height - this.PaddingVertical - this.BorderVertical;

                // calculate the height of children
                this.CalcChildrenHeight();
            }
            else if (this.IsFixedHeight)//fixed height
            {
                // calculate the height
                this.Rect.Height = this.MinHeight;

                if (this.Rect.Height - this.PaddingVertical - this.BorderVertical < 1)
                {
                    throw new LayoutException($"The height of Group<{this.Id}> is too small to hold any children.");
                }
                this.ContentHeight = this.Rect.Height - this.PaddingVertical - this.BorderVertical;

                // calculate the height of children
                this.CalcChildrenHeight();
            }
            else // default height
            {
                if (this.IsVertical) // vertical group
                {
                    var temp = 0d;
                    foreach (var entry in this.Children)
                    {
                        entry.CalcHeight();
                        temp += entry.Rect.Height + this.CellSpacingVertical;
                    }
                    temp -= this.CellSpacingVertical;
                    this.ContentHeight = temp < 0 ? 0 : temp;
                }
                else // horizontal group
                {
                    var temp = 0d;
                    // get the max height of children
                    foreach (var entry in this.Children)
                    {
                        entry.CalcHeight();
                        temp = Math.Max(temp, entry.Rect.Height);
                    }
                    this.ContentHeight = temp;
                }
                this.Rect.Height = this.ContentHeight + this.PaddingVertical + this.BorderVertical;
            }
        }

        private void CalcChildrenHeight()
        {
            if (this.IsVertical) // vertical group
            {
                // calculate the unitPartHeight for stretched children
                // calculate the height of fixed-size children

                var childCount = this.Children.Count;
                var cellSpacingHeight = (childCount - 1) * this.CellSpacingVertical;
                if(cellSpacingHeight >= this.ContentWidth)
                {
                    throw new LayoutException(
                        $"Group<{this.Id}> doesn't have enough height for vertical-cell-spacing<{this.CellSpacingVertical}> with {childCount} children.");
                }

                var heightWithoutCellSpacing = this.ContentHeight - cellSpacingHeight;

                double minHeightOfEntries = 0;
                double minStretchedHeight = 0;
                foreach (var entry in this.Children)
                {
                    if (entry.VerticallyStretched)
                    {
                        var defaultHeight = entry.GetDefaultHeight();
                        minHeightOfEntries += defaultHeight;
                        minStretchedHeight += defaultHeight;
                    }
                    else if (entry.IsFixedHeight)
                    {
                        minHeightOfEntries += entry.MinHeight;
                    }
                    else
                    {
                        minHeightOfEntries += entry.GetDefaultHeight();
                    }
                }

                if (minHeightOfEntries > heightWithoutCellSpacing)//overflow
                {
                    var factor = 0;
                    foreach (var entry in this.Children)
                    {
                        if (entry.VerticallyStretched)
                        {
                            factor += entry.VerticalStretchFactor;
                        }
                    }
                    var unit = minStretchedHeight / factor;
                    // change all VerticallyStretched entries to fixed height
                    foreach (var entry in this.Children)
                    {
                        if (entry.VerticallyStretched)
                        {
                            entry.MinHeight = entry.MaxHeight = unit * entry.VerticalStretchFactor;
                            entry.VerticalStretchFactor = 0;
                        }
                        entry.CalcHeight();
                    }
                }
                else
                {
                    var factor = 0;
                    foreach (var entry in this.Children)
                    {
                        if (entry.VerticallyStretched)
                        {
                            factor += entry.VerticalStretchFactor;
                        }
                        else
                        {
                            entry.CalcHeight();
                        }
                    }

                    if (factor > 0)
                    {
                        var stretchedHeight = heightWithoutCellSpacing - minHeightOfEntries + minStretchedHeight;
                        var unit = stretchedHeight / factor;
                        // calculate the height of stretched children
                        foreach (var entry in this.Children)
                        {
                            if (entry.VerticallyStretched)
                            {
                                entry.CalcHeight(unit);
                            }
                        }
                    }
                }

            }
            else // horizontal group
            {
                foreach (var entry in this.Children)
                {
                    if (entry.VerticallyStretched)
                    {
                        entry.CalcHeight(this.ContentHeight);
                        //the unitPartHeight
                        //(actually every entry will have this height, because entry.VerticalStretchFactor is always 1 in this case. See `LayoutGroup.Add`.)
                        //for stretched children is the content-box height of the group
                    }
                    else
                    {
                        entry.CalcHeight();
                    }
                }
            }
        }

        public void Group_SetX(double x)
        {
            this.Rect.X = x;
            if (this.IsVertical)
            {
                var childX = 0d;
                foreach (var entry in this.Children)
                {
                    switch (this.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                            childX = x + this.BorderLeft + this.PaddingLeft;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childX = x + this.BorderLeft + this.PaddingLeft + (this.ContentWidth - entry.Rect.Width) / 2;
                            break;
                        case Alignment.End:
                            childX = x + this.Rect.Width - this.BorderRight - this.PaddingRight - entry.Rect.Width;
                            break;
                    }
                    entry.SetX(childX);
                }
            }
            else
            {
                double nextX;

                var childWidthWithCellSpcaing = 0d;
                var childWidthWithoutCellSpcaing = 0d;
                foreach (var entry in this.Children)
                {
                    childWidthWithCellSpcaing += entry.Rect.Width + this.CellSpacingHorizontal;
                    childWidthWithoutCellSpcaing += entry.Rect.Width;
                }
                childWidthWithCellSpcaing -= this.CellSpacingVertical;

                switch (this.AlignmentHorizontal)
                {
                    case Alignment.Start:
                        nextX = x + this.BorderLeft + this.PaddingLeft;
                        break;
                    case Alignment.Center:
                        nextX = x + this.BorderLeft + this.PaddingLeft + (this.ContentWidth - childWidthWithCellSpcaing) / 2;
                        break;
                    case Alignment.End:
                        nextX = x + this.Rect.Width - this.BorderRight - this.PaddingRight - childWidthWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextX = x + this.BorderLeft + this.PaddingLeft +
                                (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.Children.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextX = x + this.BorderLeft + this.PaddingLeft;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in this.Children)
                {
                    entry.SetX(nextX);
                    switch (this.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                        case Alignment.Center:
                        case Alignment.End:
                            nextX += entry.Rect.Width + this.CellSpacingHorizontal;
                            break;
                        case Alignment.SpaceAround:
                            nextX += entry.Rect.Width + (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.Children.Count + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextX += entry.Rect.Width + (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.Children.Count - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public void Group_SetY(double y)
        {
            this.Rect.Y = y;
            if (this.IsVertical)
            {
                double nextY;

                var childHeightWithCellSpcaing = 0d;
                var childHeightWithoutCellSpcaing = 0d;
                foreach (var entry in this.Children)
                {
                    childHeightWithCellSpcaing += entry.Rect.Height + this.CellSpacingVertical;
                    childHeightWithoutCellSpcaing += entry.Rect.Height;
                }
                childHeightWithCellSpcaing -= this.CellSpacingVertical;

                switch (this.AlignmentVertical)
                {
                    case Alignment.Start:
                        nextY = y + this.BorderTop + this.PaddingTop;
                        break;
                    case Alignment.Center:
                        nextY = y + this.BorderTop + this.PaddingTop + (this.ContentHeight - childHeightWithCellSpcaing) / 2;
                        break;
                    case Alignment.End:
                        nextY = y + this.Rect.Height - this.BorderBottom - this.PaddingBottom - childHeightWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextY = y + this.BorderTop + this.PaddingTop +
                                (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.Children.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextY = y + this.BorderTop + this.PaddingTop;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in this.Children)
                {
                    entry.SetY(nextY);
                    switch (this.AlignmentVertical)
                    {
                        case Alignment.Start:
                        case Alignment.Center:
                        case Alignment.End:
                            nextY += entry.Rect.Height + this.CellSpacingVertical;
                            break;
                        case Alignment.SpaceAround:
                            nextY += entry.Rect.Height + (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.Children.Count + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextY += entry.Rect.Height + (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.Children.Count - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                var childY = 0d;
                foreach (var entry in this.Children)
                {
                    switch (this.AlignmentVertical)
                    {
                        case Alignment.Start:
                            childY = y + this.BorderTop + this.PaddingTop;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childY = y + this.BorderTop + this.PaddingTop + (this.ContentHeight - entry.Rect.Height) / 2;
                            break;
                        case Alignment.End:
                            childY += y + this.Rect.Height - this.BorderBottom - this.PaddingBottom - entry.Rect.Height;
                            break;
                    }
                    entry.SetY(childY);
                }
            }
        }
    }
}
