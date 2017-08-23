using System;
using System.Collections.Generic;
using ImGui.Common;
using ImGui.Common.Primitive;

namespace ImGui
{
    internal partial class DrawList
    {
        #region primitives

        private static readonly List<Point> Path = new List<Point>();

        /// <summary>
        /// Add a line segment.
        /// </summary>
        /// <param name="start">start point</param>
        /// <param name="end">end point</param>
        /// <param name="color">color</param>
        /// <param name="thickness">thickness</param>
        public void AddLine(Point start, Point end, Color color, double thickness = 1.0)
        {
            if (MathEx.AmostZero(color.A))
                return;
            PathLineTo(start + new Vector(0.5, 0.5));
            PathLineTo(end + new Vector(0.5, 0.5));
            PathStroke(color, false, thickness);
        }

        /// <summary>
        /// Add a poly line.
        /// </summary>
        /// <param name="points">points</param>
        /// <param name="color">color</param>
        /// <param name="close">Should this method close the polyline for you? A line segment from the last point to first point will be added if this is true.</param>
        /// <param name="thickness">thickness</param>
        /// <param name="antiAliased">anti-aliased</param>
        public void AddPolyline(IList<Point> points, Color color, bool close, double thickness, bool antiAliased = false)
        {
            var pointsCount = points.Count;
            if (pointsCount < 2)
                return;

            int count = pointsCount;
            if (!close)
                count = pointsCount - 1;

            if (antiAliased)
            {
                throw new NotImplementedException();
            }
            else
            {
                // Non Anti-aliased Stroke
                int idxCount = count * 6;
                int vtxCount = count * 4; // FIXME: Not sharing edges
                this.ShapeMesh.PrimReserve(idxCount, vtxCount);

                for (int i1 = 0; i1 < count; i1++)
                {
                    int i2 = (i1 + 1) == pointsCount ? 0 : i1 + 1;
                    Point p1 = points[i1];
                    Point p2 = points[i2];
                    Vector diff = p2 - p1;
                    diff *= MathEx.InverseLength(diff, 1.0f);

                    float dx = (float)(diff.X * (thickness * 0.5f));
                    float dy = (float)(diff.Y * (thickness * 0.5f));
                    var vertex0 = new DrawVertex { pos = new Point(p1.X + dy, p1.Y - dx), uv = Point.Zero, color = color };
                    var vertex1 = new DrawVertex { pos = new Point(p2.X + dy, p2.Y - dx), uv = Point.Zero, color = color };
                    var vertex2 = new DrawVertex { pos = new Point(p2.X - dy, p2.Y + dx), uv = Point.Zero, color = color };
                    var vertex3 = new DrawVertex { pos = new Point(p1.X - dy, p1.Y + dx), uv = Point.Zero, color = color };
                    this.ShapeMesh.AppendVertex(vertex0);
                    this.ShapeMesh.AppendVertex(vertex1);
                    this.ShapeMesh.AppendVertex(vertex2);
                    this.ShapeMesh.AppendVertex(vertex3);

                    this.ShapeMesh.AppendIndex(0);
                    this.ShapeMesh.AppendIndex(1);
                    this.ShapeMesh.AppendIndex(2);
                    this.ShapeMesh.AppendIndex(0);
                    this.ShapeMesh.AppendIndex(2);
                    this.ShapeMesh.AppendIndex(3);

                    this.ShapeMesh.currentIdx += 4;
                }
            }
        }

        /// <summary>
        /// Add a filled convex polygon.
        /// </summary>
        /// <param name="points">points</param>
        /// <param name="color">color</param>
        /// <param name="antiAliased">anti-aliased</param>
        public void AddConvexPolyFilled(IList<Point> points, Color color, bool antiAliased)
        {
            antiAliased = false;//TODO remove this when antiAliased branch is implemented

            var pointsCount = points.Count;
            if (antiAliased)
            {
                throw new NotImplementedException();
            }
            else
            {
                // Non Anti-aliased Fill
                int idxCount = (pointsCount - 2) * 3;
                int vtxCount = pointsCount;
                this.ShapeMesh.PrimReserve(idxCount, vtxCount);
                for (int i = 0; i < vtxCount; i++)
                {
                    this.ShapeMesh.AppendVertex(new DrawVertex { pos = points[i], uv = Point.Zero, color = color });
                }
                for (int i = 2; i < pointsCount; i++)
                {
                    this.ShapeMesh.AppendIndex(0);
                    this.ShapeMesh.AppendIndex(i - 1);
                    this.ShapeMesh.AppendIndex(i);
                }
                this.ShapeMesh.currentIdx += vtxCount;
            }
        }

        public void AddCircle(Point center, float radius, Color col, int num_segments, float thickness)
        {
            if (MathEx.AmostZero(col.A))
                return;

            float a_max = (float)Math.PI*2.0f * ((float)num_segments - 1.0f) / (float)num_segments;
            PathArcTo(center, radius-0.5f, 0.0f, a_max, num_segments);
            PathStroke(col, true, thickness);
        }

        public void AddCircleFilled(Point center, float radius, Color col, int num_segments)
        {
            if (MathEx.AmostZero(col.A))
                return;

            float a_max = (float)Math.PI * 2.0f * ((float)num_segments - 1.0f) / (float)num_segments;
            PathArcTo(center, radius, 0.0f, a_max, num_segments);
            PathFill(col);
        }

        private void PrimRect(Point a, Point c, Color color)
        {
            Point b = new Point(c.X, a.Y);
            Point d = new Point(a.X, c.Y);
            Point uv = Point.Zero;

            this.ShapeMesh.AppendVertex(new DrawVertex { pos = a, uv = Point.Zero, color = color });
            this.ShapeMesh.AppendVertex(new DrawVertex { pos = b, uv = Point.Zero, color = color });
            this.ShapeMesh.AppendVertex(new DrawVertex { pos = c, uv = Point.Zero, color = color });
            this.ShapeMesh.AppendVertex(new DrawVertex { pos = d, uv = Point.Zero, color = color });

            this.ShapeMesh.AppendIndex(0);
            this.ShapeMesh.AppendIndex(1);
            this.ShapeMesh.AppendIndex(2);
            this.ShapeMesh.AppendIndex(0);
            this.ShapeMesh.AppendIndex(2);
            this.ShapeMesh.AppendIndex(3);

            this.ShapeMesh.currentIdx += 4;
        }

        private void PrimRectUV(Point a, Point c, Point uvA, Point uvC, Color color)
        {
            Point b = new Point(c.X, a.Y);
            Point d = new Point(a.X, c.Y);
            Point uvB = new Point(uvC.X, uvA.Y);
            Point uvD = new Point(uvA.X, uvC.Y);

            this.ShapeMesh.AppendVertex(new DrawVertex { pos = a, uv = uvA, color = color });
            this.ShapeMesh.AppendVertex(new DrawVertex { pos = b, uv = uvB, color = color });
            this.ShapeMesh.AppendVertex(new DrawVertex { pos = c, uv = uvC, color = color });
            this.ShapeMesh.AppendVertex(new DrawVertex { pos = d, uv = uvD, color = color });

            this.ShapeMesh.AppendIndex(0);
            this.ShapeMesh.AppendIndex(1);
            this.ShapeMesh.AppendIndex(2);
            this.ShapeMesh.AppendIndex(0);
            this.ShapeMesh.AppendIndex(2);
            this.ShapeMesh.AppendIndex(3);

            this.ShapeMesh.currentIdx += 4;
        }

        /// <summary>
        /// Add a rectangle. Note 1 px sized rectangles won't be rendered properly.
        /// </summary>
        /// <param name="a">upper-left point</param>
        /// <param name="b">lower-right point</param>
        /// <param name="color">color</param>
        /// <param name="rounding">radius of rounded corners</param>
        /// <param name="roundingCorners">which corner(s) will be rounded</param> TODO finish this documentation on how to use roundingCorners
        /// <param name="thickness">thickness</param>
        public void AddRect(Point a, Point b, Color color, float rounding = 0.0f, int roundingCorners = 0x0F, float thickness = 1.0f)
        {
            if (MathEx.AmostZero(color.A))
                return;
            PathRect(a + new Vector(0.5f, 0.5f), b - new Vector(0.5f, 0.5f), rounding, roundingCorners);
            PathStroke(color, true, thickness);
        }

        /// <summary>
        /// Add a filled rectangle. Note 1 px sized rectangles won't be rendered properly.
        /// </summary>
        /// <param name="a">top-left point</param>
        /// <param name="b">bottom-right point</param>
        /// <param name="color">color</param>
        /// <param name="rounding">radius of rounded corners</param>
        /// <param name="roundingCorners">which corner(s) will be rounded</param> TODO finish this documentation on how to use roundingCorners
        public void AddRectFilled(Point a, Point b, Color color, float rounding = 0.0f, int roundingCorners = 0x0F)
        {
            if (MathEx.AmostZero(color.A))
                return;
            if (rounding > 0.0f)
            {
                PathRect(a, b, rounding, roundingCorners);
                PathFill(color);
            }
            else
            {
                this.ShapeMesh.PrimReserve(6, 4);
                PrimRect(a, b, color);
            }
        }

        public void AddRectFilled(Rect rect, Color color, float rounding = 0.0f, int roundingCorners = 0x0F)
        {
            AddRectFilled(rect.TopLeft, rect.BottomRight, color, rounding, roundingCorners);
        }

        /// <summary>
        /// Add a filled triangle. Make sure the points a->b->c is clockwise.
        /// </summary>
        /// <param name="a">point A</param>
        /// <param name="b">point B</param>
        /// <param name="c">point C</param>
        /// <param name="color"></param>
        public void AddTriangleFilled(Point a, Point b, Point c, Color color)
        {
            if (MathEx.AmostZero(color.A))
                return;

            PathLineTo(a);
            PathLineTo(b);
            PathLineTo(c);
            PathFill(color);
        }

        #endregion

        #region stateful path constructing methods

        private static void PathBezierToCasteljau(IList<Point> path, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double tessTol, int level)
        {
            double dx = x4 - x1;
            double dy = y4 - y1;
            double d2 = ((x2 - x4) * dy - (y2 - y4) * dx);
            double d3 = ((x3 - x4) * dy - (y3 - y4) * dx);
            d2 = (d2 >= 0) ? d2 : -d2;
            d3 = (d3 >= 0) ? d3 : -d3;
            if ((d2 + d3) * (d2 + d3) < tessTol * (dx * dx + dy * dy))
            {
                path.Add(new Point(x4, y4));
            }
            else if (level < 10)
            {
                double x12 = (x1 + x2) * 0.5f, y12 = (y1 + y2) * 0.5f;
                double x23 = (x2 + x3) * 0.5f, y23 = (y2 + y3) * 0.5f;
                double x34 = (x3 + x4) * 0.5f, y34 = (y3 + y4) * 0.5f;
                double x123 = (x12 + x23) * 0.5f, y123 = (y12 + y23) * 0.5f;
                double x234 = (x23 + x34) * 0.5f, y234 = (y23 + y34) * 0.5f;
                double x1234 = (x123 + x234) * 0.5f, y1234 = (y123 + y234) * 0.5f;

                PathBezierToCasteljau(path, x1, y1, x12, y12, x123, y123, x1234, y1234, tessTol, level + 1);
                PathBezierToCasteljau(path, x1234, y1234, x234, y234, x34, y34, x4, y4, tessTol, level + 1);
            }
        }
        private const double CurveTessellationTol = 1.25;

        /// <summary>
        /// Adds a cubic Bézier spline to the path from the current point to position end, using c1 and c2 as the control points.
        /// </summary>
        /// <param name="c1">first control point</param>
        /// <param name="c2">second control point</param>
        /// <param name="end">end point</param>
        /// <param name="numSegments">number of segments used when tessellating the curve. Use 0 to do automatic tessellation.</param>
        public static void PathBezierCurveTo(Point c1, Point c2, Point end, int numSegments = 0)
        {
            Point p1 = Path[Path.Count - 1];
            if (numSegments == 0)
            {
                // Auto-tessellated
                PathBezierToCasteljau(Path, p1.X, p1.Y, c1.X, c1.Y, c2.X, c2.Y, end.X, end.Y, CurveTessellationTol, 0);
            }
            else
            {
                float tStep = 1.0f / (float)numSegments;
                for (int iStep = 1; iStep <= numSegments; iStep++)
                {
                    float t = tStep * iStep;
                    float u = 1.0f - t;
                    float w1 = u * u * u;
                    float w2 = 3 * u * u * t;
                    float w3 = 3 * u * t * t;
                    float w4 = t * t * t;
                    Path.Add(new Point(w1 * p1.X + w2 * c1.X + w3 * c2.X + w4 * end.X, w1 * p1.Y + w2 * c1.Y + w3 * c2.Y + w4 * end.Y));
                }
            }
        }

        /// <summary>
        /// Adds a rectangle to the path.
        /// </summary>
        /// <param name="a">top-left corner</param>
        /// <param name="b">bottom-right point</param>
        /// <param name="rounding">radius of rounded corners</param>
        /// <param name="roundingCorners">which corner(s) will be rounded</param> TODO finish this documentation on how to use roundingCorners
        public void PathRect(Point a, Point b, float rounding = 0.0f, int roundingCorners = 0x0F)
        {
            double r = rounding;
            r = Math.Min(r, Math.Abs(b.X - a.X) * (((roundingCorners & (1 | 2)) == (1 | 2)) || ((roundingCorners & (4 | 8)) == (4 | 8)) ? 0.5f : 1.0f) - 1.0f);
            r = Math.Min(r, Math.Abs(b.Y - a.Y) * (((roundingCorners & (1 | 8)) == (1 | 8)) || ((roundingCorners & (2 | 4)) == (2 | 4)) ? 0.5f : 1.0f) - 1.0f);

            if (r <= 0.0f || roundingCorners == 0)
            {
                PathLineTo(a);
                PathLineTo(new Point(b.X, a.Y));
                PathLineTo(b);
                PathLineTo(new Point(a.X, b.Y));
            }
            else
            {
                var r0 = (roundingCorners & 1) != 0 ? r : 0.0f;
                var r1 = (roundingCorners & 2) != 0 ? r : 0.0f;
                var r2 = (roundingCorners & 4) != 0 ? r : 0.0f;
                var r3 = (roundingCorners & 8) != 0 ? r : 0.0f;
                PathArcToFast(new Point(a.X + r0, a.Y + r0), r0, 6, 9);
                PathArcToFast(new Point(b.X - r1, a.Y + r1), r1, 9, 12);
                PathArcToFast(new Point(b.X - r2, b.Y - r2), r2, 0, 3);
                PathArcToFast(new Point(a.X + r3, b.Y - r3), r3, 3, 6);
            }
        }

        /// <summary>
        /// Strokes the current path.
        /// </summary>
        /// <param name="color">color</param>
        /// <param name="close">Set to true if you want the path be closed. A line segment from the last point to first point will be added if this is true.</param>
        /// <param name="thickness">thickness</param>
        public void PathStroke(Color color, bool close, double thickness = 1)
        {
            AddPolyline(Path, color, close, thickness);
            PathClear();
        }

        /// <summary>
        /// Fills the current path.
        /// </summary>
        /// <param name="color">fill color</param>
        public void PathFill(Color color)
        {
            AddConvexPolyFilled(Path, color, true);
            PathClear();
        }

        /// <summary>
        /// Clears the current path.
        /// </summary>
        public void PathClear()
        {
            Path.Clear();
        }

        /// <summary>
        /// Moves the current point of the current path.
        /// </summary>
        /// <param name="point">position that current point will be moved to</param>
        public void PathMoveTo(Point point)
        {
            if (Path.Count == 0)
            {
                Path.Add(point);
            }
            else
            {
                Path[Path.Count - 1] = point;
            }
        }

        /// <summary>
        /// Adds a line to the path from the current point to position p.
        /// </summary>
        /// <param name="p">next point</param>
        public void PathLineTo(Point p)
        {
            Path.Add(p);
        }

        private static readonly Point[] CirclePoints = InitCirclePoints();

        private static Point[] InitCirclePoints()
        {
            Point[] result = new Point[12];
            for (int i = 0; i < 12; i++)
            {
                var a = (float)i / 12 * 2 * Math.PI;
                result[i].X = Math.Cos(a);
                result[i].Y = Math.Sin(a);
            }
            return result;
        }

        /// <summary>
        /// (Fast) adds an arc from angle1 to angle2 to the current path.
        /// </summary>
        /// <param name="center">the center of the arc</param>
        /// <param name="radius">the radius of the arc</param>
        /// <param name="amin">angle1 = amin * π * 1/12</param>
        /// <param name="amax">angle1 = amax * π * 1/12</param>
        public void PathArcToFast(Point center, double radius, int amin, int amax)
        {
            if (amin > amax) return;
            if (MathEx.AmostZero(radius))
            {
                Path.Add(center);
            }
            else
            {
                Path.Capacity = Path.Count + amax - amin + 1;
                for (int a = amin; a <= amax; a++)
                {
                    Point c = CirclePoints[a % CirclePoints.Length];
                    Path.Add(new Point(center.X + c.X * radius, center.Y + c.Y * radius));
                }
            }
        }

        public void PathArcTo(Point center, float radius, float amin, float amax, int num_segments)
        {
            if (radius == 0.0f)
                Path.Add(center);
            Path.Capacity = Path.Count + (num_segments + 1);
            for (int i = 0; i <= num_segments; i++)
            {
                float a = amin + ((float)i / (float)num_segments) * (amax - amin);
                Path.Add(new Point(center.X + Math.Cos(a) * radius, center.Y + Math.Sin(a) * radius));
            }
        }

        /// <summary>
        /// Closes the current path.
        /// </summary>
        public void PathClose()
        {
            Path.Add(Path[0]);
        }

        #endregion

    }
}
