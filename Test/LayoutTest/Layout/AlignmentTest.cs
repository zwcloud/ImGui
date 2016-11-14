using System;
using ImGui;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class AlignmentTest
    {
        [TestMethod]
        public void Show3HorizontalGroupOf1ItemWithDifferentAlignment()
        {
            Group group = new Group(true);

            Group group1 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            group1.style.AlignmentVertical = Alignment.Start;
            Group group2 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            group2.style.AlignmentVertical = Alignment.Center;
            Group group3 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            group3.style.AlignmentVertical = Alignment.End;

            Item item1 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item2 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item3 = new Item(GUILayout.Width(50), GUILayout.Height(50));

            group1.Add(item1);
            group2.Add(item2);
            group3.Add(item3);
            group.Add(group1);
            group.Add(group2);
            group.Add(group3);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod]
        public void Show3VerticalGroupOf1ItemWithDifferentAlignment()
        {
            Group group = new Group(false);

            Group group1 = new Group(true, GUILayout.Width(200), GUILayout.Height(200));
            group1.style.AlignmentHorizontal = Alignment.Start;
            Group group2 = new Group(true, GUILayout.Width(200), GUILayout.Height(200));
            group2.style.AlignmentHorizontal = Alignment.Center;
            Group group3 = new Group(true, GUILayout.Width(200), GUILayout.Height(200));
            group3.style.AlignmentHorizontal = Alignment.End;

            Item item1 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item2 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item3 = new Item(GUILayout.Width(50), GUILayout.Height(50));

            group1.Add(item1);
            group2.Add(item2);
            group3.Add(item3);
            group.Add(group1);
            group.Add(group2);
            group.Add(group3);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod]
        public void Show9GroupOf1ItemWithDifferentAlignment()
        {
            Group group = new Group(true);

            Group groupRow1 = new Group(false);
            Group groupRow2 = new Group(false);
            Group groupRow3 = new Group(false);

            Group group1 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            Group group2 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            Group group3 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            Group group4 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            Group group5 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            Group group6 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            Group group7 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            Group group8 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));
            Group group9 = new Group(false, GUILayout.Width(200), GUILayout.Height(200));

            Item item1 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item2 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item3 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item4 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item5 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item6 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item7 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item8 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item9 = new Item(GUILayout.Width(50), GUILayout.Height(50));

            group1.style.AlignmentHorizontal = Alignment.Start;
            group1.style.AlignmentVertical = Alignment.Start;
            group2.style.AlignmentHorizontal = Alignment.Center;
            group2.style.AlignmentVertical = Alignment.Start;
            group3.style.AlignmentHorizontal = Alignment.End;
            group3.style.AlignmentVertical = Alignment.Start;

            group4.style.AlignmentHorizontal = Alignment.Start;
            group4.style.AlignmentVertical = Alignment.Center;
            group5.style.AlignmentHorizontal = Alignment.Center;
            group5.style.AlignmentVertical = Alignment.Center;
            group6.style.AlignmentHorizontal = Alignment.End;
            group6.style.AlignmentVertical = Alignment.Center;

            group7.style.AlignmentHorizontal = Alignment.Start;
            group7.style.AlignmentVertical = Alignment.End;
            group8.style.AlignmentHorizontal = Alignment.Center;
            group8.style.AlignmentVertical = Alignment.End;
            group9.style.AlignmentHorizontal = Alignment.End;
            group9.style.AlignmentVertical = Alignment.End;

            group1.Add(item1);
            group2.Add(item2);
            group3.Add(item3);
            group4.Add(item4);
            group5.Add(item5);
            group6.Add(item6);
            group7.Add(item7);
            group8.Add(item8);
            group9.Add(item9);

            groupRow1.Add(group1);
            groupRow1.Add(group2);
            groupRow1.Add(group3);
            groupRow2.Add(group4);
            groupRow2.Add(group5);
            groupRow2.Add(group6);
            groupRow3.Add(group7);
            groupRow3.Add(group8);
            groupRow3.Add(group9);

            group.Add(groupRow1);
            group.Add(groupRow2);
            group.Add(groupRow3);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod]
        public void Show5HorizontalGroupWithDifferentAlignment()
        {
            Group group = new Group(true);

            Group group1 = new Group(false, GUILayout.Width(600), GUILayout.Height(150));
            Group group2 = new Group(false, GUILayout.Width(600), GUILayout.Height(150));
            Group group3 = new Group(false, GUILayout.Width(600), GUILayout.Height(150));
            Group group4 = new Group(false, GUILayout.Width(600), GUILayout.Height(150));
            Group group5 = new Group(false, GUILayout.Width(600), GUILayout.Height(150));

            Item item1 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item2 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item3 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item4 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item5 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item6 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item7 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item8 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item9 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item10 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item11 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item12 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item13 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item14 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item15 = new Item(GUILayout.Width(50), GUILayout.Height(50));

            group1.style.AlignmentHorizontal = Alignment.Start;
            group2.style.AlignmentHorizontal = Alignment.Center;
            group3.style.AlignmentHorizontal = Alignment.End;
            group4.style.AlignmentHorizontal = Alignment.SpaceAround;
            group5.style.AlignmentHorizontal = Alignment.SpaceBetween;

            group1.Add(item1);
            group1.Add(item2);
            group1.Add(item3);
            group2.Add(item4);
            group2.Add(item5);
            group2.Add(item6);
            group3.Add(item7);
            group3.Add(item8);
            group3.Add(item9);
            group4.Add(item10);
            group4.Add(item11);
            group4.Add(item12);
            group5.Add(item13);
            group5.Add(item14);
            group5.Add(item15);

            group.Add(group1);
            group.Add(group2);
            group.Add(group3);
            group.Add(group4);
            group.Add(group5);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod]
        public void Show5VerticalGroupWithDifferentAlignment()
        {
            Group group = new Group(false);

            Group group1 = new Group(true, GUILayout.Width(150), GUILayout.Height(600));
            Group group2 = new Group(true, GUILayout.Width(150), GUILayout.Height(600));
            Group group3 = new Group(true, GUILayout.Width(150), GUILayout.Height(600));
            Group group4 = new Group(true, GUILayout.Width(150), GUILayout.Height(600));
            Group group5 = new Group(true, GUILayout.Width(150), GUILayout.Height(600));

            Item item1 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item2 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item3 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item4 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item5 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item6 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item7 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item8 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item9 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item10 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item11 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item12 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item13 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item14 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            Item item15 = new Item(GUILayout.Width(50), GUILayout.Height(50));

            group1.style.AlignmentVertical = Alignment.Start;
            group2.style.AlignmentVertical = Alignment.Center;
            group3.style.AlignmentVertical = Alignment.End;
            group4.style.AlignmentVertical = Alignment.SpaceAround;
            group5.style.AlignmentVertical = Alignment.SpaceBetween;

            group1.Add(item1);
            group1.Add(item2);
            group1.Add(item3);
            group2.Add(item4);
            group2.Add(item5);
            group2.Add(item6);
            group3.Add(item7);
            group3.Add(item8);
            group3.Add(item9);
            group4.Add(item10);
            group4.Add(item11);
            group4.Add(item12);
            group5.Add(item13);
            group5.Add(item14);
            group5.Add(item15);

            group.Add(group1);
            group.Add(group2);
            group.Add(group3);
            group.Add(group4);
            group.Add(group5);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }
    }
}
