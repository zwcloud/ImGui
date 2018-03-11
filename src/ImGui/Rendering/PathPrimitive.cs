using ImGui.Common.Primitive;
using System.Collections.Generic;

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
        }

        //TODO PathArcTo and other path APIs
    }
}
