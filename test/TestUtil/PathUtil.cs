using System;
using System.Collections.Generic;
using System.Text;
using ImGui;

namespace TextRenderingTest
{
    class PathUtil
    {
        public static void SaveToPng(List<List<Point>> paths, string filePath)
        {
            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 2000, 2000))
            using (Cairo.Context g = new Cairo.Context(surface))
            {
                foreach (var path in paths)
                {
                    if (path == null || path.Count <= 1) return;
                    g.MoveTo(path[0].X, path[0].Y);
                    for (int i = 1; i < path.Count; i++)
                    {
                        var x0 = path[i - 1].X;
                        var y0 = path[i - 1].Y;
                        var x1 = path[i].X;
                        var y1 = path[i].Y;

                        g.LineTo(x1, y1);

                        var dx = x1 - x0;
                        var dy = y1 - y0;

                        if (MathEx.AmostZero(dx) && MathEx.AmostZero(dy)) continue;

                        var n0 = new Vector(-dy, dx); n0.Normalize();
                        var n1 = new Vector(dy, -dx); n1.Normalize();

                        var B = new Point(x1, y1);
                        var d = new Vector(x0 - x1, y0 - y1); d.Normalize();

                        var arrowEnd0 = B + 5 * (d + n0);
                        var arrowEnd1 = B + 5 * (d + n1);
                        g.MoveTo(x1, y1);
                        g.LineTo(new Cairo.PointD(arrowEnd0.X, arrowEnd0.Y));
                        g.MoveTo(x1, y1);
                        g.LineTo(new Cairo.PointD(arrowEnd1.X, arrowEnd1.Y));
                        g.MoveTo(x1, y1);
                    }
                    g.Stroke();
                }

                surface.WriteToPng(filePath);
            }
        }

        public static void SaveToPng(IList<Point> path, string filePath)
        {
            if (path == null || path.Count <= 1) return;
            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 2000, 2000))
            using (Cairo.Context g = new Cairo.Context(surface))
            {
                g.MoveTo(path[0].X, path[0].Y);
                for (int i = 1; i < path.Count; i++)
                {
                    var x0 = path[i - 1].X;
                    var y0 = path[i - 1].Y;
                    var x1 = path[i].X;
                    var y1 = path[i].Y;

                    g.LineTo(x1, y1);

                    var dx = x1 - x0;
                    var dy = y1 - y0;

                    if (MathEx.AmostZero(dx) && MathEx.AmostZero(dy)) continue;

                    var n0 = new Vector(-dy, dx); n0.Normalize();
                    var n1 = new Vector(dy, -dx); n1.Normalize();

                    var B = new Point(x1, y1);
                    var d = new Vector(x0 - x1, y0 - y1); d.Normalize();

                    var arrowEnd0 = B + 5 * (d + n0);
                    var arrowEnd1 = B + 5 * (d + n1);
                    g.MoveTo(x1, y1);
                    g.LineTo(new Cairo.PointD(arrowEnd0.X, arrowEnd0.Y));
                    g.MoveTo(x1, y1);
                    g.LineTo(new Cairo.PointD(arrowEnd1.X, arrowEnd1.Y));
                    g.MoveTo(x1, y1);
                }
                g.Stroke();
                surface.WriteToPng(filePath);
            }
        }

        public static void SaveToPng(List<LibTessDotNet.ContourVertex> path)
        {
            if (path == null || path.Count <= 1) return;
            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, (int)Form.current.Size.Width, (int)Form.current.Size.Height))
            using (Cairo.Context g = new Cairo.Context(surface))
            {
                g.MoveTo(path[0].Position.X, path[0].Position.Y);
                for (int i = 1; i < path.Count; i++)
                {
                    var x0 = path[i - 1].Position.X;
                    var y0 = path[i - 1].Position.Y;
                    var x1 = path[i].Position.X;
                    var y1 = path[i].Position.Y;

                    g.LineTo(x1, y1);

                    {
                        // draw index number
                        g.RelMoveTo(5, 5);
                        g.ShowText(i.ToString());
                        g.MoveTo(x1, y1);

                        // draw arrow
                        var dx = x1 - x0;
                        var dy = y1 - y0;

                        if (MathEx.AmostZero(dx) && MathEx.AmostZero(dy)) continue;

                        var n0 = new Vector(-dy, dx); n0.Normalize();
                        var n1 = new Vector(dy, -dx); n1.Normalize();

                        var B = new Point(x1, y1);
                        var d = new Vector(x0 - x1, y0 - y1); d.Normalize();

                        var arrowEnd0 = B + 5 * (d + n0);
                        var arrowEnd1 = B + 5 * (d + n1);
                        g.MoveTo(x1, y1);
                        g.LineTo(new Cairo.PointD(arrowEnd0.X, arrowEnd0.Y));
                        g.MoveTo(x1, y1);
                        g.LineTo(new Cairo.PointD(arrowEnd1.X, arrowEnd1.Y));
                        g.MoveTo(x1, y1);
                    }
                }
                g.Stroke();
                surface.WriteToPng(@"D:\contour_test.png");
            }
        }
    }
}
