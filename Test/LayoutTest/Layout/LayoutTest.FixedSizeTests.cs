using System.Collections.Generic;
using ImGui;
using System;
using Xunit;

namespace Test
{
    public class FixedSizeTest
    {
        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of an entry is correctly calculated")]
        public void TheSizeOfAnEntryIsCorrectlyCalculated()
        {
            Item item = new Item(GUILayout.Width(50), GUILayout.Height(50));

            item.CalcWidth();
            item.CalcHeight();

            Assert.Equal(item.contentWidth + Const.ItemPaddingLeft + Const.ItemPaddingRight + Const.ItemBorderLeft + Const.ItemBorderRight, item.rect.Width);
            Assert.Equal(item.contentHeight + Const.ItemPaddingTop + Const.ItemPaddingBottom + Const.ItemBorderTop + Const.ItemBorderBottom, item.rect.Height);
        }

        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of an empty vertical group is correctly calculated")]
        public void TheSizeOfAEmptyVerticalGroupIsCorrectlyCalculated()
        {
            Group group = new Group(true, GUILayout.Width(100), GUILayout.Height(200));

            group.CalcWidth();
            group.CalcHeight();

            Assert.Equal(100, group.rect.Width);
            Assert.Equal(200, group.rect.Height);
        }

        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of a vertical group that contains a single entry is correctly calculated")]
        public void TheSizeOfAVerticalGroupThatContainsASingleEntryIsCorrectlyCalculated()
        {
            Group group = new Group(true);
            Item item = new Item(GUILayout.Width(50), GUILayout.Height(50));
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();

            Assert.Equal(item.rect.Width + group.style.PaddingHorizontal + group.style.BorderHorizontal, group.rect.Width);
            Assert.Equal(item.rect.Height + group.style.PaddingVertical + group.style.BorderVertical, group.rect.Height);
        }

        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of a vertical group that contains multiple entries is correctly calculated")]
        public void TheSizeOfAVerticalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
        {
            Group group = new Group(true,GUILayout.Width(100), GUILayout.Height(200));
            Item[] items =
            {
                new Item(GUILayout.Width(10), GUILayout.Height(20)),
                new Item(GUILayout.Width(20), GUILayout.Height(30)),
                new Item(GUILayout.Width(30), GUILayout.Height(40)),
                new Item(GUILayout.Width(40), GUILayout.Height(50)),
                new Item(GUILayout.Width(50), GUILayout.Height(60)),
            };
            foreach (var item in items)
            {
                group.Add(item);
            }

            group.CalcWidth();
            group.CalcHeight();

            Assert.Equal(100, group.rect.Width);
            Assert.Equal(200, group.rect.Height);
        }

        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of a horizontal group that contains a single entry is correctly calculated")]
        public void TheSizeOfAHorizontalGroupThatContainsASingleEntryIsCorrectlyCalculated()
        {
            Group group = new Group(false);
            Item item = new Item(GUILayout.Width(50), GUILayout.Height(50));
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();

            Assert.Equal(item.rect.Width + group.style.PaddingHorizontal + group.style.BorderHorizontal, group.rect.Width);
            Assert.Equal(item.rect.Height + group.style.PaddingVertical + group.style.BorderVertical, group.rect.Height);
        }

        [Fact, Trait("Category", "rect & content size"), Trait("Description", "The size of a horizontal group that contains multiple entries is correctly calculated")]
        public void TheSizeOfAHorizontalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
        {
            Group group = new Group(false, GUILayout.Width(100), GUILayout.Height(200));
            Item item = new Item(GUILayout.Width(50), GUILayout.Height(50));
            for (int i = 0; i < 5; i++)
            {
                var newItem = item.Clone();
                newItem.contentWidth += i * 5;
                newItem.contentHeight += i * 8;
                group.Add(newItem);
            }

            group.CalcWidth();
            group.CalcHeight();

            Assert.Equal(100, group.rect.Width);
            Assert.Equal(200, group.rect.Height);
        }


        [Fact, Trait("Category", "layout"), Trait("Description", "Show an empty horizontal group")]
        public void ShowAnEmptyHorizontalGroup()
        {
            Group group = new Group(false,GUILayout.Width(100), GUILayout.Height(200));

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a horizontal group of 1 item")]
        public void ShowAHorizontalGroupOf1Item()
        {
            Group group = new Group(false, GUILayout.Width(100), GUILayout.Height(200));
            Item item = new Item(GUILayout.Width(50), GUILayout.Height(50));
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
            Group group = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            Item item = new Item(GUILayout.Width(50), GUILayout.Height(50));
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
            Group group = new Group(true, GUILayout.Width(100), GUILayout.Height(200));

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a vertical group of 1 items")]
        public void ShowAVerticalGroupOf1Items()
        {
            Group group = new Group(true, GUILayout.Width(100), GUILayout.Height(200));
            Item item = new Item(GUILayout.Width(50), GUILayout.Height(50));
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
            Group group = new Group(true, GUILayout.Width(100), GUILayout.Height(200));
            Item item = new Item(GUILayout.Width(50), GUILayout.Height(50));
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
            Group outterGroup = new Group(true, GUILayout.Width(600), GUILayout.Height(600));
            Group innerGroup0 = new Group(false, GUILayout.Width(200), GUILayout.Height(100));
            Group innerGroup1 = new Group(false, GUILayout.Width(200), GUILayout.Height(100));
            Group innerGroup2 = new Group(false, GUILayout.Width(200), GUILayout.Height(100));
            Item item = new Item(GUILayout.Width(50), GUILayout.Height(50));
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
            Group outterGroup = new Group(false, GUILayout.Width(600), GUILayout.Height(600));
            Group innerGroup0 = new Group(true, GUILayout.Width(100), GUILayout.Height(200));
            Group innerGroup1 = new Group(true, GUILayout.Width(100), GUILayout.Height(200));
            Group innerGroup2 = new Group(true, GUILayout.Width(100), GUILayout.Height(200));
            Item item = new Item(GUILayout.Width(50), GUILayout.Height(50));
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
            Group group1 = new Group(true, GUILayout.Width(600), GUILayout.Height(600));
            Group group2 = new Group(false, GUILayout.Width(200), GUILayout.Height(100));
            Group group3 = new Group(false, GUILayout.Width(200), GUILayout.Height(100));
            Group group4 = new Group(false, GUILayout.Width(200), GUILayout.Height(100));
            Group group5 = new Group(true, GUILayout.Width(100), GUILayout.Height(200));
            Item item1 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item2 = new Item(GUILayout.Width(50), GUILayout.Height(80));
            Item item3 = new Item(GUILayout.Width(80), GUILayout.Height(50));

            group1.Add(group3);
            group1.Add(group2);
            group1.Add(group4);

            group2.Add(item1.Clone());
            group2.Add(item2.Clone());
            group2.Add(item3.Clone());

            group3.Add(item1.Clone());
            group3.Add(group5);
            group3.Add(item1.Clone());

            group4.Add(item1.Clone());

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