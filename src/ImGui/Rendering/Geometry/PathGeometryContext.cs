using System;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal class PathGeometryContext : IDisposable
    {
        public PathGeometryContext(PathGeometry pathGeometry)
        {
            list = pathGeometry.Path;
        }

        /// <summary>
        /// Moves the current point of the current path.
        /// </summary>
        /// <param name="point">position that current point will be moved to</param>
        public void MoveTo(Point point)
        {
            list.Add(new MoveToCommand(point));
        }

        /// <summary>
        /// Adds a line to the path from the current point to position p.
        /// </summary>
        /// <param name="point">next point</param>
        public void LineTo(Point point)
        {
            list.Add(new LineToCommand(point));
        }

        /// <summary>
        /// Adds an arc segement the the path. It is not started from current point.
        /// </summary>
        /// <param name="center">center</param>
        /// <param name="radius">radius</param>
        /// <param name="amin">min angle factor</param>
        /// <param name="amax">max angle factor</param>
        /// <remarks>
        /// about amin and amax:
        /// range: {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}
        /// 0: →
        /// 3: ↓
        /// 6: ←
        /// 9: ↑
        /// 12: →
        /// amin &lt; amax
        /// </remarks>
        public void ArcFast(Point center, double radius, int amin, int amax)
        {
            list.Add(new ArcCommand(center, radius, amin, amax));
        }

        public void Arc(Point center, double radius, double minAngle, double maxAngle)
        {
            Ellipse(center, radius, radius, minAngle, maxAngle);
        }

        public void Ellipse(Point center, double radiusX, double radiusY, double fromAngle, double toAngle)
        {
            list.Add(new EllipseCommand(center, radiusX, radiusY, fromAngle, toAngle));
        }

        public void CurveTo(Point c1, Point c2, Point end, int numSegments = 0)
        {
            list.Add(new CurveToCommand(c1, c2, end));
        }

        public void Finish()
        {
            if (list.Count > 0)
            {
                list.Add(list[0]);
            }
        }

        public void Close()
        {
            if(disposed)
            {
                throw new ObjectDisposedException(nameof(PathGeometryContext));
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
            }
        }

        private bool disposed;
        private List<PathCommand> list;
    }
}