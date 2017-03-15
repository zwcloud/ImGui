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

        private const string ModelViewerPath = @"E:\Program Files\Autodesk\FBX Review\fbxreview.exe";

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
                Form.current.DrawList.AddBezier(p0, c0, p, Color.Blue);
                Form.current.DrawList.AddBezier(p, c1, p1, Color.Red);
            }));
        }

        [Fact]
        public void ShouldGenerateARightTexMesh()
        {
            var textMesh = new TextMesh();
            var style = Style.Default;
            var font = style.Font;
            var textStyle = style.TextStyle;
            var rect = new Rect(0, 0, 200, 200);
            var textContext = Application._map.CreateTextContext(
                "ABC",
                font.FontFamily, font.Size, font.FontStretch, font.FontStyle, font.FontWeight,
                (int)Math.Ceiling(rect.Size.Width), (int)Math.Ceiling(rect.Size.Height),
                textStyle.TextAlignment);

            textMesh.Build(Point.Zero, Style.Default, textContext);
            var objFilePath = "D:\\TextRenderingTest_ShouldGenerateARightTexMesh.obj";
            Utility.SaveToObjFile(objFilePath, textMesh.VertexBuffer, textMesh.IndexBuffer);
            Process.Start(ModelViewerPath, objFilePath);
        }

        [Fact]
        public void ShouldGetARightMeshAfterAppendingATextMesh()
        {
            var style = Style.Default;
            var font = style.Font;
            var textStyle = style.TextStyle;
            var rect = new Rect(0, 0, 200, 200);
            var textContext = Application._map.CreateTextContext(
                "ij = I::oO(0xB81l);",
                font.FontFamily, font.Size, font.FontStretch, font.FontStyle, font.FontWeight,
                (int)Math.Ceiling(rect.Size.Width), (int)Math.Ceiling(rect.Size.Height),
                textStyle.TextAlignment);

            var textMesh = new TextMesh();
            textMesh.Build(Point.Zero, Style.Default, textContext);
            
            var anotherTextContext = Application._map.CreateTextContext(
                "auto-sized",
                font.FontFamily, font.Size, font.FontStretch, font.FontStyle, font.FontWeight,
                200, 200,
                textStyle.TextAlignment);

            var anotherTextMesh = new TextMesh();
            anotherTextMesh.Build(new Point(50, 100), Style.Default, anotherTextContext);

            DrawList drawList = new DrawList();
            var expectedVertexCount = 0;
            var expectedIndexCount = 0;

            drawList.AddRectFilled(Point.Zero, new Point(200,100), Color.Metal);
            expectedVertexCount += 4;
            expectedIndexCount += 6;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            drawList.Append(textMesh);
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

            drawList.Append(anotherTextMesh);
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

                Skin.current.Label["Normal"].Font = new Font
                {
                    FontFamily = "Consolas",
                    FontStyle = FontStyle.Normal,
                    FontWeight = FontWeight.Normal,
                    FontStretch = FontStretch.Normal,
                    Size = 400,
                    Color = Color.Black
                };

                GUILayout.Label("D", GUILayout.Height(410), GUILayout.Width(410));

            }));
        }

        [Fact]
        public void ShouldRenderAMidiumGlyph()
        {
            Application.Run(new Form1(() => {

                Skin.current.Label["Normal"].Font = new Font
                {
                    FontFamily = "Consolas",
                    FontStyle = FontStyle.Normal,
                    FontWeight = FontWeight.Normal,
                    FontStretch = FontStretch.Normal,
                    Size = 32,
                    Color = Color.Black
                };

                GUILayout.Label("D", GUILayout.Height(410), GUILayout.Width(410));

            }));
        }
        
        [Fact]
        public void ShouldRenderASmallGlyph()
        {
            Application.Run(new Form1(() => {

                Skin.current.Label["Normal"].Font = new Font
                {
                    FontFamily = "Consolas",
                    FontStyle = FontStyle.Normal,
                    FontWeight = FontWeight.Normal,
                    FontStretch = FontStretch.Normal,
                    Size = 12,
                    Color = Color.Black
                };

                GUILayout.Label("D", GUILayout.Height(410), GUILayout.Width(410));

            }));
        }

        [Fact]
        public void ShouldRenderAString()
        {
            Application.Run(new Form1(() => {

                Skin.current.Label["Normal"].Font = new Font
                {
                    FontFamily = "Consolas",
                    FontStyle = FontStyle.Normal,
                    FontWeight = FontWeight.Normal,
                    FontStretch = FontStretch.Normal,
                    Size = 32,
                    Color = Color.Black
                };

                GUILayout.Label("A");
                GUILayout.Label("B");
                GUILayout.Label("C");
            }));
        }
    }
}
