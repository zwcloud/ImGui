using System;
using System.Collections.Generic;
using System.Linq;
using ImGui;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;
using ImGui.Rendering.Composition;

namespace ImGui.UnitTest
{
    internal class ContentCheckerBase : GeometryRenderer
    {
        public interface IStrategy
        {
            void Reset();
            bool ReadRecord(List<object> list, object record);
        }

        #region Comparers
        private class PathSegmentComparer : IEqualityComparer<PathSegment>
        {
            public bool Equals(PathSegment x, PathSegment y)
            {
                if (x == null && y != null) return false;
                if (x != null && y == null) return false;
                if (ReferenceEquals(x, y)) return true;

                do
                {
                    if (!x.IsStroked.Equals(y.IsStroked)) break;
                    if (x is LineSegment lx && y is LineSegment ly)
                    {
                        if (!lx.Point.Equals(ly.Point)) break;
                    }
                    else if (x is ArcSegment ax && y is ArcSegment ay)
                    {
                        if (!ax.Point.Equals(ay.Point)) break;
                        if (!ax.Size.Equals(ay.Size)) break;
                        if (!ax.IsLargeArc.Equals(ay.IsLargeArc)) break;
                        if (!ax.RotationAngle.Equals(ay.RotationAngle)) break;
                        if (!ax.SweepDirection.Equals(ay.SweepDirection)) break;
                    }
                    else if (x is PolyLineSegment plx && y is PolyLineSegment ply)
                    {
                        if (!plx.Points.SequenceEqual(ply.Points)) break;
                    }
                    else if (x is QuadraticBezierSegment qx && y is QuadraticBezierSegment qy)
                    {
                        if (!qx.ControlPoint.Equals(qy.ControlPoint)) break;
                        if (!qx.EndPoint.Equals(qy.EndPoint)) break;
                    }
                    else if (x is PolyQuadraticBezierSegment pqx && y is PolyQuadraticBezierSegment pqy)
                    {
                        if (!pqx.Points.SequenceEqual(pqy.Points)) break;
                    }
                    else if (x is CubicBezierSegment cx && y is CubicBezierSegment cy)
                    {
                        if (!cx.ControlPoint1.Equals(cy.ControlPoint1)) break;
                        if (!cx.ControlPoint2.Equals(cy.ControlPoint2)) break;
                        if (!cx.EndPoint.Equals(cy.EndPoint)) break;
                    }
                    else if (x is PolyCubicBezierSegment pcx && y is PolyCubicBezierSegment pcy)
                    {
                        if (!pcx.Points.SequenceEqual(pcy.Points)) break;
                    }
                    else
                    {
                        break;
                    }

                    return true;
                } while (false);

                return false;
            }

            public int GetHashCode(PathSegment obj)
            {
                return obj.GetHashCode();
            }
        }

        private class PathFigureComparer : IEqualityComparer<PathFigure>
        {
            public bool Equals(PathFigure x, PathFigure y)
            {
                if (x == null && y != null) return false;
                if (x != null && y == null) return false;
                if (ReferenceEquals(x, y)) return true;

                do
                {
                    if (!x.StartPoint.Equals(y.StartPoint)) break;
                    if (!x.IsClosed.Equals(y.IsClosed)) break;
                    if (!x.IsFilled.Equals(y.IsFilled)) break;
                    if (!x.Segments.SequenceEqual(y.Segments, new PathSegmentComparer())) break;
                    return true;
                } while (false);

                return false;
            }

            public int GetHashCode(PathFigure obj)
            {
                return obj.GetHashCode();
            }
        }

        private class PathGeometryComparer : IEqualityComparer<PathGeometry>
        {
            public bool Equals(PathGeometry x, PathGeometry y)
            {
                if (x == null && y != null) return false;
                if (x != null && y == null) return false;
                if (ReferenceEquals(x, y)) return true;

                do
                {
                    if (x.FillRule != y.FillRule) break;
                    if (x.Figures.Count != y.Figures.Count) break;
                    if (!x.Figures.SequenceEqual(y.Figures, new PathFigureComparer())) break;
                    return true;
                } while (false);

                return false;
            }

            public int GetHashCode(PathGeometry obj)
            {
                return obj.GetHashCode();
            }
        }

        private class RectangleGeometryComparer : IEqualityComparer<RectangleGeometry>
        {
            public bool Equals(RectangleGeometry x, RectangleGeometry y)
            {
                if (x == null && y != null) return false;
                if (x != null && y == null) return false;
                if (ReferenceEquals(x, y)) return true;

                do
                {
                    if (!x.Offset.Equals(y.Offset)) break;
                    if (!x.Rect.Equals(y.Rect)) break;
                    if (x.RadiusX != y.RadiusX) break;
                    if (x.RadiusY != y.RadiusY) break;
                    return true;
                } while (false);

                return false;
            }

            public int GetHashCode(RectangleGeometry obj)
            {
                return obj.GetHashCode();
            }
        }

        private class GlyphDataComparer : IEqualityComparer<GlyphData>
        {
            public bool Equals(GlyphData x, GlyphData y)
            {
                if (x == null && y != null) return false;
                if (x != null && y == null) return false;
                if (ReferenceEquals(x, y)) return true;

                do
                {
                    if(x.Character              != y.Character             ) break;
                    if(x.FontFamily             != y.FontFamily            ) break;
                    if(x.Polygons               != y.Polygons              ) break;
                    if(x.QuadraticCurveSegments != y.QuadraticCurveSegments) break;
                    return true;
                } while (false);

                return false;
            }

            public int GetHashCode(GlyphData obj)
            {
                return obj.GetHashCode();
            }
        }

        private class GlyphRunComparer : IEqualityComparer<GlyphRun>
        {
            private static readonly GlyphDataComparer s_comparer = new GlyphDataComparer();

            public bool Equals(GlyphRun x, GlyphRun y)
            {
                if (x == null && y != null) return false;
                if (x != null && y == null) return false;
                if (ReferenceEquals(x, y)) return true;

                do
                {
                    if(x.Text          != y.Text         ) break;
                    if(x.FontFamily    != y.FontFamily   ) break;
                    if(x.FontSize      != y.FontSize     ) break;
                    if(!x.GlyphDataList.SequenceEqual(y.GlyphDataList, s_comparer)) break;
                    return true;
                } while (false);

                return false;
            }

            public int GetHashCode(GlyphRun obj)
            {
                return obj.GetHashCode();
            }
        }

        private class FormattedTextComparer : IEqualityComparer<FormattedText>
        {
            private static readonly GlyphDataComparer s_comparer = new GlyphDataComparer();

            public bool Equals(FormattedText x, FormattedText y)
            {
                if (x == null && y != null) return false;
                if (x != null && y == null) return false;
                if (ReferenceEquals(x, y)) return true;

                do
                {
                    if (x.Text != y.Text) break;
                    if (x.FontFamily != y.FontFamily) break;
                    if (x.FontSize != y.FontSize) break;
                    if (!x.GlyphDataList.SequenceEqual(y.GlyphDataList, s_comparer)) break;
                    return true;
                } while (false);

                return false;
            }

            public int GetHashCode(FormattedText obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion

        #region Record Types

        class LineRecord : IEquatable<LineRecord>
        {
            public Pen pen;
            public Point point0;
            public Point point1;

            public LineRecord(Pen pen, Point point0, Point point1)
            {
                this.pen = pen;
                this.point0 = point0;
                this.point1 = point1;
            }

            public bool Equals(LineRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.pen, other.pen) && this.point0.Equals(other.point0) &&
                       this.point1.Equals(other.point1);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return this.Equals((LineRecord) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (this.pen != null ? this.pen.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ this.point0.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.point1.GetHashCode();
                    return hashCode;
                }
            }
        }

        class RectangleRecord : IEquatable<RectangleRecord>
        {
            public Brush brush;
            public Pen pen;
            public Rect rectangle;

            public RectangleRecord(Brush brush, Pen pen, Rect rectangle)
            {
                this.brush = brush;
                this.pen = pen;
                this.rectangle = rectangle;
            }

            public bool Equals(RectangleRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.brush, other.brush) && Equals(this.pen, other.pen) &&
                       this.rectangle.Equals(other.rectangle);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return this.Equals((RectangleRecord) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (this.brush != null ? this.brush.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (this.pen != null ? this.pen.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ this.rectangle.GetHashCode();
                    return hashCode;
                }
            }
        }

        class RoundedRectangleRecord : IEquatable<RoundedRectangleRecord>
        {
            private Brush brush;
            private Pen pen;
            private Rect rectangle;
            private double radiusX;
            private double radiusY;

            public RoundedRectangleRecord(Brush brush, Pen pen, Rect rectangle, double radiusX, double radiusY)
            {
                this.brush = brush;
                this.pen = pen;
                this.rectangle = rectangle;
                this.radiusX = radiusX;
                this.radiusY = radiusY;
            }

            public bool Equals(RoundedRectangleRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.brush, other.brush) && Equals(this.pen, other.pen) &&
                       this.rectangle.Equals(other.rectangle) && this.radiusX.Equals(other.radiusX) &&
                       this.radiusY.Equals(other.radiusY);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return this.Equals((RoundedRectangleRecord) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (this.brush != null ? this.brush.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (this.pen != null ? this.pen.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ this.rectangle.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.radiusX.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.radiusY.GetHashCode();
                    return hashCode;
                }
            }
        }

        class EllipseRecord : IEquatable<EllipseRecord>
        {
            private Brush brush;
            private Pen pen;
            private Point center;
            private double radiusX;
            private double radiusY;

            public EllipseRecord(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
            {
                this.brush = brush;
                this.pen = pen;
                this.center = center;
                this.radiusX = radiusX;
                this.radiusY = radiusY;
            }

            public bool Equals(EllipseRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.brush, other.brush) && Equals(this.pen, other.pen) &&
                       this.center.Equals(other.center) && this.radiusX.Equals(other.radiusX) &&
                       this.radiusY.Equals(other.radiusY);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return this.Equals((EllipseRecord) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (this.brush != null ? this.brush.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (this.pen != null ? this.pen.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ this.center.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.radiusX.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.radiusY.GetHashCode();
                    return hashCode;
                }
            }
        }

        class GeometryRecord : IEquatable<GeometryRecord>
        {
            private Brush brush;
            private Pen pen;
            private Geometry geometry;

            static PathGeometryComparer s_PathGeometryComparer = new PathGeometryComparer();

            public GeometryRecord(Brush brush, Pen pen, Geometry geometry)
            {
                this.brush = brush;
                this.pen = pen;
                this.geometry = geometry;
            }

            public bool Equals(GeometryRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                if (Equals(this.brush, other.brush) && Equals(this.pen, other.pen))
                {
                    if (this.geometry is PathGeometry xg && other.geometry is PathGeometry yg)
                    {
                        return s_PathGeometryComparer.Equals(xg, yg);//deep comparison instead of just comparing the refererence
                    }
                    else
                    {
                        throw new NotImplementedException("Comparison methods for other Geometry types are not implemented.");
                    }
                }
                else
                {
                    return false;
                }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((GeometryRecord) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (this.brush != null ? this.brush.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (this.pen != null ? this.pen.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (this.geometry != null ? this.geometry.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        class ImageRecord : IEquatable<ImageRecord>
        {
            private readonly ITexture image;
            private readonly Rect rectangle;
            private readonly Point uvMin;
            private readonly Point uvMax;

            public ImageRecord(ITexture image, Rect rectangle, Point uvMin, Point uvMax)
            {
                this.image = image;
                this.rectangle = rectangle;
                this.uvMin = uvMin;
                this.uvMax = uvMax;
            }

            public bool Equals(ImageRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.image, other.image)
                       && Rect.AlmostEqual(this.rectangle, other.rectangle)
                       && Point.AlmostEqual(this.uvMin, other.uvMin)
                       && Point.AlmostEqual(this.uvMax, other.uvMax);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ImageRecord) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (this.image != null ? this.image.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ this.rectangle.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.uvMin.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.uvMax.GetHashCode();
                    return hashCode;
                }
            }
        }

        class SliceImageRecord :IEquatable<SliceImageRecord>
        {
            private ITexture image;
            private Rect rectangle;
            private (double top, double right, double bottom, double left) slice;

            public SliceImageRecord(ITexture image, Rect rectangle, (double top, double right, double bottom, double left) slice)
            {
                this.image = image;
                this.rectangle = rectangle;
                this.slice = slice;
            }

            public bool Equals(SliceImageRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.image, other.image) && this.rectangle.Equals(other.rectangle) && this.slice.Equals(other.slice);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((SliceImageRecord) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (this.image != null ? this.image.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ this.rectangle.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.slice.GetHashCode();
                    return hashCode;
                }
            }
        }

        class GlyphRunRecord : IEquatable<GlyphRunRecord>
        {
            private readonly Brush foregroundBrush;
            private readonly GlyphRun glyphRun;

            private static readonly GlyphRunComparer s_GlyphRunComparer = new GlyphRunComparer();

            public GlyphRunRecord(Brush foregroundBrush, GlyphRun glyphRun)
            {
                this.foregroundBrush = foregroundBrush;
                this.glyphRun = glyphRun;
            }

            public bool Equals(GlyphRunRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.foregroundBrush, other.foregroundBrush)
                       && s_GlyphRunComparer.Equals(this.glyphRun, other.glyphRun);
            }
        }

        class FormattedTextRecord : IEquatable<FormattedTextRecord>
        {
            private readonly Brush foregroundBrush;
            private readonly FormattedText formattedText;

            private static readonly FormattedTextComparer s_comparer = new FormattedTextComparer();

            public FormattedTextRecord(Brush foregroundBrush, FormattedText formattedText)
            {
                this.foregroundBrush = foregroundBrush;
                this.formattedText = formattedText;
            }

            public bool Equals(FormattedTextRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.foregroundBrush, other.foregroundBrush)
                       && s_comparer.Equals(this.formattedText, other.formattedText);
            }
        }

        class DrawingRecord : IEquatable<DrawingRecord>
        {
            private Drawing drawing;

            public DrawingRecord(Drawing drawing)
            {
                this.drawing = drawing;
            }

            public bool Equals(DrawingRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.drawing, other.drawing);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((DrawingRecord) obj);
            }

            public override int GetHashCode()
            {
                return (this.drawing != null ? this.drawing.GetHashCode() : 0);
            }
        }

        class ClipRecord : IEquatable<ClipRecord>
        {
            private readonly Geometry geometry;

            static PathGeometryComparer s_PathGeometryComparer = new PathGeometryComparer();
            private static RectangleGeometryComparer s_RectangleGeometryComparer
                = new RectangleGeometryComparer();

            public ClipRecord(Geometry geometry)
            {
                this.geometry = geometry;
            }

            public bool Equals(ClipRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                if (this.geometry is PathGeometry xg && other.geometry is PathGeometry yg)
                {
                    return s_PathGeometryComparer
                            .Equals(xg, yg);
                           //deep comparison instead of just comparing the reference
                }

                if (this.geometry is RectangleGeometry x && other.geometry is RectangleGeometry y)
                {
                    return s_RectangleGeometryComparer.Equals(x, y);
                }

                throw new NotImplementedException(
                    "Comparison methods for other Geometry types are not implemented." +
                    " Or other geometry is of different type.");
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ClipRecord) obj);
            }

            public override int GetHashCode()
            {
                int hashCode = this.geometry != null ? this.geometry.GetHashCode() : 0;
                return hashCode;
            }
        }

        class PopRecord : IEquatable<PopRecord>
        {
            public bool Equals(PopRecord other)
            {
                return other != null;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((PopRecord) obj);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        #endregion

        #region Overrides of DrawingContext
        public override void DrawLine(Pen pen, Point point0, Point point1)
        {
            this.strategy.ReadRecord(this.records, new LineRecord(pen, point0, point1));
        }

        public override void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
            this.strategy.ReadRecord(this.records, new RectangleRecord(brush, pen, rectangle));
        }

        public override void DrawRoundedRectangle(Brush brush, Pen pen, Rect rectangle, double radiusX, double radiusY)
        {
            this.strategy.ReadRecord(this.records, new RoundedRectangleRecord(brush, pen, rectangle, radiusX, radiusY));
        }

        public override void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
        {
            this.strategy.ReadRecord(this.records, new EllipseRecord(brush, pen, center, radiusX, radiusY));
        }

        public override void DrawGeometry(Brush brush, Pen pen, Geometry geometry)
        {
            this.strategy.ReadRecord(this.records, new GeometryRecord(brush, pen, geometry));
        }

        public override void DrawImage(ITexture image, Rect rectangle, Point uvMin, Point uvMax)
        {
            this.strategy.ReadRecord(this.records, new ImageRecord(image, rectangle, uvMin, uvMax));
        }

        public override void DrawImage(ITexture image, Rect rectangle,
            (double top, double right, double bottom, double left) slice)
        {
            this.strategy.ReadRecord(this.records, new SliceImageRecord(image, rectangle, slice));
        }

        public override void DrawGlyphRun(Brush foregroundBrush, GlyphRun glyphRun)
        {
            this.strategy.ReadRecord(this.records, new GlyphRunRecord(foregroundBrush, glyphRun));
        }

        public override void DrawText(Brush foregroundBrush, FormattedText formattedText)
        {
            this.strategy.ReadRecord(this.records, new FormattedTextRecord(foregroundBrush, formattedText));
        }

        public override void PushClip(Geometry clipGeometry)
        {
            this.strategy.ReadRecord(this.records, new ClipRecord(clipGeometry));
        }

        public override void Pop()
        {
            this.strategy.ReadRecord(this.records, new PopRecord());
        }
        #endregion

        #region Overrides of RecordReader
        public override void OnBeforeRead()
        {
        }

        public override void OnAfterRead(MeshList meshList)
        {
        }
        #endregion

        protected void SetStrategy(IStrategy strategy)
        {
            this.strategy = strategy;
        }

        private readonly List<object> records = new List<object>();
        private IStrategy strategy;
    }
}