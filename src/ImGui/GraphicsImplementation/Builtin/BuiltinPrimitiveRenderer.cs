using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal partial class BuiltinPrimitiveRenderer : IPrimitiveRenderer
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

        private void AddImageRect(Rect rect, Point uvA, Point uvC, Color color) =>
            AddImageRect(rect.Min, rect.Max, uvA, uvC, color);
        #endregion

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
                    PathMoveTo(p);
                }
                else
                {
                    PathLineTo(p);
                }
            }
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
                PathLineTo(a);
                PathLineTo(new Point(b.X, a.Y));
                PathLineTo(b);
                PathLineTo(new Point(a.X, b.Y));
            }
            else
            {
                var r0 = (roundingCorners & 1) != 0 ? r : 0.0f;
                var r1 = (roundingCorners & 2) != 0 ? r : 0.0f;
                var r2 = (roundingCorners & 4) != 0 ? r : 0.0f;
                var r3 = (roundingCorners & 8) != 0 ? r : 0.0f;
                PathArcFast(new Point(a.X + r0, a.Y + r0), r0, 6, 9);
                PathArcFast(new Point(b.X - r1, a.Y + r1), r1, 9, 12);
                PathArcFast(new Point(b.X - r2, b.Y - r2), r2, 0, 3);
                PathArcFast(new Point(a.X + r3, b.Y - r3), r3, 3, 6);
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
            PrimRectGradient(a, b, topColor, bottomColor);
        }
        
        public void AddRectFilledGradient(Rect rect, Color topColor, Color bottomColor)
        {
            AddRectFilledGradient(rect.Min, rect.Max, topColor, bottomColor);
        }

        #endregion

        #endregion

        public void DrawPath(PathPrimitive primitive)
        {
            foreach (var command in primitive.Path)
            {
                switch (command.Type)
                {
                    case PathCommandType.PathMoveTo:
                    {
                        var cmd = (MoveToCommand)command;
                        this.PathMoveTo(cmd.Point);
                        break;
                    }
                    case PathCommandType.PathLineTo:
                    {
                        var cmd = (LineToCommand)command;
                        this.PathLineTo(cmd.Point);
                        break;
                    }
                    case PathCommandType.PathCurveTo:
                    {
                        throw new NotImplementedException();
                        break;
                    }
                    case PathCommandType.PathClosePath:
                    {
                        this.PathClose();
                        break;
                    }
                    case PathCommandType.PathArc:
                    {
                        var cmd = (ArcCommand) command;
                        this.PathArcFast(cmd.Center, cmd.Radius, cmd.Amin, cmd.Amax);
                        break;
                    }
                    case PathCommandType.Stroke:
                    {
                        var cmd = (StrokeCommand) command;
                        this.PathStroke(cmd.Color, false, cmd.LineWidth);
                        break;
                    }
                    case PathCommandType.Fill:
                    {
                        var cmd = (FillCommand) command;
                        this.PathFill(cmd.Color);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private bool CheckTextPrimitive(TextPrimitive primitive, GUIStyle style)
        {
            do
            {
                double fontSize = style.FontSize;
                if (!MathEx.AmostEqual(primitive.FontSize, fontSize))
                {
                    break;
                }

                string fontFamily = style.FontFamily;
                if (primitive.FontFamily != fontFamily)
                {
                    break;
                }

                FontStyle fontStyle = style.FontStyle;
                if (primitive.FontStyle != fontStyle)
                {
                    break;
                }

                FontWeight fontWeight = style.FontWeight;
                if (primitive.FontWeight != fontWeight)
                {
                    break;
                }

                return false;
            } while (false);

            return true;
        }

        /// <summary>
        /// Draw a text primitive and merge the result to the text mesh.
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="rect"></param>
        /// <param name="style"></param>
        /// TODO apply text alignment
        public void DrawText(TextPrimitive primitive, Rect rect, GUIStyle style)
        {
            primitive.Offset = (Vector)rect.TopLeft;

            //check if we need to rebuild the glyph data of this text primitive
            var needRebuild = this.CheckTextPrimitive(primitive, style);

            var fontFamily = style.FontFamily;
            var fontSize = style.FontSize;
            var fontColor = style.FontColor;

            //build text mesh
            if (needRebuild)
            {
                var fontStretch = FontStretch.Normal;
                var fontStyle = style.FontStyle;
                var fontWeight = style.FontWeight;
                var textAlignment = (TextAlignment) style.Get<int>(GUIStyleName.TextAlignment);

                var textContext = new OSImplentation.TypographyTextContext(primitive.Text,
                    fontFamily,
                    fontSize,
                    fontStretch,
                    fontStyle,
                    fontWeight,
                    (int)rect.Size.Width,
                    (int)rect.Size.Height,
                    textAlignment);
                textContext.Build((Point)primitive.Offset);

                primitive.Offsets.AddRange(textContext.GlyphOffsets);

                foreach (var character in primitive.Text)
                {
                    if (char.IsWhiteSpace(character))
                    {
                        continue;
                    }

                    Typography.OpenFont.Glyph glyph = OSImplentation.TypographyTextContext.LookUpGlyph(fontFamily, character);
                    Typography.OpenFont.GlyphLoader.Read(glyph, out var polygons, out var bezierSegments);
                    var glyphData = GlyphCache.Default.GetGlyph(character, fontFamily, fontStyle, fontWeight);
                    if (glyphData == null)
                    {
                        glyphData = GlyphCache.Default.AddGlyph(character, fontFamily, fontStyle, fontWeight, polygons, bezierSegments);
                    }
                    Debug.Assert(glyphData != null);

                    primitive.Glyphs.Add(glyphData);
                }
            }

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
                if (char.IsWhiteSpace(character))
                {
                    continue;
                }
                index++;
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
        /// <param name="rect"></param>
        /// <param name="style"></param>
        public void DrawImage(ImagePrimitive primitive, Rect rect, GUIStyle style)
        {
            Color tintColor = style.BackgroundColor;
            
            //BUG The texture is not cached!
            var texture = new OSImplentation.Windows.OpenGLTexture();
            texture.LoadImage(primitive.Image.Data, primitive.Image.Width, primitive.Image.Height);

            //TODO check if we need to add a new draw command
            //add a new draw command
            DrawCommand cmd = new DrawCommand();
            cmd.ClipRect = Rect.Big;
            cmd.TextureData = texture;
            this.ImageMesh.CommandBuffer.Add(cmd);

            //construct and merge the mesh of this Path into ShapeMesh
            var uvMin = new Point(0,0);
            var uvMax = new Point(1,1);

            this.ImageMesh.PrimReserve(6, 4);
            AddImageRect(rect, uvMin, uvMax, tintColor);
        }
    }
}