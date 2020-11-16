//#define ForceStrokePathGeometry //Open this when debugging PathGeometry rendering
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;
using ImGui.Rendering.Composition;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.GraphicsImplementation
{
    internal partial class BuiltinGeometryRenderer : GeometryRenderer
    {
        private bool EnsureDrawCommand()
        {
            var clipRect = ClipRectStack.Peek();
            if (clipRect.IsEmpty)
            {
                //completely clipped
                //no render
                return false;
            }

            var cmdBuf = this.ShapeMesh.CommandBuffer;
            if (cmdBuf.Count == 0)
            {
                AddCmd();
                return true;
            }

            var lastCmd = cmdBuf[cmdBuf.Count - 1];
            if (lastCmd.ClipRect != clipRect)
            {
                AddCmd();
                return true;
            }

            return true;

            void AddCmd()
            {
                var cmd = new DrawCommand
                {
                    ClipRect = clipRect,
                    ElemCount = 0,
                    TextureData = null,
                };
                cmdBuf.Add(cmd);
            }
        }

        public override void DrawLine(Pen pen, Point point0, Point point1)
        {
            Debug.Assert(pen != null);
            if (!EnsureDrawCommand())
            {
                return;
            }

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
            if (!EnsureDrawCommand())
            {
                return;
            }

            unsafe
            {
                Point* scratchForRectangle = stackalloc Point[4];
                scratchForRectangle[0] = rectangle.TopLeft;
                scratchForRectangle[1] = rectangle.TopRight;
                scratchForRectangle[2] = rectangle.BottomRight;
                scratchForRectangle[3] = rectangle.BottomLeft;

                if (brush != null)
                {
                    this.AddConvexPolyFilled(scratchForRectangle, 4, brush.FillColor, false);
                }

                if (pen != null)
                {
                    this.AddPolyline(scratchForRectangle, 4, pen.LineColor, true, pen.LineWidth);
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
            Debug.Assert(brush != null || pen != null);
            Debug.Assert(radiusX >= 0 && radiusY >= 0);
            if (!EnsureDrawCommand())
            {
                return;
            }

            var curve = new EllipseCurve(center.X, center.Y, radiusX, radiusY, 0, Math.PI * 2, true, 0);
            var count = (int)((radiusX + radiusY));
            var unit = 1.0 / count;
            IList<Point> points = new List<Point>(count);
            for (int i = 0; i < count; i++)
            {
                var p = curve.getPoint(unit * i);
                points.Add(p);
            }

            if (pen != null)
            {
                this.AddPolyline(points, pen.LineColor, true, pen.LineWidth);
            }

            if (brush != null)
            {
                this.AddConvexPolyFilled(points, brush.FillColor, false);
            }
        }

        public override void DrawGeometry(Brush brush, Pen pen, Geometry geometry)
        {
            Debug.Assert((brush != null || pen != null) && geometry != null);
            if (!EnsureDrawCommand())
            {
                return;
            }

            if (geometry is PathGeometry pathGeometry)
            {
                var offset = geometry.Offset;
                foreach (var figure in pathGeometry.Figures)
                {
                    Path.Clear();
                    var currentPoint = figure.StartPoint;
                    Path.Add(currentPoint + offset);
                    foreach (var segment in figure.Segments)
                    {
                        switch (segment)
                        {
                            case ArcSegment arcSegment:
                            {
                                if (arcSegment.Size == Size.Zero)
                                {
                                    continue;
                                }
                                var generatedPoints = arcSegment.Flatten(currentPoint);
                                if (arcSegment.IsStroked && pen != null)
                                {
                                    this.AddPolyline(generatedPoints, offset, pen.LineColor, false,
                                        pen.LineWidth);
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
                                    this.AddPolyline(generatedPoints, offset, pen.LineColor, false, pen.LineWidth);
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
                                        this.AddPolyline(scratch, 2, offset, pen.LineColor, false, pen.LineWidth);
                                    }
                                }
                                Path.Add(lineSegment.Point);
                                break;
                            case PolyCubicBezierSegment polyCubicBezierSegment:
                            {
                                var generatedPoints = this.CubicBezier_GeneratePolyLinePoints(currentPoint, polyCubicBezierSegment.Points);
                                if (polyCubicBezierSegment.IsStroked && pen != null)
                                {
                                    this.AddPolyline(generatedPoints, offset, pen.LineColor, false, pen.LineWidth);
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
                                        this.AddPolyline(scratch, pointCount, offset, pen.LineColor, false, pen.LineWidth);
                                    }
                                }
                                Path.AddRange(points);
                                break;
                            case PolyQuadraticBezierSegment polyQuadraticBezierSegment:
                            {
                                var generatedPoints = this.QuadraticBezier_GeneratePolyLinePoints(currentPoint, polyQuadraticBezierSegment.Points);
                                if (polyQuadraticBezierSegment.IsStroked && pen != null)
                                {
                                    this.AddPolyline(generatedPoints, offset, pen.LineColor, false, pen.LineWidth);
                                }
                                Path.AddRange(generatedPoints);
                            }
                            break;
                            case QuadraticBezierSegment quadraticBezierSegment:
                            {
                                List<Point> generatedPoints;
                                unsafe
                                {
                                    var scratch = stackalloc Point[2];
                                    scratch[0] = quadraticBezierSegment.ControlPoint;
                                    scratch[1] = quadraticBezierSegment.EndPoint;
                                    generatedPoints = this.QuadraticBezier_GeneratePolyLinePoints(currentPoint, scratch, 2);
                                }
                                if (quadraticBezierSegment.IsStroked && pen != null)
                                {
                                    this.AddPolyline(generatedPoints, offset, pen.LineColor, false, pen.LineWidth);
                                }
                                Path.AddRange(generatedPoints);
                            }
                            break;
                        }
                        currentPoint = Path[Path.Count - 1];
                    }

                    if (figure.IsFilled && brush != null)
                    {
                        this.AddConvexPolyFilled(Path, offset, brush.FillColor, true);
                    }

#if ForceStrokePathGeometry
                    if (pen != null)
                    {
                        this.AddPolyline(Path, offset, pen.LineColor, false, pen.LineWidth);
                    }
#endif
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void DrawImage(ITexture texture, Rect rectangle)
        {
            var uvMin = new Point(0, 0);
            var uvMax = new Point(1, 1);
            DrawImage(texture, rectangle, uvMin, uvMax);
        }
        
        public override void DrawImage(ITexture texture, Rect rectangle, Point uvMin, Point uvMax)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            Color tintColor = Color.White;//TODO define tint color as a brush property

            //add a new draw command
            //TODO check if we need to add a new draw command
            DrawCommand cmd = new DrawCommand();
            cmd.ClipRect = ClipRectStack.Peek();
            cmd.TextureData = texture;
            this.ImageMesh.CommandBuffer.Add(cmd);

            this.ImageMesh.PrimReserve(6, 4);
            AddImageRect(rectangle, uvMin, uvMax, tintColor);
        }

        public override void DrawImage(ITexture image, Rect rect,
            (double top, double right, double bottom, double left) slice)
        {
            if (!image.Valid) { throw new InvalidOperationException("Texture is not valid for rendering."); }

            if (!ClipRectStack.Peek().IntersectsWith(rect))
            {
                return;
            }

            if (slice.top == 0 && slice.right == 0 && slice.bottom == 0 && slice.left == 0)
            {
                DrawImage(image, rect);
                return;
            }

            var (top, right, bottom, left) = slice;
            Point uv0 = new Point(left / image.Width, top / image.Height);
            Point uv1 = new Point(1 - right / image.Width, 1 - bottom / image.Height);

            //     | L |   | R |
            // ----a---b---c---+
            //   T | 1 | 2 | 3 |
            // ----d---e---f---g
            //     | 4 | 5 | 6 |
            // ----h---i---j---k
            //   B | 7 | 8 | 9 |
            // ----+---l---m---n

            var a = rect.TopLeft;
            var b = a + new Vector(left, 0);
            var c = rect.TopRight + new Vector(-right, 0);

            var d = a + new Vector(0, top);
            var e = b + new Vector(0, top);
            var f = c + new Vector(0, top);
            var g = f + new Vector(right, 0);

            var h = rect.BottomLeft + new Vector(0, -bottom);
            var i = h + new Vector(left, 0);
            var j = rect.BottomRight + new Vector(-right, -bottom);
            var k = j + new Vector(right, 0);

            var l = i + new Vector(0, bottom);
            var m = rect.BottomRight + new Vector(-right, 0);
            var n = rect.BottomRight;

            var uv_a = new Point(0, 0);
            var uv_b = new Point(uv0.X, 0);
            var uv_c = new Point(uv1.X, 0);

            var uv_d = new Point(0, uv0.Y);
            var uv_e = new Point(uv0.X, uv0.Y);
            var uv_f = new Point(uv1.X, uv0.Y);
            var uv_g = new Point(1, uv0.Y);

            var uv_h = new Point(0, uv1.Y);
            var uv_i = new Point(uv0.X, uv1.Y);
            var uv_j = new Point(uv1.X, uv1.Y);
            var uv_k = new Point(1, uv1.Y);

            var uv_l = new Point(uv0.X, 1);
            var uv_m = new Point(uv1.X, 1);
            var uv_n = new Point(1, 1);

            Color tintColor = Color.White;//TODO define tint color as a brush property

            //add a new draw command
            DrawCommand cmd = new DrawCommand();
            cmd.ClipRect = ClipRectStack.Peek();
            cmd.TextureData = image;
            this.ImageMesh.CommandBuffer.Add(cmd);

            this.ImageMesh.PrimReserve(6 * 9, 4 * 9);

            this.AddImageRect(a, e, uv_a, uv_e, tintColor); //1
            this.AddImageRect(b, f, uv_b, uv_f, tintColor); //2
            this.AddImageRect(c, g, uv_c, uv_g, tintColor); //3
            this.AddImageRect(d, i, uv_d, uv_i, tintColor); //4
            this.AddImageRect(e, j, uv_e, uv_j, tintColor); //5
            this.AddImageRect(f, k, uv_f, uv_k, tintColor); //6
            this.AddImageRect(h, l, uv_h, uv_l, tintColor); //7
            this.AddImageRect(i, m, uv_i, uv_m, tintColor); //8
            this.AddImageRect(j, n, uv_j, uv_n, tintColor); //9
        }

        public override void DrawGlyphRun(Brush foregroundBrush, GlyphRun glyphRun)
        {
            AddText(glyphRun.Origin, glyphRun.GlyphDataList, glyphRun.GlyphOffsets, glyphRun.FontFamily, glyphRun.FontSize, foregroundBrush.FillColor);
        }

        public override void DrawText(Brush foregroundBrush, FormattedText formattedText)
        {
            AddText(formattedText.OriginPoint, formattedText.GlyphDataList, formattedText.GlyphOffsets, formattedText.FontFamily, formattedText.FontSize, foregroundBrush.FillColor);
        }

        public override void PushClip(Geometry clipGeometry)
        {
            if (!(clipGeometry is RectangleGeometry))
            {
                throw new NotImplementedException(
                    "Non-rectangle clip geometry isn't supported");
            }

            var rectangleGeometry = (RectangleGeometry)clipGeometry;
            var rect = rectangleGeometry.Rect;
            //TODO Only a rectangle clip region is supported.
            PushClipRect(rect);
        }

        public override void Pop()
        {
            //TODO Only the clip rect stack is being popped
            PopClipRect();
        }

        #region Overrides of RecordReader
        public override void OnBeforeRead()
        {
            var shapeMesh = MeshPool.ShapeMeshPool.Get();
            shapeMesh.Clear();
            shapeMesh.CommandBuffer.Add(DrawCommand.Default);//TODO remove this, since we have EnsureDrawCommand().
            var textMesh = MeshPool.TextMeshPool.Get();
            textMesh.Clear();
            var imageMesh = MeshPool.ImageMeshPool.Get();
            imageMesh.Clear();

            SetShapeMesh(shapeMesh);
            SetTextMesh(textMesh);
            SetImageMesh(imageMesh);
        }

        public override void OnAfterRead(MeshList meshList)
        {
            meshList.AddOrUpdateShapeMesh(this.ShapeMesh);
            meshList.AddOrUpdateTextMesh(this.TextMesh);
            meshList.AddOrUpdateImageMesh(this.ImageMesh);

            SetShapeMesh(null);
            SetTextMesh(null);
            SetImageMesh(null);
        }
        #endregion

        protected override void DisposeCore()
        {

        }
    }
}