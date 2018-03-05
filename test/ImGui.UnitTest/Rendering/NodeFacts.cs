using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public partial class NodeFacts
    {
        public class TheLayoutMethod
        {
            [Fact]
            public void GetRectAfterRelayout1() // Add rect; Add rect then remove rect
            {
                Node a = new Node();
                a.id = 1;
                a.Rect = new Rect(0, 0, 300, 400);
                a.AttachLayoutGroup(true);
                
                Node b = new Node();
                b.id = 2;
                b.Rect = new Rect(0, 0, 100, 100);
                b.AttachLayoutGroup(true);
                
                Node c = new Node();
                c.id = 3;
                c.Rect = new Rect(0, 0, 100, 200);
                c.AttachLayoutGroup(true);

                a.Children.Add(b);
                a.Children.Add(c);

                a.Layout();
            }

        }

    }
}
