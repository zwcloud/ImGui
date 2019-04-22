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
                foreach (var figure in pathGeometry.Figures)
                {
                    Path.Clear();
                    var currentPoint = figure.StartPoint;
                    Path.Add(currentPoint);
                    foreach (var segment in figure.Segments)
                    {
                        switch (segment)
                        {
                            case ArcSegment arcSegment:
                                break;
                            case CubicBezierSegment cubicBezierSegment:
                                break;
                            case LineSegment lineSegment:
                                if (lineSegment.IsStroked)
                                {
                                    unsafe
                                    {
                                        var scratch = stackalloc Point[2];
                                        scratch[0] = currentPoint;
                                        scratch[1] = lineSegment.Point;
                                        AddPolyline(scratch, 2, pen.LineColor, false, pen.LineWidth);
                                        Path.Add(lineSegment.Point);
                                        currentPoint = lineSegment.Point;
                                    }
                                }
                                break;
                            case PolyCubicBezierSegment polyCubicBezierSegment:
                                break;
                            case PolyLineSegment polyLineSegment:
                                break;
                            case PolyQuadraticBezierSegment polyQuadraticBezierSegment:
                                break;
                            case QuadraticBezierSegment quadraticBezierSegment:
                                break;
                        }
                    }

                    if (figure.IsFilled && brush != null)
                    {
                        AddConvexPolyFilled(Path, brush.FillColor, true);
                    }
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