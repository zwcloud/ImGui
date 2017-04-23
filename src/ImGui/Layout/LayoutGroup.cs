using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui
{
    internal class LayoutGroup : LayoutEntry
    {
        public bool isForm;
        public bool isVertical;
        public bool isClipped;

        public List<LayoutEntry> entries = new List<LayoutEntry>();
        private int cursor;

        public LayoutGroup(bool isVertical, GUIStyle style, params LayoutOption[] options) : base(style, options)
        {
            this.isVertical = isVertical;
            cursor = 0;
        }

        public LayoutEntry GetNext()
        {
            if (this.cursor < this.entries.Count)
            {
                LayoutEntry result = this.entries[this.cursor];
                this.cursor++;
                return result;
            }
            throw new InvalidOperationException();
        }

        public Rect GetLast()
        {
            Rect result;
            if (this.cursor == 0)
            {
                Log.Error("No last rect available.");
                result = Rect.Empty;
            }
            else if (this.cursor <= this.entries.Count)
            {
                LayoutEntry gUILayoutEntry = this.entries[this.cursor - 1];
                result = gUILayoutEntry.rect;
            }
            else
            {
                Log.Error("No last rect available.");// this rarely happens
                result = Rect.Empty;
            }
            return result;
        }

        public void ResetCursor()
        {
            this.cursor = 0;
        }


        public void Add(LayoutEntry item)
        {
            if (this.IsFixedWidth)
            {
                Debug.Assert(!this.HorizontallyStretched);
                if (this.isVertical && item.horizontalStretchFactor > 1)
                {
                    item.horizontalStretchFactor = 1;
                }
            }
            else if (this.HorizontallyStretched)
            {
                if (this.isVertical && item.horizontalStretchFactor > 1)
                {
                    item.horizontalStretchFactor = 1;
                }
            }
            else
            {
                item.horizontalStretchFactor = 0;
            }

            if (this.IsFixedHeight)
            {
                Debug.Assert(!this.VerticallyStretched);
                if (!this.isVertical && item.verticalStretchFactor > 1)
                {
                    item.verticalStretchFactor = 1;
                }
            }
            else if (this.VerticallyStretched)
            {
                if (!this.isVertical && item.verticalStretchFactor > 1)
                {
                    item.verticalStretchFactor = 1;
                }
            }
            else
            {
                item.verticalStretchFactor = 0;
            }

            this.entries.Add(item);
        }

        public override void CalcWidth(double unitPartWidth = -1)
        {
            if (this.HorizontallyStretched)//stretched width
            {
                // calculate the width
                this.rect.Width = unitPartWidth * horizontalStretchFactor;
                this.contentWidth = this.rect.Width - this.style.PaddingHorizontal - this.style.BorderHorizontal;

                if (this.contentWidth <= 0) return;//container has no space to hold the children

                // calculate the width of children
                CalcChildrenWidth();
            }
            else if (this.IsFixedWidth)//fiexed width
            {
                // calculate the width
                this.rect.Width = this.minWidth;
                this.contentWidth = this.rect.Width - this.style.PaddingHorizontal - this.style.BorderHorizontal;

                if (this.contentWidth <= 0) return;//container has no space to hold the children

                // calculate the width of children
                CalcChildrenWidth();
            }
            else // default width
            {
                if (this.isVertical) //vertical group
                {
                    var temp = 0d;
                    // get the max width of children
                    foreach (var entry in entries)
                    {
                        entry.CalcWidth();
                        temp = Math.Max(temp, entry.rect.Width);
                    }
                    this.contentWidth = temp;
                }
                else
                {
                    var temp = 0d;
                    foreach (var entry in entries)
                    {
                        entry.CalcWidth();
                        temp += entry.rect.Width + this.style.CellingSpacingHorizontal;
                    }
                    temp -= this.style.CellingSpacingHorizontal;
                    this.contentWidth = temp < 0 ? 0 : temp;
                }
                this.rect.Width = this.contentWidth + this.style.PaddingHorizontal + this.style.BorderHorizontal;
            }
        }

        private void CalcChildrenWidth()
        {
            if (this.isVertical) //vertical group
            {
                foreach (var entry in entries)
                {
                    if (entry.HorizontallyStretched)
                    {
                        entry.CalcWidth(this.contentWidth); //the unitPartWidth for stretched children is the content-box width of the group
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
                var childCount = this.entries.Count;
                var totalFactor = 0;
                var totalStretchedPartWidth = this.contentWidth -
                                              this.style.CellingSpacingHorizontal * (childCount - 1);
                foreach (var entry in entries)
                {
                    if (entry.HorizontallyStretched)
                    {
                        totalFactor += entry.horizontalStretchFactor;
                    }
                    else
                    {
                        entry.CalcWidth();
                        totalStretchedPartWidth -= entry.rect.Width;
                    }
                }
                var childUnitPartWidth = totalStretchedPartWidth / totalFactor;
                // calculate the width of stretched children
                foreach (var entry in entries)
                {
                    if (entry.HorizontallyStretched)
                    {
                        entry.CalcWidth(childUnitPartWidth);
                    }
                }
            }
        }

        public override void CalcHeight(double unitPartHeight = -1)
        {
            if (this.VerticallyStretched)
            {
                // calculate the height
                this.rect.Height = unitPartHeight * this.verticalStretchFactor;
                this.contentHeight = this.rect.Height - this.style.PaddingVertical - this.style.BorderVertical;

                if (this.contentHeight < 1) return;//container has no space to hold the children

                // calculate the height of children
                CalcChildrenHeight();
            }
            else if (this.IsFixedHeight)//fiexed height
            {
                // calculate the height
                this.rect.Height = this.minHeight;
                this.contentHeight = this.rect.Height - this.style.PaddingVertical - this.style.BorderVertical;

                if (this.contentHeight < 1) return;//container has no space to hold the children

                // calculate the height of children
                CalcChildrenHeight();
            }
            else // default height
            {
                if (this.isVertical) // vertical group
                {
                    var temp = 0d;
                    foreach (var entry in entries)
                    {
                        entry.CalcHeight();
                        temp += entry.rect.Height + this.style.CellingSpacingVertical;
                    }
                    temp -= this.style.CellingSpacingVertical;
                    this.contentHeight = temp < 0 ? 0 : temp;
                }
                else // horizontal group
                {
                    var temp = 0d;
                    // get the max height of children
                    foreach (var entry in entries)
                    {
                        entry.CalcHeight();
                        temp = Math.Max(temp, entry.rect.Height);
                    }
                    this.contentHeight = temp;
                }
                this.rect.Height = this.contentHeight + this.style.PaddingVertical + this.style.BorderVertical;
            }
        }

        private void CalcChildrenHeight()
        {
            if (this.isVertical) // vertical group
            {
                // calculate the unitPartHeight for stretched children
                // calculate the height of fixed-size children
                var childCount = this.entries.Count;
                var totalStretchedPartHeight = this.contentHeight - (childCount - 1) * this.style.CellingSpacingVertical;
                var totalFactor = 0;
                foreach (var entry in entries)
                {
                    if (entry.VerticallyStretched)
                    {
                        totalFactor += entry.verticalStretchFactor;
                    }
                    else
                    {
                        entry.CalcHeight();
                        totalStretchedPartHeight -= entry.rect.Height;
                    }
                }
                var childUnitPartHeight = totalStretchedPartHeight / totalFactor;
                // calculate the height of stretched children
                foreach (var entry in entries)
                {
                    if (entry.VerticallyStretched)
                    {
                        entry.CalcHeight(childUnitPartHeight);
                    }
                }
            }
            else // horizontal group
            {
                foreach (var entry in entries)
                {
                    if (entry.VerticallyStretched)
                    {
                        entry.CalcHeight(this.contentHeight);
                        //the unitPartHeight for stretched children is the content-box height of the group
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
            this.rect.X = x;
            if (this.isVertical)
            {
                var childX = 0d;
                foreach (var entry in entries)
                {
                    switch (this.style.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                            childX = x + this.style.BorderLeft + this.style.PaddingLeft;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childX = x + this.style.BorderLeft + this.style.PaddingLeft + (this.contentWidth - entry.rect.Width) / 2;
                            break;
                        case Alignment.End:
                            childX = x + this.rect.Width - this.style.BorderRight - this.style.PaddingRight - entry.rect.Width;
                            break;
                    }
                    entry.SetX(childX);
                }
            }
            else
            {
                var nextX = 0d;

                var childWidthWithCellSpcaing = 0d;
                var childWidthWithoutCellSpcaing = 0d;
                foreach (var entry in entries)
                {
                    childWidthWithCellSpcaing += entry.rect.Width + this.style.CellingSpacingHorizontal;
                    childWidthWithoutCellSpcaing += entry.rect.Width;
                }
                childWidthWithCellSpcaing -= this.style.CellingSpacingVertical;

                switch (this.style.AlignmentHorizontal)
                {
                    case Alignment.Start:
                        nextX = x + this.style.BorderLeft + this.style.PaddingLeft;
                        break;
                    case Alignment.Center:
                        nextX = x + this.style.BorderLeft + this.style.PaddingLeft + (this.contentWidth - childWidthWithCellSpcaing) / 2;
                        break;
                    case Alignment.End:
                        nextX = x + this.rect.Width - this.style.BorderRight - this.style.PaddingRight - childWidthWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextX = x + this.style.BorderLeft + this.style.PaddingLeft +
                                (this.contentWidth - childWidthWithoutCellSpcaing) / (this.entries.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextX = x + this.style.BorderLeft + this.style.PaddingLeft;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in entries)
                {
                    entry.SetX(nextX);
                    switch (this.style.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                        case Alignment.Center:
                        case Alignment.End:
                            nextX += entry.rect.Width + this.style.CellingSpacingHorizontal;
                            break;
                        case Alignment.SpaceAround:
                            nextX += entry.rect.Width + (this.contentWidth - childWidthWithoutCellSpcaing) / (this.entries.Count + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextX += entry.rect.Width + (this.contentWidth - childWidthWithoutCellSpcaing) / (this.entries.Count - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public override void SetY(double y)
        {
            this.rect.Y = y;
            if (this.isVertical)
            {
                var nextY = 0d;

                var childHeightWithCellSpcaing = 0d;
                var childHeightWithoutCellSpcaing = 0d;
                foreach (var entry in this.entries)
                {
                    childHeightWithCellSpcaing += entry.rect.Height + this.style.CellingSpacingVertical;
                    childHeightWithoutCellSpcaing += entry.rect.Height;
                }
                childHeightWithCellSpcaing -= this.style.CellingSpacingVertical;

                switch (this.style.AlignmentVertical)
                {
                    case Alignment.Start:
                        nextY = y + this.style.BorderTop + this.style.PaddingTop;
                        break;
                    case Alignment.Center:
                        nextY = y + this.style.BorderTop + this.style.PaddingTop + (this.contentHeight - childHeightWithCellSpcaing) / 2;
                        break;
                    case Alignment.End:
                        nextY = y + this.rect.Height - this.style.BorderBottom - this.style.PaddingBottom - childHeightWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextY = y + this.style.BorderTop + this.style.PaddingTop +
                                (this.contentHeight - childHeightWithoutCellSpcaing) / (this.entries.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextY = y + this.style.BorderTop + this.style.PaddingTop;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in entries)
                {
                    entry.SetY(nextY);
                    switch (this.style.AlignmentVertical)
                    {
                        case Alignment.Start:
                        case Alignment.Center:
                        case Alignment.End:
                            nextY += entry.rect.Height + this.style.CellingSpacingVertical;
                            break;
                        case Alignment.SpaceAround:
                            nextY += entry.rect.Height + (this.contentHeight - childHeightWithoutCellSpcaing) / (this.entries.Count + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextY += entry.rect.Height + (this.contentHeight - childHeightWithoutCellSpcaing) / (this.entries.Count - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                var childY = 0d;
                foreach (var entry in entries)
                {
                    switch (this.style.AlignmentVertical)
                    {
                        case Alignment.Start:
                            childY = y + this.style.BorderTop + this.style.PaddingTop;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childY = y + this.style.BorderTop + this.style.PaddingTop + (this.contentHeight - entry.rect.Height) / 2;
                            break;
                        case Alignment.End:
                            childY += y + this.rect.Height - this.style.BorderBottom - this.style.PaddingBottom - entry.rect.Height;
                            break;
                    }
                    entry.SetY(childY);
                }
            }
        }

    }
}
