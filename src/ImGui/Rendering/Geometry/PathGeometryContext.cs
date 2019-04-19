namespace ImGui.Rendering
{
    internal class PathGeometryContext
    {
        public PathGeometryContext(IPathList pathList)
        {
            list = pathList;
        }

        /// <summary>
        /// Moves the current point of the current path.
        /// </summary>
        /// <param name="point">position that current point will be moved to</param>
        public void MoveTo(Point point)
        {
            list.Path.Add(new MoveToCommand(point));
        }

        /// <summary>
        /// Adds a line to the path from the current point to position p.
        /// </summary>
        /// <param name="point">next point</param>
        public void LineTo(Point point)
        {
            list.Path.Add(new LineToCommand(point));
        }

        public void ArcFast(Point center, double radius, int amin, int amax)
        {
            list.Path.Add(new ArcCommand(center, radius, amin, amax));
        }

        public void CurveTo(Point c1, Point c2, Point end, int numSegments = 0)
        {
            list.Path.Add(new CurveToCommand(c1, c2, end));
        }

        public void Finish()
        {
            if (list.Path.Count > 0)
            {
                list.Path.Add(list.Path[0]);
            }
        }

        private IPathList list;
    }
}