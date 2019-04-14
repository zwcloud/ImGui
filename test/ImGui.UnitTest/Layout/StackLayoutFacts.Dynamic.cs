using ImGui.Layout;
using Xunit;

namespace ImGui.UnitTest.Layout
{
    public partial class StackLayoutFacts
    {
        public partial class TheGetRectMethod
        {

            [Fact]
            public void GetRectAfterRelayout1() // Add rect; Add rect then remove rect
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);

                // Frame 0
                layout.Begin();
                {
                    layout.GetRect(1, size, null, null); //add rect 1
                    //entry 1 active
                }
                layout.Layout();

                // Frame 1
                layout.Begin();
                {
                    var rect1 = layout.GetRect(1, size, null, null); //get rect 1

                    Assert.Equal(0, rect1.X);
                    Assert.Equal(0, rect1.Y);
                    Assert.Equal(size.Width, rect1.Width);
                    Assert.Equal(size.Height, rect1.Height);

                    layout.GetRect(2, size, null, null); //add rect 2
                    //entry 1 active
                    //entry 2 active
                    //layout dirty
                }
                layout.Layout();

                // Frame 2
                layout.Begin();
                {
                    // remove rect 1 (works on next frame)
                    var rect2 = layout.GetRect(2, size, null, null); //get rect 2

                    Assert.Equal(0, rect2.X);
                    Assert.Equal(300 + GUIStyle.Default.CellSpacingVertical, rect2.Y);
                    Assert.Equal(size.Width, rect2.Width);
                    Assert.Equal(size.Height, rect2.Height);

                    //entry 1 active
                    //entry 2 active
                }
                layout.Layout();

                // Frame 3
                layout.Begin();
                {
                    // rect 1 removed
                    var rect2 = layout.GetRect(2, size, null, null); //get rect 2

                    Assert.Equal(0, rect2.X);
                    Assert.Equal(0, rect2.Y);
                    Assert.Equal(size.Width, rect2.Width);
                    Assert.Equal(size.Height, rect2.Height);

                    //entry 2 active
                }
                layout.Layout();
            }

            [Fact]
            public void GetRectAfterRelayout2() //Add two group, then remove first group
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);

                // Frame 0
                layout.Begin();
                {
                    layout.BeginLayoutGroup(1, true); //add group 1
                    layout.GetRect(1, size); //add rect 1
                    layout.EndLayoutGroup();
                    layout.BeginLayoutGroup(2, true); //add group 2
                    layout.GetRect(2, size); //add rect 2
                    layout.EndLayoutGroup();
                }
                layout.Layout();

                // Frame 1
                layout.Begin();
                {
                    //remove group 1
                    layout.BeginLayoutGroup(2, true); //get group 2
                    layout.GetRect(2, size); //get rect 2
                    layout.EndLayoutGroup();
                }
                layout.Layout();

                // Frame 2
                layout.Begin();
                {
                    //group 1 removed
                    layout.BeginLayoutGroup(2, true); //get group 2
                    var rect2 = layout.GetRect(2, size); //get rect 3
                    layout.EndLayoutGroup();

                    Assert.Equal(0, rect2.X);
                    Assert.Equal(0, rect2.Y);
                    Assert.Equal(size.Width, rect2.Width);
                    Assert.Equal(size.Height, rect2.Height);
                }
                layout.Layout();
            }

            [Fact]
            public void GetRectAfterRelayout3()
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);

                // Frame 0
                layout.Begin();
                {
                    layout.BeginLayoutGroup(1, true); //add group 1
                    layout.GetRect(1, size); //add rect 1
                    layout.EndLayoutGroup();
                    layout.BeginLayoutGroup(2, true); //add group 2
                    layout.GetRect(2, size); //add rect 2
                    layout.EndLayoutGroup();
                }
                layout.Layout();

                // Frame 1
                layout.Begin();
                {
                    //remove group 1
                    layout.BeginLayoutGroup(2, true); //get group 2
                    layout.GetRect(2, size); //get rect 2
                    layout.EndLayoutGroup();
                }
                layout.Layout();

                // Frame 2
                layout.Begin();
                {
                    //group 1 removed
                    layout.BeginLayoutGroup(2, true); //get group 2
                    var rect2 = layout.GetRect(2, size); //get rect 3
                    layout.EndLayoutGroup();

                    Assert.Equal(0, rect2.X);
                    Assert.Equal(0, rect2.Y);
                    Assert.Equal(size.Width, rect2.Width);
                    Assert.Equal(size.Height, rect2.Height);
                }
                layout.Layout();

                // Frame 3
                layout.Begin();
                {
                    layout.BeginLayoutGroup(1, true); //add group 1
                    layout.GetRect(1, size); //add rect 1
                    layout.EndLayoutGroup();
                    layout.BeginLayoutGroup(2, true); //get group 2
                    var rect2 = layout.GetRect(2, size); //get rect 3
                    layout.EndLayoutGroup();

                    Assert.Equal(0, rect2.X);
                    Assert.Equal(0, rect2.Y);
                    Assert.Equal(size.Width, rect2.Width);
                    Assert.Equal(size.Height, rect2.Height);
                }
                layout.Layout();

                // Frame 4
                layout.Begin();
                {
                    layout.BeginLayoutGroup(1, true); //get group 1
                    var rect1 = layout.GetRect(1, size); //get rect 1
                    layout.EndLayoutGroup();
                    layout.BeginLayoutGroup(2, true); //get group 2
                    var rect2 = layout.GetRect(2, size); //get rect 3
                    layout.EndLayoutGroup();

                    Assert.Equal(0, rect1.X);
                    Assert.Equal(0, rect1.Y);
                    Assert.Equal(size.Width, rect1.Width);
                    Assert.Equal(size.Height, rect1.Height);
                }
                layout.Layout();
            }

            [Fact]
            public void GetRectAfterRelayout4()
            {
                var layout = new StackLayout(0, new Size(800, 600));
                var size = new Size(200, 300);

                // Frame 0
                layout.Begin();
                {
                    layout.GetRect(1, size);

                    layout.BeginLayoutGroup(1, true);
                    layout.GetRect(2, size);
                    layout.EndLayoutGroup();

                    layout.GetRect(3, size);

                    layout.BeginLayoutGroup(2, true);
                    layout.GetRect(4, size);
                    layout.EndLayoutGroup();
                }
                layout.Layout();

                layout.Begin();
                {
                    layout.GetRect(1, size);

                    layout.BeginLayoutGroup(1, true);
                    layout.GetRect(2, size);
                    layout.EndLayoutGroup();

                    layout.GetRect(3, size);

                    layout.BeginLayoutGroup(2, true);
                    layout.GetRect(4, size);
                    layout.EndLayoutGroup();
                }
                layout.Layout();
            }
        }
    }
}