using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.GraphicsImplementation
{
    internal partial class BuiltinGeometryRenderer
    {
        #region Mesh

        /// <summary>
        /// Set the shape mesh that will be rendered into.
        /// </summary>
        /// <param name="mesh"></param>
        public void SetShapeMesh(Mesh mesh)
        {
            this.ShapeMesh = mesh;
        }
        /// <summary>
        /// Set the image mesh that will be rendered into.
        /// </summary>
        /// <param name="mesh"></param>
        public void SetImageMesh(Mesh mesh)
        {
            this.ImageMesh = mesh;
        }

        /// <summary>
        /// Set the text mesh that will be rendered into.
        /// </summary>
        /// <param name="textMesh"></param>
        public void SetTextMesh(TextMesh textMesh)
        {
            this.TextMesh = textMesh;
        }
        #endregion

        #region Shape
        /// <summary>
        /// Mesh (colored triangles)
        /// </summary>
        public Mesh ShapeMesh { get; private set; }

        //TODO remove duplicated code in AddPolyline(IList<Point> points,...) and AddPolyline(Point* points,...)

        
        /// <summary>
        /// Add a poly line.
        /// </summary>
        /// <param name="points">points</param>
        /// <param name="color">color</param>
        /// <param name="close">Should this method close the polyline for you? A line segment from the last point to first point will be added if this is true.</param>
        /// <param name="thickness">thickness</param>
        /// <param name="antiAliased">anti-aliased</param>
        public void AddPolyline(IList<Point> points, Color color, bool close,
            double thickness, bool antiAliased = false)
        {
            AddPolyline(points, Vector.Zero, color, close, thickness, antiAliased);
        }

        /// <summary>
        /// Add a poly line with offset.
        /// </summary>
        /// <param name="points">points</param>
        /// <param name="offset">offset that applies to every point</param>
        /// <param name="color">color</param>
        /// <param name="close">Should this method close the polyline for you? A line segment from the last point to first point will be added if this is true.</param>
        /// <param name="thickness">thickness</param>
        /// <param name="antiAliased">anti-aliased</param>
        public void AddPolyline(IList<Point> points, Vector offset, Color color, bool close,
            double thickness, bool antiAliased = false)
        {
            var pointsCount = points.Count;
            if (pointsCount < 2)
                return;

            int count = pointsCount;
            if (!close)
                count = pointsCount - 1;

            if (antiAliased)
            {
                throw new NotImplementedException();
            }
            else
            {
                // Non Anti-aliased Stroke
                int idxCount = count * 6;
                int vtxCount = count * 4; // FIXME: Not sharing edges
                this.ShapeMesh.PrimReserve(idxCount, vtxCount);

                for (int i1 = 0; i1 < count; i1++)
                {
                    int i2 = (i1 + 1) == pointsCount ? 0 : i1 + 1;
                    Point p1 = points[i1];
                    Point p2 = points[i2];
                    double dx = p2.x - p1.x;
                    double dy = p2.y - p1.y;
                    MathEx.NORMALIZE2F_OVER_ZERO(ref dx, ref dy);
                    dx *= (thickness * 0.5);
                    dy *= (thickness * 0.5);

                    p1 += offset;
                    p2 += offset;

                    var vertex0 = new DrawVertex { pos = new Point(p1.X + dy, p1.Y - dx), uv = Point.Zero, color = color };
                    var vertex1 = new DrawVertex { pos = new Point(p2.X + dy, p2.Y - dx), uv = Point.Zero, color = color };
                    var vertex2 = new DrawVertex { pos = new Point(p2.X - dy, p2.Y + dx), uv = Point.Zero, color = color };
                    var vertex3 = new DrawVertex { pos = new Point(p1.X - dy, p1.Y + dx), uv = Point.Zero, color = color };
                    this.ShapeMesh.AppendVertex(vertex0);
                    this.ShapeMesh.AppendVertex(vertex1);
                    this.ShapeMesh.AppendVertex(vertex2);
                    this.ShapeMesh.AppendVertex(vertex3);

                    this.ShapeMesh.AppendIndex(0);
                    this.ShapeMesh.AppendIndex(1);
                    this.ShapeMesh.AppendIndex(2);
                    this.ShapeMesh.AppendIndex(0);
                    this.ShapeMesh.AppendIndex(2);
                    this.ShapeMesh.AppendIndex(3);

                    this.ShapeMesh.currentIdx += 4;
                }
            }
        }

        /// <summary>
        /// Add a poly line.
        /// </summary>
        /// <param name="points">pointer to points data</param>
        /// <param name="pointsCount">number of points</param>
        /// <param name="color">color</param>
        /// <param name="close">Should this method close the polyline for you? A line segment from the last point to first point will be added if this is true.</param>
        /// <param name="thickness">thickness</param>
        /// <param name="antiAliased">anti-aliased</param>
        public unsafe void AddPolyline(Point* points, int pointsCount, Color color, bool close,
            double thickness, bool antiAliased = false)
        {
            AddPolyline(points, pointsCount, Vector.Zero, color, close, thickness, antiAliased);
        }

        /// <summary>
        /// Add a poly line with offset.
        /// </summary>
        /// <param name="points">pointer to points data</param>
        /// <param name="pointsCount">number of points</param>
        /// <param name="offset">offset that applies to every point</param>
        /// <param name="color">color</param>
        /// <param name="close">Should this method close the polyline for you? A line segment from the last point to first point will be added if this is true.</param>
        /// <param name="thickness">thickness</param>
        /// <param name="antiAliased">anti-aliased</param>
        public unsafe void AddPolyline(Point* points, int pointsCount, Vector offset, Color color, bool close, double thickness, bool antiAliased = false)
        {
            if (pointsCount < 2)
                return;

            int count = pointsCount;
            if (!close)
                count = pointsCount - 1;

            if (antiAliased)
            {
                throw new NotImplementedException();
            }
            else
            {
                // Non Anti-aliased Stroke
                int idxCount = count * 6;
                int vtxCount = count * 4; // FIXME: Not sharing edges
                this.ShapeMesh.PrimReserve(idxCount, vtxCount);

                for (int i1 = 0; i1 < count; i1++)
                {
                    int i2 = (i1 + 1) == pointsCount ? 0 : i1 + 1;
                    Point p1 = points[i1];
                    Point p2 = points[i2];
                    double dx = p2.x - p1.x;
                    double dy = p2.y - p1.y;
                    MathEx.NORMALIZE2F_OVER_ZERO(ref dx, ref dy);
                    dx *= (thickness * 0.5);
                    dy *= (thickness * 0.5);

                    p1 += offset;
                    p2 += offset;

                    var vertex0 = new DrawVertex { pos = new Point(p1.X + dy, p1.Y - dx), uv = Point.Zero, color = color };
                    var vertex1 = new DrawVertex { pos = new Point(p2.X + dy, p2.Y - dx), uv = Point.Zero, color = color };
                    var vertex2 = new DrawVertex { pos = new Point(p2.X - dy, p2.Y + dx), uv = Point.Zero, color = color };
                    var vertex3 = new DrawVertex { pos = new Point(p1.X - dy, p1.Y + dx), uv = Point.Zero, color = color };
                    this.ShapeMesh.AppendVertex(vertex0);
                    this.ShapeMesh.AppendVertex(vertex1);
                    this.ShapeMesh.AppendVertex(vertex2);
                    this.ShapeMesh.AppendVertex(vertex3);

                    this.ShapeMesh.AppendIndex(0);
                    this.ShapeMesh.AppendIndex(1);
                    this.ShapeMesh.AppendIndex(2);
                    this.ShapeMesh.AppendIndex(0);
                    this.ShapeMesh.AppendIndex(2);
                    this.ShapeMesh.AppendIndex(3);

                    this.ShapeMesh.currentIdx += 4;
                }
            }
        }
        
        /// <summary>
        /// Add a filled convex polygon.
        /// </summary>
        /// <param name="points">points</param>
        /// <param name="color">color</param>
        /// <param name="antiAliased">anti-aliased</param>
        public void AddConvexPolyFilled(IList<Point> points, Color color, bool antiAliased)
        {
            AddConvexPolyFilled(points, Vector.Zero, color, antiAliased);
        }

        /// <summary>
        /// Add a filled convex polygon with offset.
        /// </summary>
        /// <param name="points">points</param>
        /// <param name="offset">offset that applies to every point</param>
        /// <param name="color">color</param>
        /// <param name="antiAliased">anti-aliased</param>
        public void AddConvexPolyFilled(IList<Point> points, Vector offset, Color color, bool antiAliased)
        {
            antiAliased = false;//TODO remove this when antiAliased branch is implemented

            var pointsCount = points.Count;
            if (antiAliased)
            {
                throw new NotImplementedException();
            }
            else
            {
                // Non Anti-aliased Fill
                int idxCount = (pointsCount - 2) * 3;
                int vtxCount = pointsCount;
                this.ShapeMesh.PrimReserve(idxCount, vtxCount);
                for (int i = 0; i < vtxCount; i++)
                {
                    this.ShapeMesh.AppendVertex(new DrawVertex { pos = points[i], uv = Point.Zero, color = color });
                }
                for (int i = 2; i < pointsCount; i++)
                {
                    this.ShapeMesh.AppendIndex(0);
                    this.ShapeMesh.AppendIndex(i - 1);
                    this.ShapeMesh.AppendIndex(i);
                }
                this.ShapeMesh.currentIdx += vtxCount;
            }
        }

        /// <summary>
        /// Add a filled convex polygon.
        /// </summary>
        /// <param name="points">points</param>
        /// <param name="pointsCount"></param>
        /// <param name="color">color</param>
        /// <param name="antiAliased">anti-aliased</param>
        public unsafe void AddConvexPolyFilled(Point* points, int pointsCount, Color color, bool antiAliased)
        {
            antiAliased = false;//TODO remove this when antiAliased branch is implemented

            if (antiAliased)
            {
                throw new NotImplementedException();
            }
            else
            {
                // Non Anti-aliased Fill
                int idxCount = (pointsCount - 2) * 3;
                int vtxCount = pointsCount;
                this.ShapeMesh.PrimReserve(idxCount, vtxCount);
                for (int i = 0; i < vtxCount; i++)
                {
                    this.ShapeMesh.AppendVertex(new DrawVertex { pos = points[i], uv = Point.Zero, color = color });
                }
                for (int i = 2; i < pointsCount; i++)
                {
                    this.ShapeMesh.AppendIndex(0);
                    this.ShapeMesh.AppendIndex(i - 1);
                    this.ShapeMesh.AppendIndex(i);
                }
                this.ShapeMesh.currentIdx += vtxCount;
            }
        }

        public List<Point> CubicBezier_GeneratePolyLinePoints(Point startPoint, IList<Point> segmentPoints)
        {
            Debug.Assert(segmentPoints.Count % 3 == 0);
            var generatedPoints = new List<Point>();
            generatedPoints.Add(startPoint);
            for (var i = 0; i < segmentPoints.Count; i+=3)
            {
                Point p1 = generatedPoints[generatedPoints.Count - 1];
                var c1 = segmentPoints[i];
                var c2 = segmentPoints[i+1];
                var end = segmentPoints[i+2];
                // Auto-tessellated
                PathBezierToCasteljau(generatedPoints, p1.X, p1.Y, c1.X, c1.Y, c2.X, c2.Y, end.X, end.Y, CurveTessellationTol, 0);
            }
            return generatedPoints;
        }

        public unsafe List<Point> CubicBezier_GeneratePolyLinePoints(Point startPoint, Point* segmentPoints, int pointCount)
        {
            Debug.Assert(pointCount % 3 == 0);
            var generatedPoints = new List<Point>();
            generatedPoints.Add(startPoint);
            for (var i = 0; i < pointCount; i+=3)
            {
                Point p1 = generatedPoints[generatedPoints.Count - 1];
                var c1 = segmentPoints[i];
                var c2 = segmentPoints[i+1];
                var end = segmentPoints[i+2];
                // Auto-tessellated
                PathBezierToCasteljau(generatedPoints, p1.X, p1.Y, c1.X, c1.Y, c2.X, c2.Y, end.X, end.Y, CurveTessellationTol, 0);
            }
            return generatedPoints;
        }

        public List<Point> QuadraticBezier_GeneratePolyLinePoints(Point startPoint, IList<Point> segmentPoints)
        {
            Debug.Assert(segmentPoints.Count % 2 == 0);
            var generatedPoints = new List<Point>();
            generatedPoints.Add(startPoint);

            for (var i = 0; i < segmentPoints.Count; i += 2)
            {
                Point start = generatedPoints[generatedPoints.Count - 1];//start point

                //convert quaratic bezier segment points to cubic bezier segment points
                var x0 = start.X;
                var y0 = start.Y;
                var x1 = segmentPoints[i].X;
                var y1 = segmentPoints[i].Y;
                var x2 = segmentPoints[i + 1].X;
                var y2 = segmentPoints[i + 1].Y;

                var c1 = new Point(2.0 / 3.0 * x1 + 1.0 / 3.0 * x0, 2.0 / 3.0 * y1 + 1.0 / 3.0 * y0);//control point 1
                var c2 = new Point(2.0 / 3.0 * x1 + 1.0 / 3.0 * x2, 2.0 / 3.0 * y1 + 1.0 / 3.0 * y2);//control point 2
                var end = new Point(x2, y2);//end point

                // Auto-tessellated
                PathBezierToCasteljau(generatedPoints, start.X, start.Y, c1.X, c1.Y, c2.X, c2.Y, end.X, end.Y, CurveTessellationTol, 0);
            }
            return generatedPoints;
        }

        public unsafe List<Point> QuadraticBezier_GeneratePolyLinePoints(Point startPoint, Point* segmentPoints, int pointCount)
        {
            Debug.Assert(pointCount % 2 == 0);
            var generatedPoints = new List<Point>();
            generatedPoints.Add(startPoint);

            for (var i = 0; i < pointCount; i += 2)
            {
                Point start = generatedPoints[generatedPoints.Count - 1];//start point

                //convert quaratic bezier segment points to cubic bezier segment points
                var x0 = start.X;
                var y0 = start.Y;
                var x1 = segmentPoints[i].X;
                var y1 = segmentPoints[i].Y;
                var x2 = segmentPoints[i+1].X;
                var y2 = segmentPoints[i+1].Y;
                
                var c1 = new Point(2.0 / 3.0 * x1 + 1.0 / 3.0 * x0, 2.0 / 3.0 * y1 + 1.0 / 3.0 * y0);//control point 1
                var c2 = new Point(2.0 / 3.0 * x1 + 1.0 / 3.0 * x2, 2.0 / 3.0 * y1 + 1.0 / 3.0 * y2);//control point 2
                var end = new Point(x2, y2);//end point

                // Auto-tessellated
                PathBezierToCasteljau(generatedPoints, start.X, start.Y, c1.X, c1.Y, c2.X, c2.Y, end.X, end.Y, CurveTessellationTol, 0);
            }
            return generatedPoints;
        }
        #endregion

        #region Text

        /// <summary>
        /// Text mesh
        /// </summary>
        public TextMesh TextMesh { get; private set; }

        public void AddText(Point baselineOrigin, IList<GlyphData> glyphs, IList<Vector> offsets, string fontFamily, double fontSize, Color color)
        {
            if (glyphs.Count != offsets.Count)
            {
                throw new ArgumentException($"The length of {nameof(glyphs)} must be equal to {nameof(offsets)}.");
            }

            DrawCommand cmd = new DrawCommand();
            cmd.ClipRect = ClipRectStack.Peek();
            cmd.TextureData = null;
            this.TextMesh.Commands.Add(cmd);

            var oldIndexBufferCount = this.TextMesh.IndexBuffer.Count;

            var scale = OSImplementation.TypographyTextContext.GetScale(fontFamily, fontSize);

            // get glyph data from typeface
            for(var i=0;i<glyphs.Count;i++)
            {
                var glyphData = glyphs[i];
                Vector glyphOffset = offsets[i];
                this.TextMesh.Append((Vector)baselineOrigin, glyphData, glyphOffset, scale, color, false);
            }

            var newIndexBufferCount = this.TextMesh.IndexBuffer.Count;

            // Update command
            var command = this.TextMesh.Commands[this.TextMesh.Commands.Count - 1];
            command.ElemCount += newIndexBufferCount - oldIndexBufferCount;
            this.TextMesh.Commands[this.TextMesh.Commands.Count - 1] = command;
        }

        #endregion

        #region Image

        /// <summary>
        /// Mesh (textured triangles)
        /// </summary>
        public Mesh ImageMesh { get; private set; }

        /// <summary>
        /// Add textured rect, used for rendering images parts.
        /// </summary>
        /// <param name="a">top-left point</param>
        /// <param name="c">bottom-right point</param>
        /// <param name="uvA">texture coordinate of point a</param>
        /// <param name="uvC">texture coordinate of point c</param>
        /// <param name="color">tint color</param>
        private void AddImageRect(Point a, Point c, Point uvA, Point uvC, Color color)
        {
            Point b = new Point(c.X, a.Y);
            Point d = new Point(a.X, c.Y);
            Point uvB = new Point(uvC.X, uvA.Y);
            Point uvD = new Point(uvA.X, uvC.Y);

            this.ImageMesh.AppendVertex(new DrawVertex { pos = a, uv = uvA, color = color });
            this.ImageMesh.AppendVertex(new DrawVertex { pos = b, uv = uvB, color = color });
            this.ImageMesh.AppendVertex(new DrawVertex { pos = c, uv = uvC, color = color });
            this.ImageMesh.AppendVertex(new DrawVertex { pos = d, uv = uvD, color = color });
            this.ImageMesh.AppendIndex(0);
            this.ImageMesh.AppendIndex(1);
            this.ImageMesh.AppendIndex(2);
            this.ImageMesh.AppendIndex(0);
            this.ImageMesh.AppendIndex(2);
            this.ImageMesh.AppendIndex(3);
            this.ImageMesh.currentIdx += 4;
        }

        private void AddImageRect(Rect rect, Point uvA, Point uvC, Color color) =>
            this.AddImageRect(rect.Min, rect.Max, uvA, uvC, color);
        #endregion

        #region Path APIs

        private static readonly List<Point> Path = new List<Point>();

        #region Basic

        //TODO confirm PathMoveTo logic

        /// <summary>
        /// Moves the current point of the current path.
        /// </summary>
        /// <param name="point">position that current point will be moved to</param>
        public void PathMoveTo(Point point)
        {
            if (Path.Count == 0)
            {
                Path.Add(point);
            }
            else
            {
                Path[Path.Count - 1] = point;
            }
        }

        /// <summary>
        /// Adds a line to the path from the current point to position p.
        /// </summary>
        /// <param name="p">next point</param>
        public void PathLineTo(Point p)
        {
            Path.Add(p);
        }

        private static void PathBezierToCasteljau(IList<Point> path, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double tessTol, int level)
        {
            double dx = x4 - x1;
            double dy = y4 - y1;
            double d2 = ((x2 - x4) * dy - (y2 - y4) * dx);
            double d3 = ((x3 - x4) * dy - (y3 - y4) * dx);
            d2 = (d2 >= 0) ? d2 : -d2;
            d3 = (d3 >= 0) ? d3 : -d3;
            if ((d2 + d3) * (d2 + d3) < tessTol * (dx * dx + dy * dy))
            {
                path.Add(new Point(x4, y4));
            }
            else if (level < 10)
            {
                double x12 = (x1 + x2) * 0.5f, y12 = (y1 + y2) * 0.5f;
                double x23 = (x2 + x3) * 0.5f, y23 = (y2 + y3) * 0.5f;
                double x34 = (x3 + x4) * 0.5f, y34 = (y3 + y4) * 0.5f;
                double x123 = (x12 + x23) * 0.5f, y123 = (y12 + y23) * 0.5f;
                double x234 = (x23 + x34) * 0.5f, y234 = (y23 + y34) * 0.5f;
                double x1234 = (x123 + x234) * 0.5f, y1234 = (y123 + y234) * 0.5f;

                PathBezierToCasteljau(path, x1, y1, x12, y12, x123, y123, x1234, y1234, tessTol, level + 1);
                PathBezierToCasteljau(path, x1234, y1234, x234, y234, x34, y34, x4, y4, tessTol, level + 1);
            }
        }
        private const double CurveTessellationTol = 1.25;

        /// <summary>
        /// Adds a cubic Bézier spline to the path from the current point to position end, using c1 and c2 as the control points.
        /// </summary>
        /// <param name="c1">first control point</param>
        /// <param name="c2">second control point</param>
        /// <param name="end">end point</param>
        /// <param name="numSegments">number of segments used when tessellating the curve. Use 0 to do automatic tessellation.</param>
        public static void PathBezierCurveTo(Point c1, Point c2, Point end, int numSegments = 0)
        {
            Point p1 = Path[Path.Count - 1];
            if (numSegments == 0)
            {
                // Auto-tessellated
                PathBezierToCasteljau(Path, p1.X, p1.Y, c1.X, c1.Y, c2.X, c2.Y, end.X, end.Y, CurveTessellationTol, 0);
            }
            else
            {
                float tStep = 1.0f / (float)numSegments;
                for (int iStep = 1; iStep <= numSegments; iStep++)
                {
                    float t = tStep * iStep;
                    float u = 1.0f - t;
                    float w1 = u * u * u;
                    float w2 = 3 * u * u * t;
                    float w3 = 3 * u * t * t;
                    float w4 = t * t * t;
                    Path.Add(new Point(w1 * p1.X + w2 * c1.X + w3 * c2.X + w4 * end.X, w1 * p1.Y + w2 * c1.Y + w3 * c2.Y + w4 * end.Y));
                }
            }
        }

        /// <summary>
        /// Closes the current path.
        /// </summary>
        public void PathClose()
        {
            Path.Add(Path[0]);
        }

        /// <summary>
        /// Clears the current path.
        /// </summary>
        public void PathClear()
        {
            Path.Clear();
        }

        /// <summary>
        /// Strokes the current path.
        /// </summary>
        /// <param name="color">color</param>
        /// <param name="close">Set to true if you want the path be closed. A line segment from the last point to first point will be added if this is true.</param>
        /// <param name="thickness">thickness</param>
        public void PathStroke(Color color, bool close, double thickness = 1)
        {
            this.AddPolyline(Path, color, close, thickness);
            this.PathClear();
        }

        /// <summary>
        /// Fills the current path without clear current path. The path must be a convex.
        /// </summary>
        /// <param name="color">fill color</param>
        public void PathFillPreserve(Color color)
        {
            this.AddConvexPolyFilled(Path, color, true);
        }

        /// <summary>
        /// Strokes the current path without clear current path.
        /// </summary>
        /// <param name="color">color</param>
        /// <param name="close">Set to true if you want the path be closed. A line segment from the last point to first point will be added if this is true.</param>
        /// <param name="thickness">thickness</param>
        public void PathStrokePreserve(Color color, bool close, double thickness = 1)
        {
            this.AddPolyline(Path, color, close, thickness);
        }

        /// <summary>
        /// Fills the current path. The path must be a convex.
        /// </summary>
        /// <param name="color">fill color</param>
        public void PathFill(Color color)
        {
            this.AddConvexPolyFilled(Path, color, true);
            this.PathClear();
        }

        #endregion

        #region Compond

        private static readonly Point[] CirclePoints = InitCirclePoints();
        private static Point[] InitCirclePoints()
        {
            Point[] result = new Point[12];
            for (int i = 0; i < 12; i++)
            {
                var a = (float)i / 12 * 2 * Math.PI;
                result[i].X = Math.Cos(a);
                result[i].Y = Math.Sin(a);
            }
            return result;
        }
        /// <summary>
        /// (Fast) adds an arc from angle1 to angle2 to the current path.
        /// Starts from +x, then clock-wise to +y, -x,-y, then ends at +x.
        /// </summary>
        /// <param name="center">the center of the arc</param>
        /// <param name="radius">the radius of the arc</param>
        /// <param name="amin">angle1 = amin * 2π * 1/12</param>
        /// <param name="amax">angle1 = amax * 2π * 1/12</param>
        public void PathArcFast(Point center, double radius, int amin, int amax)
        {
            if (amin > amax) return;
            if (MathEx.AmostZero(radius))
            {
                return;
            }

            Path.Capacity = Path.Count + amax - amin + 1;
            for (int a = amin; a <= amax; a++)
            {
                Point c = CirclePoints[a % CirclePoints.Length];
                var p = new Point(center.X + c.X * radius, center.Y + c.Y * radius);
                if (a == amin)
                {
                    this.PathMoveTo(p);
                }
                else
                {
                    this.PathLineTo(p);
                }
            }
        }

        public void PathEllipse(Point center, double radiusX, double radiusY, double fromAngle, double toAngle)
        {
            if(fromAngle == toAngle) return;
            if (MathEx.AmostZero(radiusX) && MathEx.AmostZero(radiusY))
            {
                return;
            }
            var segmentCount = Math.Max((int)(radiusX + radiusY - 1), 1);

            if (fromAngle < toAngle)
            {
                var unit = (toAngle - fromAngle) / segmentCount;
                for (int i = 0; i <= segmentCount; i++)
                {
                    var angle = fromAngle + unit * i;
                    var p = MathEx.EvaluateEllipse(center, radiusX, radiusY, angle);
                    PathLineTo(p);
                }
            }
            else
            {
                var unit = (fromAngle - toAngle) / segmentCount;
                for (int i = 0; i <= segmentCount; i++)
                {
                    var angle = fromAngle - unit * i;
                    var p = MathEx.EvaluateEllipse(center, radiusX, radiusY, angle);
                    PathLineTo(p);
                }
            }
        }

        public void PathArc(Point center, double radius, double minAngle, double maxAngle)
        {
            PathEllipse(center, radius, radius, minAngle, maxAngle);
        }

        #endregion

        #region Complex

        public void PathRect(Point a, Point b, float rounding = 0.0f, int roundingCorners = 0x0F)
        {
            double r = rounding;
            r = Math.Min(r, Math.Abs(b.X - a.X) * (((roundingCorners & (1 | 2)) == (1 | 2)) || ((roundingCorners & (4 | 8)) == (4 | 8)) ? 0.5f : 1.0f) - 1.0f);
            r = Math.Min(r, Math.Abs(b.Y - a.Y) * (((roundingCorners & (1 | 8)) == (1 | 8)) || ((roundingCorners & (2 | 4)) == (2 | 4)) ? 0.5f : 1.0f) - 1.0f);

            if (r <= 0.0f || roundingCorners == 0)
            {
                this.PathLineTo(a);
                this.PathLineTo(new Point(b.X, a.Y));
                this.PathLineTo(b);
                this.PathLineTo(new Point(a.X, b.Y));
            }
            else
            {
                var r0 = (roundingCorners & 1) != 0 ? r : 0.0f;
                var r1 = (roundingCorners & 2) != 0 ? r : 0.0f;
                var r2 = (roundingCorners & 4) != 0 ? r : 0.0f;
                var r3 = (roundingCorners & 8) != 0 ? r : 0.0f;
                this.PathArcFast(new Point(a.X + r0, a.Y + r0), r0, 6, 9);
                this.PathArcFast(new Point(b.X - r1, a.Y + r1), r1, 9, 12);
                this.PathArcFast(new Point(b.X - r2, b.Y - r2), r2, 0, 3);
                this.PathArcFast(new Point(a.X + r3, b.Y - r3), r3, 3, 6);
            }
        }

        public void PathRect(Rect rect, float rounding = 0.0f, int roundingCorners = 0x0F) =>
            this.PathRect(rect.Min, rect.Max, rounding, roundingCorners);

        private void PrimRectGradient(Point a, Point c, Color topColor, Color bottomColor)
        {
            Point b = new Point(c.X, a.Y);
            Point d = new Point(a.X, c.Y);
            Point uv = Point.Zero;

            this.ShapeMesh.AppendVertex(new DrawVertex { pos = a, uv = Point.Zero, color = topColor });
            this.ShapeMesh.AppendVertex(new DrawVertex { pos = b, uv = Point.Zero, color = topColor });
            this.ShapeMesh.AppendVertex(new DrawVertex { pos = c, uv = Point.Zero, color = bottomColor });
            this.ShapeMesh.AppendVertex(new DrawVertex { pos = d, uv = Point.Zero, color = bottomColor });

            this.ShapeMesh.AppendIndex(0);
            this.ShapeMesh.AppendIndex(1);
            this.ShapeMesh.AppendIndex(2);
            this.ShapeMesh.AppendIndex(0);
            this.ShapeMesh.AppendIndex(2);
            this.ShapeMesh.AppendIndex(3);

            this.ShapeMesh.currentIdx += 4;
        }

        public void AddRectFilledGradient(Point a, Point b, Color topColor, Color bottomColor)
        {
            if (MathEx.AmostZero(topColor.A) && MathEx.AmostZero(bottomColor.A))
                return;

            this.ShapeMesh.PrimReserve(6, 4);
            this.PrimRectGradient(a, b, topColor, bottomColor);
        }

        public void AddRectFilledGradient(Rect rect, Color topColor, Color bottomColor)
        {
            this.AddRectFilledGradient(rect.Min, rect.Max, topColor, bottomColor);
        }

        #endregion

        #endregion
    }
}
