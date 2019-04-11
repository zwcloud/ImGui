using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class PathGeometryContext
    {
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

        public void PathArcFast(Point center, double radius, int amin, int amax)
        {
            Path.Add(new ArcCommand(center, radius, amin, amax));
        }

        private List<PathCommand> Path = new List<PathCommand>();
    }
}