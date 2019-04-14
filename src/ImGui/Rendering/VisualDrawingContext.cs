using System;
using ImGui.GraphicsAbstraction;

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
            EnsureContent();

            //var p = content.PathContent.Path;
            //p.Add(new MoveToCommand(point0));
            //p.Add(new LineToCommand(point1));
            //p.Add(new StrokeCommand(pen.LineWidth, pen.LineColor));
        }

        public override void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
            throw new System.NotImplementedException();
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