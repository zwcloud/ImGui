using System.Collections.Generic;
using System.Diagnostics;
using Cairo;
using ImGui;
using System;

namespace Test
{
    public class Const
    {
        public static double ItemBorderTop = 2;
        public static double ItemBorderRight = 2;
        public static double ItemBorderBottom = 2;
        public static double ItemBorderLeft = 2;

        public static double ItemPaddingTop = 5;
        public static double ItemPaddingRight = 5;
        public static double ItemPaddingBottom = 5;
        public static double ItemPaddingLeft = 5;

        public static double GroupBorderTop = 10;
        public static double GroupBorderRight = 10;
        public static double GroupBorderBottom = 10;
        public static double GroupBorderLeft = 10;

        public static double GroupPaddingTop = 5;
        public static double GroupPaddingRight = 5;
        public static double GroupPaddingBottom = 5;
        public static double GroupPaddingLeft = 5;

        public static double CellingSpacingVertical = 10;
        public static double CellingSpacingHorizontal = 5;
    }

    public class Item
    {
        public Rect rect;//border-box
        public double contentWidth;//exact content width, pre-calculated from content and style
        public double contentHeight;//exact content height, pre-calculated from content and style
        public double minWidth = 1;//minimum width of content-box
        public double maxWidth = 9999;//maximum width of content-box
        public double minHeight = 1;//minimum height of content-box
        public double maxHeight = 9999;//maximum height of content-box
        public int horizontalStretchFactor;//horizontal stretch factor
        public int verticalStretchFactor;//vertical stretch factor

        public Style style = Style.Make(
                new[]{
                        new StyleModifier{Name = "BorderTop", Value = Const.ItemBorderTop},
                        new StyleModifier{Name = "BorderRight", Value = Const.ItemBorderRight},
                        new StyleModifier{Name = "BorderBottom", Value = Const.ItemBorderBottom},
                        new StyleModifier{Name = "BorderLeft", Value = Const.ItemBorderLeft},

                        new StyleModifier{Name = "PaddingTop", Value = Const.ItemPaddingTop},
                        new StyleModifier{Name = "PaddingRight", Value = Const.ItemPaddingRight},
                        new StyleModifier{Name = "PaddingBottom", Value = Const.ItemPaddingBottom},
                        new StyleModifier{Name = "PaddingLeft", Value = Const.ItemPaddingLeft},
                }
            );

        public bool HorizontallyStretched { get { return !IsFixedWidth && horizontalStretchFactor > 0; } }
        public bool VerticallyStretched { get { return !IsFixedHeight && verticalStretchFactor > 0; } }

        public bool IsFixedWidth { get { return MathEx.AmostEqual(this.minWidth, this.maxWidth); } }
        public bool IsFixedHeight { get { return MathEx.AmostEqual(this.minHeight, this.maxHeight); } }


        public Item(params LayoutOption[] options)
        {
            if (options != null)
            {
                this.ApplyOptions(options);
            }
        }

        protected void ApplyOptions(LayoutOption[] options)
        {
            if (options == null)
            {
                return;
            }
            //TODO handle min/max width/height
            for (var i = 0; i < options.Length; i++)
            {
                var option = options[i];
                switch (option.type)
                {
                    case LayoutOption.Type.fixedWidth:
                        if ((double) option.value < this.style.PaddingHorizontal + this.style.BorderHorizontal)
                        {
                            throw new InvalidOperationException(
                                string.Format("The specified width is too small. It must bigger than the horizontal padding and border size ({0}).", this.style.PaddingHorizontal + this.style.BorderHorizontal));
                        }
                        this.minWidth = this.maxWidth = (double)option.value;
                        this.horizontalStretchFactor = 0;
                        break;
                    case LayoutOption.Type.fixedHeight:
                        if ((double)option.value < this.style.PaddingVertical + this.style.BorderVertical)
                        {
                            throw new InvalidOperationException(
                                string.Format("The specified height is too small. It must bigger than the vertical padding and border size ({0}).", this.style.PaddingVertical + this.style.BorderVertical));
                        }
                        this.minHeight = this.maxHeight = (double)option.value;
                        this.verticalStretchFactor = 0;
                        break;
                    case LayoutOption.Type.stretchWidth:
                        this.horizontalStretchFactor = (int)option.value;
                        break;
                    case LayoutOption.Type.stretchHeight:
                        this.verticalStretchFactor = (int)option.value;
                        break;
                }
            }
        }

        public virtual void CalcWidth(double unitPartWidth = -1d)
        {
            if (this.HorizontallyStretched)
            {
                if (unitPartWidth > 0)
                {
                    this.rect.Width = unitPartWidth * this.horizontalStretchFactor;
                    this.contentWidth = this.rect.Width - this.style.PaddingHorizontal - this.style.BorderHorizontal;
                }
                else
                {
                    throw new ArgumentException("The unit part width is invalid", "unitPartWidth");
                }
            }
            else if (this.IsFixedWidth)
            {
                this.rect.Width = this.minWidth;
                this.contentWidth = this.rect.Width - this.style.PaddingHorizontal - this.style.BorderHorizontal;
            }
            else
            {
                this.rect.Width = this.contentWidth + this.style.PaddingHorizontal + this.style.BorderHorizontal;
            }
        }

        public virtual void CalcHeight(double unitPartHeight = -1d)
        {
            if (this.VerticallyStretched)
            {
                if (unitPartHeight > 0)
                {
                    this.rect.Height = unitPartHeight * this.verticalStretchFactor;
                    this.contentHeight = this.rect.Height - this.style.PaddingVertical - this.style.BorderVertical;
                }
                else
                {
                    throw new ArgumentException("The unit part height is invalid", "unitPartHeight");
                }
            }
            else if (this.IsFixedHeight)
            {
                this.rect.Height = this.minHeight;
                this.contentHeight = this.rect.Height - this.style.PaddingVertical - this.style.BorderVertical;
            }
            else
            {
                this.rect.Height = this.contentHeight + this.style.PaddingVertical + this.style.BorderVertical;
            }
        }

        public virtual void SetX(double x)
        {
            this.rect.X = x;
        }

        public virtual void SetY(double y)
        {
            this.rect.Y = y;
        }

        public Item Clone() { return (Item)this.MemberwiseClone(); }
    }

    public class Group : Item
    {
        public bool isVertical;
        public bool isClipped;
        public List<Item> entries = new List<Item>();

        public Style style = Style.Make(
                new[]{
                        new StyleModifier{Name = "BorderTop", Value = Const.GroupBorderTop},
                        new StyleModifier{Name = "BorderRight", Value = Const.GroupBorderRight},
                        new StyleModifier{Name = "BorderBottom", Value = Const.GroupBorderBottom},
                        new StyleModifier{Name = "BorderLeft", Value = Const.GroupBorderLeft},

                        new StyleModifier{Name = "PaddingTop", Value = Const.GroupPaddingTop},
                        new StyleModifier{Name = "PaddingRight", Value = Const.GroupPaddingRight},
                        new StyleModifier{Name = "PaddingBottom", Value = Const.GroupPaddingBottom},
                        new StyleModifier{Name = "PaddingLeft", Value = Const.GroupPaddingLeft},

                        new StyleModifier{Name = "CellingSpacingVertical", Value = Const.CellingSpacingVertical},
                        new StyleModifier{Name = "CellingSpacingHorizontal", Value = Const.CellingSpacingHorizontal},
                }
            );

        public Group(bool isVertical, params LayoutOption[] options)
        {
            this.isVertical = isVertical;
            base.ApplyOptions(options);
        }

        public void Add(Item item)
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

        private void Draw(Cairo.Context context, bool needClip)
        {
            if (needClip)
            {
                context.Rectangle(rect.X + style.PaddingLeft + style.BorderLeft, rect.Y + style.PaddingTop + style.BorderTop,
                    rect.Width - style.PaddingHorizontal - style.BorderHorizontal, rect.Height - style.PaddingVertical - style.BorderVertical);
                //context.StrokePreserve();
                context.Clip();
            }
            foreach (var entry in this.entries)
            {
                if (entry.HorizontallyStretched || entry.VerticallyStretched)
                {
                    context.FillRectangle(entry.rect, CairoEx.ColorLightBlue);
                }
                else if (entry.IsFixedWidth || entry.IsFixedHeight)
                {
                    context.FillRectangle(entry.rect, CairoEx.ColorOrange);
                }
                else
                {
                    context.FillRectangle(entry.rect, CairoEx.ColorPink);
                }
                context.StrokeRectangle(entry.rect, CairoEx.ColorBlack);
                var innerGroup = entry as Group;
                if (innerGroup != null)
                {
                    context.Save();
                    innerGroup.Draw(context, needClip);
                    context.Restore();
                }
            }
            if (needClip)
            {
                context.ResetClip();
            }
        }

        public void ShowResult()
        {
            var surface = CairoEx.BuildSurface((int)rect.Width, (int)rect.Height, CairoEx.ColorMetal, Format.Rgb24);
            var context = new Cairo.Context(surface);

            Draw(context, needClip: true);

            string outputPath = "D:\\LayoutTest";
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }

            string filePath = outputPath + "\\" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff_") + surface.GetHashCode() + ".png";
            surface.WriteToPng(filePath);
            surface.Dispose();
            context.Dispose();

            Process.Start("rundll32.exe", @"""C:\Program Files\Windows Photo Viewer\PhotoViewer.dll"",ImageView_Fullscreen " + filePath);
        }
    }
}
