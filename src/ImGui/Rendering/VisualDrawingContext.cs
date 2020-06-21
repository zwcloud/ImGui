using System;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering.Composition;

namespace ImGui.Rendering
{
    //TODO consider insert new record instead of overwriting all following records
    internal class VisualDrawingContext : DrawingContext
    {
        public VisualDrawingContext(Visual visual)
        {
            this.ownerVisual = visual;
        }

        public override void DrawLine(Pen pen, Point point0, Point point1)
        {
            if (pen == null)
            {
                throw new ArgumentNullException(nameof(pen));
            }

            unsafe
            {
                EnsureContent();

                if (!this.content.ReadRecord(out DrawLineCommand record))
                {//different record type: append new record
                    record.PenIndex = this.content.AddDependentResource(pen);
                    record.StartPoint = point0;
                    record.EndPoint = point1;
                    this.content.WriteRecord(RecordType.DrawLine, (byte*) &record, sizeof(DrawLineCommand));
                    return;
                }

                //same type: update record if different
                bool recordNeedOverwrite = false;
                if (!Point.AlmostEqual(record.StartPoint, point0))
                {
                    record.StartPoint = point0;
                    recordNeedOverwrite = true;
                }

                if (!Point.AlmostEqual(record.EndPoint, point1))
                {
                    record.EndPoint = point0;
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.PenIndex, out Pen oldPen))
                {
                    if (!Equals(oldPen, pen))
                    {
                        record.PenIndex = this.content.AddDependentResource(pen);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.PenIndex = this.content.AddDependentResource(pen);
                    recordNeedOverwrite = true;
                }

                if (recordNeedOverwrite)
                {
                    content.WriteRecord(RecordType.DrawLine, (byte*) &record, sizeof(DrawLineCommand));
                }
            }
        }

        public override void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
            if (brush == null && pen == null)
            {
                return;
            }

            unsafe
            {
                EnsureContent();

                if (!this.content.ReadRecord(out DrawRectangleCommand record))
                {//different record type: append new record
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    record.PenIndex = this.content.AddDependentResource(pen);
                    record.Rectangle = rectangle;
                    this.content.WriteRecord(RecordType.DrawRectangle, (byte*) &record, sizeof(DrawRectangleCommand));
                    return;
                }

                //same type: update record if different
                bool recordNeedOverwrite = false;
                if (!Rect.AlmostEqual(record.Rectangle, rectangle))
                {
                    record.Rectangle = rectangle;
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.PenIndex, out Pen oldPen))
                {
                    if (!oldPen.Equals(pen))
                    {
                        record.PenIndex = this.content.AddDependentResource(pen);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.PenIndex = this.content.AddDependentResource(pen);
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.BrushIndex, out Brush oldBrush))
                {
                    if (!oldBrush.Equals(brush))
                    {
                        record.BrushIndex = this.content.AddDependentResource(brush);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    recordNeedOverwrite = true;
                }

                if (recordNeedOverwrite)
                {
                    content.WriteRecord(RecordType.DrawRectangle, (byte*)&record, sizeof(DrawRectangleCommand));
                }

            }
        }

        public override void DrawRoundedRectangle(Brush brush, Pen pen, Rect rectangle, double radiusX, double radiusY)
        {
            if (brush == null && pen == null)
            {
                return;
            }
            EnsureContent();

            unsafe
            {
                if (!this.content.ReadRecord(out DrawRoundedRectangleCommand record))
                {//different record type: append new record
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    record.PenIndex = this.content.AddDependentResource(pen);
                    record.Rectangle = rectangle;
                    record.radiusX = radiusX;
                    record.radiusY = radiusY;
                    this.content.WriteRecord(RecordType.DrawRoundedRectangle, (byte*) &record, sizeof(DrawRoundedRectangleCommand));
                    return;
                }

                //same type: update record if different
                bool recordNeedOverwrite = false;
                if (!Rect.AlmostEqual(record.Rectangle, rectangle))
                {
                    record.Rectangle = rectangle;
                    recordNeedOverwrite = true;
                }

                if (!MathEx.AmostEqual(record.radiusX, radiusX))
                {
                    record.radiusX = radiusX;
                    recordNeedOverwrite = true;
                }

                if (!MathEx.AmostEqual(record.radiusY, radiusY))
                {
                    record.radiusY = radiusY;
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.PenIndex, out Pen oldPen))
                {
                    if (!oldPen.Equals(pen))
                    {
                        record.PenIndex = this.content.AddDependentResource(pen);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.PenIndex = this.content.AddDependentResource(pen);
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.BrushIndex, out Brush oldBrush))
                {
                    if (!oldBrush.Equals(brush))
                    {
                        record.BrushIndex = this.content.AddDependentResource(brush);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    recordNeedOverwrite = true;
                }

                if (recordNeedOverwrite)
                {
                    content.WriteRecord(RecordType.DrawRoundedRectangle, (byte*)&record, sizeof(DrawRoundedRectangleCommand));
                }

            }
        }

        public override void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
        {
            if (brush == null && pen == null || radiusX <= 0 || radiusY <= 0)
            {
                return;
            }
            EnsureContent();

            unsafe
            {
                if (!this.content.ReadRecord(out DrawEllipseCommand record))
                {//different record type: append new record
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    record.PenIndex = this.content.AddDependentResource(pen);
                    record.Center = center;
                    record.RadiusX = radiusX;
                    record.RadiusY = radiusY;
                    this.content.WriteRecord(RecordType.DrawEllipse, (byte*)&record, sizeof(DrawEllipseCommand));
                    return;
                }

                //same type: update record if different
                bool recordNeedOverwrite = false;
                if (!Point.AlmostEqual(record.Center, center))
                {
                    record.Center = center;
                    recordNeedOverwrite = true;
                }
                if (!MathEx.AmostEqual(record.RadiusX, radiusX))
                {
                    record.RadiusX = radiusX;
                    recordNeedOverwrite = true;
                }
                if (!MathEx.AmostEqual(record.RadiusY, radiusY))
                {
                    record.RadiusY = radiusY;
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.PenIndex, out Pen oldPen))
                {
                    if (!oldPen.Equals(pen))
                    {
                        record.PenIndex = this.content.AddDependentResource(pen);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.PenIndex = this.content.AddDependentResource(pen);
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.BrushIndex, out Brush oldBrush))
                {
                    if (!oldBrush.Equals(brush))
                    {
                        record.BrushIndex = this.content.AddDependentResource(brush);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    recordNeedOverwrite = true;
                }

                if (recordNeedOverwrite)
                {
                    content.WriteRecord(RecordType.DrawEllipse, (byte*)&record, sizeof(DrawEllipseCommand));
                }
            }
        }

        public override void DrawGeometry(Brush brush, Pen pen, Geometry geometry)
        {
            if (brush == null && pen == null || geometry == null)
            {
                return;
            }

            unsafe
            {
                EnsureContent();

                if (!this.content.ReadRecord(out DrawGeometryCommand record))
                {//different record type: append new record
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    record.PenIndex = this.content.AddDependentResource(pen);
                    record.GeometryIndex = this.content.AddDependentResource(geometry);
                    this.content.WriteRecord(RecordType.DrawGeometry, (byte*) &record, sizeof(DrawGeometryCommand));
                    return;
                }

                //same type: update record if different
                bool recordNeedOverwrite = false;
                if (this.content.ReadDependentResource(record.PenIndex, out Pen oldPen))
                {
                    if (!oldPen.Equals(pen))
                    {
                        record.PenIndex = this.content.AddDependentResource(pen);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.PenIndex = this.content.AddDependentResource(pen);
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.BrushIndex, out Brush oldBrush))
                {
                    if (!oldBrush.Equals(brush))
                    {
                        record.BrushIndex = this.content.AddDependentResource(brush);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.GeometryIndex, out Geometry oldGeometry))
                {
                    if (!ReferenceEquals(oldGeometry, geometry))
                    {
                        record.GeometryIndex = this.content.AddDependentResource(geometry);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.GeometryIndex = this.content.AddDependentResource(geometry);
                    recordNeedOverwrite = true;
                }

                if (recordNeedOverwrite)
                {
                    content.WriteRecord(RecordType.DrawGeometry, (byte*) &record, sizeof(DrawGeometryCommand));
                }
            }
        }

        public override void DrawImage(ITexture image, Rect rectangle, Point uvMin, Point uvMax)
        {
            if (image == null)
            {
                return;
            }
            EnsureContent();

            unsafe
            {
                if (!this.content.ReadRecord(out DrawImageCommand record))
                {//different record type: append new record
                    record.ImageSourceIndex = this.content.AddDependentResource(image);
                    record.rectangle = rectangle;
                    record.UVMin = uvMin;
                    record.UVMax = uvMax;
                    this.content.WriteRecord(RecordType.DrawImage, (byte*) &record, sizeof(DrawImageCommand));
                    return;
                }

                //same type: update record if different
                bool recordNeedOverwrite = false;
                if (this.content.ReadDependentResource(record.ImageSourceIndex, out ITexture oldImage))
                {
                    if (!image.Equals(oldImage))
                    {
                        record.ImageSourceIndex = this.content.AddDependentResource(image);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.ImageSourceIndex = this.content.AddDependentResource(image);
                    recordNeedOverwrite = true;
                }
                
                if (!Point.AlmostEqual(record.UVMin, uvMin))
                {
                    record.UVMin = uvMin;
                    recordNeedOverwrite = true;
                }
                if (!Point.AlmostEqual(record.UVMax, uvMax))
                {
                    record.UVMax = uvMax;
                    recordNeedOverwrite = true;
                }


                if (recordNeedOverwrite)
                {
                    content.WriteRecord(RecordType.DrawImage, (byte*)&record, sizeof(DrawImageCommand));
                }
            }
        }

        public override void DrawImage(ITexture image, Rect rectangle, (double top, double right, double bottom, double left) slice)
        {
            if (image == null)
            {
                return;
            }
            EnsureContent();

            unsafe
            {
                if (!this.content.ReadRecord(out DrawSlicedImageCommand record))
                {//different record type: append new record
                    record.ImageSourceIndex = this.content.AddDependentResource(image);
                    record.Rectangle = rectangle;
                    (record.sliceTop, record.sliceRight, record.sliceBottom, record.sliceLeft) = slice;
                    this.content.WriteRecord(RecordType.DrawSlicedImage, (byte*) &record, sizeof(DrawSlicedImageCommand));
                    return;
                }

                //same type: update record if different
                bool recordNeedOverwrite = false;
                if (!Rect.AlmostEqual(record.Rectangle, rectangle))
                {
                    record.Rectangle = rectangle;
                    recordNeedOverwrite = true;
                }

                if (!MathEx.AmostEqual(record.sliceTop, slice.top))
                {
                    record.sliceTop = slice.top;
                    recordNeedOverwrite = true;
                }
                if (!MathEx.AmostEqual(record.sliceRight, slice.right))
                {
                    record.sliceRight = slice.right;
                    recordNeedOverwrite = true;
                }
                if (!MathEx.AmostEqual(record.sliceBottom, slice.bottom))
                {
                    record.sliceBottom = slice.bottom;
                    recordNeedOverwrite = true;
                }
                if (!MathEx.AmostEqual(record.sliceLeft, slice.left))
                {
                    record.sliceLeft = slice.left;
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.ImageSourceIndex, out ITexture oldImage))
                {
                    if (!image.Equals(oldImage))
                    {
                        record.ImageSourceIndex = this.content.AddDependentResource(image);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.ImageSourceIndex = this.content.AddDependentResource(image);
                    recordNeedOverwrite = true;
                }

                if (recordNeedOverwrite)
                {
                    content.WriteRecord(RecordType.DrawSlicedImage, (byte*)&record, sizeof(DrawSlicedImageCommand));
                }
            }
        }

        public override void DrawGlyphRun(Brush brush, GlyphRun glyphRun)
        {
            if (brush == null || glyphRun == null)
            {
                return;
            }
            EnsureContent();

            unsafe
            {
                if (!this.content.ReadRecord(out DrawGlyphRunCommand record))
                {//different record type: append new record
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    record.GlyphRunIndex = this.content.AddDependentResource(glyphRun);
                    this.content.WriteRecord(RecordType.DrawGlyphRun, (byte*) &record, sizeof(DrawGlyphRunCommand));
                    return;
                }

                //same type: update record if different
                bool recordNeedOverwrite = false;
                if (this.content.ReadDependentResource(record.BrushIndex, out Brush oldBrush))
                {
                    if (!oldBrush.Equals(brush))
                    {
                        record.BrushIndex = this.content.AddDependentResource(brush);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.BrushIndex, out GlyphRun oldGlyphRun))
                {
                    if (!glyphRun.Equals(oldGlyphRun))
                    {
                        record.GlyphRunIndex = this.content.AddDependentResource(glyphRun);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.GlyphRunIndex = this.content.AddDependentResource(glyphRun);
                    recordNeedOverwrite = true;
                }

                if (recordNeedOverwrite)
                {
                    content.WriteRecord(RecordType.DrawGlyphRun, (byte*)&record, sizeof(DrawGlyphRunCommand));
                }
            }
        }

        public override void DrawText(Brush brush, FormattedText formattedText)
        {
            if (brush == null || formattedText == null)
            {
                return;
            }
            EnsureContent();

            unsafe
            {
                if (!this.content.ReadRecord(out DrawTextCommand record))
                {//different record type: append new record
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    record.FormattedTextIndex = this.content.AddDependentResource(formattedText);
                    this.content.WriteRecord(RecordType.DrawText, (byte*) &record, sizeof(DrawTextCommand));
                    return;
                }

                //same type: update record if different
                bool recordNeedOverwrite = false;
                if (this.content.ReadDependentResource(record.BrushIndex, out Brush oldBrush))
                {
                    if (!oldBrush.Equals(brush))
                    {
                        record.BrushIndex = this.content.AddDependentResource(brush);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.BrushIndex = this.content.AddDependentResource(brush);
                    recordNeedOverwrite = true;
                }

                if (this.content.ReadDependentResource(record.BrushIndex, out FormattedText oldFormattedText))
                {
                    if (!formattedText.Equals(oldFormattedText))
                    {
                        record.FormattedTextIndex = this.content.AddDependentResource(formattedText);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.FormattedTextIndex = this.content.AddDependentResource(formattedText);
                    recordNeedOverwrite = true;
                }

                if (recordNeedOverwrite)
                {
                    content.WriteRecord(RecordType.DrawText, (byte*)&record, sizeof(DrawTextCommand));
                }
            }
        }

        public override void PushClip(Geometry clipGeometry)
        {
            if (clipGeometry == null)
            {
                return;
            }
            EnsureContent();

            unsafe
            {
                if (!this.content.ReadRecord(out PushClipCommand record))
                {//different record type: append new record
                    record.ClipGeometryIndex = this.content.AddDependentResource(clipGeometry);
                    this.content.WriteRecord(RecordType.PushClip, (byte*)&record, sizeof(PushClipCommand));
                    return;
                }

                //same type: update record if different
                bool recordNeedOverwrite = false;
                if (this.content.ReadDependentResource(record.ClipGeometryIndex, out Geometry oldClipGeometry))
                {
                    if (!ReferenceEquals(oldClipGeometry, clipGeometry))
                    {
                        record.ClipGeometryIndex = this.content.AddDependentResource(clipGeometry);
                        recordNeedOverwrite = true;
                    }
                }
                else
                {
                    record.ClipGeometryIndex = this.content.AddDependentResource(clipGeometry);
                    recordNeedOverwrite = true;
                }

                if (recordNeedOverwrite)
                {
                    content.WriteRecord(RecordType.PushClip, (byte*)&record, sizeof(PushClipCommand));
                }
            }
        }

        public override void Pop()
        {
            EnsureContent();
            
            unsafe
            {
                if (!this.content.ReadRecord(out PopCommand record))
                {//different record type: append new record
                    this.content.WriteRecord(RecordType.Pop, (byte*)&record, 0);
                    return;
                }

                //same type: no need to update a pop command record
            }
        }

        public override void Close()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(VisualDrawingContext));
            }

            ((IDisposable)this).Dispose();
        }

        protected override void DisposeCore()
        {
            if (!disposed)
            {
                ownerVisual.RenderClose(content);
                disposed = true;
            }
        }

        private void EnsureContent()
        {
            if (content == null)
            {
                content = new DrawingContent();
            }
        }

        private bool disposed;
        private Visual ownerVisual;
        private DrawingContent content;
    }
}