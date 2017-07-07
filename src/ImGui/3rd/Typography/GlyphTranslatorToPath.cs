//Apache2, 2014-2016, Samuel Carlsson, WinterDev

using System;
using ImGui;
using Typography.OpenFont;

namespace Typography.Rendering
{
    /// <summary>
    /// glyph-to-path translator
    /// </summary>
    internal class GlyphTranslatorToPath : IGlyphTranslator
    {
        #region Debug

        private readonly bool writeToText;
        private const string MeshTextPath = "D:\\typography_mesh_text.txt";
        private readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();

        #endregion

        private ITextPathBuilder g;
        private float lastX;
        private float lastY;

        public ITextPathBuilder PathBuilder { set { this.g = value; } }

        public Color Color { get; internal set; }

        public GlyphTranslatorToPath()
        {
            writeToText = false;
        }

        public void BeginRead(int countourCount)
        {
            if (this.g == null)
            {
                throw new InvalidOperationException();
            }
        }

        public void EndRead()
        {
        }

        public void MoveTo(float x, float y)
        {
            lastX = (float)x;
            lastY = (float)y;
            if (writeToText)
            {
                builder.AppendLine(string.Format("move to ({0:0.00}, {1:0.00})", x, y));
            }

            g.PathMoveTo(new Point(x, y));
        }

        public void CloseContour()
        {
            if (writeToText)
            {
                builder.AppendLine("Figure end.");
            }

            g.PathClose();
            g.AddContour(this.Color);
            g.PathClear();
        }

        public void Curve3(float x1, float y1, float x2, float y2)
        {
            if (writeToText)
            {
                builder.AppendLine(string.Format("bezier curve: start ({0:0.00}, {1:0.00}) control ({2:0.00}, {3:0.00}) end ({4:0.00}, {5:0.00})", lastX, lastY, x1, y1, x2, y2));
            }

            g.PathAddBezier(new Point(lastX, lastY), new Point(x1, y1), new Point(x2, y2));

            //var p = new { X = (c0x + c1x) / 2, Y = (c0y + c1y) / 2 };
            //g.PathLineTo(new Point(p.X, p.Y));
            //g.PathLineTo(new Point(p1x, p1y));
            lastX = x2;
            lastY = y2;
        }

        public void Curve4(float x1, float y1, float x2, float y2, float x3, float y3)//not called
        {
            throw new NotSupportedException();
        }

        public void LineTo(float x, float y)
        {
            if (writeToText)
            {
                builder.AppendLine(string.Format("line to ({0:0.00}, {1:0.00})", x, y));
            }

            g.PathLineTo(new Point(x, y));

            lastX = x;
            lastY = y;
        }

        public void OutputDebugResult()
        {
            // show debug results
            if (writeToText)
            {
                if (System.IO.File.Exists(MeshTextPath))
                {
                    System.IO.File.Delete(MeshTextPath);
                }
                System.IO.File.WriteAllText(MeshTextPath, builder.ToString());
            }
        }
    }
}

