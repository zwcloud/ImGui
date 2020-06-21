using System;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;

namespace ImGui.Rendering
{
    internal class DrawingDrawingContext : DrawingContext
    {
        //DrawingDrawingContext will be implemented once DrawingContext.DrawDrawing is implemented
        //TODO implement DrawXXX methods: fill DrawingGroupDrawingContext.drawingGroup

        public override void DrawLine(Pen pen, Point point0, Point point1)
        {
            throw new NotImplementedException();
        }

        public override void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
            throw new NotImplementedException();
        }

        public override void DrawRoundedRectangle(Brush brush, Pen pen, Rect rectangle, double radiusX, double radiusY)
        {
            throw new NotImplementedException();
        }

        public override void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
        {
            throw new NotImplementedException();
        }

        public override void DrawGeometry(Brush brush, Pen pen, Geometry geometry)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(ITexture image, Rect rectangle, Point uvMin, Point uvMax)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(ITexture image, Rect rectangle, (double top, double right, double bottom, double left) slice)
        {
            throw new NotImplementedException();
        }

        public override void DrawGlyphRun(Brush foregroundBrush, GlyphRun glyphRun)
        {
            throw new NotImplementedException();
        }

        public override void DrawText(Brush foregroundBrush, FormattedText formattedText)
        {
            throw new NotImplementedException();
        }

        public override void PushClip(Geometry clipGeometry)
        {
            throw new NotImplementedException();
        }

        public override void Pop()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        protected override void DisposeCore()
        {
            // Dispose may be called multiple times without throwing
            // an exception.
            if (!this.disposed)
            {
                this.disposed = true;
            }
        }

        private bool disposed;
    }
}