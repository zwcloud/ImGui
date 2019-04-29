//#define ForceStrokePathGeometry //Open this when debugging PathGeometry rendering
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.OSAbstraction.Graphics;
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
                this.AddPolyline(scratchForLine, 2, pen.LineColor, false, pen.LineWidth);
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

                if (pen != null)
                {
                    this.AddPolyline(scratchForRectangle, 4, pen.LineColor, true, pen.LineWidth);
                }

                if (brush != null)
                {
                    this.AddConvexPolyFilled(scratchForRectangle, 4, brush.FillColor, false);
                }
            }
        }

        public override void DrawRoundedRectangle(Brush brush, Pen pen, Rect rectangle, double radiusX, double radiusY)
        {
            Debug.Assert(brush != null || pen != null);

            var rectangleGeometry = new RectangleGeometry(rectangle, radiusX, radiusY);
            var geometry = new PathGeometry();
            geometry.Figures.Add(rectangleGeometry.GetPathFigure());

            DrawGeometry(brush, pen, geometry);
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
                            {
                                var generatedPoints = arcSegment.Flatten(currentPoint);
                                if (arcSegment.IsStroked && pen != null)
                                {
                                    this.AddPolyline(generatedPoints, pen.LineColor, false, pen.LineWidth);
                                }
                                Path.AddRange(generatedPoints);
                            }
                            break;
                            case CubicBezierSegment cubicBezierSegment:
                            {
                                List<Point> generatedPoints;
                                unsafe
                                {
                                    var scratch = stackalloc Point[3];
                                    scratch[0] = cubicBezierSegment.ControlPoint1;
                                    scratch[1] = cubicBezierSegment.ControlPoint2;
                                    scratch[2] = cubicBezierSegment.EndPoint;
                                    generatedPoints = this.CubicBezier_GeneratePolyLinePoints(currentPoint, scratch, 3);
                                }
                                if (cubicBezierSegment.IsStroked && pen != null)
                                {
                                    this.AddPolyline(generatedPoints, pen.LineColor, false, pen.LineWidth);
                                }
                                Path.AddRange(generatedPoints);
                            }
                            break;
                            case LineSegment lineSegment:
                                if (lineSegment.IsStroked && pen != null)
                                {
                                    unsafe
                                    {
                                        var scratch = stackalloc Point[2];
                                        scratch[0] = currentPoint;
                                        scratch[1] = lineSegment.Point;
                                        this.AddPolyline(scratch, 2, pen.LineColor, false, pen.LineWidth);
                                    }
                                }
                                Path.Add(lineSegment.Point);
                                break;
                            case PolyCubicBezierSegment polyCubicBezierSegment:
                            {
                                var generatedPoints = this.CubicBezier_GeneratePolyLinePoints(currentPoint, polyCubicBezierSegment.Points);
                                if (polyCubicBezierSegment.IsStroked && pen != null)
                                {
                                    this.AddPolyline(generatedPoints, pen.LineColor, false, pen.LineWidth);
                                }
                                Path.AddRange(generatedPoints);
                            }
                            break;
                            case PolyLineSegment polyLineSegment:
                                var points = polyLineSegment.Points;
                                if (polyLineSegment.IsStroked && pen != null)
                                {
                                    unsafe
                                    {
                                        var pointCount = 1 + points.Count;
                                        var scratch = stackalloc Point[pointCount];
                                        scratch[0] = currentPoint;
                                        for (int i = 0; i < points.Count; i++)
                                        {
                                            scratch[1 + i] = points[i];
                                        }
                                        this.AddPolyline(scratch, pointCount, pen.LineColor, false, pen.LineWidth);
                                    }
                                }
                                Path.AddRange(points);
                                break;
                            case PolyQuadraticBezierSegment polyQuadraticBezierSegment:
                                throw new NotImplementedException();
                                break;
                            case QuadraticBezierSegment quadraticBezierSegment:
                                throw new NotImplementedException();
                                break;
                        }
                        currentPoint = Path[Path.Count - 1];
                    }

                    if (figure.IsFilled && brush != null)
                    {
                        this.AddConvexPolyFilled(Path, brush.FillColor, true);
                    }

#if ForceStrokePathGeometry
                    if (pen != null)
                    {
                        this.AddPolyline(Path, pen.LineColor, false, pen.LineWidth);
                    }
#endif
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override void DrawImage(ITexture texture, Rect rectangle)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            Color tintColor = Color.White;//TODO define tint color, possibly as a style rule

            //add a new draw command
            //TODO check if we need to add a new draw command
            DrawCommand cmd = new DrawCommand();
            cmd.ClipRect = Rect.Big;
            cmd.TextureData = texture;
            this.ImageMesh.CommandBuffer.Add(cmd);

            var uvMin = new Point(0, 0);
            var uvMax = new Point(1, 1);
            this.ImageMesh.PrimReserve(6, 4);
            AddImageRect(rectangle, uvMin, uvMax, tintColor);
        }

        public override void DrawDrawing(Drawing drawing)
        {
            throw new System.NotImplementedException();
        }

    }
}