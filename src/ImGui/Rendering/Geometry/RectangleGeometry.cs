using System;
using System.Diagnostics;

namespace ImGui.Rendering
{
    internal class RectangleGeometry : Geometry
    {
        public RectangleGeometry(Rect rect)
        {
            this.Rect = rect;
        }

        public RectangleGeometry(Rect rect, double radiusX, double radiusY)
        {
            Rect = rect;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public double RadiusX { get; set; }

        public double RadiusY { get; set; }

        public Rect Rect { get; set; }

        public bool IsEmpty() => this.Rect.IsEmpty;

        internal static bool IsRounded(double radiusX, double radiusY)
        {
            return (radiusX != 0.0) && (radiusY != 0.0);
        }

        internal bool IsRounded()
        {
            return RadiusX != 0.0 && RadiusY != 0.0;
        }

        internal override PathGeometryData GetPathGeometryData()
        {
            throw new NotImplementedException();
        }

        internal PathFigure GetPathFigure()
        {
            if (IsEmpty())
            {
                return null;
            }

            double radiusX = RadiusX;
            double radiusY = RadiusY;
            Rect rect = Rect;

            if (IsRounded(radiusX, radiusY))
            {
                Point[] points = GetPointList(rect, radiusX, radiusY);

                var figure = new PathFigure(
                    points[0],
                    new PathSegment[]
                    {
                        new CubicBezierSegment(points[1], points[2], points[3], true),
                        new LineSegment(points[4], true),
                        new CubicBezierSegment(points[5], points[6], points[7], true),
                        new LineSegment(points[8], true),
                        new CubicBezierSegment(points[9], points[10], points[11], true),
                        new LineSegment(points[12], true),
                        new CubicBezierSegment(points[13], points[14], points[15], true),
                        new LineSegment(points[16], true)
                    },
                    true // closed
                );

                return figure;
            }
            else
            {
                var figure = new PathFigure(
                    rect.TopLeft,
                    new PathSegment[]{
                        new PolyLineSegment(
                        new Point[]
                        {
                            rect.TopRight,
                            rect.BottomRight,
                            rect.BottomLeft,
                            rect.TopLeft
                        },
                        true)},
                        true    // closed
                    );

                return figure;
            }
        }

        private Point[] GetPointList(Rect rect, double radiusX, double radiusY)
        {
            uint pointCount = GetPointCount(rect, radiusX, radiusY);
            Point[] points = new Point[pointCount];

            unsafe
            {
                fixed(Point *pPoints = points)
                {
                    RectangleGeometry.GetPointList(pPoints, pointCount, rect, radiusX, radiusY);
                }
            }

            return points;
        }

        private uint GetPointCount(Rect rect, double radiusX, double radiusY)
        {
            if (rect.IsEmpty)
            {
                return 0;
            }
            else if (IsRounded(radiusX, radiusY))
            {
                return c_roundedPointCount;
            }
            else
            {
                return c_squaredPointCount;
            }
        }

        private static unsafe void GetPointList(Point * points, uint pointsCount, Rect rect, double radiusX, double radiusY)
        {
            if (IsRounded(radiusX, radiusY))
            {
                // It is a rounded rectangle
                Debug.Assert(pointsCount >= c_roundedPointCount);

                radiusX = Math.Min(rect.Width * (1.0 / 2.0), Math.Abs(radiusX));
                radiusY = Math.Min(rect.Height * (1.0 / 2.0), Math.Abs(radiusY));

                double bezierX = ((1.0 - c_arcAsBezier) * radiusX);
                double bezierY = ((1.0 - c_arcAsBezier) * radiusY);

                points[1].X = points[0].X = points[15].X = points[14].X = rect.X;
                points[2].X = points[13].X = rect.X + bezierX;
                points[3].X = points[12].X = rect.X + radiusX;
                points[4].X = points[11].X = rect.Right - radiusX;
                points[5].X = points[10].X = rect.Right - bezierX;
                points[6].X = points[7].X = points[8].X = points[9].X = rect.Right;

                points[2].Y = points[3].Y = points[4].Y = points[5].Y = rect.Y;
                points[1].Y = points[6].Y = rect.Y + bezierY;
                points[0].Y = points[7].Y = rect.Y + radiusY;
                points[15].Y = points[8].Y = rect.Bottom - radiusY;
                points[14].Y = points[9].Y = rect.Bottom - bezierY;
                points[13].Y = points[12].Y = points[11].Y = points[10].Y = rect.Bottom;

                points[16] = points[0];
            }
            else
            {
                // The rectangle is not rounded
                Debug.Assert(pointsCount >= c_squaredPointCount);

                points[0].X = points[3].X = points[4].X = rect.X;
                points[1].X = points[2].X = rect.Right;

                points[0].Y = points[1].Y = points[4].Y = rect.Y;
                points[2].Y = points[3].Y = rect.Bottom;
            }
        }

        // Approximating a 1/4 circle with a Bezier curve                _
        internal const double c_arcAsBezier = 0.5522847498307933984; // =( \/2 - 1)*4/3
        // Rouneded
        static private UInt32 c_roundedPointCount = 17;
        // Squared
        private const UInt32 c_squaredPointCount = 5;
    }
}