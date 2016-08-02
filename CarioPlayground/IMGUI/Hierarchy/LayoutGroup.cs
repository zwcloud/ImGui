using System;
using System.Collections.Generic;

namespace ImGui
{
    internal class LayoutGroup : LayoutEntry
    {
        public bool isVertical;
        public bool isForm;

        public List<LayoutEntry> entries = new List<LayoutEntry>();
        private int cursor;

        protected double m_ChildMinWidth = 100;
        protected double m_ChildMaxWidth = 100;
        protected double m_ChildMinHeight = 100;
        protected double m_ChildMaxHeight = 100;

        public LayoutGroup(Style style) : base(style)
        {
            cursor = 0;
        }

        public LayoutGroup() : base(Style.None)
        {
            cursor = 0;
        }

        public void Add(LayoutEntry layoutEntry)
        {
            this.entries.Add(layoutEntry);
            layoutEntry.parent = this;
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

        public void ResetCursor()
        {
            this.cursor = 0;
        }

        //Filled layout, not used
        public override void CalcRect()
        {
            if (this.isForm)
            {
                this.rect = new Rect(0, 0, Form.current.Size);
            }

            if (this.entries.Count == 0)
            {
                this.rect.Width = (float)base.style.PaddingHorizontal;
                return;
            }
            if (isVertical)
            {
                var X = this.rect.X;
                var Y = this.rect.Y;
                var W = this.rect.Width;
                var H = this.rect.Height;
                var childCount = this.entries.Count;
                var childX = X + style.PaddingLeft;
                var nextChildY = Y + (float)style.PaddingTop;
                var childWidth = W - style.PaddingHorizontal;
                var childHeight = H / childCount - style.PaddingVertical;
                for (int i = 0; i < childCount; ++i)
                {
                    var entry = this.entries[i];
                    entry.rect.X = childX;
                    entry.rect.Y = nextChildY;
                    entry.rect.Width = childWidth;
                    entry.rect.Height = childHeight;
                    nextChildY += style.PaddingVertical + entry.rect.Height;
                    entry.CalcRect();
                }
            }
            else
            {
                var X = this.rect.X;
                var Y = this.rect.Y;
                var W = this.rect.Width;
                var H = this.rect.Height;
                var childCount = this.entries.Count;
                var nextChildX = X + (float)style.PaddingLeft;
                var childY = Y + style.PaddingTop;
                var childWidth = W / childCount - style.PaddingHorizontal;
                var childHeight = H - style.PaddingVertical;
                for (var i=0; i<childCount; ++i)
                {
                    var entry = this.entries[i];
                    entry.rect.X = nextChildX;
                    entry.rect.Y = childY;
                    entry.rect.Width = childWidth;
                    entry.rect.Height = childHeight;
                    nextChildX += style.PaddingHorizontal + (float)childWidth;
                    entry.CalcRect();
                }
            }
        }

        public override void CalcWidth()
        {
            if(this.entries.Count == 0)
            {
                this.maxWidth = this.minWidth = base.style.PaddingHorizontal;
                return;
            }
            this.m_ChildMinWidth = 0;
            this.m_ChildMaxWidth = 0;
            if(this.isVertical)
            {
                foreach (var entry in this.entries)
                {
                    entry.CalcWidth();
                    this.m_ChildMinWidth = Math.Max(entry.minWidth, this.m_ChildMinWidth);
                    this.m_ChildMaxWidth = Math.Max(entry.maxWidth, this.m_ChildMaxWidth);
                }
            }
            else
            {
                foreach (var entry in this.entries)
                {
                    entry.CalcWidth();
                    this.m_ChildMinWidth += entry.minWidth;
                    this.m_ChildMaxWidth += entry.maxWidth;
                }
            }
        }

        public override void CalcHeight()
        {
            if (this.entries.Count == 0)
            {
                this.maxHeight = this.minHeight = base.style.PaddingVertical;
                return;
            }
            this.m_ChildMinHeight = 0;
            this.m_ChildMaxHeight = 0;
            if (this.isVertical)
            {
                foreach (var entry in this.entries)
                {
                    entry.CalcHeight();
                    this.m_ChildMinHeight += entry.minHeight;
                    this.m_ChildMaxHeight += entry.maxHeight;
                }
            }
            else
            {
                foreach (var entry in this.entries)
                {
                    entry.CalcHeight();
                    this.m_ChildMinHeight = Math.Max(entry.minHeight, this.m_ChildMinHeight);
                    this.m_ChildMaxHeight = Math.Max(entry.maxHeight, this.m_ChildMaxHeight);
                }
            }
            //this.minHeight = Math.Max(this.minHeight, this.m_ChildMinHeight);
            //if (this.maxHeight == 0f)
            //{
            //    this.maxHeight = this.m_ChildMaxHeight;
            //}
            //this.maxHeight = Math.Max(this.maxHeight, this.minHeight);
        }

        public override void SetHorizontal(double x, double width)
        {
            base.SetHorizontal(x, width);
            if (this.isVertical)
            {
                if (base.style != Style.None)
                {
                    //
                }
                else
                {
                    var childWidth = width + this.style.PaddingHorizontal;
                    foreach (var entry in this.entries)
                    {
                        entry.SetHorizontal(x, MathEx.Clamp(width, entry.minWidth, entry.maxWidth));
                    }
                }
            }
            else
            {
                if (base.style != Style.None)
                {
                    //
                }
                double t = 0;
                if (this.m_ChildMinWidth != this.m_ChildMaxWidth)
                {
                    t = MathEx.Clamp((width - this.m_ChildMinWidth) / (this.m_ChildMaxWidth - this.m_ChildMinWidth), 0, 1);
                }
                foreach (var entry in this.entries)
                {
                    var lerpedWidth = MathEx.Lerp(entry.minWidth, entry.maxWidth, t);
                    entry.SetHorizontal(Math.Round(x), Math.Round(lerpedWidth));
                    x += lerpedWidth;
                }
            }
            //this.minWidth = Math.Max(this.minWidth, this.m_ChildMinWidth);
            //if (this.maxWidth == 0f)
            //{
            //    this.maxWidth = this.m_ChildMaxWidth;
            //}
            //this.maxWidth = Math.Max(this.maxWidth, this.minWidth);
        }

        public override void SetVertical(double y, double height)
        {
            base.SetVertical(y, height);
            if (this.entries.Count == 0)
            {
                return;
            }
            if (this.isVertical)
            {
                if (base.style!=Style.None)
                {
                    //
                }
                double t = 0;
                if (this.m_ChildMinHeight != this.m_ChildMaxHeight)
                {
                    t = MathEx.Clamp((height - this.m_ChildMinHeight) / (this.m_ChildMaxHeight - this.m_ChildMinHeight), 0, 1);
                }
                foreach (var entry in this.entries)
                {
                    var lerpedHeight = MathEx.Lerp(entry.minHeight, entry.maxHeight, t);
                    entry.SetVertical(Math.Round(y), Math.Round(lerpedHeight));
                    y += lerpedHeight;
                }
            }
            else
            {
                if (base.style != Style.None)
                {
                    //
                }
                else
                {
                    var childHeight = height + this.style.PaddingVertical;
                    foreach (var entry in this.entries)
                    {
                        entry.SetVertical(y, MathEx.Clamp(height, entry.minHeight, entry.maxHeight));
                    }
                }
            }
        }

    }
}
