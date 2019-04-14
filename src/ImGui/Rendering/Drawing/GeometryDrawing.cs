using System;

namespace ImGui.Rendering
{
    /// <summary>
    /// GeometryDrawing represents a drawing operation that combines
    /// a geometry with and brush and/or pen to produce rendered
    /// content.
    /// </summary>
    internal class GeometryDrawing : Drawing
    {
        public Geometry Geometry
        {
            get => geometry;
            protected set
            {
                if (value == geometry)
                {
                    return;
                }
                geometry = value;
                dirty = true;
            }
        }

        public Brush Brush
        {
            get => brush;
            protected set
            {
                if (value == brush)
                {
                    return;
                }
                brush = value;
                dirty = true;
            }
        }

        public Pen Pen
        {
            get => pen;
            protected set
            {
                if (value == pen)
                {
                    return;
                }
                pen = value;
                dirty = true;
            }
        }

        internal override void RenderContent(RenderContext context)
        {
            if (dirty)
            {
                //TODO update content here,
                //we don't need to update content immediately when property Geometry, Pen and Brush is changed.
                throw new NotImplementedException();
                dirty = false;
            }
            context.ConsumeContent(content);
        }

        private DrawingContent content = new DrawingContent();

        private Geometry geometry;
        private Brush brush;
        private Pen pen;
        private bool dirty;
    }
}