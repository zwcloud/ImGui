using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.Rendering
{
    internal class DrawingGroupDrawingContext : DrawingDrawingContext
    {
        /// <summary>
        /// DrawingGroupDrawingContext populates a DrawingGroup from the Draw
        /// commands that are called on it.
        /// </summary>
        /// <param name="drawingGroup"> DrawingGroup this context populates </param>
        internal DrawingGroupDrawingContext(DrawingGroup drawingGroup)
        {
            Debug.Assert(null != drawingGroup);

            this.drawingGroup = drawingGroup;
        }

        public override void Close()
        {
            DisposeCore();
        }

        protected override void DisposeCore()
        {
            //TODO inspect if closing logic below applies properly

            // Dispose may be called multiple times without throwing
            // an exception.
            if (!disposed)
            {
                // Call Close with the root DrawingGroup's children

                List<Drawing> rootChildren;

                if (currentDrawingGroup != null)
                 {
                    // If we created a root DrawingGroup because multiple elements
                    // exist at the root level, provide it's Children collection
                    // directly.
                    rootChildren = currentDrawingGroup.Children;
                }
                else
                {
                    // Create a new DrawingCollection if we didn't create a
                    // root DrawingGroup because the root level only contained
                    // a single child.
                    //
                    // This collection is needed by DrawingGroup.Open because
                    // Open always replaces it's Children collection.
                    rootChildren = new List<Drawing>();

                    if (rootDrawing != null)
                    {
                        rootChildren.Add(rootDrawing);
                    }
                }

                drawingGroup.Close(rootChildren);

                disposed = true;
            }
        }

        private DrawingGroup drawingGroup;
        private DrawingGroup currentDrawingGroup = null;
        private Drawing rootDrawing = null;
        private bool disposed;
    }
}
