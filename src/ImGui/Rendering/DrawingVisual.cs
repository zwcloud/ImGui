namespace ImGui.Rendering
{
    internal class DrawingVisual : Visual
    {
        public DrawingVisual(int id) : base(id)
        {
        }

        public DrawingVisual(string name) : base(name)
        {
        }

        public DrawingVisual(int id, string name) : base(id, name)
        {
        }

        public override Rect GetClipRect()
        {
            return Rect.Big;
        }

        /// <summary>
        /// Convert content into GPU renderable resources: Mesh/TextMesh
        /// </summary>
        /// <param name="context"></param>
        internal override bool RenderContent(RenderContext context)
        {
            context.ConsumeContent(content);
            return true;
        }

        /// <summary>
        /// Opens the DrawingVisual for rendering. The returned DrawingContext can be used to
        /// render into the DrawingVisual.
        /// </summary>
        internal DrawingContext RenderOpen()
        {
            return new VisualDrawingContext(this);
        }

        /// <summary>
        /// Called from the DrawingContext when the DrawingContext is closed.
        /// </summary>
        internal override void RenderClose(DrawingContent newContent)
        {
            DrawingContent oldContent;

            oldContent = content;
            content = newContent;

            SetFlags(true, VisualFlags.IsContentDirty);

            if (oldContent != null)
            {
                //TODO consider if we need to release/reuse old content via object pool or leave it to GC
            }

            //PropagateFlags(this,VisualFlags.IsSubtreeDirtyForRender);//TODO
        }

        private DrawingContent content;
    }
}