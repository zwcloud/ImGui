using ImGui.GraphicsAbstraction;

namespace ImGui.Rendering
{
    internal class DrawingDrawingContext : DrawingContext
    {
        //TODO implement DrawXXX methods: fill DrawingGroupDrawingContext.drawingGroup

        public override void DrawLine(Pen pen, Point point0, Point point1)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        protected override void DisposeCore()
        {
            // Dispose may be called multiple times without throwing
            // an exception.
            if (!disposed)
            {
                disposed = true;
            }
        }

        private bool disposed;
    }
}