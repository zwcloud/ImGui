using ImGui;
using System;
using Xunit;

namespace PrimitiveRenderingTest
{
    public partial class Form1 : Form
    {
        private Action onGUI;

        public Form1(Action onGUI) : base(new Point(400, 300), new Size(800, 600)) { this.onGUI = onGUI; }

        protected override void OnGUI()
        {
            this.onGUI?.Invoke();
        }
    }
    
    public class PathRenderingTest
    {
        public PathRenderingTest()
        {
            Application.InitSysDependencies();
        }

        [Fact]
        public void ShouldRenderAnAnimatedTriangle()
        {
            var A = new Point(200, 200);
            var B = new Point(600, 200);
            var C = new Point(400, 400);

            bool open = true;

            Application.Run(new Form1(() => {

                GUILayout.Begin("PrimitiveRenderingTest", ref open, new Point(43, 256), new Size(400, 300), 1, WindowFlags.Default);
                {
                    var normal = (Application.Time % 1000) / 1000f * 2 - 1;
                    var rad = normal * Math.PI;
                    var A_ = A + 50 * new Vector(Math.Cos(rad) - Math.Sin(rad), Math.Sin(rad) + Math.Cos(rad));
                    rad += Math.PI * 0.333;
                    var B_ = B + 30 * new Vector(Math.Cos(rad) - Math.Sin(rad), Math.Sin(rad) + Math.Cos(rad));
                    rad += Math.PI * 0.666;
                    var C_ = C + 70 * new Vector(Math.Cos(rad) - Math.Sin(rad), Math.Sin(rad) + Math.Cos(rad));

                    var d = Form.current.uiContext.CurrentWindow.DrawList;
                    d.PathMoveTo(A_);
                    d.PathLineTo(B_);
                    d.PathLineTo(C_);
                    d.PathStroke(Color.Blue, true, 2);
                }

            }));
        }
    }
}
