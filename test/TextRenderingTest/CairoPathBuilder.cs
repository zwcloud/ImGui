using ImGui;
using ImGui.UnitTest;

namespace TextRenderingTest
{
    public class CairoPathBuilder
    {
        private readonly Cairo.Context g;
        private readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();

        public CairoPathBuilder(Cairo.Context context, double offsetX, double offsetY, double scale)
        {
            this.g = context;
        }

        #region Implementation of ITextPathBuilder

        public void PathClear()
        {
            g.NewPath();
            builder.AppendLine("PathClear.");
        }

        public void PathMoveTo(Point point)
        {
            g.MoveTo(point.X, point.Y);
            var x = point.X;
            var y = point.Y;
            g.UserToDevice(ref x, ref y);
            builder.AppendLine(string.Format("move to ({0:0.00}, {1:0.00})", x, y));
        }

        public void PathLineTo(Point point)
        {
            g.LineTo(point.X, point.Y);
            var x = point.X;
            var y = point.Y;
            g.UserToDevice(ref x, ref y);
            builder.AppendLine(string.Format("line to ({0:0.00}, {1:0.00})", x, y));
        }

        public void PathClose()
        {
            g.ClosePath();
            builder.AppendLine("PathClose.");
        }

        public void PathAddBezier(Point start, Point control, Point end)
        {
            builder.AppendLine("bezier start");
            PathLineTo(start);
            PathLineTo(control);
            PathLineTo(end);

            g.CurveTo(start.ToPointD(), control.ToPointD(), end.ToPointD());

            builder.AppendLine("bezier end");
        }

        public void AddContour(Color color)
        {
            g.ClosePath();
            g.Stroke();
            builder.AppendLine("AddContour.");
        }

        #endregion

        internal string Result => builder.ToString();
    }
}