using System;
using ImGui.Common.Primitive;
using System.Collections.Generic;
using ImGui.Common;

namespace ImGui.Rendering
{
    internal class PathPrimitive : Primitive
    {
        public List<PathCommand> Path { get; set; } = new List<PathCommand>();

        /// <summary>
        /// Moves the current point of the current path.
        /// </summary>
        /// <param name="point">position that current point will be moved to</param>
        public void PathMoveTo(Point point)
        {
            Path.Add(new MoveToCommand(point));
        }

        /// <summary>
        /// Adds a line to the path from the current point to position p.
        /// </summary>
        /// <param name="point">next point</param>
        public void PathLineTo(Point point)
        {
            Path.Add(new LineToCommand(point));
        }

        /// <summary>
        /// Adds a line segment to the path from the current point to
        /// the beginning of the current sub-path, (the most recent 
        /// point passed to PathMoveTo()), and closes this sub-path.
        /// After this call the current point will be at the joined endpoint of the sub-path.
        /// </summary>
        public void PathClose()
        {
            Path.Add(new ClosePathCommand());
        }

        public void PathCurveTo(Point control0, Point control1, Point end)
        {
            Path.Add(new CurveToCommand(control0, control1, end));
        }

        public StrokeCommand PathStroke(double lineWidth, Color lineColor)
        {
            var cmd = new StrokeCommand(lineWidth, lineColor);
            Path.Add(cmd);
            return cmd;
        }

        public FillCommand PathFill(Color fillColor)
        {
            var cmd = new FillCommand(fillColor);
            Path.Add(cmd);
            return cmd;
        }

        public void PathRect(Rect rect, float rounding = 0.0f, int roundingCorners = 0x0F) =>
            this.PathRect(rect.Min, rect.Max, rounding, roundingCorners);

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
                this.PathArcFast(new Point(a.X + r0, a.Y + r0), r0, 6, 9);
                this.PathArcFast(new Point(b.X - r1, a.Y + r1), r1, 9, 12);
                this.PathArcFast(new Point(b.X - r2, b.Y - r2), r2, 0, 3);
                this.PathArcFast(new Point(a.X + r3, b.Y - r3), r3, 3, 6);
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
        /// Starts from +x, then clock-wise to +y, -x,-y, then ends at +x.
        /// </summary>
        /// <param name="center">the center of the arc</param>
        /// <param name="radius">the radius of the arc</param>
        /// <param name="amin">angle1 = amin * 2π * 1/12</param>
        /// <param name="amax">angle1 = amax * 2π * 1/12</param>
        public void PathArcFast(Point center, double radius, int amin, int amax)
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

        public void PathClear()
        {
            this.Path.Clear();
        }

        //TODO PathArcTo and other path APIs
    }
}
