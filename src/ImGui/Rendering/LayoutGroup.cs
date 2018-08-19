using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.Rendering
{
    internal class LayoutGroup : LayoutEntry
    {
        public double CellSpacingHorizontal { get; set; } = 0;
        public double CellSpacingVertical { get; set; } = 0;
        public Alignment AlignmentHorizontal { get; set; } = Alignment.Start;
        public Alignment AlignmentVertical { get; set; } = Alignment.Start;

        public bool IsVertical { get; private set; }

        public LayoutGroup(Node node) : base(node)
        {
        }

        protected void Group_Reset()
        {
            Entry_Reset();
            if (this.node.Children == null)
            {
                this.node.Children = new List<Node>();
            }
            else
            {
                this.node.Children.Clear();
            }

            this.CellSpacingHorizontal = 0;
            this.CellSpacingVertical = 0;
            this.AlignmentHorizontal = Alignment.Start;
            this.AlignmentVertical = Alignment.Start;
        }

        public void Group_Init(bool isVertical, LayoutOptions? options)
        {
            this.Group_Reset();

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
            if (this.node.Children == null)
            {
                this.node.Children = new List<Node>();
            }

            if (this.IsFixedWidth)
            {
                Debug.Assert(!this.HorizontallyStretched);
                if (this.IsVertical && item.layoutEntry.HorizontalStretchFactor > 1)
                {
                    item.layoutEntry.HorizontalStretchFactor = 1;
                }
            }
            else if (this.HorizontallyStretched)
            {
                if (this.IsVertical && item.layoutEntry.HorizontalStretchFactor > 1)
                {
                    item.layoutEntry.HorizontalStretchFactor = 1;
                }
            }
            else
            {
                item.layoutEntry.HorizontalStretchFactor = 0;
            }

            if (this.IsFixedHeight)
            {
                Debug.Assert(!this.VerticallyStretched);
                if (!this.IsVertical && item.layoutEntry.VerticalStretchFactor > 1)
                {
                    item.layoutEntry.VerticalStretchFactor = 1;
                }
            }
            else if (this.VerticallyStretched)
            {
                if (!this.IsVertical && item.layoutEntry.VerticalStretchFactor > 1)
                {
                    item.layoutEntry.VerticalStretchFactor = 1;
                }
            }
            else
            {
                item.layoutEntry.VerticalStretchFactor = 0;
            }

            item.Parent = this.node;
            this.node.Children.Add(item);
        }

        public override void CalcWidth(double unitPartWidth = -1)
        {
            if (this.HorizontallyStretched)//stretched width
            {
                // calculate the width
                this.node.Rect.Width = unitPartWidth * this.HorizontalStretchFactor;
                if (this.node.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal < 1)
                {
                    throw new LayoutException(
                        $"The width of Group<{this.node.Id}> is too small to hold any children.");
                }
                this.ContentWidth = this.node.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal;

                // calculate the width of children
                this.CalcChildrenWidth();
            }
            else if (this.IsFixedWidth)//fixed width
            {
                // calculate the width
                this.node.Rect.Width = this.MinWidth;

                if (this.node.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal < 1)
                {
                    throw new LayoutException($"The width of Group<{this.node.Id}> is too small to hold any children.");
                }
                this.ContentWidth = this.node.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal;

                // calculate the width of children
                this.CalcChildrenWidth();
            }
            else // default width
            {
                if (this.IsVertical) //vertical group
                {
                    var temp = 0d;
                    // get the max width of children
                    foreach (var childNode in this.node.Children)
                    {
                        childNode.CalcWidth();
                        temp = Math.Max(temp, childNode.Rect.Width);
                    }
                    this.ContentWidth = temp;
                }
                else
                {
                    var temp = 0d;
                    foreach (var entry in this.node.Children)
                    {
                        entry.CalcWidth();
                        temp += entry.Rect.Width + this.CellSpacingHorizontal;
                    }
                    temp -= this.CellSpacingHorizontal;
                    this.ContentWidth = temp < 0 ? 0 : temp;
                }
                this.node.Rect.Width = this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
            }
        }

        private void CalcChildrenWidth()
        {
            if (this.IsVertical) //vertical group
            {
                foreach (var childNode in this.node.Children)
                {
                    if (childNode.layoutEntry.HorizontallyStretched)
                    {
                        childNode.CalcWidth(this.ContentWidth);
                        //the unitPartWidth
                        //(actually every entry will have this width, because entry.HorizontalStretchFactor is always 1 in this case. See `LayoutGroup.Add`.)
                        //for stretched children is the content-box width of the group
                    }
                    else
                    {
                        childNode.CalcWidth();
                    }
                }
            }
            else //horizontal group
            {
                // calculate the unitPartWidth for stretched children
                // calculate the width of fixed-size children

                var childCount = this.node.Children.Count;
                var cellSpacingWidth = this.CellSpacingHorizontal * (childCount - 1);
                if(cellSpacingWidth >= this.ContentWidth)
                {
                    throw new LayoutException(
                        $"Group<{this.node.Id}> doesn't have enough width for horizontal-cell-spacing<{this.CellSpacingHorizontal}> with {childCount} children.");
                }

                var widthWithoutCellSpacing = this.ContentWidth - cellSpacingWidth;

                double minWidthOfEntries = 0;
                double minStretchedWidth = 0;
                foreach (var childNode in this.node.Children)
                {
                    if (childNode.layoutEntry.HorizontallyStretched)
                    {
                        var defaultWidth = childNode.layoutEntry.GetDefaultWidth();
                        minWidthOfEntries += defaultWidth;
                        minStretchedWidth += defaultWidth;
                    }
                    else if(childNode.layoutEntry.IsFixedWidth)
                    {
                        minWidthOfEntries += childNode.layoutEntry.MinWidth;
                    }
                    else
                    {
                        minWidthOfEntries += childNode.layoutEntry.GetDefaultWidth();
                    }
                }

                if(minWidthOfEntries > widthWithoutCellSpacing)//overflow
                {
                    var factor = 0;
                    foreach (var entry in this.node.Children)
                    {
                        if (entry.layoutEntry.HorizontallyStretched)
                        {
                            factor += entry.layoutEntry.HorizontalStretchFactor;
                        }
                    }
                    var unit = minStretchedWidth / factor;
                    // change all HorizontallyStretched entries to fixed width
                    foreach (var childNode in this.node.Children)
                    {
                        if (childNode.layoutEntry.HorizontallyStretched)
                        {
                            childNode.layoutEntry.MinWidth = childNode.layoutEntry.MaxWidth = unit * childNode.layoutEntry.HorizontalStretchFactor;
                            childNode.layoutEntry.HorizontalStretchFactor = 0;
                        }
                        childNode.CalcWidth();
                    }
                }
                else
                {
                    var factor = 0;
                    foreach (var childNode in this.node.Children)
                    {
                        if (childNode.layoutEntry.HorizontallyStretched)
                        {
                            factor += childNode.layoutEntry.HorizontalStretchFactor;
                        }
                        else
                        {
                            childNode.CalcWidth();
                        }
                    }

                    if (factor > 0)
                    {
                        var stretchedWidth = widthWithoutCellSpacing - minWidthOfEntries + minStretchedWidth;
                        var unit = stretchedWidth / factor;
                        // calculate the width of stretched children
                        foreach (var childNode in this.node.Children)
                        {
                            if (childNode.layoutEntry.HorizontallyStretched)
                            {
                                childNode.CalcWidth(unit);
                            }
                        }
                    }
                }

            }
        }

        public override void CalcHeight(double unitPartHeight = -1)
        {
            if (this.VerticallyStretched)
            {
                // calculate the height
                this.node.Rect.Height = unitPartHeight * this.VerticalStretchFactor;
                if (this.node.Rect.Height - this.PaddingVertical - this.BorderVertical < 1)
                {
                    throw new LayoutException($"The height of Group<{this.node.Id}> is too small to hold any children.");
                }
                this.ContentHeight = this.node.Rect.Height - this.PaddingVertical - this.BorderVertical;

                // calculate the height of children
                this.CalcChildrenHeight();
            }
            else if (this.IsFixedHeight)//fixed height
            {
                // calculate the height
                this.node.Rect.Height = this.MinHeight;

                if (this.node.Rect.Height - this.PaddingVertical - this.BorderVertical < 1)
                {
                    throw new LayoutException($"The height of Group<{this.node.Id}> is too small to hold any children.");
                }
                this.ContentHeight = this.node.Rect.Height - this.PaddingVertical - this.BorderVertical;

                // calculate the height of children
                this.CalcChildrenHeight();
            }
            else // default height
            {
                if (this.IsVertical) // vertical group
                {
                    var temp = 0d;
                    foreach (var childNode in this.node.Children)
                    {
                        childNode.CalcHeight();
                        temp += childNode.Rect.Height + this.CellSpacingVertical;
                    }
                    temp -= this.CellSpacingVertical;
                    this.ContentHeight = temp < 0 ? 0 : temp;
                }
                else // horizontal group
                {
                    var temp = 0d;
                    // get the max height of children
                    foreach (var entry in this.node.Children)
                    {
                        entry.CalcHeight();
                        temp = Math.Max(temp, entry.Rect.Height);
                    }
                    this.ContentHeight = temp;
                }
                this.node.Rect.Height = this.ContentHeight + this.PaddingVertical + this.BorderVertical;
            }
        }

        private void CalcChildrenHeight()
        {
            if (this.IsVertical) // vertical group
            {
                // calculate the unitPartHeight for stretched children
                // calculate the height of fixed-size children

                var childCount = this.node.Children.Count;
                var cellSpacingHeight = (childCount - 1) * this.CellSpacingVertical;
                if(cellSpacingHeight >= this.ContentWidth)
                {
                    throw new LayoutException(
                        $"Group<{this.node.Id}> doesn't have enough height for vertical-cell-spacing<{this.CellSpacingVertical}> with {childCount} children.");
                }

                var heightWithoutCellSpacing = this.ContentHeight - cellSpacingHeight;

                double minHeightOfEntries = 0;
                double minStretchedHeight = 0;
                foreach (var entry in this.node.Children)
                {
                    if (entry.layoutEntry.VerticallyStretched)
                    {
                        var defaultHeight = entry.layoutEntry.GetDefaultHeight();
                        minHeightOfEntries += defaultHeight;
                        minStretchedHeight += defaultHeight;
                    }
                    else if (entry.layoutEntry.IsFixedHeight)
                    {
                        minHeightOfEntries += entry.layoutEntry.MinHeight;
                    }
                    else
                    {
                        minHeightOfEntries += entry.layoutEntry.GetDefaultHeight();
                    }
                }

                if (minHeightOfEntries > heightWithoutCellSpacing)//overflow
                {
                    var factor = 0;
                    foreach (var entry in this.node.Children)
                    {
                        if (entry.layoutEntry.VerticallyStretched)
                        {
                            factor += entry.layoutEntry.VerticalStretchFactor;
                        }
                    }
                    var unit = minStretchedHeight / factor;
                    // change all VerticallyStretched entries to fixed height
                    foreach (var entry in this.node.Children)
                    {
                        if (entry.layoutEntry.VerticallyStretched)
                        {
                            entry.layoutEntry.MinHeight = entry.layoutEntry.MaxHeight = unit * entry.layoutEntry.VerticalStretchFactor;
                            entry.layoutEntry.VerticalStretchFactor = 0;
                        }
                        entry.CalcHeight();
                    }
                }
                else
                {
                    var factor = 0;
                    foreach (var entry in this.node.Children)
                    {
                        if (entry.layoutEntry.VerticallyStretched)
                        {
                            factor += entry.layoutEntry.VerticalStretchFactor;
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
                        foreach (var entry in this.node.Children)
                        {
                            if (entry.layoutEntry.VerticallyStretched)
                            {
                                entry.CalcHeight(unit);
                            }
                        }
                    }
                }

            }
            else // horizontal group
            {
                foreach (var entry in this.node.Children)
                {
                    if (entry.layoutEntry.VerticallyStretched)
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

        public void SetX(double x)
        {
            this.node.Rect.X = x;
            if (this.IsVertical)
            {
                var childX = 0d;
                foreach (var entry in this.node.Children)
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
                            childX = x + this.node.Rect.Width - this.BorderRight - this.PaddingRight - entry.Rect.Width;
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
                foreach (var entry in this.node.Children)
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
                        nextX = x + this.node.Rect.Width - this.BorderRight - this.PaddingRight - childWidthWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextX = x + this.BorderLeft + this.PaddingLeft +
                                (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.node.Children.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextX = x + this.BorderLeft + this.PaddingLeft;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in this.node.Children)
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
                            nextX += entry.Rect.Width + (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.node.Children.Count + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextX += entry.Rect.Width + (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.node.Children.Count - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public void SetY(double y)
        {
            this.node.Rect.Y = y;
            if (this.IsVertical)
            {
                double nextY;

                var childHeightWithCellSpcaing = 0d;
                var childHeightWithoutCellSpcaing = 0d;
                foreach (var entry in this.node.Children)
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
                        nextY = y + this.node.Rect.Height - this.BorderBottom - this.PaddingBottom - childHeightWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextY = y + this.BorderTop + this.PaddingTop +
                                (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.node.Children.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextY = y + this.BorderTop + this.PaddingTop;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in this.node.Children)
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
                            nextY += entry.Rect.Height + (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.node.Children.Count + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextY += entry.Rect.Height + (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.node.Children.Count - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                var childY = 0d;
                foreach (var entry in this.node.Children)
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
                            childY += y + this.node.Rect.Height - this.BorderBottom - this.PaddingBottom - entry.Rect.Height;
                            break;
                    }
                    entry.SetY(childY);
                }
            }
        }
    }
}
