using ImGui;
using System;
using System.Diagnostics;
using Xunit;

namespace TextRenderingTest
{
    public class Form1 : Form
    {
        private Action onGUI;

        public Form1(Action onGUI) : base(new Point(400, 300), new Size(800, 600)) { this.onGUI = onGUI; Form.current = this; }

        protected override void OnGUI()
        {
            if (this.onGUI != null)
                this.onGUI();
        }
    }
    
    public class TextRenderingTest
    {
        public TextRenderingTest()
        {
            Application.InitSysDependencies();
        }

        private const string ModelViewerPath = @"E:\Program Files (green)\open3mod_1_1_standalone\open3mod.exe";

        /// <summary>
        /// This should render a filled cubic bezier curve that commonly used in font
        /// </summary>
        /// <remarks>
        /// The cubic curve used by ttf font is very special that can be perfectly split into two quadratic curves, which is easy to be filled in fragment shader.
        /// See the following links for more information.
        /// https://developer.apple.com/fonts/TrueType-Reference-Manual/RM01/Chap1.html#points
        /// http://stackoverflow.com/questions/20733790/truetype-fonts-glyph-are-made-of-quadratic-bezier-why-do-more-than-one-consecu/20772557?noredirect=1#comment68243476_20772557
        /// </remarks>
        [Fact]
        public void ShouldRenderAFilledCubicBezierCurve()
        {
            // 76, 410,   93, 312,   119, 188,   193, 190,
            // 193, 190,  267, 190,  292, 366,   311, 521,
            var p0 = new Point(76,  410);// start point
            var c0 = new Point(115, 190);// control point 0
            var c1 = new Point(273, 190);// control point 1
            var p1 = new Point(311, 521);// end point
            
            var p = new Point((c0.X + c1.X) / 2, (c0.Y + c1.Y) / 2);

            Application.Run(new Form1(() => {
                var d = Form.current.OverlayDrawList;
                d.AddBezier(p0, c0, p, Color.Blue);
                d.AddBezier(p, c1, p1, Color.Red);
                d.PathMoveTo(p0);
                d.PathLineTo(p);
                d.PathLineTo(p1);
                d.PathFill(Color.Black);
            }));
        }

        [Fact]
        public void ShouldRenderAFilledCubicBezierCurve2()
        {
            // (625,1549) (1040,1508) (1444, 1168) (1442,794)
            // * 0.1
            var p0 = new Point(62.5, 154.9);// start point
            var c0 = new Point(104.0, 150.8);// control point 0
            var c1 = new Point(144.4, 116.8);// control point 1
            var p1 = new Point(144.2, 79.4);// end point

            var p = new Point((c0.X + c1.X) / 2, (c0.Y + c1.Y) / 2);

            Application.Run(new Form1(() => {
                var d = Form.current.OverlayDrawList;
                d.AddBezier(p0, c0, p, Color.Blue);
                d.AddBezier(p, c1, p1, Color.Red);
                d.PathMoveTo(p0);
                d.PathLineTo(p);
                d.PathLineTo(p1);
                d.PathFill(Color.Black);
            }));
        }


        [Fact]
        public void ShouldGenerateARightTexMesh()
        {
            var style = GUIStyle.Default;
            style.Set<double>(GUIStyleName.FontSize, 36);

            var state = GUIState.Normal;
            var fontFamily = style.Get<string>(GUIStyleName.FontFamily, state);
            var fontSize = style.Get<double>(GUIStyleName.FontSize, state);
            var fontStretch = (FontStretch)style.Get<int>(GUIStyleName.FontStretch, state);
            var fontStyle = (FontStyle)style.Get<int>(GUIStyleName.FontStyle, state);
            var fontWeight = (FontWeight)style.Get<int>(GUIStyleName.FontWeight, state);
            var textAlignment = (TextAlignment)style.Get<int>(GUIStyleName.TextAlignment, state);

            var rect = new Rect(0, 0, 200, 200);
            var textContext = Application.platformContext.CreateTextContext(
                "ABC",
                fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                (int)Math.Ceiling(rect.Size.Width), (int)Math.Ceiling(rect.Size.Height),
                textAlignment);

            var textMesh = new TextMesh();
            textMesh.Build(Point.Zero, style, textContext);
            var objFilePath = "D:\\TextRenderingTest_ShouldGenerateARightTexMesh.obj";
            Utility.SaveToObjFile(objFilePath, textMesh.VertexBuffer, textMesh.IndexBuffer);
            Process.Start(ModelViewerPath, objFilePath);
        }

        [Fact]
        public void ShouldGetARightMeshAfterAppendingATextMesh()
        {
            var style = GUIStyle.Default;

            var state = GUIState.Normal;
            var fontFamily = style.Get<string>(GUIStyleName.FontFamily, state);
            var fontSize = style.Get<double>(GUIStyleName.FontSize, state);
            var fontStretch = (FontStretch)style.Get<int>(GUIStyleName.FontStretch, state);
            var fontStyle = (FontStyle)style.Get<int>(GUIStyleName.FontStyle, state);
            var fontWeight = (FontWeight)style.Get<int>(GUIStyleName.FontWeight, state);
            var textAlignment = (TextAlignment)style.Get<int>(GUIStyleName.TextAlignment, state);

            var rect = new Rect(0, 0, 200, 200);
            var textContext = Application.platformContext.CreateTextContext(
                "ij = I::oO(0xB81l);",
                fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                (int)Math.Ceiling(rect.Size.Width), (int)Math.Ceiling(rect.Size.Height),
                textAlignment);

            var textMesh = new TextMesh();
            textMesh.Build(Point.Zero, style, textContext);
            
            var anotherTextContext = Application.platformContext.CreateTextContext(
                "auto-sized",
                fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                200, 200,
                textAlignment);

            var anotherTextMesh = new TextMesh();
            anotherTextMesh.Build(new Point(50, 100), style, anotherTextContext);

            DrawList drawList = new DrawList();
            var expectedVertexCount = 0;
            var expectedIndexCount = 0;

            drawList.AddRectFilled(Point.Zero, new Point(200,100), Color.Metal);
            expectedVertexCount += 4;
            expectedIndexCount += 6;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            drawList.Append(textMesh, Vector.Zero);
            expectedVertexCount += textMesh.VertexBuffer.Count;
            expectedIndexCount += textMesh.IndexBuffer.Count;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            drawList.AddRectFilled(new Point(0, 110), new Point(200, 150), Color.Metal);
            expectedVertexCount += 4;
            expectedIndexCount += 6;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            drawList.AddRectFilled(new Point(0, 160), new Point(200, 200), Color.Metal);
            expectedVertexCount += 4;
            expectedIndexCount += 6;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            drawList.Append(anotherTextMesh, Vector.Zero);
            expectedVertexCount += anotherTextMesh.VertexBuffer.Count;
            expectedIndexCount += anotherTextMesh.IndexBuffer.Count;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            var objFilePath = "D:\\TextRenderingTest_ShouldGetARightMeshAfterAppendingATextMesh.obj";
            Utility.SaveToObjFile(objFilePath, drawList.DrawBuffer.VertexBuffer, drawList.DrawBuffer.IndexBuffer);
            Process.Start(ModelViewerPath, objFilePath);
        }


        [Fact]
        public void ShouldRenderABigGlyph()
        {
            Application.Run(new Form1(()=> {
                var d = Form.current.OverlayDrawList;

                GUIStyle labelStyle = "Label";
                labelStyle.Set<double>(GUIStyleName.FontSize, 400);
                labelStyle.Set<string>(GUIStyleName.FontFamily, Utility.FontDir + "msjh.ttf");
                GUILayout.Label("D", GUILayout.Height(410), GUILayout.Width(410));

            }));
        }

        [Fact]
        public void ShouldRenderAMidiumGlyph()
        {
            Application.Run(new Form1(() => {

                GUIStyle labelStyle = "Label";
                labelStyle.Set<double>(GUIStyleName.FontSize, 32);

                GUILayout.Label("D", GUILayout.Height(410), GUILayout.Width(410));

            }));
        }
        
        [Fact]
        public void ShouldRenderASmallGlyph()
        {
            Application.Run(new Form1(() => {

                GUIStyle labelStyle = "Label";
                labelStyle.Set<double>(GUIStyleName.FontSize, 12);

                GUILayout.Label("D", GUILayout.Height(410), GUILayout.Width(410));

            }));
        }

        [Fact]
        public void ShouldRenderAString()
        {
            Application.Run(new Form1(() => {

                GUIStyle labelStyle = "Label";
                labelStyle.Set<double>(GUIStyleName.FontSize, 32);

                GUILayout.Label("A");
                GUILayout.Label("B");
                GUILayout.Label("C");
            }));
        }

        [Fact]
        public void ShouldRenderAStringInMeasuredRectangle()
        {
            string text = "Hello ImGui!你好";
            GUIStyle style = new GUIStyle();
            style.FontSize = 20;

            Application.Run(new Form1(() => {
                GUILayout.Button("dummy");
                var d = Form.current.OverlayDrawList;
                var size = style.MeasureText(GUIState.Normal, text);
                var rect = new Rect(10, 100, size);
                d.AddRect(rect.Min, rect.Max, Color.Red);
                d.DrawText(rect, text, style, GUIState.Normal);
            }));
        }
    }
}
