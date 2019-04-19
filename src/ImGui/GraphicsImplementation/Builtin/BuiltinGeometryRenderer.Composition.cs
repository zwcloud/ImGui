using System;
using System.Diagnostics;
using ImGui.GraphicsAbstraction;
using ImGui.Rendering;
using ImGui.Rendering.Composition;

namespace ImGui.GraphicsImplementation
{
    internal partial class BuiltinGeometryRenderer : RecordReader
    {
        public override void DrawLine(Pen pen, Point point0, Point point1)
        {
            Debug.Assert(pen != null);
            unsafe
            {
                Point* scratchForLine = stackalloc Point[2];
                scratchForLine[0] = point0;
                scratchForLine[1] = point1;
                AddPolyline(scratchForLine, 2, pen.LineColor, false, pen.LineWidth);
            }
        }

        public override void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
            Debug.Assert(brush != null || pen != null);
            unsafe
            {
                Point* scratchForRectangle = stackalloc Point[4];
                scratchForRectangle[0] = rectangle.TopLeft;
                scratchForRectangle[1] = rectangle.TopRight;
                scratchForRectangle[2] = rectangle.BottomRight;
                scratchForRectangle[3] = rectangle.BottomLeft;

                if(pen != null)
                {
                    AddPolyline(scratchForRectangle, 4, pen.LineColor, true, pen.LineWidth);
                }

                if (brush != null)
                {
                    AddConvexPolyFilled(scratchForRectangle, 4, brush.FillColor, false);
                }
            }
        }

        public override void DrawRoundedRectangle(Brush brush, Pen pen, Rect rectangle, double radiusX, double radiusY)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawGeometry(Brush brush, Pen pen, Geometry geometry)
        {
            Debug.Assert((brush != null || pen != null) && geometry != null);

            if (geometry is PathGeometry pathGeometry)
            {
                var paths = pathGeometry.Path;
                foreach (var pathCommand in paths)
                {
                    switch (pathCommand)
                    {
                        case MoveToCommand moveToCommand:
                            PathMoveTo(moveToCommand.Point);
                            break;
                        case LineToCommand lineToCommand:
                            PathLineTo(lineToCommand.Point);
                            break;
                        case ClosePathCommand closePathCommand:
                            PathClose();
                            break;
                        case CurveToCommand curveToCommand:
                            PathBezierCurveTo(curveToCommand.ControlPoint0, curveToCommand.ControlPoint1, curveToCommand.EndPoint);
                            break;
                        case ArcCommand a:
                            PathArcFast(a.Center, a.Radius, a.Amin, a.Amax);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                if (brush != null)
                {
                    PathFillPreserve(brush.FillColor);
                }

                if (pen != null)
                {
                    PathStroke(pen.LineColor, false, pen.LineWidth);
                }
            }
        }

        public override void DrawImage(Image image, Rect rectangle)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawDrawing(Drawing drawing)
        {
            throw new System.NotImplementedException();
        }
    }
}