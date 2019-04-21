using System.Diagnostics;
using ImGui.Development.DebuggerViews;

namespace ImGui.Rendering
{
    internal class MoveToCommand : PathCommand
    {
        public Point Point { get; set;}

        public MoveToCommand(Point point) : base(PathCommandType.PathMoveTo)
        {
            this.Point = point;
        }
    }

    internal class LineToCommand : PathCommand
    {
        public Point Point { get; set;}

        public LineToCommand(Point point) : base(PathCommandType.PathLineTo)
        {
            this.Point = point;
        }
    }

    internal class CurveToCommand : PathCommand
    {
        public Point ControlPoint0 { get; set;}
        public Point ControlPoint1 { get; set;}
        public Point EndPoint { get; set;}

        public CurveToCommand(Point control0, Point control1, Point end) : base(PathCommandType.PathCurveTo)
        {
            this.ControlPoint0 = control0;
            this.ControlPoint1 = control1;
            this.EndPoint = end;
        }
    }

    internal class ClosePathCommand : PathCommand
    {
        public ClosePathCommand() : base(PathCommandType.PathClosePath)
        {
        }
    }

    internal class ArcCommand : PathCommand
    {
        public Point Center { get; set; }
        public double Radius { get; }
        public int Amin { get; set; }
        public int Amax { get; set; }
        public ArcCommand(Point center, double radius, int amin, int amax) : base(PathCommandType.PathArc)
        {
            this.Center = center;
            this.Radius = radius;
            this.Amin = amin;
            this.Amax = amax;
        }
    }

    internal class EllipseCommand : PathCommand
    {
        public Point Center { get; set; }
        public double RadiusX { get; }
        public double RadiusY { get; }
        public double FromAngle { get; set; }
        public double ToAngle { get; set; }

        public EllipseCommand(Point center, double radiusX, double radiusY, double fromAngle, double toAngle) : base(PathCommandType.PathEllipse)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
            FromAngle = fromAngle;
            ToAngle = toAngle;
        }
    }

    internal class StrokeCommand : PathCommand
    {
        public double LineWidth { get; set;}
        public Color Color { get; set;}
        public StrokeCommand(double lineWidh, Color lineColor) : base(PathCommandType.Stroke)
        {
            this.LineWidth = lineWidh;
            this.Color = lineColor;
        }
    }

    internal class FillCommand : PathCommand
    {
        public Color Color { get; set; }
        public FillCommand(Color fillColor) : base(PathCommandType.Fill)
        {
            this.Color = fillColor;
        }
    }

    [DebuggerTypeProxy(typeof(PathCommandDebuggerView))]
    internal class PathCommand
    {
        public PathCommandType Type { get; set; }

        protected PathCommand(PathCommandType type)
        {
            this.Type = type;
        }
    }
}
