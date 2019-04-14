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

        //TODO implement DrawXXX methods: fill VisualDrawingContext.content

        public override void DrawLine(Pen pen, Point point0, Point point1)
        {
            if (pen == null)
            {
                throw new ArgumentNullException(nameof(pen));
            }

            unsafe
            {
                EnsureContent();

                var penIndex = content.AddReferenceToResource(pen);
                var record = new DrawLineCommand(penIndex, point0, point1);

                content.WriteRecord(RecordType.DrawLine, (byte*)&record, sizeof(DrawLineCommand));
            }
        }

        public override void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
            if (brush == null)
            {
                throw new ArgumentNullException(nameof(brush));
            }
            if (pen == null)
            {
                throw new ArgumentNullException(nameof(pen));
            }

            unsafe
            {
                EnsureContent();

                var brushIndex = content.AddReferenceToResource(brush);
                var penIndex = content.AddReferenceToResource(pen);
                var record = new DrawRectangleCommand(brushIndex, penIndex, rectangle);

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
            throw new System.NotImplementedException();
        }

        public override void DrawImage(Image image, Rect rectangle)
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