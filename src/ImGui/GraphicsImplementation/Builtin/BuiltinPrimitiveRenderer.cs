using System;
using System.Collections.Generic;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.OSAbstraction.Graphics;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal class BuiltinPrimitiveRenderer : IPrimitiveRenderer
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

        #region Shape
        /// <summary>
        /// Mesh (colored triangles)
        /// </summary>
        public Mesh ShapeMesh { get; private set; }

        /// <summary>
        /// Add a poly line.
        /// </summary>
        /// <param name="points">points</param>
        /// <param name="color">color</param>
        /// <param name="close">Should this method close the polyline for you? A line segment from the last point to first point will be added if this is true.</param>
        /// <param name="thickness">thickness</param>
        /// <param name="antiAliased">anti-aliased</param>
        public void AddPolyline(IList<Point> points, Color color, bool close, double thickness, bool antiAliased = false)
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
                    Vector diff = p2 - p1;
                    diff *= MathEx.InverseLength(diff, 1.0f);

                    float dx = (float)(diff.X * (thickness * 0.5f));
                    float dy = (float)(diff.Y * (thickness * 0.5f));
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

        #endregion

        #region Text

        /// <summary>
        /// Text mesh
        /// </summary>
        public TextMesh TextMesh { get; private set; }

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
        #endregion

        #endregion

        #region Path APIs

        private static readonly List<Point> Path = new List<Point>();

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
            AddPolyline(Path, color, close, thickness);
            PathClear();
        }

        /// <summary>
        /// Fills the current path. The path must be a convex.
        /// </summary>
        /// <param name="color">fill color</param>
        public void PathFill(Color color)
        {
            AddConvexPolyFilled(Path, color, true);
            PathClear();
        }

        #endregion

        /// <summary>
        /// Stroke a primitive and merge the result to the mesh.
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="brush"></param>
        /// <param name="strokeStyle"></param>
        public void Stroke(Primitive primitive, Brush brush, StrokeStyle strokeStyle)
        {
            var offset = primitive.Offset;
            //TODO apply offset, brush and strokeStyle
            var pathPrimitive = primitive as PathPrimitive;
            if (pathPrimitive == null) return;

            //build path
            var path = pathPrimitive.Path;
            foreach (var cmd in path)
            {
                switch (cmd.Type)
                {
                    case PathDataType.PathMoveTo:
                        PathMoveTo(cmd.Points[0]);
                        break;
                    case PathDataType.PathLineTo:
                        PathLineTo(cmd.Points[0]);
                        break;
                    case PathDataType.PathCurveTo:
                        PathBezierCurveTo(cmd.Points[0], cmd.Points[1], cmd.Points[2]);
                        break;
                    case PathDataType.PathClosePath:
                        PathClose(); 
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            //construct and merge the mesh of this Path into ShapeMesh
            PathStroke(brush.LineColor, true, brush.LineWidth);
        }

        /// <summary>
        /// Fill a primitive and merge the result to the mesh.
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="brush"></param>
        public void Fill(Primitive primitive, Brush brush)
        {
            var offset = primitive.Offset;
            //TODO apply offset, brush and strokeStyle
            var pathPrimitive = primitive as PathPrimitive;
            if (pathPrimitive == null) return;

            //build path
            var path = pathPrimitive.Path;
            foreach (var cmd in path)
            {
                switch (cmd.Type)
                {
                    case PathDataType.PathMoveTo:
                        PathMoveTo(cmd.Points[0]);
                        break;
                    case PathDataType.PathLineTo:
                        PathLineTo(cmd.Points[0]);
                        break;
                    case PathDataType.PathCurveTo:
                        PathBezierCurveTo(cmd.Points[0], cmd.Points[1], cmd.Points[2]);
                        break;
                    case PathDataType.PathClosePath:
                        PathClose();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //construct and merge the mesh of this Path into ShapeMesh
            PathFill(brush.FillColor);
        }

        /// <summary>
        /// Draw a text primitive and merge the result to the text mesh.
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="fontFamily"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontColor"></param>
        /// <param name="fontStyle"></param>
        /// <param name="fontWeight"></param>
        public void DrawText(TextPrimitive primitive, string fontFamily, double fontSize, Color fontColor,
            FontStyle fontStyle, FontWeight fontWeight)
        {
            //FIXME Should each text segment consume a draw call? NO!

            //add a new draw command
            DrawCommand cmd = new DrawCommand();
            cmd.ClipRect = Rect.Big;
            cmd.TextureData = null;
            this.TextMesh.Commands.Add(cmd);

            var oldIndexBufferCount = this.TextMesh.IndexBuffer.Count;

            var scale = OSImplentation.TypographyTextContext.GetScale(fontFamily, fontSize);

            int index = -1;

            // get glyph data from typeface
            foreach (var character in primitive.Text)
            {
                index++;
                if (char.IsWhiteSpace(character))
                {
                    continue;
                }

                var glyphData = primitive.Glyphs[index];
                Vector glyphOffset = primitive.Offsets[index];
                this.TextMesh.Append(primitive.Offset, glyphData, glyphOffset, scale, fontColor, false);
            }

            var newIndexBufferCount = this.TextMesh.IndexBuffer.Count;

            // Update command
            var command = this.TextMesh.Commands[this.TextMesh.Commands.Count - 1];
            command.ElemCount += newIndexBufferCount - oldIndexBufferCount;
            this.TextMesh.Commands[this.TextMesh.Commands.Count - 1] = command;
        }

        /// <summary>
        /// Draw an image primitive and merge the result to the image mesh.
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="brush"></param>
        public void DrawImage(ImagePrimitive primitive, Brush brush)
        {
            var offset = primitive.Offset;

            //TODO check if we need to add a new draw command
            //add a new draw command
            DrawCommand cmd = new DrawCommand();
            cmd.ClipRect = Rect.Big;
            cmd.TextureData = null;
            this.ImageMesh.CommandBuffer.Add(cmd);

            var texture = new OSImplentation.Windows.OpenGLTexture();
            texture.LoadImage(primitive.Image.Data, primitive.Image.Width, primitive.Image.Height);

            //construct and merge the mesh of this Path into ShapeMesh
            var uvMin = new Point(0,0);
            var uvMax = new Point(1,1);

            this.ImageMesh.PrimReserve(6, 4);
            AddImageRect(
                (Point)primitive.Offset,
                (Point)primitive.Offset + new Vector(primitive.Image.Width, primitive.Image.Height),
                uvMin, uvMax, brush.FillColor);
        }
    }
}