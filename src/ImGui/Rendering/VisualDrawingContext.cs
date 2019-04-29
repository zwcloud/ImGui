using System;
using ImGui.GraphicsAbstraction;
using ImGui.Rendering.Composition;

namespace ImGui.Rendering
{
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

                var penIndex = content.AddDependentResource(pen);
                var record = new DrawLineCommand(penIndex, point0, point1);

                content.WriteRecord(RecordType.DrawLine, (byte*)&record, sizeof(DrawLineCommand));
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

                var record = new DrawRectangleCommand(
                    content.AddDependentResource(brush),
                    content.AddDependentResource(pen),
                    rectangle);

                content.WriteRecord(RecordType.DrawRectangle, (byte*)&record, sizeof(DrawRectangleCommand));
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
            if (brush == null && pen == null || geometry == null)
            {
                return;
            }

            unsafe
            {
                EnsureContent();

                var record =
                    new DrawGeometryCommand (
                        content.AddDependentResource(brush),
                        content.AddDependentResource(pen),
                        content.AddDependentResource(geometry)
                    );

                content.WriteRecord(RecordType.DrawGeometry, (byte*) &record, sizeof(DrawGeometryCommand));
            }
        }

        public override void DrawImage(ImGui.OSAbstraction.Graphics.ITexture image, Rect rectangle)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawDrawing(Drawing drawing)
        {
            throw new System.NotImplementedException();
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