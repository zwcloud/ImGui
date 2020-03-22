using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui.Rendering
{
    internal sealed partial class EllipseGeometry : Geometry
    {
        #region Constructors

        /// <summary>
        ///
        /// </summary>
        public EllipseGeometry()
        {
        }

        /// <summary>
        /// Constructor - sets the ellipse to the paramters with the given transformation
        /// </summary>
        public EllipseGeometry(Rect rect)
        {
            if (rect.IsEmpty)
            {
                throw new System.ArgumentNullException(nameof(rect));
            }

            RadiusX = (rect.Right - rect.X) * (1.0 / 2.0);
            RadiusY = (rect.Bottom - rect.Y) * (1.0 / 2.0);
            Center = new Point(rect.X + RadiusX, rect.Y + RadiusY);
        }

        /// <summary>
        /// Constructor - sets the ellipse to the parameters
        /// </summary>
        public EllipseGeometry(
            Point center,
            double radiusX,
            double radiusY)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        //TODO support rotation

        #endregion

        public Point Center { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }

        internal override PathGeometryData GetPathGeometryData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        internal Point[] GetPointList()
        {
            Point[] points = new Point[GetPointCount()];

            unsafe
            {
                fixed (Point* pPoints = points)
                {
                    EllipseGeometry.GetPointList(pPoints, GetPointCount(), Center, RadiusX, RadiusY);
                }
            }

            return points;
        }

        private unsafe static void GetPointList(Point* points, uint pointsCount, Point center, double radiusX, double radiusY)
        {
            System.Diagnostics.Debug.Assert(pointsCount >= c_pointCount);

            radiusX = Math.Abs(radiusX);
            radiusY = Math.Abs(radiusY);

            // Set the X coordinates
            double mid = radiusX * c_arcAsBezier;

            points[0].X = points[1].X = points[11].X = points[12].X = center.X + radiusX;
            points[2].X = points[10].X = center.X + mid;
            points[3].X = points[9].X = center.X;
            points[4].X = points[8].X = center.X - mid;
            points[5].X = points[6].X = points[7].X = center.X - radiusX;

            // Set the Y coordinates
            mid = radiusY * c_arcAsBezier;

            points[2].Y = points[3].Y = points[4].Y = center.Y + radiusY;
            points[1].Y = points[5].Y = center.Y + mid;
            points[0].Y = points[6].Y = points[12].Y = center.Y;
            points[7].Y = points[11].Y = center.Y - mid;
            points[8].Y = points[9].Y = points[10].Y = center.Y - radiusY;
        }

        private uint GetPointCount() { return c_pointCount; }
        private uint GetSegmentCount() { return c_segmentCount; }

        #region Static Data

        // Approximating a 1/4 circle with a Bezier curve                _
        internal const double c_arcAsBezier = 0.5522847498307933984; // =( \/2 - 1)*4/3

        private const UInt32 c_segmentCount = 4;
        private const UInt32 c_pointCount = 13;

        #endregion
    }
}
