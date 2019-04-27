using System;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal class PathGeometry : Geometry
    {
        public List<PathFigure> Figures { get; set; } = new List<PathFigure>();

        /// <summary>
        /// Gets or sets a value that determines how the intersecting areas contained in this PathGeometry are combined.
        /// </summary>
        public FillRule FillRule { get; set; } = FillRule.EvenOdd;

        /// <summary>
        /// GetPathGeometryData - returns a struct which contains this Geometry represented
        /// as a path geometry's serialized format.
        /// </summary>
        internal override PathGeometryData GetPathGeometryData()
        {
            PathGeometryData data = new PathGeometryData();
            data.FillRule = FillRule;
            data.Offset = Offset;

            //TODO implement this when StreamGeometry is implemented as a light-weight alternative to PathGeometry
#if StreamGeometry
            ByteStreamGeometryContext ctx = new ByteStreamGeometryContext();

            int figureCount = Figures == null ? 0 : Figures.Count;

            for (int i = 0; i < figureCount; i++)
            {
                Figures[i.SerializeData(ctx);
            }

            ctx.Close();
            data.SerializedData = ctx.GetData();
#endif
            return data;
        }

        public void PathMoveTo(Point point)
        {
        }

        public void PathLineTo(Point point)
        {
        }

        public void PathClose()
        {
        }

        public void PathCurveTo(Point control0, Point control1, Point end)
        {
        }

        public StrokeCommand PathStroke(double lineWidth, Color lineColor)
        {
            var cmd = new StrokeCommand(lineWidth, lineColor);
            return cmd;
        }

        public FillCommand PathFill(Color fillColor)
        {
            var cmd = new FillCommand(fillColor);
            return cmd;
        }

        public void PathRect(Rect rect, float rounding = 0.0f, int roundingCorners = 0x0F) =>
            this.PathRect(rect.Min, rect.Max, rounding, roundingCorners);

        public void PathRect(Point a, Point b, float rounding = 0.0f, int roundingCorners = 0x0F)
        {
        }

        public void PathArcFast(Point center, double radius, int amin, int amax)
        {
        }

        public void PathClear()
        {
        }


    }
}
