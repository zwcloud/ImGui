namespace ImGui.Rendering
{
    public class LineSegment : PathSegment
    {
        public Point Point { get; set; }

        public LineSegment(Point point, bool isStroked)
        {
            Point = point;
            IsStroked = isStroked;
        }
    }
}