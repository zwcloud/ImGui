using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Common.Primitive;

namespace ImGui.Layout
{
    [DebuggerDisplay("Group {Id}, Count={Entries.Count}")]
    internal class LayoutGroup : LayoutEntry
    {
        public double CellSpacingHorizontal { get; set; } = 0;
        public double CellSpacingVertical { get; set; } = 0;
        public Alignment AlignmentHorizontal { get; set; } = Alignment.Start;
        public Alignment AlignmentVertical { get; set; } = Alignment.Start;

        public LayoutGroup() { }

        public LayoutGroup(int id, bool isVertical, Size contentSize) : base(id, contentSize)
        {
            this.IsVertical = isVertical;

            this.ApplyStyle();
        }

        public void Init(int id, bool isVertical, Size contentSize, LayoutOptions? options)
        {
            this.Id = id;
            this.ContentWidth = contentSize.Width;
            this.ContentHeight = contentSize.Height;

            this.IsVertical = isVertical;

            this.ApplyStyle();
            if(options.HasValue)
            {
                this.ApplyOptions(options.Value);
            }
        }

        protected override void ApplyStyle()
        {
            base.ApplyStyle();

            var style = Form.current.uiContext.StyleStack.Style;

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

        public bool IsVertical { get; private set; }

        public List<LayoutEntry> Entries { get; } = new List<LayoutEntry>();

        public LayoutEntry GetEntry(int id)
        {
            foreach (var entry in this.Entries)
            {
                if (entry.Id == id)
                {
                    return entry;
                }
            }
            return null;
        }

        /// <summary>
        /// Append child entry to this group
        /// </summary>
        /// <param name="item"></param>
        public void Add(LayoutEntry item)
        {
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

            this.Entries.Add(item);
        }

        public override void CalcWidth(double unitPartWidth = -1)
        {
            if (this.HorizontallyStretched)//stretched width
            {
                // calculate the width
                this.Rect.Width = unitPartWidth * this.HorizontalStretchFactor;
                if (this.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal < 1)
                {
                    Log.Warning(string.Format("The width of Group<{0}> is too small to hold any children.", this.Id));
                    return;
                }
                this.ContentWidth = this.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal;

                // calculate the width of children
                CalcChildrenWidth();
            }
            else if (this.IsFixedWidth)//fixed width
            {
                // calculate the width
                this.Rect.Width = this.MinWidth;

                if (this.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal < 1)
                {
                    Log.Warning(string.Format("The width of Group<{0}> is too small to hold any children.", this.Id));
                    return;
                }
                this.ContentWidth = this.Rect.Width - this.PaddingHorizontal - this.BorderHorizontal;

                // calculate the width of children
                CalcChildrenWidth();
            }
            else // default width
            {
                if (this.IsVertical) //vertical group
                {
                    var temp = 0d;
                    // get the max width of children
                    foreach (var entry in this.Entries)
                    {
                        entry.CalcWidth();
                        temp = Math.Max(temp, entry.Rect.Width);
                    }
                    this.ContentWidth = temp;
                }
                else
                {
                    var temp = 0d;
                    foreach (var entry in this.Entries)
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
                foreach (var entry in this.Entries)
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

                var childCount = this.Entries.Count;
                var cellSpacingWidth = this.CellSpacingHorizontal * (childCount - 1);
                if(cellSpacingWidth >= this.ContentWidth)
                {
                    Log.Warning(string.Format("Group<{0}> doesn't have enough width for horizontal-cell-spacing<{1}> with {2} children.",
                        this.Id, this.CellSpacingHorizontal, childCount));
                    return;
                }

                var widthWithoutCellSpacing = this.ContentWidth - cellSpacingWidth;

                double minWidthOfEntries = 0;
                double minStretchedWidth = 0;
                foreach (var entry in this.Entries)
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
                    foreach (var entry in this.Entries)
                    {
                        if (entry.HorizontallyStretched)
                        {
                            factor += entry.HorizontalStretchFactor;
                        }
                    }
                    var unit = minStretchedWidth / factor;
                    // change all HorizontallyStretched entries to fixed width
                    foreach (var entry in this.Entries)
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
                    foreach (var entry in this.Entries)
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
                        foreach (var entry in this.Entries)
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

        public override void CalcHeight(double unitPartHeight = -1)
        {
            if (this.VerticallyStretched)
            {
                // calculate the height
                this.Rect.Height = unitPartHeight * this.VerticalStretchFactor;
                if (this.Rect.Height - this.PaddingVertical - this.BorderVertical < 1)
                {
                    Log.Warning(string.Format("The height of Group<{0}> is too small to hold any children.", this.Id));
                    return;
                }
                this.ContentHeight = this.Rect.Height - this.PaddingVertical - this.BorderVertical;

                // calculate the height of children
                CalcChildrenHeight();
            }
            else if (this.IsFixedHeight)//fixed height
            {
                // calculate the height
                this.Rect.Height = this.MinHeight;

                if (this.Rect.Height - this.PaddingVertical - this.BorderVertical < 1)
                {
                    Log.Warning(string.Format("The height of Group<{0}> is too small to hold any children.", this.Id));
                    return;
                }
                this.ContentHeight = this.Rect.Height - this.PaddingVertical - this.BorderVertical;

                // calculate the height of children
                CalcChildrenHeight();
            }
            else // default height
            {
                if (this.IsVertical) // vertical group
                {
                    var temp = 0d;
                    foreach (var entry in this.Entries)
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
                    foreach (var entry in this.Entries)
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

                var childCount = this.Entries.Count;
                var cellSpacingHeight = (childCount - 1) * this.CellSpacingVertical;
                if(cellSpacingHeight >= this.ContentWidth)
                {
                    Log.Warning(string.Format("Group<{0}> doesn't have enough height for vertical-cell-spacing<{1}> with {2} children.",
                        this.Id, this.CellSpacingVertical, childCount));
                    return;
                }

                var heightWithoutCellSpacing = this.ContentHeight - cellSpacingHeight;

                double minHeightOfEntries = 0;
                double minStretchedHeight = 0;
                foreach (var entry in this.Entries)
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
                    foreach (var entry in this.Entries)
                    {
                        if (entry.VerticallyStretched)
                        {
                            factor += entry.VerticalStretchFactor;
                        }
                    }
                    var unit = minStretchedHeight / factor;
                    // change all VerticallyStretched entries to fixed height
                    foreach (var entry in this.Entries)
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
                    foreach (var entry in this.Entries)
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
                        foreach (var entry in this.Entries)
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
                foreach (var entry in this.Entries)
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

        public override void SetX(double x)
        {
            this.Rect.X = x;
            if (this.IsVertical)
            {
                var childX = 0d;
                foreach (var entry in this.Entries)
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
                foreach (var entry in this.Entries)
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
                                (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.Entries.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextX = x + this.BorderLeft + this.PaddingLeft;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in this.Entries)
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
                            nextX += entry.Rect.Width + (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.Entries.Count + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextX += entry.Rect.Width + (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.Entries.Count - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public override void SetY(double y)
        {
            this.Rect.Y = y;
            if (this.IsVertical)
            {
                double nextY;

                var childHeightWithCellSpcaing = 0d;
                var childHeightWithoutCellSpcaing = 0d;
                foreach (var entry in this.Entries)
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
                                (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.Entries.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextY = y + this.BorderTop + this.PaddingTop;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in this.Entries)
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
                            nextY += entry.Rect.Height + (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.Entries.Count + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextY += entry.Rect.Height + (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.Entries.Count - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                var childY = 0d;
                foreach (var entry in this.Entries)
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
