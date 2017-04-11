using System.Collections.Generic;
using System.Diagnostics;
using ImGui;
using System;
using Xunit;

namespace Test
{
    //[TestClass]
    public class LayoutTest1
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

            private readonly GUIStyle style = new GUIStyle();

            public Item()
            {
                style.Set<double>(GUIStyleName.BorderTop, Const.ItemBorderTop);
                style.Set<double>(GUIStyleName.BorderRight, Const.ItemBorderRight);
                style.Set<double>(GUIStyleName.BorderBottom, Const.ItemBorderBottom);
                style.Set<double>(GUIStyleName.BorderLeft, Const.ItemBorderLeft);

                style.Set<double>(GUIStyleName.PaddingTop, Const.ItemPaddingTop);
                style.Set<double>(GUIStyleName.PaddingRight, Const.ItemPaddingRight);
                style.Set<double>(GUIStyleName.PaddingBottom, Const.ItemPaddingBottom);
                style.Set<double>(GUIStyleName.PaddingLeft, Const.ItemPaddingLeft);
            }

            public virtual void CalcWidth()
            {
                this.rect.Width = this.contentWidth + this.style.PaddingHorizontal + this.style.BorderHorizontal;
            }

            public virtual void CalcHeight()
            {
                this.rect.Height = this.contentHeight + this.style.PaddingVertical + this.style.BorderVertical;
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
            public bool isForm = false;
            public bool isVertical;
            public bool isClipped;
            public List<Item> entries = new List<Item>();

            internal readonly GUIStyle style = new GUIStyle();
            
            public Group()
            {
                style.Set<double>(GUIStyleName.BorderTop, Const.GroupBorderTop);
                style.Set<double>(GUIStyleName.BorderRight, Const.GroupBorderRight);
                style.Set<double>(GUIStyleName.BorderBottom, Const.GroupBorderBottom);
                style.Set<double>(GUIStyleName.BorderLeft, Const.GroupBorderLeft);

                style.Set<double>(GUIStyleName.PaddingTop, Const.GroupPaddingTop);
                style.Set<double>(GUIStyleName.PaddingRight, Const.GroupPaddingRight);
                style.Set<double>(GUIStyleName.PaddingBottom, Const.GroupPaddingBottom);
                style.Set<double>(GUIStyleName.PaddingLeft, Const.GroupPaddingLeft);

                style.Set<double>(GUIStyleName.CellingSpacingVertical, Const.CellingSpacingVertical);
                style.Set<double>(GUIStyleName.CellingSpacingHorizontal, Const.CellingSpacingHorizontal);
            }

            public Group(bool isVertical, bool isForm = false) : this()
            {
                this.isVertical = isVertical;
                this.isForm = isForm;
            }

            public Group(Rect rect, bool isVertical, bool isForm = false) : this()
            {
                this.rect = rect;
                this.isVertical = isVertical;
                this.isForm = isForm;
            }

            public void Add(Item item)
            {
                this.entries.Add(item);
            }

            public override void CalcWidth()
            {
                if (this.entries.Count == 0)
                {
                    this.rect.Width = this.style.PaddingHorizontal + this.style.BorderHorizontal;
                    return;
                }

                if (this.isVertical)
                {
                    for (int i = 0; i < this.entries.Count; i++)
                    {
                        var entry = this.entries[i];
                        entry.CalcWidth();
                        this.contentWidth = Math.Max(this.contentWidth, entry.rect.Width);
                    }
                }
                else
                {
                    this.contentWidth = 0;
                    for (int i = 0; i < this.entries.Count; i++)
                    {
                        var entry = this.entries[i];
                        entry.CalcWidth();
                        this.contentWidth += entry.rect.Width;
                        if (i > 0)
                        {
                            this.contentWidth += this.style.CellingSpacingHorizontal;
                        }
                    }
                }

                this.rect.Width = this.contentWidth + this.style.PaddingHorizontal + this.style.BorderHorizontal;
            }

            public override void CalcHeight()
            {
                if (this.entries.Count == 0)
                {
                    this.rect.Height = this.style.PaddingVertical + this.style.BorderVertical;
                    return;
                }

                if (this.isVertical)
                {
                    this.contentHeight = 0;
                    for (int i = 0; i < this.entries.Count; i++)
                    {
                        var entry = this.entries[i];
                        entry.CalcHeight();
                        this.contentHeight += entry.rect.Height;
                        if (i > 0)
                        {
                            this.contentHeight += this.style.CellingSpacingVertical;
                        }
                    }
                }
                else
                {
                    this.contentHeight = 0;
                    for (int i = 0; i < this.entries.Count; i++)
                    {
                        var entry = this.entries[i];
                        entry.CalcHeight();
                        this.contentHeight = Math.Max(this.contentHeight, entry.rect.Height);
                    }
                }

                this.rect.Height = this.contentHeight + this.style.PaddingVertical + this.style.BorderVertical;
            }

            public override void SetX(double x)
            {
                this.rect.X = x;
                var nextChildX = x + this.style.BorderLeft + this.style.PaddingLeft;
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    entry.SetX(nextChildX);
                    if (!this.isVertical)
                    {
                        nextChildX += this.style.CellingSpacingHorizontal + entry.rect.Width;
                    }
                }
            }

            public override void SetY(double y)
            {
                this.rect.Y = y;
                var nextChildY = y + this.style.BorderTop + this.style.PaddingTop;
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    entry.SetY(nextChildY);
                    if (this.isVertical)
                    {
                        nextChildY += this.style.CellingSpacingVertical + entry.rect.Height;
                    }
                }
            }

            private void Draw(Cairo.Context context, bool needClip)
            {
                if (needClip)
                {
                    context.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                    context.Clip();
                }
                foreach (var entry in this.entries)
                {
                    if (entry.horizontalStretchFactor != 0 || entry.verticalStretchFactor != 0)
                    {
                        context.FillRectangle(entry.rect, CairoEx.ColorLightBlue);
                        context.StrokeRectangle(entry.rect, CairoEx.ColorBlack);
                    }
                    else
                    {
                        context.FillRectangle(entry.rect, CairoEx.ColorPink);
                        context.StrokeRectangle(entry.rect, CairoEx.ColorBlack);
                    }
                    var innerGroup = entry as Group;
                    if (innerGroup != null)
                    {
                        innerGroup.Draw(context, needClip);
                    }
                }
                if (needClip)
                {
                    context.ResetClip();
                }
            }

            public void ShowResult()
            {
                var surface = CairoEx.BuildSurface((int)rect.Width, (int)rect.Height, CairoEx.ColorMetal, Cairo.Format.Rgb24);
                var context = new Cairo.Context(surface);

                Draw(context, isClipped);

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

        #region helper methods

        private Item CreateItem()
        {
            return new Item
            {
                contentWidth = 50,
                contentHeight = 50,
                horizontalStretchFactor = 0,
                verticalStretchFactor = 0,
            };
        }

        private Group CreateVerticalGroup()
        {
            return new Group(true)
            {
                horizontalStretchFactor = 0,
                verticalStretchFactor = 0,
            };
        }

        private Group CreateHorizontalGroup()
        {
            return new Group(false)
            {
                horizontalStretchFactor = 0,
                verticalStretchFactor = 0,
            };
        }

        #endregion

        [Fact, Trait("Category", "Rect & content size"), Trait("Description", "The size of an entry is correctly calculated")]
        public void TheSizeOfAnEntryIsCorrectlyCalculated()
        {
            Item item = CreateItem();
            item.CalcWidth();
            item.CalcHeight();

            Assert.Equal(item.contentWidth + Const.ItemPaddingLeft + Const.ItemPaddingRight + Const.ItemBorderLeft + Const.ItemBorderRight, item.rect.Width);
            Assert.Equal(item.contentHeight + Const.ItemPaddingTop + Const.ItemPaddingBottom + Const.ItemBorderTop + Const.ItemBorderBottom, item.rect.Height);
        }

        [Fact, Trait("Category", "Rect & content size"), Trait("Description", "The size of a vertical group that contains a single entry is correctly calculated")]
        public void TheSizeOfAVerticalGroupThatContainsASingleEntryIsCorrectlyCalculated()
        {
            Group group = CreateVerticalGroup();
            Item item = CreateItem();

            group.Add(item);
            group.CalcWidth();
            group.CalcHeight();

            Assert.Equal(item.rect.Width + group.style.PaddingHorizontal + group.style.BorderHorizontal, group.rect.Width);
            Assert.Equal(item.rect.Height + group.style.PaddingVertical + group.style.BorderVertical, group.rect.Height);
        }

        [Fact, Trait("Category", "Rect & content size"), Trait("Description", "The size of a vertical group that contains multiple entries is correctly calculated")]
        public void TheSizeOfAVerticalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
        {
            Group group = CreateVerticalGroup();
            Item item = CreateItem();

            for (int i = 0; i < 5; i++)
            {
                var newItem = item.Clone();
                newItem.contentWidth += i * 5;
                newItem.contentHeight += i * 8;
                group.Add(newItem);
            }
            group.CalcWidth();
            group.CalcHeight();

            var expectedWidth = group.entries[0].rect.Width;
            var expectedHeight = group.entries[0].rect.Height;
            for (int i = 1; i < group.entries.Count; i++)
            {
                var entry = group.entries[i];
                expectedWidth = Math.Max(expectedWidth, entry.rect.Width);
                expectedHeight += entry.rect.Height + group.style.CellingSpacingVertical;
            }
            expectedWidth += group.style.PaddingHorizontal + group.style.BorderHorizontal;
            expectedHeight += group.style.PaddingVertical + group.style.BorderVertical;

            Assert.Equal(expectedWidth, group.rect.Width);
            Assert.Equal(expectedHeight, group.rect.Height);
        }

        [Fact, Trait("Category", "Rect & content size"), Trait("Description", "The size of a horizontal group that contains a single entry is correctly calculated")]
        public void TheSizeOfAHorizontalGroupThatContainsASingleEntryIsCorrectlyCalculated()
        {
            Group group = CreateHorizontalGroup();
            Item item = CreateItem();

            group.Add(item);
            group.CalcWidth();
            group.CalcHeight();

            Assert.Equal(item.rect.Width + group.style.PaddingHorizontal + group.style.BorderHorizontal, group.rect.Width);
            Assert.Equal(item.rect.Height + group.style.PaddingVertical + group.style.BorderVertical, group.rect.Height);
        }

        [Fact, Trait("Category", "Rect & content size"), Trait("Description", "The size of a horizontal group that contains multiple entries is correctly calculated")]
        public void TheSizeOfAHorizontalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
        {
            Group group = CreateHorizontalGroup();
            Item item = CreateItem();

            for (int i = 0; i < 5; i++)
            {
                var newItem = item.Clone();
                newItem.contentWidth += i * 5;
                newItem.contentHeight += i * 8;
                group.Add(newItem);
            }
            group.CalcWidth();
            group.CalcHeight();

            var expectedWidth = group.entries[0].rect.Width;
            var expectedHeight = group.entries[0].rect.Height;
            for (int i = 1; i < group.entries.Count; i++)
            {
                var entry = group.entries[i];
                expectedWidth += entry.rect.Width + group.style.CellingSpacingHorizontal;
                expectedHeight = Math.Max(expectedHeight, entry.rect.Height);
            }
            expectedWidth += group.style.PaddingHorizontal + group.style.BorderHorizontal;
            expectedHeight += group.style.PaddingVertical + group.style.BorderVertical;

            Assert.Equal(expectedWidth, group.rect.Width);
            Assert.Equal(expectedHeight, group.rect.Height);
        }


        [Fact, Trait("Category", "Layout(non-stretch)"), Trait("Description", "Show an empty horizontal group")]
        public void ShowAnEmptyHorizontalGroup()
        {
            Group group = CreateHorizontalGroup();
            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "Layout(non-stretch)"), Trait("Description", "Show a horizontal group of 1 item")]
        public void ShowAHorizontalGroupOf1Item()
        {
            Group group = CreateHorizontalGroup();
            Item item = CreateItem();

            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "Layout(non-stretch)"), Trait("Description", "Show a horizontal group of 3 items")]
        public void ShowAHorizontalGroupOf3Items()
        {
            Group group = CreateHorizontalGroup();
            Item item = CreateItem();

            group.Add(item.Clone());
            group.Add(item.Clone());
            group.Add(item.Clone());


            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "Layout(non-stretch)"), Trait("Description", "Show an empty vertical group")]
        public void ShowAnEmptyVerticalGroup()
        {
            Group group = CreateVerticalGroup();
            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "Layout(non-stretch)"), Trait("Description", "Show a vertical group of 1 items")]
        public void ShowAVerticalGroupOf1Items()
        {
            Group group = CreateVerticalGroup();
            Item item = CreateItem();

            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "Layout(non-stretch)"), Trait("Description", "Show a vertical group of 3 items")]
        public void ShowAVerticalGroupOf3Items()
        {
            Group group = CreateVerticalGroup();
            Item item = CreateItem();

            group.Add(item.Clone());
            group.Add(item.Clone());
            group.Add(item.Clone());

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "Layout(non-stretch)"), Trait("Description", "Show a group of 3x3 items, outter group is vertical")]
        public void ShowAGroupOf3x3Items_OutterGroupIsVertical()
        {
            Group outterGroup = CreateVerticalGroup();
            Group innerGroup0 = CreateHorizontalGroup();
            Group innerGroup1 = CreateHorizontalGroup();
            Group innerGroup2 = CreateHorizontalGroup();
            Item item = CreateItem();
            innerGroup0.Add(item.Clone());
            innerGroup0.Add(item.Clone());
            innerGroup0.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            outterGroup.Add(innerGroup0);
            outterGroup.Add(innerGroup1);
            outterGroup.Add(innerGroup2);

            outterGroup.CalcWidth();
            outterGroup.CalcHeight();
            outterGroup.SetX(0);
            outterGroup.SetY(0);

            outterGroup.ShowResult();
        }

        [Fact, Trait("Category", "Layout(non-stretch)"), Trait("Description", "Show a group of 3x3 items, outter group is horizontal")]
        public void ShowAGroupOf3x3Items_OutterGroupIsHorizontal()
        {
            Group outterGroup = CreateHorizontalGroup();
            Group innerGroup0 = CreateVerticalGroup();
            Group innerGroup1 = CreateVerticalGroup();
            Group innerGroup2 = CreateVerticalGroup();
            Item item = CreateItem();
            innerGroup0.Add(item.Clone());
            innerGroup0.Add(item.Clone());
            innerGroup0.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            outterGroup.Add(innerGroup0);
            outterGroup.Add(innerGroup1);
            outterGroup.Add(innerGroup2);

            outterGroup.CalcWidth();
            outterGroup.CalcHeight();
            outterGroup.SetX(0);
            outterGroup.SetY(0);

            outterGroup.ShowResult();
        }

        [Fact, Trait("Category", "Layout(non-stretch)"), Trait("Description", "Show a 3 layer group")]
        public void ShowA3LayerGroup()
        {
            Group group1 = CreateVerticalGroup();
            Group group2 = CreateHorizontalGroup();
            Group group3 = CreateHorizontalGroup();
            Group group4 = CreateHorizontalGroup();
            Group group5 = CreateVerticalGroup();
            Item item = CreateItem();

            group1.Add(group2);
            group1.Add(group3);
            group1.Add(group4);

            group2.Add(item.Clone());

            group3.Add(item.Clone());
            group3.Add(group5);
            group3.Add(item.Clone());

            group4.Add(item.Clone());

            group5.Add(item.Clone());
            group5.Add(item.Clone());
            group5.Add(item.Clone());

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }

    }
}