using System.Collections.Generic;
using Typography.OpenFont;

namespace ImGui.OSAbstraction.Text
{
    internal static class GlyphLoader
    {
        public static void Read(
            Glyph glyph,
            out List<List<Point>> polygons,
            out List<(Point, Point, Point)> quadraticBezierSegments)
        {
            GlyphPointF[] points = glyph.GlyphPoints;
            ushort[] endPoints = glyph.EndPoints;

            List<List<GlyphPointF>> glyphPointList = new List<List<GlyphPointF>>();

            // split all continued off-curve cubic bezier segment (c1, c2, end) to two quadratic bezier segments (c1, mid) (c2, end)
            // See https://stackoverflow.com/a/20772557/3427520
            for (int i = 0; i < endPoints.Length; i++)
            {
                var firstPointIndex = i == 0 ? 0 : endPoints[i - 1] + 1;
                var endPointIndex = endPoints[i];
                glyphPointList.Add(new List<GlyphPointF>());
                for (int j = firstPointIndex; j <= endPointIndex; j++)
                {
                    var p = points[j];
                    var prevIndex = j - 1;
                    if (j - 1 < firstPointIndex)
                    {
                        prevIndex = endPointIndex;
                    }
                    var prev = points[prevIndex];

                    if (!prev.onCurve && !p.onCurve)
                    {
                        var midPoint = new GlyphPointF((prev.X + p.X) / 2, (prev.Y + p.Y) / 2, true);//mid
                        glyphPointList[i].Add(midPoint);
                    }
                    glyphPointList[i].Add(p);
                }
            }

            polygons = new List<List<Point>>();
            quadraticBezierSegments = new List<(Point, Point, Point)>();

            /*
             * Important note
             *
             * Glyph points from Glyph should be flipped before adding to polygons and bezierSegments.
             * Because the coordinate system of a Glyph
             * +--------------------+
             * |(0,100)   (100, 100)|
             * |                    |
             * |                    |
             * |                    |
             * |                    |
             * |                    |
             * |                    |
             * |                    |
             * |                    |
             * |(0,0)        (100,0)|
             * +--------------------+
             *
             * is upside down compared to the one we used
             * +--------------------+
             * |(0,0)       (100, 0)|
             * |                    |
             * |                    |
             * |                    |
             * |                    |
             * |                    |
             * |                    |
             * |                    |
             * |                    |
             * |(0,100)   (100, 100)|
             * +--------------------+
             */

            for (int i = 0; i < glyphPointList.Count; i++)//for each contour
            {
                var contourPoints = glyphPointList[i];
                var polygon = new List<Point>();
                for (int j = 0; j < contourPoints.Count; j++)
                {
                    var glyphpoint = contourPoints[j];
                    var point = new Point(glyphpoint.X, -glyphpoint.Y);
                    if (glyphpoint.onCurve)
                    {
                        polygon.Add(point);
                    }
                    else
                    {
                        var prevGlyphPoint = contourPoints[j - 1 >= 0 ? j - 1 : contourPoints.Count - 1];
                        var nextGlyphPoint = contourPoints[j + 1 <= contourPoints.Count - 1 ? j + 1 : 0];
                        var prev = new Point(prevGlyphPoint.X, -prevGlyphPoint.Y);
                        var next = new Point(nextGlyphPoint.X, -nextGlyphPoint.Y);
                        quadraticBezierSegments.Add((prev, point, next));
                    }
                }
                if (polygon.Count < 3)//don't add degenerated contour
                {
                    continue;
                }
                polygons.Add(polygon);
            }

        }
    }
}
