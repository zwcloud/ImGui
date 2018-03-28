using System;
using ImGui.Common.Primitive;
using System.Collections.Generic;
using ImGui.Common;

namespace ImGui.Rendering
{
    internal class PathPrimitive : Primitive
    {
        public List<PathData> Path { get; set; } = new List<PathData>();

        /// <summary>
        /// Moves the current point of the current path.
        /// </summary>
        /// <param name="point">position that current point will be moved to</param>
        public void PathMoveTo(Point point)
        {
            var pathData = new PathData(PathDataType.PathMoveTo);
            pathData.Points[0] = point;
            Path.Add(pathData);
        }

        /// <summary>
        /// Adds a line to the path from the current point to position p.
        /// </summary>
        /// <param name="point">next point</param>
        public void PathLineTo(Point point)
        {
            var pathData = new PathData(PathDataType.PathLineTo);
            pathData.Points[0] = point;
            Path.Add(pathData);
        }

        /// <summary>
        /// Adds a line segment to the path from the current point to
        /// the beginning of the current sub-path, (the most recent 
        /// point passed to PathMoveTo()), and closes this sub-path.
        /// After this call the current point will be at the joined endpoint of the sub-path.
        /// </summary>
        public void PathClose()
        {
            var pathData = new PathData(PathDataType.PathClosePath);
            Path.Add(pathData);
        }

        public void PathCurveTo(Point control0, Point control1, Point end)
        {
            var pathData = new PathData(PathDataType.PathCurveTo);
            pathData.Points[0] = control0;
            pathData.Points[1] = control1;
            pathData.Points[2] = end;
            Path.Add(pathData);
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
                return;
            }

            Path.Capacity = Path.Count + amax - amin + 1;
            for (int a = amin; a <= amax; a++)
            {
                Point c = CirclePoints[a % CirclePoints.Length];
                var p = new Point(center.X + c.X * radius, center.Y + c.Y * radius);
                if (a == amin)
                {
                    PathMoveTo(p);
                }
                else
                {
                    PathLineTo(p);
                }
            }
        }

        //TODO PathArcTo and other path APIs
    }
}
