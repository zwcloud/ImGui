using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Common.Primitive;

namespace ImGui.Layout
{
    [DebuggerDisplay("{Id}, Count={Entries.Count}")]
    internal class LayoutGroup : LayoutEntry
    {
        private int cursor;

        public LayoutGroup(bool isVertical, GUIStyle style, params LayoutOption[] options) : base(style, options)
        {
            this.IsVertical = isVertical;
            this.cursor = 0;
        }

        public bool IsVertical { get; }

        public List<LayoutEntry> Entries { get; } = new List<LayoutEntry>();

        public LayoutEntry GetNext()
        {
            if (this.cursor < this.Entries.Count)
            {
                LayoutEntry result = this.Entries[this.cursor];
                this.cursor++;
                return result;
            }
            return null;
        }

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

        public Rect GetLast()
        {
            Rect result;
            if (this.cursor == 0)
            {
                Log.Error("No last rect available.");
                result = Rect.Empty;
            }
            else if (this.cursor <= this.Entries.Count)
            {
                LayoutEntry entry = this.Entries[this.cursor - 1];
                result = entry.Rect;
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
                this.ContentWidth = this.Rect.Width - this.Style.PaddingHorizontal - this.Style.BorderHorizontal;

                if (this.ContentWidth <= 0) return;//container has no space to hold the children

                // calculate the width of children
                CalcChildrenWidth();
            }
            else if (this.IsFixedWidth)//fiexed width
            {
                // calculate the width
                this.Rect.Width = this.MinWidth;
                this.ContentWidth = this.Rect.Width - this.Style.PaddingHorizontal - this.Style.BorderHorizontal;

                if (this.ContentWidth <= 0) return;//container has no space to hold the children

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
                        temp += entry.Rect.Width + this.Style.CellingSpacingHorizontal;
                    }
                    temp -= this.Style.CellingSpacingHorizontal;
                    this.ContentWidth = temp < 0 ? 0 : temp;
                }
                this.Rect.Width = this.ContentWidth + this.Style.PaddingHorizontal + this.Style.BorderHorizontal;
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
                        entry.CalcWidth(this.ContentWidth); //the unitPartWidth for stretched children is the content-box width of the group
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
                var totalFactor = 0;
                var totalStretchedPartWidth = this.ContentWidth -
                                              this.Style.CellingSpacingHorizontal * (childCount - 1);
                foreach (var entry in this.Entries)
                {
                    if (entry.HorizontallyStretched)
                    {
                        totalFactor += entry.HorizontalStretchFactor;
                    }
                    else
                    {
                        entry.CalcWidth();
                        totalStretchedPartWidth -= entry.Rect.Width;
                    }
                }
                var childUnitPartWidth = totalStretchedPartWidth / totalFactor;
                // calculate the width of stretched children
                foreach (var entry in this.Entries)
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
                this.Rect.Height = unitPartHeight * this.VerticalStretchFactor;
                this.ContentHeight = this.Rect.Height - this.Style.PaddingVertical - this.Style.BorderVertical;

                if (this.ContentHeight < 1) return;//container has no space to hold the children

                // calculate the height of children
                CalcChildrenHeight();
            }
            else if (this.IsFixedHeight)//fiexed height
            {
                // calculate the height
                this.Rect.Height = this.MinHeight;
                this.ContentHeight = this.Rect.Height - this.Style.PaddingVertical - this.Style.BorderVertical;

                if (this.ContentHeight < 1) return;//container has no space to hold the children

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
                        temp += entry.Rect.Height + this.Style.CellingSpacingVertical;
                    }
                    temp -= this.Style.CellingSpacingVertical;
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
                this.Rect.Height = this.ContentHeight + this.Style.PaddingVertical + this.Style.BorderVertical;
            }
        }

        private void CalcChildrenHeight()
        {
            if (this.IsVertical) // vertical group
            {
                // calculate the unitPartHeight for stretched children
                // calculate the height of fixed-size children
                var childCount = this.Entries.Count;
                var totalStretchedPartHeight = this.ContentHeight - (childCount - 1) * this.Style.CellingSpacingVertical;
                var totalFactor = 0;
                foreach (var entry in this.Entries)
                {
                    if (entry.VerticallyStretched)
                    {
                        totalFactor += entry.VerticalStretchFactor;
                    }
                    else
                    {
                        entry.CalcHeight();
                        totalStretchedPartHeight -= entry.Rect.Height;
                    }
                }
                var childUnitPartHeight = totalStretchedPartHeight / totalFactor;
                // calculate the height of stretched children
                foreach (var entry in this.Entries)
                {
                    if (entry.VerticallyStretched)
                    {
                        entry.CalcHeight(childUnitPartHeight);
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
            this.Rect.X = x;
            if (this.IsVertical)
            {
                var childX = 0d;
                foreach (var entry in this.Entries)
                {
                    switch (this.Style.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                            childX = x + this.Style.BorderLeft + this.Style.PaddingLeft;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childX = x + this.Style.BorderLeft + this.Style.PaddingLeft + (this.ContentWidth - entry.Rect.Width) / 2;
                            break;
                        case Alignment.End:
                            childX = x + this.Rect.Width - this.Style.BorderRight - this.Style.PaddingRight - entry.Rect.Width;
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
                    childWidthWithCellSpcaing += entry.Rect.Width + this.Style.CellingSpacingHorizontal;
                    childWidthWithoutCellSpcaing += entry.Rect.Width;
                }
                childWidthWithCellSpcaing -= this.Style.CellingSpacingVertical;

                switch (this.Style.AlignmentHorizontal)
                {
                    case Alignment.Start:
                        nextX = x + this.Style.BorderLeft + this.Style.PaddingLeft;
                        break;
                    case Alignment.Center:
                        nextX = x + this.Style.BorderLeft + this.Style.PaddingLeft + (this.ContentWidth - childWidthWithCellSpcaing) / 2;
                        break;
                    case Alignment.End:
                        nextX = x + this.Rect.Width - this.Style.BorderRight - this.Style.PaddingRight - childWidthWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextX = x + this.Style.BorderLeft + this.Style.PaddingLeft +
                                (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.Entries.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextX = x + this.Style.BorderLeft + this.Style.PaddingLeft;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in this.Entries)
                {
                    entry.SetX(nextX);
                    switch (this.Style.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                        case Alignment.Center:
                        case Alignment.End:
                            nextX += entry.Rect.Width + this.Style.CellingSpacingHorizontal;
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
                    childHeightWithCellSpcaing += entry.Rect.Height + this.Style.CellingSpacingVertical;
                    childHeightWithoutCellSpcaing += entry.Rect.Height;
                }
                childHeightWithCellSpcaing -= this.Style.CellingSpacingVertical;

                switch (this.Style.AlignmentVertical)
                {
                    case Alignment.Start:
                        nextY = y + this.Style.BorderTop + this.Style.PaddingTop;
                        break;
                    case Alignment.Center:
                        nextY = y + this.Style.BorderTop + this.Style.PaddingTop + (this.ContentHeight - childHeightWithCellSpcaing) / 2;
                        break;
                    case Alignment.End:
                        nextY = y + this.Rect.Height - this.Style.BorderBottom - this.Style.PaddingBottom - childHeightWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextY = y + this.Style.BorderTop + this.Style.PaddingTop +
                                (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.Entries.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextY = y + this.Style.BorderTop + this.Style.PaddingTop;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in this.Entries)
                {
                    entry.SetY(nextY);
                    switch (this.Style.AlignmentVertical)
                    {
                        case Alignment.Start:
                        case Alignment.Center:
                        case Alignment.End:
                            nextY += entry.Rect.Height + this.Style.CellingSpacingVertical;
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
                    switch (this.Style.AlignmentVertical)
                    {
                        case Alignment.Start:
                            childY = y + this.Style.BorderTop + this.Style.PaddingTop;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childY = y + this.Style.BorderTop + this.Style.PaddingTop + (this.ContentHeight - entry.Rect.Height) / 2;
                            break;
                        case Alignment.End:
                            childY += y + this.Rect.Height - this.Style.BorderBottom - this.Style.PaddingBottom - entry.Rect.Height;
                            break;
                    }
                    entry.SetY(childY);
                }
            }
        }

    }
}
