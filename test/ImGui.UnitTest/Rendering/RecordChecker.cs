using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;
using ImGui.Rendering.Composition;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    internal class ContentChecker : RecordReader
    {
        private interface IStrategy
        {
            void Reset();
            void ReadRecord(List<object> list, object record);
        }

        private class FillExpectedRecordStrategy : IStrategy
        {
            public void Reset()
            {
            }

            public void ReadRecord(List<object> list, object record)
            {
                list.Add(record);
            }
        }

        private class CompareActualRecordStrategy : IStrategy
        {
            private int currentIndex = 0;

            public void Reset()
            {
                this.currentIndex = 0;
            }

            public void ReadRecord(List<object> list, object record)
            {
                var expected = list[this.currentIndex];
                var actual = record;
                Assert.Equal(expected, actual);
                this.currentIndex++;
            }
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
                return base.GetHashCode();
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
                return base.GetHashCode();
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
                return base.GetHashCode();
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
                        return s_PathGeometryComparer.Equals(xg, yg);
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
            private ITexture image;
            private Rect rectangle;

            public ImageRecord(ITexture image, Rect rectangle)
            {
                this.image = image;
                this.rectangle = rectangle;
            }

            public bool Equals(ImageRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.image, other.image) && this.rectangle.Equals(other.rectangle);
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
                    return ((this.image != null ? this.image.GetHashCode() : 0) * 397) ^ this.rectangle.GetHashCode();
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
            private Brush foregroundBrush;
            private GlyphRun glyphRun;

            public GlyphRunRecord(Brush foregroundBrush, GlyphRun glyphRun)
            {
                this.foregroundBrush = foregroundBrush;
                this.glyphRun = glyphRun;
            }

            public bool Equals(GlyphRunRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.foregroundBrush, other.foregroundBrush) && Equals(this.glyphRun, other.glyphRun);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((GlyphRunRecord) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((this.foregroundBrush != null ? this.foregroundBrush.GetHashCode() : 0) * 397) ^ (this.glyphRun != null ? this.glyphRun.GetHashCode() : 0);
                }
            }
        }

        class TextRecord : IEquatable<TextRecord>
        {
            private Brush foregroundBrush;
            private GlyphRun glyphRun;
            private Point origin;
            private double maxTextWidth;
            private double maxTextHeight;

            public TextRecord(Brush foregroundBrush, GlyphRun glyphRun, Point origin, double maxTextWidth, double maxTextHeight)
            {
                this.foregroundBrush = foregroundBrush;
                this.glyphRun = glyphRun;
                this.origin = origin;
                this.maxTextWidth = maxTextWidth;
                this.maxTextHeight = maxTextHeight;
            }

            public bool Equals(TextRecord other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(this.foregroundBrush, other.foregroundBrush) && Equals(this.glyphRun, other.glyphRun) && this.origin.Equals(other.origin) && this.maxTextWidth.Equals(other.maxTextWidth) && this.maxTextHeight.Equals(other.maxTextHeight);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TextRecord) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (this.foregroundBrush != null ? this.foregroundBrush.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (this.glyphRun != null ? this.glyphRun.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ this.origin.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.maxTextWidth.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.maxTextHeight.GetHashCode();
                    return hashCode;
                }
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

        #endregion

        public ContentChecker()
        {
            this.strategy = s_fillExpectedRecordStrategy;
        }

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

        public override void DrawImage(ITexture image, Rect rectangle)
        {
            this.strategy.ReadRecord(this.records, new ImageRecord(image, rectangle));
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

        public override void DrawGlyphRun(Brush foregroundBrush, GlyphRun glyphRun, Point origin, double maxTextWidth,
            double maxTextHeight)
        {
            this.strategy.ReadRecord(this.records, new TextRecord(foregroundBrush, glyphRun, origin, maxTextWidth, maxTextHeight));
        }

        public override void DrawDrawing(Drawing drawing)
        {
            this.strategy.ReadRecord(this.records, new DrawingRecord(drawing));
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

        public void StartCheck()
        {
            this.strategy = s_compareActualRecordStrategy;
            this.strategy.Reset();
        }

        private List<object> records = new List<object>();
        private IStrategy strategy;

        private static readonly IStrategy s_fillExpectedRecordStrategy = new FillExpectedRecordStrategy();
        private static readonly IStrategy s_compareActualRecordStrategy = new CompareActualRecordStrategy();
    }
}