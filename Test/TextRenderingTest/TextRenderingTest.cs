using ImGui;
using System;
using Xunit;

namespace TextRenderingTest
{
    public partial class Form1 : Form
    {
        private Action onGUI;

        public Form1(Action onGUI) : base(new Point(400, 300), new Size(800, 600)) { this.onGUI = onGUI; }

        protected override void OnGUI()
        {
            if (this.onGUI != null)
                this.onGUI();
        }
    }
    
    public class TextRenderingTest
    {
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
    }
}
