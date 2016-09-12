using System.Collections.Generic;
using System.Diagnostics;
using Cairo;
using ImGui;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test
{
    public class Item : ICloneable
    {
        public Rect rect;//border-box
        public double contentWidth;//exact content width, calculated from content and style
        public double contentHeight;//exact content height, calculated from content and style
        public double minWidth;//minimum width of content-box
        public double maxWidth;//maximum width of content-box
        public double minHeight;//minimum height of content-box
        public double maxHeight;//maximum height of content-box
        public int horizontalStretchFactor;//horizontal stretch factor
        public int verticalStretchFactor;//vertical stretch factor

        public Item Clone() { return (Item)this.MemberwiseClone(); }
        object ICloneable.Clone() { return Clone(); }
    }

    public class Group : Item
    {
        public bool isVertical;
        public bool isClipped;
        public List<Item> entries = new List<Item>();

        public Style style = Skin.current.Box;

        public Group(Rect rect, bool isVertical)
        {
            this.rect = rect;
            this.isVertical = isVertical;
        }

        public void CalcWidthAndX()
        {
            if (this.entries.Count == 0)
            {
                this.rect.Width = 2;
                return;
            }

            if(this.isVertical)
            {
                var X = this.rect.X;
                var W = this.rect.Width;
                var childCount = this.entries.Count;
                var childX = X + style.PaddingLeft + style.BorderLeft;
                var childWidth = W - style.PaddingHorizontal - style.BorderHorizontal;
                for (int i = 0; i < childCount; ++i)
                {
                    var entry = this.entries[i];
                    entry.rect.X = childX;
                    entry.rect.Width = childWidth;

                    //Recursively calc possible inner group
                    var innerGroup = entry as Group;
                    if(innerGroup!=null)
                    {
                        innerGroup.CalcWidthAndX();
                    }
                }
            }
            else
            {
                var X = this.rect.X;
                var W = this.rect.Width;
                var childCount = this.entries.Count;
                var nextChildX = X + style.BorderLeft + style.PaddingLeft;
                var partCount = 0;
                var non_partWidth = 0d;
                for (var i = 0; i < childCount; ++i)
                {
                    var entry = this.entries[i];
                    if (entry.horizontalStretchFactor==0)
                    {
                        non_partWidth += entry.contentWidth;
                    }
                    else
                    {
                        partCount += entry.horizontalStretchFactor;
                    }
                }
                if (partCount != 0)// has stretched entry
                {
                    var partWidth = (W
                        - style.BorderHorizontal
                        - style.PaddingHorizontal
                        - (childCount - 1) * style.CellingSpacingHorizontal
                        - non_partWidth
                        ) / partCount;
                    for (var i = 0; i < childCount; ++i)
                    {
                        var entry = this.entries[i];
                        var childWidth = 0d;
                        if (entry.horizontalStretchFactor == 0)
                        {
                            childWidth = entry.contentWidth;
                        }
                        else
                        {
                            childWidth = partWidth * entry.horizontalStretchFactor;
                        }
                        childWidth = MathEx.Clamp(childWidth, entry.minWidth, entry.maxWidth);
                        entry.rect.X = nextChildX;
                        entry.rect.Width = childWidth;
                        nextChildX += style.CellingSpacingHorizontal + childWidth;

                        //Recursively calc possible inner group
                        var innerGroup = entry as Group;
                        if (innerGroup != null)
                        {
                            innerGroup.CalcWidthAndX();
                        }
                    }
                }
                else// no stretched entry
                {
                    for (var i = 0; i < childCount; ++i)
                    {
                        var entry = this.entries[i];
                        var childWidth = 0d;
                        childWidth = entry.contentWidth;
                        childWidth = MathEx.Clamp(childWidth, entry.minWidth, entry.maxWidth);
                        entry.rect.X = nextChildX;
                        entry.rect.Width = childWidth;
                        nextChildX += style.CellingSpacingHorizontal + childWidth;

                        //Recursively calc possible inner group
                        var innerGroup = entry as Group;
                        if (innerGroup != null)
                        {
                            innerGroup.CalcWidthAndX();
                        }
                    }
                }
            }
        }

        public void CalcHeightAndY()
        {
            if (this.entries.Count == 0)
            {
                this.rect.Height = 2;
                return;
            }

            if (this.isVertical)
            {
                var Y = this.rect.Y;
                var H = this.rect.Height;
                var childCount = this.entries.Count;
                var nextChildY = Y + style.BorderTop + style.PaddingTop;
                var partCount = 0;
                var non_partHeight = 0d;
                for (var i = 0; i < childCount; ++i)
                {
                    var entry = this.entries[i];
                    if (entry.verticalStretchFactor == 0)
                    {
                        non_partHeight += entry.contentHeight;
                    }
                    else
                    {
                        partCount += entry.verticalStretchFactor;
                    }
                }
                if (partCount != 0)// has stretched entry
                {
                    var partHeight = (H
                        - style.BorderVertical
                        - style.PaddingVertical
                        - (childCount - 1) * style.CellingSpacingVertical
                        - non_partHeight
                        ) / partCount;
                    for (var i = 0; i < childCount; ++i)
                    {
                        var entry = this.entries[i];
                        var childHeight = 0d;
                        if (entry.verticalStretchFactor == 0)
                        {
                            childHeight = entry.contentHeight;
                        }
                        else
                        {
                            childHeight = partHeight * entry.verticalStretchFactor;
                        }
                        childHeight = MathEx.Clamp(childHeight, entry.minHeight, entry.maxHeight);
                        entry.rect.Y = nextChildY;
                        entry.rect.Height = childHeight;
                        nextChildY += style.CellingSpacingVertical + childHeight;

                        //Recursively calc possible inner group
                        var innerGroup = entry as Group;
                        if (innerGroup != null)
                        {
                            innerGroup.CalcHeightAndY();
                        }
                    }
                }
                else// no stretched entry
                {
                    for (var i = 0; i < childCount; ++i)
                    {
                        var entry = this.entries[i];
                        var childHeight = entry.contentHeight;
                        childHeight = MathEx.Clamp(childHeight, entry.minHeight, entry.maxHeight);
                        entry.rect.Y = nextChildY;
                        entry.rect.Height = childHeight;
                        nextChildY += style.CellingSpacingVertical + childHeight;

                        //Recursively calc possible inner group
                        var innerGroup = entry as Group;
                        if (innerGroup != null)
                        {
                            innerGroup.CalcHeightAndY();
                        }
                    }
                }
            }
            else
            {
                var Y = this.rect.Y;
                var H = this.rect.Height;
                var childCount = this.entries.Count;
                var childY = Y + style.PaddingTop + style.BorderTop;
                var childHeight = H - style.PaddingVertical - style.BorderVertical;
                for (int i = 0; i < childCount; ++i)
                {
                    var entry = this.entries[i];
                    entry.rect.Y = childY;
                    entry.rect.Height = childHeight;

                    //Recursively calc possible inner group
                    var innerGroup = entry as Group;
                    if (innerGroup != null)
                    {
                        innerGroup.CalcHeightAndY();
                    }
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
                if(entry.horizontalStretchFactor != 0 || entry.verticalStretchFactor != 0)
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
                if(innerGroup!=null)
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
            var surface = CairoEx.BuildSurface((int)rect.Width, (int)rect.Height, CairoEx.ColorMetal, Format.Rgb24);
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

    [TestClass]
    public class LayoutTest
    {
        [TestMethod]
        public void TestHorizontalLayout_HorizontalOnly_HasStrechedItem()
        {
            Item a = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 2,
            };

            Item b = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 3,
            };

            Item c = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 1,
            };

            Item d = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
            };

            Group group = new Group(new Rect(600, 240), false);
            group.entries.Add(a);
            group.entries.Add(b);
            group.entries.Add(c);
            group.entries.Add(d);
            group.CalcWidthAndX();

            group.ShowResult();
        }

        [TestMethod]
        public void TestHorizontalLayout_HorizontalOnly_NoStrechedItem()
        {
            Item a = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
            };

            Item b = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
            };

            Item c = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
            };

            Item d = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
            };

            Group group = new Group(new Rect(600, 240), false);
            group.entries.Add(a);
            group.entries.Add(b);
            group.entries.Add(c);
            group.entries.Add(d);
            group.CalcWidthAndX();

            group.ShowResult();
        }

        [TestMethod]
        public void TestVerticalLayout_VerticalOnly_HasStrechedItem()
        {
            Item a = new Item
            {
                rect = new Rect(20, 30),
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 2,
            };

            Item b = new Item
            {
                rect = new Rect(20, 30),
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 1,
            };

            Item c = new Item
            {
                rect = new Rect(20, 30),
                minHeight = 50,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 1,
            };

            Item d = new Item
            {
                rect = new Rect(20, 30),
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Group group = new Group(new Rect(600, 240), true);
            group.entries.Add(a);
            group.entries.Add(b);
            group.entries.Add(c);
            group.entries.Add(d);
            group.CalcHeightAndY();

            group.ShowResult();
        }

        [TestMethod]
        public void TestVerticalLayout_VerticalOnly_NoStrechedItem()
        {
            Item a = new Item
            {
                rect = new Rect(20, 30),
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item b = new Item
            {
                rect = new Rect(20, 30),
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item c = new Item
            {
                rect = new Rect(20, 30),
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item d = new Item
            {
                rect = new Rect(20, 30),
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Group group = new Group(new Rect(600, 240), true);
            group.entries.Add(a);
            group.entries.Add(b);
            group.entries.Add(c);
            group.entries.Add(d);
            group.CalcHeightAndY();

            group.ShowResult();
        }

        [TestMethod]
        public void TestHorizontalLayout_NoStrechedItem()
        {
            Item a = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item b = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item c = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item d = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Group group = new Group(new Rect(600, 600), false);
            group.entries.Add(a);
            group.entries.Add(b);
            group.entries.Add(c);
            group.entries.Add(d);
            group.CalcHeightAndY();
            group.CalcWidthAndX();

            group.ShowResult();
        }

        [TestMethod]
        public void TestVerticalLayout_NoStrechedItem()
        {
            Item a = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item b = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item c = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item d = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Group group = new Group(new Rect(600, 600), true);
            group.entries.Add(a);
            group.entries.Add(b);
            group.entries.Add(c);
            group.entries.Add(d);
            group.CalcHeightAndY();
            group.CalcWidthAndX();

            group.ShowResult();
        }

        [TestMethod]
        public void TestEmbedLayout1()
        {
            Item a = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Group b = new Group(new Rect(100, 100), false)
            {
                minWidth = 1,
                contentWidth = 100,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 100,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item c = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item d = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item e = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            b.entries.Add(c);
            b.entries.Add(d);
            b.entries.Add(e);

            Group group = new Group(new Rect(600, 600), true);
            group.entries.Add(a);
            group.entries.Add(b);
            group.CalcHeightAndY();
            group.CalcWidthAndX();

            group.ShowResult();
        }

        [TestMethod]
        public void TestEmbedLayout2()
        {
            Group a = new Group(new Rect(100, 100), false)
            {
                minWidth = 1,
                contentWidth = 100,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 100,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Group b = new Group(new Rect(100, 100), false)
            {
                minWidth = 1,
                contentWidth = 100,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 100,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item c = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item d = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item e = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            a.entries.Add(c);
            b.entries.Add(d);
            b.entries.Add(e);

            Group group = new Group(new Rect(600, 600), true);
            group.entries.Add(a);
            group.entries.Add(b);
            group.CalcWidthAndX();
            group.CalcHeightAndY();

            group.ShowResult();
        }

        [TestMethod]
        public void TestEmbedLayout_Clipped()
        {
            Group a = new Group(new Rect(100, 100), false)
            {
                minWidth = 1,
                contentWidth = 100,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 100,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Group b = new Group(new Rect(100, 100), false)
            {
                minWidth = 1,
                contentWidth = 100,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 100,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Group c = new Group(new Rect(100, 100), false)
            {
                minWidth = 1,
                contentWidth = 100,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 100,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item d = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            Item e = new Item
            {
                rect = new Rect(20, 30),
                minWidth = 1,
                contentWidth = 50,
                maxWidth = 1000,
                horizontalStretchFactor = 0,
                minHeight = 1,
                contentHeight = 50,
                maxHeight = 1000,
                verticalStretchFactor = 0,
            };

            a.entries.Add(e.Clone());
            a.entries.Add(e.Clone());
            a.entries.Add(e.Clone());

            b.entries.Add(e.Clone());
            b.entries.Add(e.Clone());
            b.entries.Add(e.Clone());

            c.entries.Add(e.Clone());
            c.entries.Add(e.Clone());
            c.entries.Add(e.Clone());

            a.entries.Add(c);

            Group group = new Group(new Rect(600, 600), true);
            group.isClipped = true;
            group.entries.Add(a);
            group.entries.Add(b);
            group.CalcWidthAndX();
            group.CalcHeightAndY();

            group.ShowResult();
        }
    }
}
