using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using ImGui;

namespace Typography.OpenFont
{
    internal static class GlyphReader
    {
        private static GlyphPointF LoopGet(this GlyphPointF[] points, int index, int firstIndex, int lastIndex)
        {
            if(index > lastIndex)
            {
                index -= lastIndex;
            }
            index = index % points.Length;
            return points[index];
        }

        public static void Read(GlyphPointF[] points, ushort[] endPoints, float offsetX, float offsetY, float scale,
            out List<List<Point>> polygons,
            out List<(Point, Point, Point)> bezierSegments,
            bool flipY = true
            )
        {
            List<List<GlyphPointF>> glyphPointList = new List<List<GlyphPointF>>();

            // split all continued off-curve segment
            for (int i = 0; i < endPoints.Length; i++)
            {
                var firstPointIndex = i == 0 ? 0 : endPoints[i - 1] + 1;
                var endPointIndex = endPoints[i];
                glyphPointList.Add(new List<GlyphPointF>());
                for (int j = firstPointIndex; j<= endPointIndex; j++)
                {
                    var p = points[j];
                    var prevIndex = j - 1;
                    if (j - 1 < firstPointIndex)
                    {
                        prevIndex = endPointIndex;
                    }
                    var prev = points[prevIndex];

                    if(!prev.onCurve && !p.onCurve)
                    {
                        var midPoint = new GlyphPointF((prev.X + p.X)/2, (prev.Y + p.Y)/2, true);
                        glyphPointList[i].Add(midPoint);
                    }
                    glyphPointList[i].Add(p);
                }
            }

            polygons = new List<List<Point>>();
            bezierSegments = new List<(Point, Point, Point)>();

            for (int i = 0; i < glyphPointList.Count; i++)//for each contour
            {
                var contourPoints = glyphPointList[i];
                polygons.Add(new List<Point>());
                var polygon = polygons[polygons.Count - 1];
                for (int j = 0; j < contourPoints.Count; j++)
                {
                    var glyphpoint = contourPoints[j];
                    var point = new Point(glyphpoint.X, glyphpoint.Y);
                    point = new Point(point.X * scale + offsetX, point.Y * scale * (flipY ? -1 : 1) + offsetY);//apply scale, flipping and offset
                    if (glyphpoint.onCurve)
                    {
                        polygon.Add(point);
                    }
                    else
                    {
                        var prevGlyphPoint = contourPoints[j - 1 >= 0 ? j - 1 : contourPoints.Count - 1];
                        var nextGlyphPoint = contourPoints[j + 1 <= contourPoints.Count - 1 ? j + 1 : 0];
                        var prev = new Point(prevGlyphPoint.X, prevGlyphPoint.Y);
                        prev = ApplyOffsetScale(prev, offsetX, offsetY, scale, flipY);
                        var next = new Point(nextGlyphPoint.X, nextGlyphPoint.Y);
                        next = ApplyOffsetScale(next, offsetX, offsetY, scale, flipY);
                        bezierSegments.Add((prev, point, next));
                    }
                }
            }

        }

        private static Point ApplyOffsetScale(Point point, float offsetX, float offsetY, float scale, bool flipY)
        {
            return new Point(point.X * scale + offsetX, point.Y * scale * (flipY ? -1 : 1) + offsetY);
        }

        public static void Read_(GlyphPointF[] points, ushort[] endPoints, float offsetX, float offsetY, float scale,
            out List<List<Point>> polygons,
            out List<(Point, Point, Point)> bezierSegments,
            bool flipY = true
            )
        {
            polygons = new List<List<Point>>();
            polygons.Add(new List<Point>());
            bezierSegments = new List<(Point, Point, Point)>();

            Point lastControlPoint = Point.Zero;
            bool lastPointIsControlPoint = false;
            int startPointIndex = 0;
            int currentEndPointIndex = 0;
            int k = 0;
            var polygon = polygons[0];
            while (k < points.Length)
            {
                var point = points[k];
                float p_x = point.X * scale;
                float p_y = point.Y * scale * (flipY ? -1 : 1);
                p_x += offsetX;
                p_y += offsetY;

                if (point.onCurve)
                {
                    var p = new Point(p_x, p_y);
                    if (lastPointIsControlPoint)
                    {
                        var p0 = polygon[polygon.Count - 1];
                        var c = lastControlPoint;
                        bezierSegments.Add((p0, c, p));
                    }
                    polygon.Add(p);
                    lastPointIsControlPoint = false;
                }
                else if (lastPointIsControlPoint)
                {
                    var p0 = polygon[polygon.Count - 1];
                    var cA = lastControlPoint;
                    var p = new Point((p_x + lastControlPoint.X) / 2, (p_y + lastControlPoint.Y) / 2);//p is on curve
                    var cB = new Point(p_x, p_y);

                    polygon.Add(p);
                    bezierSegments.Add((p0, cA, p));

                    lastControlPoint = cB;
                    lastPointIsControlPoint = true;
                }
                else
                {
                    var c = new Point(p_x, p_y);
                    lastControlPoint = c;
                    lastPointIsControlPoint = true;
                }

                if (k == endPoints[currentEndPointIndex])//current point (p) is the end point of a contour
                {
                    //connect end point to start point
                    var _tmp = points[startPointIndex];
                    var startPoint = new Point(_tmp.X, _tmp.Y);//start point of this ended contour
                    polygon.Add(startPoint);
                    if (lastPointIsControlPoint)// current point is control point
                    {
                        var _tmp1 = points[k - 1];
                        var p0 = new Point(_tmp1.X, _tmp1.Y);
                        var c = new Point(p_x, p_y);
                        var p1 = startPoint;
                        bezierSegments.Add((p0, c, p1));
                    }

                    currentEndPointIndex++;
                    startPointIndex = k + 1;

                    polygons.Add(new List<Point>());
                    polygon = polygons[polygons.Count - 1];

                    //reset
                    lastPointIsControlPoint = false;
                }
                k++;
            }
        }
    }
}
