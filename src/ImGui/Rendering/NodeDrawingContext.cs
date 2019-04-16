using System;

namespace ImGui.Rendering
{
    internal class NodeDrawingContext : IDisposable
    {
        public NodeDrawingContext(Node node)
        {
            ownerNode = node;
            dc = new VisualDrawingContext(node);
        }

        public void DrawLine(Point point0, Point point1)
        {
            var rule = ownerNode.RuleSet;
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            dc.DrawLine(pen, point0, point1);
        }

        public void DrawRectangle(Rect rectangle)
        {
            var rule = ownerNode.RuleSet;
            Pen pen = new Pen(rule.StrokeColor, rule.StrokeWidth);
            Brush brush = new Brush(rule.FillColor);
            dc.DrawRectangle(brush, pen, rectangle);
        }

        public void Close()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(VisualDrawingContext));
            }

            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            if (!disposed)
            {
                dc.Close();
                disposed = true;
            }
        }

        private bool disposed;
        private readonly Node ownerNode;
        private readonly VisualDrawingContext dc;
    }
}
