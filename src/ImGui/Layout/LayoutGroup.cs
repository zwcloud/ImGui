using System;
using System.Diagnostics;
using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class LayoutGroup : LayoutEntry
    {
        public bool IsVertical { get; }

        /// <summary>
        /// Width of the content box
        /// </summary>
        public override double ContentWidth { get; set; }

        /// <summary>
        /// Height of the content box
        /// </summary>
        public override double ContentHeight { get; set; }

        private ILayoutGroup group;

        public LayoutGroup(ILayoutGroup group, bool isVertical) : base(group, Size.Zero)
        {
            //NOTE content size is always a calculated value
            this.IsVertical = isVertical;
            this.group = group;
        }

        public void OnAddLayoutEntry(IStyleRuleSet layoutEntry)
        {
            if (this.node.RuleSet.IsFixedWidth)
            {
                Debug.Assert(!this.node.RuleSet.HorizontallyStretched);
                if (this.IsVertical && layoutEntry.RuleSet.HorizontalStretchFactor > 1)
                {
                    layoutEntry.RuleSet.HorizontalStretchFactor = 1;
                }
            }
            else if (this.node.RuleSet.HorizontallyStretched)
            {
                if (this.IsVertical && layoutEntry.RuleSet.HorizontalStretchFactor > 1)
                {
                    layoutEntry.RuleSet.HorizontalStretchFactor = 1;
                }
            }
            else
            {
                layoutEntry.RuleSet.HorizontalStretchFactor = 0;
            }

            if (this.node.RuleSet.IsFixedHeight)
            {
                Debug.Assert(!this.node.RuleSet.VerticallyStretched);
                if (!this.IsVertical && layoutEntry.RuleSet.VerticalStretchFactor > 1)
                {
                    layoutEntry.RuleSet.VerticalStretchFactor = 1;
                }
            }
            else if (this.node.RuleSet.VerticallyStretched)
            {
                if (!this.IsVertical && layoutEntry.RuleSet.VerticalStretchFactor > 1)
                {
                    layoutEntry.RuleSet.VerticalStretchFactor = 1;
                }
            }
            else
            {
                layoutEntry.RuleSet.VerticalStretchFactor = 0;
            }
        }

        public override void CalcWidth(double unitPartWidth = -1)
        {
            if (this.node.RuleSet.HorizontallyStretched)//stretched width
            {
                // calculate the width
                this.node.Width = unitPartWidth * this.node.RuleSet.HorizontalStretchFactor;
                if (this.node.Width - this.PaddingHorizontal - this.BorderHorizontal < 1)
                {
                    throw new LayoutException(
                        $"The width of Group<{this.node}> is too small to hold any children.");
                }
                this.ContentWidth = this.node.Width - this.PaddingHorizontal - this.BorderHorizontal;

                // calculate the width of children
                this.CalcChildrenWidth();
            }
            else if (this.node.RuleSet.IsFixedWidth)//fixed width
            {
                // calculate the width
                this.node.Width = this.node.RuleSet.MinWidth;

                if (this.node.Width - this.PaddingHorizontal - this.BorderHorizontal < 1)
                {
                    throw new LayoutException($"The width of Group<{this.node}> is too small to hold any children.");
                }
                this.ContentWidth = this.node.Width - this.PaddingHorizontal - this.BorderHorizontal;

                // calculate the width of children
                this.CalcChildrenWidth();
            }
            else // default width
            {
                if (this.IsVertical) //vertical group
                {
                    var temp = 0d;
                    // get the max width of children
                    foreach (var entry in this.group)
                    {
                        if (!entry.ActiveSelf)
                        {
                            continue;
                        }
                        entry.LayoutEntry.CalcWidth();
                        temp = Math.Max(temp, entry.Width);
                    }
                    this.ContentWidth = temp;
                }
                else
                {
                    var temp = 0d;
                    foreach (var entry in this.group)
                    {
                        if (!entry.ActiveSelf)
                        {
                            continue;
                        }
                        entry.LayoutEntry.CalcWidth();
                        temp += entry.Width + this.node.RuleSet.CellSpacingHorizontal;
                    }
                    temp -= this.node.RuleSet.CellSpacingHorizontal;
                    this.ContentWidth = temp < 0 ? 0 : temp;
                }
                this.node.Width = this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
            }
        }

        private void CalcChildrenWidth()
        {
            if (this.IsVertical) //vertical group
            {
                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        continue;
                    }
                    if (entry.RuleSet.HorizontallyStretched)
                    {
                        entry.LayoutEntry.CalcWidth(this.ContentWidth);
                        //the unitPartWidth
                        //(actually every entry will have this width, because entry.HorizontalStretchFactor is always 1 in this case. See `LayoutGroup.Add`.)
                        //for stretched children is the content-box width of the group
                    }
                    else
                    {
                        entry.LayoutEntry.CalcWidth();
                    }
                }
            }
            else //horizontal group
            {
                // calculate the unitPartWidth for stretched children
                // calculate the width of fixed-size children

                var childCount = this.group.ChildCount;

                //only count active children
                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        childCount--;
                    }
                }

                //no child, do nothing
                if (childCount <= 0)
                {
                    return;
                }

                var cellSpacing = this.node.RuleSet.CellSpacingHorizontal;
                if (this.ContentWidth < this.node.RuleSet.CellSpacingHorizontal)//the content box is too small to hold any child
                {
                    return;
                }

                //get the total size of fixed/default-sized, namely known-sized, children
                var knownSizedChildrenWidth = 0.0;
                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        continue;
                    }
                    if(entry.RuleSet.IsFixedWidth)
                    {
                        knownSizedChildrenWidth += entry.RuleSet.MinWidth;
                        knownSizedChildrenWidth += cellSpacing;
                    }
                    else if(entry.RuleSet.IsDefaultWidth)
                    {
                        knownSizedChildrenWidth += entry.LayoutEntry.GetDefaultWidth();
                        knownSizedChildrenWidth += cellSpacing;
                    }
                }

                var spaceLeftForStretchedChildren = this.ContentWidth - knownSizedChildrenWidth;
                if (spaceLeftForStretchedChildren < 0)//overflow, stretched children will be hidden
                {
                    foreach (var entry in this.group)
                    {
                        if (!entry.ActiveSelf)
                        {
                            continue;
                        }
                        if(entry.RuleSet.IsFixedWidth || entry.RuleSet.IsDefaultWidth)
                        {
                            entry.LayoutEntry.CalcWidth();
                        }
                    }
                }
                else
                {
                    var factor = 0;
                    foreach (var entry in this.group)
                    {
                        if (!entry.ActiveSelf)
                        {
                            continue;
                        }
                        if (entry.RuleSet.HorizontallyStretched)
                        {
                            factor += entry.RuleSet.HorizontalStretchFactor;
                        }
                        else
                        {
                            entry.LayoutEntry.CalcWidth();
                        }
                    }

                    if (factor > 0)
                    {
                        var unit = spaceLeftForStretchedChildren / factor;
                        // calculate the width of stretched children
                        foreach (var entry in this.group)
                        {
                            if (!entry.ActiveSelf)
                            {
                                continue;
                            }
                            if (entry.RuleSet.HorizontallyStretched)
                            {
                                entry.LayoutEntry.CalcWidth(unit);
                            }
                        }
                    }
                }

            }
        }

        public override void CalcHeight(double unitPartHeight = -1)
        {
            if (this.node.RuleSet.VerticallyStretched)
            {
                // calculate the height
                this.node.Height = unitPartHeight * this.node.RuleSet.VerticalStretchFactor;
                if (this.node.Height - this.PaddingVertical - this.BorderVertical < 1)
                {
                    throw new LayoutException($"The height of Group<{this.node}> is too small to hold any children.");
                }
                this.ContentHeight = this.node.Height - this.PaddingVertical - this.BorderVertical;

                // calculate the height of children
                this.CalcChildrenHeight();
            }
            else if (this.node.RuleSet.IsFixedHeight)//fixed height
            {
                // calculate the height
                this.node.Height = this.node.RuleSet.MinHeight;

                if (this.node.Height - this.PaddingVertical - this.BorderVertical < 1)
                {
                    throw new LayoutException($"The height of Group<{this.node}> is too small to hold any children.");
                }
                this.ContentHeight = this.node.Height - this.PaddingVertical - this.BorderVertical;

                // calculate the height of children
                this.CalcChildrenHeight();
            }
            else // default height
            {
                if (this.IsVertical) // vertical group
                {
                    var temp = 0d;
                    foreach (var entry in this.group)
                    {
                        if (!entry.ActiveSelf)
                        {
                            continue;
                        }
                        entry.LayoutEntry.CalcHeight();
                        temp += entry.Height + this.node.RuleSet.CellSpacingVertical;
                    }
                    temp -= this.node.RuleSet.CellSpacingVertical;
                    this.ContentHeight = temp < 0 ? 0 : temp;
                }
                else // horizontal group
                {
                    var temp = 0d;
                    // get the max height of children
                    foreach (var entry in this.group)
                    {
                        if (!entry.ActiveSelf)
                        {
                            continue;
                        }
                        entry.LayoutEntry.CalcHeight();
                        temp = Math.Max(temp, entry.Height);
                    }
                    this.ContentHeight = temp;
                }
                this.node.Height = this.ContentHeight + this.PaddingVertical + this.BorderVertical;
            }
        }

        private void CalcChildrenHeight()
        {
            if (this.IsVertical) // vertical group
            {
                // calculate the unitPartHeight for stretched children
                // calculate the height of fixed-size children

                var childCount = this.group.ChildCount;

                //only count active children
                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        childCount--;
                    }
                }

                //no child, do nothing
                if (childCount <= 0)
                {
                    return;
                }

                var cellSpacing = this.node.RuleSet.CellSpacingVertical;
                if (this.ContentHeight < this.node.RuleSet.CellSpacingVertical)//the content box is too small to hold any child
                {
                    return;
                }

                //get the total size of fixed/default-sized, namely known-sized, children
                var knownSizedChildrenHeight = 0.0;
                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        continue;
                    }
                    if(entry.RuleSet.IsFixedHeight)
                    {
                        knownSizedChildrenHeight += entry.RuleSet.MinHeight;
                        knownSizedChildrenHeight += cellSpacing;
                    }
                    else if(entry.RuleSet.IsDefaultHeight)
                    {
                        knownSizedChildrenHeight += entry.LayoutEntry.GetDefaultHeight();
                        knownSizedChildrenHeight += cellSpacing;
                    }
                }

                var spaceLeftForStretchedChildren = this.ContentHeight - knownSizedChildrenHeight;
                if (spaceLeftForStretchedChildren < 0)//overflow, stretched children will be hidden
                {
                    foreach (var entry in this.group)
                    {
                        if (!entry.ActiveSelf)
                        {
                            continue;
                        }
                        if(entry.RuleSet.IsFixedHeight || entry.RuleSet.IsDefaultHeight)
                        {
                            entry.LayoutEntry.CalcHeight();
                        }
                    }
                }
                else
                {
                    var factor = 0;
                    foreach (var entry in this.group)
                    {
                        if (!entry.ActiveSelf)
                        {
                            continue;
                        }
                        if (entry.RuleSet.VerticallyStretched)
                        {
                            factor += entry.RuleSet.VerticalStretchFactor;
                        }
                        else
                        {
                            entry.LayoutEntry.CalcHeight();
                        }
                    }

                    if (factor > 0)
                    {
                        var unit = spaceLeftForStretchedChildren / factor;
                        // calculate the height of stretched children
                        foreach (var entry in this.group)
                        {
                            if (!entry.ActiveSelf)
                            {
                                continue;
                            }
                            if (entry.RuleSet.VerticallyStretched)
                            {
                                entry.LayoutEntry.CalcHeight(unit);
                            }
                        }
                    }
                }
            }
            else // horizontal group
            {
                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        continue;
                    }
                    if (entry.RuleSet.VerticallyStretched)
                    {
                        entry.LayoutEntry.CalcHeight(this.ContentHeight);
                        //the unitPartHeight
                        //(actually every entry will have this height, because entry.VerticalStretchFactor is always 1 in this case. See `LayoutGroup.Add`.)
                        //for stretched children is the content-box height of the group
                    }
                    else
                    {
                        entry.LayoutEntry.CalcHeight();
                    }
                }
            }
        }

        public override void SetX(double x)
        {
            this.node.X = x;
            if (this.IsVertical)
            {
                var childX = 0d;
                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        continue;
                    }
                    switch (this.node.RuleSet.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                            childX = x + this.BorderLeft + this.PaddingLeft;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childX = x + this.BorderLeft + this.PaddingLeft + (this.ContentWidth - entry.Width) / 2;
                            break;
                        case Alignment.End:
                            childX = x + this.node.Width - this.BorderRight - this.PaddingRight - entry.Width;
                            break;
                    }

                    entry.LayoutEntry.SetX(childX);
                }
            }
            else
            {
                double nextX;

                var childWidthWithCellSpcaing = 0d;
                var childWidthWithoutCellSpcaing = 0d;
                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        continue;
                    }
                    childWidthWithCellSpcaing += entry.Width + this.node.RuleSet.CellSpacingHorizontal;
                    childWidthWithoutCellSpcaing += entry.Width;
                }
                childWidthWithCellSpcaing -= this.node.RuleSet.CellSpacingVertical;

                switch (this.node.RuleSet.AlignmentHorizontal)
                {
                    case Alignment.Start:
                        nextX = x + this.BorderLeft + this.PaddingLeft;
                        break;
                    case Alignment.Center:
                        nextX = x + this.BorderLeft + this.PaddingLeft + (this.ContentWidth - childWidthWithCellSpcaing) / 2;
                        break;
                    case Alignment.End:
                        nextX = x + this.node.Width - this.BorderRight - this.PaddingRight - childWidthWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextX = x + this.BorderLeft + this.PaddingLeft +
                                (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.group.ChildCount + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextX = x + this.BorderLeft + this.PaddingLeft;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        continue;
                    }
                    entry.LayoutEntry.SetX(nextX);
                    switch (this.node.RuleSet.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                        case Alignment.Center:
                        case Alignment.End:
                            nextX += entry.Width + this.node.RuleSet.CellSpacingHorizontal;
                            break;
                        case Alignment.SpaceAround:
                            nextX += entry.Width + (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.group.ChildCount + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextX += entry.Width + (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.group.ChildCount - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public override void SetY(double y)
        {
            this.node.Y = y;
            if (this.IsVertical)
            {
                double nextY;

                var childHeightWithCellSpcaing = 0d;
                var childHeightWithoutCellSpcaing = 0d;
                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        continue;
                    }
                    childHeightWithCellSpcaing += entry.Height + this.node.RuleSet.CellSpacingVertical;
                    childHeightWithoutCellSpcaing += entry.Height;
                }
                childHeightWithCellSpcaing -= this.node.RuleSet.CellSpacingVertical;

                switch (this.node.RuleSet.AlignmentVertical)
                {
                    case Alignment.Start:
                        nextY = y + this.BorderTop + this.PaddingTop;
                        break;
                    case Alignment.Center:
                        nextY = y + this.BorderTop + this.PaddingTop + (this.ContentHeight - childHeightWithCellSpcaing) / 2;
                        break;
                    case Alignment.End:
                        nextY = y + this.node.Height - this.BorderBottom - this.PaddingBottom - childHeightWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextY = y + this.BorderTop + this.PaddingTop +
                                (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.group.ChildCount + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextY = y + this.BorderTop + this.PaddingTop;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        continue;
                    }
                    entry.LayoutEntry.SetY(nextY);
                    switch (this.node.RuleSet.AlignmentVertical)
                    {
                        case Alignment.Start:
                        case Alignment.Center:
                        case Alignment.End:
                            nextY += entry.Height + this.node.RuleSet.CellSpacingVertical;
                            break;
                        case Alignment.SpaceAround:
                            nextY += entry.Height + (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.group.ChildCount + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextY += entry.Height + (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.group.ChildCount - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                var childY = 0d;
                foreach (var entry in this.group)
                {
                    if (!entry.ActiveSelf)
                    {
                        continue;
                    }
                    switch (this.node.RuleSet.AlignmentVertical)
                    {
                        case Alignment.Start:
                            childY = y + this.BorderTop + this.PaddingTop;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childY = y + this.BorderTop + this.PaddingTop + (this.ContentHeight - entry.Height) / 2;
                            break;
                        case Alignment.End:
                            childY += y + this.node.Height - this.BorderBottom - this.PaddingBottom - entry.Height;
                            break;
                    }

                    entry.LayoutEntry.SetY(childY);
                }
            }
        }
    }
}
