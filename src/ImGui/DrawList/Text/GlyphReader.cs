using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using ImGui;
using ImGui.Common.Primitive;

namespace Typography.OpenFont
{
    internal static class GlyphReader
    {
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
    }
}
