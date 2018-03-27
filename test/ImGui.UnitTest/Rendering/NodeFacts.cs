using System;
using ImGui.Common.Primitive;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class NodeFacts
    {
        public class TheLayoutMethod
        {
            [Fact]
            public void ShowANodeWithTwoChildren() // Add rect; Add rect then remove rect
            {
                Node a = new Node();
                a.Id = 1;
                a.Rect = new Rect(0, 0, 300, 400);
                a.AttachLayoutGroup(true);
                
                Node b = new Node();
                b.Id = 2;
                b.Rect = new Rect(0, 0, 100, 100);
                b.AttachLayoutEntry(new Size(100, 100));
                
                Node c = new Node();
                c.Id = 3;
                c.Rect = new Rect(0, 0, 100, 200);
                c.AttachLayoutEntry(new Size(100, 200));

                a.Add(b);
                a.Add(c);

                a.Layout();

                DrawNode(a);
            }

            [Fact]
            public void ShowAHorizontalGroupOf3ItemsWithDifferentStretchFactors()
            {
                Node group = new Node(); group.AttachLayoutGroup(false, GUILayout.Width(600));
                Node item1 = new Node(); item1.AttachLayoutEntry(new Size(20, 10), GUILayout.StretchWidth(1).Height(50));
                Node item2 = new Node(); item2.AttachLayoutEntry(new Size(20, 10), GUILayout.StretchWidth(2).Height(60));
                Node item3 = new Node(); item3.AttachLayoutEntry(new Size(20, 10), GUILayout.StretchWidth(1).Height(30));
                group.Add(item1);
                group.Add(item2);
                group.Add(item3);
                
                group.Layout();

                DrawNode(group);
            }

            [Fact]
            void ShowAThreeLayerGroup()
            {
                // layer 1
                Node group1 = new Node(); group1.AttachLayoutGroup(true, GUILayout.Width(400).Height(400));

                // layer 2
                Node group2 = new Node(); group2.AttachLayoutGroup(false, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group3 = new Node(); group3.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group4 = new Node(); group4.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));

                // layer3
                Node group5 =  new Node(); group5.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group6 =  new Node(); group6.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group7 =  new Node(); group7.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group8 =  new Node(); group8.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group9 =  new Node(); group9.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group10 = new Node(); group10.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group11 = new Node(); group11.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group12 = new Node(); group12.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));
                Node group13 = new Node(); group13.AttachLayoutGroup(true, GUILayout.ExpandWidth(true).ExpandHeight(true));

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
                
                group1.Layout();

                DrawNode(group1);
            }

            private void DrawNode(Node node, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
            {
                using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, (int)node.Rect.Width, (int)node.Rect.Height))
                using (Cairo.Context context = new Cairo.Context(surface))
                {
                    Draw(context, node);

                    string outputPath = "D:\\my\\ImGui.UnitTest.Output";
                    if (!System.IO.Directory.Exists(outputPath))
                    {
                        System.IO.Directory.CreateDirectory(outputPath);
                    }

                    string filePath = outputPath + "\\" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff_") + surface.GetHashCode() + memberName + ".png";
                    surface.WriteToPng(filePath);
                    Util.OpenImage(filePath);
                }
            }

            private static void Draw(Cairo.Context context, Node node)
            {
                foreach (var entry in node.Children)
                {
                    if (entry.HorizontallyStretched || entry.VerticallyStretched)
                    {
                        context.FillRectangle(entry.Rect, CairoEx.ColorLightBlue);
                    }
                    else if (entry.IsFixedWidth || entry.IsFixedHeight)
                    {
                        context.FillRectangle(entry.Rect, CairoEx.ColorOrange);
                    }
                    else
                    {
                        context.FillRectangle(entry.Rect, CairoEx.ColorPink);
                    }
                    context.StrokeRectangle(entry.Rect, CairoEx.ColorBlack);
                    var innerGroup = entry;
                    if (innerGroup.Children != null)
                    {
                        context.Save();
                        Draw(context, innerGroup);
                        context.Restore();
                    }
                }
            }
        }
    }
}
