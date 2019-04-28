using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ImGui
{
    /// <summary>
    /// Extended class for System.Math
    /// </summary>
    internal class MathEx
    {
        /// <summary>
        /// Make sure value is greater or equal to zero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ClampTo0(double value)
        {
            if (value < 0)
            {
                return 0;
            }
            return value;
        }


        /// <summary>
        /// Clamps a value between a minimum float and maximum double value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        /// <summary>
        /// Clamps a value between a minimum float and maximum float value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        /// <summary>
        /// Clamps value between min and max and returns value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        /// <summary>
        ///   <para>Linearly interpolates between a and b by t.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * MathEx.Clamp01(t);
        }

        /// <summary>
        ///   <para>Linearly interpolates between a and b by t.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * MathEx.Clamp01(t);
        }

        /// <summary>
        /// Clamps value between 0 and 1 and returns value.
        /// </summary>
        public static double Clamp01(double value)
        {
            if (value < 0)
            {
                return 0;
            }
            if (value > 1)
            {
                return 1;
            }
            return value;
        }

        /// <summary>
        /// Clamps value between 0 and 1 and returns value.
        /// </summary>
        public static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }
            if (value > 1f)
            {
                return 1f;
            }
            return value;
        }

        public static int RoundToInt(double f)
        {
            return (int)Math.Round(f);
        }

        /// <summary>
        /// Check if number is zero, the error is 0.001
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AmostZero(double value)
        {
            return (int)(1000 * value) == 0;
        }

        /// <summary>
        /// Check if number is zero, the error is 0.001
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AmostZero(float value)
        {
            return (int)(1000 * value) == 0;
        }

        /// <summary>
        /// Check if two number is equal, the error is 0.001
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AmostEqual(double a, double b)
        {
            return AmostZero(a - b);
        }

        /// <summary>
        /// Check if two number is equal, the error is 0.001
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AmostEqual(float a, float b)
        {
            return AmostZero(a - b);
        }

        /// <summary>
        /// Get the inverse of the length of a vector
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="fail_value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NORMALIZE2F_OVER_ZERO(ref double VX, ref double VY)
        {
            var d2 = VX * VX + VY * VY;
            if (d2 > 0.0f)
            {
                var inv_len = 1.0 / Math.Sqrt(d2);
                VX *= inv_len;
                VY *= inv_len;
            }
        }

        /// <summary>
        /// Check if a point is in the __convex__ polyon.
        /// </summary>
        public static bool IsPointInPolygon(Point p, IReadOnlyList<Point> polygon)
        {
            double minX = polygon[0].X;
            double maxX = polygon[0].X;
            double minY = polygon[0].Y;
            double maxY = polygon[0].Y;
            for (int i = 1; i < polygon.Count; i++)
            {
                Point q = polygon[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if ((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y) &&
                    p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        /// <summary>
        /// Check if a point is in the __convex__ polyon with an offset. Every point in points is offset first before checking. The checking will not change points' position.
        /// </summary>
        public static bool IsPointInPolygon(Point p, IReadOnlyList<Point> polygon, Vector offset)
        {
            var polygon0 = polygon[0] + offset;
            double minX = polygon0.X;
            double maxX = polygon0.X;
            double minY = polygon0.Y;
            double maxY = polygon0.Y;
            for (int i = 1; i < polygon.Count; i++)
            {
                Point q = polygon[i] + offset;
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                var point = polygon[i] + offset;
                var point1 = polygon[j] + offset;
                if ((point.Y > p.Y) != (point1.Y > p.Y) &&
                    p.X < (point1.X - point.X) * (p.Y - point.Y) / (point1.Y - point.Y) + point.X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        /// <summary>
        /// Get the certroid of a polygon, convex or non-convex.
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/a/33852627/3427520
        ///</remarks>
        public static Point GetPolygonCentroid(Point[] verts)
        {
            var sum = 0.0;
            Point vsum = Point.Zero;

            var numVerts = verts.Length;
            for (int i = 0; i < numVerts; i++)
            {
                Point v1 = verts[i];
                Point v2 = verts[(i + 1) % numVerts];
                var cross = v1.X * v2.Y - v1.Y * v2.X;
                sum += cross;
                vsum = new Point(((v1.X + v2.X) * cross) + vsum.X, ((v1.Y + v2.Y) * cross) + vsum.Y);
            }

            var z = 1.0 / (3.0 * sum);
            return new Point(vsum.X * z, vsum.Y * z);
        }

        public static Point EvaluateCircle(Point center, double radius, double rad)
        {
            return EvaluateEllipse(center, radius, radius, rad);
        }

        public static Point EvaluateEllipse(Point center, double xHalfAxis, double yHalfAxis, double rad)
        {
            double x = xHalfAxis * Math.Cos(rad);
            double y = yHalfAxis * Math.Sin(rad);
            return new Point(x, y) + new Vector(center.X, center.Y);
        }

        public static Point EvaluateQuadraticBezier(Point startPoint, Point controlPoint, Point endPoint, double t)
        {
            var P0 = startPoint;
            var P1 = controlPoint;
            var P2 = endPoint;

            double x = (1 - t) * (1 - t) * P0.X + 2 * (1 - t) * t * P1.X + t * t * P2.X;
            double y = (1 - t) * (1 - t) * P0.Y + 2 * (1 - t) * t * P1.Y + t * t * P2.Y;
            return new Point(x, y);
        }

        public static double Deg2Rad(double degree)
        {
            return degree * Math.PI / 180;
        }

        public static double Rad2Deg(double radian)
        {
            return radian * 180 / Math.PI;
        }

        /// <summary>
        /// unit circle points: point at index i is the point at degree i of the unit circle.
        /// </summary>
        public static IReadOnlyList<Point> UnitCirclePoints = InitUnitCirclePoints();

        private static IReadOnlyList<Point> InitUnitCirclePoints()
        {
            Point[] result = new Point[360];
            for (int i = 0; i < result.Length; i++)
            {
                var a = MathEx.Deg2Rad(i);
                result[i].X = Math.Cos(a);
                result[i].Y = Math.Sin(a);
            }
            return result;
        }
    }
}