using System.Collections.Generic;
using ImGui;
using Xunit;
using System;

namespace Test
{
    public class DefaultSizeTest
    {
        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of an entry is correctly calculated")]
        public void TheSizeOfAnEntryIsCorrectlyCalculated()
        {
            LayoutEntry item = new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 50, contentHeight = 50};

            item.CalcWidth();
            item.CalcHeight();

            Assert.Equal(item.contentWidth + Const.ItemPaddingLeft + Const.ItemPaddingRight + Const.ItemBorderLeft + Const.ItemBorderRight, item.rect.Width);
            Assert.Equal(item.contentHeight + Const.ItemPaddingTop + Const.ItemPaddingBottom + Const.ItemBorderTop + Const.ItemBorderBottom, item.rect.Height);
        }

        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of an empty vertical group is correctly calculated")]
        public void TheSizeOfAEmptyVerticalGroupIsCorrectlyCalculated()
        {
            LayoutGroup group = new LayoutGroup(true, Styles.DefaultGroupStyle);

            group.CalcWidth();
            group.CalcHeight();

            Assert.Equal(group.style.PaddingHorizontal + group.style.BorderHorizontal, group.rect.Width);
            Assert.Equal(group.style.PaddingVertical + group.style.BorderVertical, group.rect.Height);
        }

        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of a vertical group that contains a single entry is correctly calculated")]
        public void TheSizeOfAVerticalGroupThatContainsASingleEntryIsCorrectlyCalculated()
        {
            LayoutGroup group = new LayoutGroup(true, Styles.DefaultGroupStyle);
            LayoutEntry item = new LayoutEntry(Styles.DefaultEntryStyle) { contentWidth = 50, contentHeight = 50 };
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();

            Assert.Equal(item.rect.Width + group.style.PaddingHorizontal + group.style.BorderHorizontal, group.rect.Width);
            Assert.Equal(item.rect.Height + group.style.PaddingVertical + group.style.BorderVertical, group.rect.Height);
        }

        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of a vertical group that contains multiple entries is correctly calculated")]
        public void TheSizeOfAVerticalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
        {
            LayoutGroup group = new LayoutGroup(true, Styles.DefaultGroupStyle);
            LayoutEntry[] items =
            {
                new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 10, contentHeight = 20},
                new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 20, contentHeight = 30},
                new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 30, contentHeight = 40},
                new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 40, contentHeight = 50},
                new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 50, contentHeight = 60},
            };
            foreach (var item in items)
            {
                group.Add(item);
            }

            group.CalcWidth();
            group.CalcHeight();

            var expectedWidth = 0d;
            var expectedHeight = 0d;
            foreach (var item in items)
            {
                expectedWidth = Math.Max(expectedWidth, item.rect.Width);
                expectedHeight += item.rect.Height + group.style.CellingSpacingVertical;
            }
            expectedHeight -= group.style.CellingSpacingVertical;
            expectedWidth += group.style.PaddingHorizontal + group.style.BorderHorizontal;
            expectedHeight += group.style.PaddingVertical + group.style.BorderVertical;
            Assert.Equal(expectedWidth, group.rect.Width);
            Assert.Equal(expectedHeight, group.rect.Height);
        }

        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of a horizontal group that contains a single entry is correctly calculated")]
        public void TheSizeOfAHorizontalGroupThatContainsASingleEntryIsCorrectlyCalculated()
        {
            LayoutGroup group = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutEntry item = new LayoutEntry(Styles.DefaultEntryStyle) { contentWidth = 50, contentHeight = 50 };
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();

            Assert.Equal(item.rect.Width + group.style.PaddingHorizontal + group.style.BorderHorizontal, group.rect.Width);
            Assert.Equal(item.rect.Height + group.style.PaddingVertical + group.style.BorderVertical, group.rect.Height);
        }

        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of a horizontal group that contains multiple entries is correctly calculated")]
        public void TheSizeOfAHorizontalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
        {
            LayoutGroup group = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutEntry[] items =
            {
                new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 10, contentHeight = 20},
                new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 20, contentHeight = 30},
                new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 30, contentHeight = 40},
                new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 40, contentHeight = 50},
                new LayoutEntry(Styles.DefaultEntryStyle) {contentWidth = 50, contentHeight = 60},
            };
            foreach (var item in items)
            {
                group.Add(item);
            }

            group.CalcWidth();
            group.CalcHeight();

            var expectedWidth = 0d;
            var expectedHeight = 0d;
            foreach (var item in items)
            {
                expectedWidth += item.rect.Width + group.style.CellingSpacingHorizontal;
                expectedHeight = Math.Max(expectedHeight, item.rect.Height);
            }
            expectedWidth -= group.style.CellingSpacingHorizontal;
            expectedWidth += group.style.PaddingHorizontal + group.style.BorderHorizontal;
            expectedHeight += group.style.PaddingVertical + group.style.BorderHorizontal;
            Assert.Equal(expectedWidth, group.rect.Width);
            Assert.Equal(expectedHeight, group.rect.Height);
        }


        [Fact, Trait("Category", "layout"), Trait("Description", "Show an empty horizontal group")]
        public void ShowAnEmptyHorizontalGroup()
        {
            LayoutGroup group = new LayoutGroup(false, Styles.DefaultGroupStyle);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a horizontal group of 1 item")]
        public void ShowAHorizontalGroupOf1Item()
        {
            LayoutGroup group = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutEntry item= new LayoutEntry(Styles.DefaultEntryStyle)  {contentWidth = 50, contentHeight = 50};
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a horizontal group of 3 items")]
        public void ShowAHorizontalGroupOf3Items()
        {
            LayoutGroup group = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutEntry item= new LayoutEntry(Styles.DefaultEntryStyle)  { contentWidth = 50, contentHeight = 50 };
            group.Add(item.Clone());
            group.Add(item.Clone());
            group.Add(item.Clone());

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show an empty vertical group")]
        public void ShowAnEmptyVerticalGroup()
        {
            LayoutGroup group = new LayoutGroup(true, Styles.DefaultGroupStyle);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a vertical group of 1 items")]
        public void ShowAVerticalGroupOf1Items()
        {
            LayoutGroup group = new LayoutGroup(true, Styles.DefaultGroupStyle);
            LayoutEntry item= new LayoutEntry(Styles.DefaultEntryStyle)  { contentWidth = 50, contentHeight = 50 };
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a vertical group of 3 items")]
        public void ShowAVerticalGroupOf3Items()
        {
            LayoutGroup group = new LayoutGroup(true, Styles.DefaultGroupStyle);
            LayoutEntry item= new LayoutEntry(Styles.DefaultEntryStyle)  { contentWidth = 50, contentHeight = 50 };
            group.Add(item.Clone());
            group.Add(item.Clone());
            group.Add(item.Clone());

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a group of 3x3 items, outter group is vertical")]
        public void ShowAGroupOf3x3Items_OutterGroupIsVertical()
        {
            LayoutGroup outterGroup = new LayoutGroup(true, Styles.DefaultGroupStyle);
            LayoutGroup innerGroup0 = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutGroup innerGroup1 = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutGroup innerGroup2 = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutEntry item= new LayoutEntry(Styles.DefaultEntryStyle)  { contentWidth = 50, contentHeight = 50 };
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

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a group of 3x3 items, outter group is horizontal")]
        public void ShowAGroupOf3x3Items_OutterGroupIsHorizontal()
        {
            LayoutGroup outterGroup = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutGroup innerGroup0 = new LayoutGroup(true, Styles.DefaultGroupStyle);
            LayoutGroup innerGroup1 = new LayoutGroup(true, Styles.DefaultGroupStyle);
            LayoutGroup innerGroup2 = new LayoutGroup(true, Styles.DefaultGroupStyle);
            LayoutEntry item= new LayoutEntry(Styles.DefaultEntryStyle)  { contentWidth = 50, contentHeight = 50 };
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

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a 3 layer group")]
        public void ShowA3LayerGroup()
        {
            LayoutGroup group1 = new LayoutGroup(true, Styles.DefaultGroupStyle);
            LayoutGroup group2 = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutGroup group3 = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutGroup group4 = new LayoutGroup(false, Styles.DefaultGroupStyle);
            LayoutGroup group5 = new LayoutGroup(true, Styles.DefaultGroupStyle);
            LayoutEntry item1 = new LayoutEntry(Styles.DefaultEntryStyle)  { contentWidth = 50, contentHeight = 50 };
            LayoutEntry item2 = new LayoutEntry(Styles.DefaultEntryStyle)  { contentWidth = 50, contentHeight = 80 };
            LayoutEntry item3 = new LayoutEntry(Styles.DefaultEntryStyle)  { contentWidth = 80, contentHeight = 50 };
            LayoutEntry item4 = new LayoutEntry(Styles.DefaultEntryStyle)  { contentWidth = 400, contentHeight = 50 };

            group1.Add(group2);
            group1.Add(group3);
            group1.Add(group4);

            group2.Add(item1.Clone());
            group2.Add(item2.Clone());
            group2.Add(item3.Clone());

            group3.Add(item1.Clone());
            group3.Add(group5);
            group3.Add(item1.Clone());

            group4.Add(item4.Clone());

            group5.Add(item1.Clone());
            group5.Add(item2.Clone());
            group5.Add(item1.Clone());

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }

    }
}