using System.Collections.Generic;
using ImGui;
using System;
using Xunit;

namespace Test
{
    public class StretchedSizeTest
    {
        [Fact, Trait("Category", "layout"), Trait("Description", "Show a horizontal group of 1 item")]
        public void ShowAHorizontalGroupOf1Item()
        {
            Group group = new Group(false, GUILayout.Width(600));
            Item item = new Item(GUILayout.ExpandWidth(true), GUILayout.Height(50));
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
            Group group = new Group(false, GUILayout.Width(600));
            Item item1 = new Item(GUILayout.ExpandWidth(true), GUILayout.Height(50));
            Item item2 = new Item(GUILayout.ExpandWidth(true), GUILayout.Height(60));
            Item item3 = new Item(GUILayout.ExpandWidth(true), GUILayout.Height(30));
            group.Add(item1);
            group.Add(item2);
            group.Add(item3);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a horizontal group of 3 items with different stretch factors")]
        public void ShowAHorizontalGroupOf3ItemsWithDifferentStretchFactors()
        {
            Group group = new Group(false, GUILayout.Width(600));
            Item item1 = new Item(GUILayout.StretchWidth(1), GUILayout.Height(50));
            Item item2 = new Item(GUILayout.StretchWidth(2), GUILayout.Height(60));
            Item item3 = new Item(GUILayout.StretchWidth(1), GUILayout.Height(30));
            group.Add(item1);
            group.Add(item2);
            group.Add(item3);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }


        [Fact, Trait("Category", "layout"), Trait("Description", "Show a vertical group of 1 item")]
        public void ShowAVerticalGroupOf1Item()
        {
            Group group = new Group(true, GUILayout.Height(600));
            Item item = new Item(GUILayout.ExpandHeight(true), GUILayout.Width(50));
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
            Group group = new Group(true, GUILayout.Height(600));
            Item item1 = new Item(GUILayout.ExpandHeight(true), GUILayout.Width(50));
            Item item2 = new Item(GUILayout.ExpandHeight(true), GUILayout.Width(60));
            Item item3 = new Item(GUILayout.ExpandHeight(true), GUILayout.Width(30));
            group.Add(item1);
            group.Add(item2);
            group.Add(item3);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a vertical group of 3 items with different stretch factors")]
        public void ShowAVerticalGroupOf3ItemsWithDifferentStretchFactors()
        {
            Group group = new Group(true, GUILayout.Height(600));
            Item item1 = new Item(GUILayout.StretchHeight(1), GUILayout.Width(50));
            Item item2 = new Item(GUILayout.StretchHeight(2), GUILayout.Width(60));
            Item item3 = new Item(GUILayout.StretchHeight(1), GUILayout.Width(30));
            group.Add(item1);
            group.Add(item2);
            group.Add(item3);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a two-layer group. Case 1: inner group horizontally stretched.")]
        public void ShowATwoLayerGroup1()
        {
            Group group1 = new Group(true, GUILayout.Width(400), GUILayout.Height(400));
            Group group2 = new Group(false, GUILayout.ExpandWidth(true));
            group1.Add(group2);

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a two-layer group. Case 2: inner group vertically stretched.")]
        public void ShowATwoLayerGroup2()
        {
            Group group1 = new Group(true, GUILayout.Width(400), GUILayout.Height(400));
            Group group2 = new Group(false, GUILayout.ExpandHeight(true));
            group1.Add(group2);

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a two-layer group. Case 3: inner group horizontally and vertically stretched.")]
        public void ShowATwoLayerGroup3()
        {
            Group group1 = new Group(true, GUILayout.Width(400), GUILayout.Height(400));
            Group group2 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            group1.Add(group2);

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a two-layer group. Case 4: multiple inner group horizontally stretched.")]
        public void ShowATwoLayerGroup4()
        {
            Group group1 = new Group(true, GUILayout.Width(400), GUILayout.Height(400));
            Group group2 = new Group(false, GUILayout.ExpandWidth(true));
            Group group3 = new Group(false, GUILayout.ExpandWidth(true));
            group1.Add(group2);
            group1.Add(group3);

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a two-layer group. Case 5: multiple inner group vertically stretched.")]
        public void ShowATwoLayerGroup5()
        {
            Group group1 = new Group(true, GUILayout.Width(400), GUILayout.Height(400));
            Group group2 = new Group(false, GUILayout.ExpandHeight(true));
            Group group3 = new Group(false, GUILayout.ExpandHeight(true));
            group1.Add(group2);
            group1.Add(group3);

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a two-layer group. Case 6: multiple inner group horizontally and vertically stretched.")]
        public void ShowATwoLayerGroup6()
        {
            Group group1 = new Group(true, GUILayout.Width(400), GUILayout.Height(400));
            Group group2 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group3 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            group1.Add(group2);
            group1.Add(group3);

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }


        [Fact, Trait("Category", "layout"), Trait("Description", "Show a three-layer group.")]
        public void ShowAThreeLayerGroup()
        {
            // layer 1
            Group group1 = new Group(true, GUILayout.Width(400), GUILayout.Height(400));

            // layer 2
            Group group2 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group3 = new Group(true, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group4 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // layer3
            Group group5 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group6 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group7 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group8 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group9 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group10 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group11 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group12 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            Group group13 = new Group(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            group1.Add(group2);
            group1.Add(group3);
            group1.Add(group4);

            group2.Add(group5);
            group2.Add(group6);
            group2.Add(group7);
            group3.Add(group8);
            group3.Add(group9);
            group3.Add(group10);
            group4.Add(group11);
            group4.Add(group12);
            group4.Add(group13);

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a group with no space to hold the child.")]
        public void ShowAGroupWithNoSpaceToHoldTheChild()
        {
            Group group = new Group(false, GUILayout.Width(400), GUILayout.Height(30));// content box height of this group is 0
            Item item1 = new Item(GUILayout.Width(20), GUILayout.Height(20));
            group.Add(item1);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [Fact, Trait("Category", "layout"), Trait("Description", "Show a three-layer group.")]
        public void ShowARealCase()
        {
            Style compactStyle = StyleTestEx.GenCompactStyle();

            Group window = new Group(true, GUILayout.Width(400), GUILayout.Height(600));
            window.style = compactStyle;

            Group titleBar = new Group(false, GUILayout.ExpandWidth(true), GUILayout.Height(30));
            titleBar.style = compactStyle;
            {
                Item icon = new Item(GUILayout.Width(20), GUILayout.Height(20));
                Item title = new Item(GUILayout.Width(100), GUILayout.ExpandHeight(true));
                Item space = new Item(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                Item minimizeButton = new Item(GUILayout.Width(30), GUILayout.Height(30));
                Item maximizeButton = new Item(GUILayout.Width(30), GUILayout.Height(30));
                Item closeButton = new Item(GUILayout.Width(30), GUILayout.Height(30));
                titleBar.Add(icon);
                titleBar.Add(title);
                titleBar.Add(space);
                titleBar.Add(minimizeButton);
                titleBar.Add(maximizeButton);
                titleBar.Add(closeButton);
            }

            Group menuBar = new Group(false, GUILayout.ExpandWidth(true), GUILayout.Height(30));
            menuBar.style = compactStyle;
            {
                Item menuItem0 = new Item(GUILayout.Width(40), GUILayout.ExpandHeight(true));
                Item menuItem1 = new Item(GUILayout.Width(40), GUILayout.ExpandHeight(true));
                Item menuItem2 = new Item(GUILayout.Width(40), GUILayout.ExpandHeight(true));
                Item menuItem3 = new Item(GUILayout.Width(40), GUILayout.ExpandHeight(true));
                menuBar.Add(menuItem0);
                menuBar.Add(menuItem1);
                menuBar.Add(menuItem2);
                menuBar.Add(menuItem3);
            }

            Group clientArea = new Group(true, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            clientArea.style = compactStyle;

            Group statusBar = new Group(false, GUILayout.ExpandWidth(true), GUILayout.Height(30));
            {
                Item statusItem0 = new Item(GUILayout.Width(40), GUILayout.ExpandHeight(true));
                Item statusItem1 = new Item(GUILayout.Width(40), GUILayout.ExpandHeight(true));
                Item space = new Item(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                Item statusItem2 = new Item(GUILayout.Width(60), GUILayout.ExpandHeight(true));
                Item statusItem3 = new Item(GUILayout.Width(30), GUILayout.ExpandHeight(true));
                statusBar.Add(statusItem0);
                statusBar.Add(statusItem1);
                statusBar.Add(space);
                statusBar.Add(statusItem2);
                statusBar.Add(statusItem3);
            }
            statusBar.style = compactStyle;

            window.Add(titleBar);
            window.Add(menuBar);
            window.Add(clientArea);
            window.Add(statusBar);

            window.CalcWidth();
            window.CalcHeight();
            window.SetX(0);
            window.SetY(0);

            window.ShowResult();
        }
    }

    public static class StyleTestEx
    {
        internal static Style GenCompactStyle()
        {
            return Style.Make(new[]{
                new StyleModifier{Name = "BorderTop", Value = 0},
                new StyleModifier{Name = "BorderRight", Value = 0},
                new StyleModifier{Name = "BorderBottom", Value = 0},
                new StyleModifier{Name = "BorderLeft", Value = 0},

                new StyleModifier{Name = "PaddingTop", Value = 0},
                new StyleModifier{Name = "PaddingRight", Value = 0},
                new StyleModifier{Name = "PaddingBottom", Value = 0},
                new StyleModifier{Name = "PaddingLeft", Value = 0}
            });
        }
    }
}