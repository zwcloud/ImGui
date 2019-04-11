using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui.Rendering
{
    /// <summary>
    /// Represents a collection of drawings that can be operated upon as a single drawing.
    /// </summary>
    internal class DrawingGroup : Drawing
    {
        /// <summary>
        /// Gets or sets the Drawing objects that are contained in this DrawingGroup.
        /// </summary>
        public List<Drawing> Children { get; set; } = new List<Drawing>();

        /// <summary>
        /// Opens the DrawingGroup for re-populating it's children, clearing any existing
        /// children.
        /// </summary>
        /// <returns>
        /// Returns DrawingContext to populate the DrawingGroup's children.
        /// </returns>
        public DrawingContext Open()
        {
            if (open)
            {
                throw new InvalidOperationException("DrawingGroup_AlreadyOpen");
            }
            open = true;
            openedForAppend = false;

            return new DrawingGroupDrawingContext(this);
        }

        /// <summary>
        /// Opens the DrawingGroup for populating it's children, appending to
        /// any existing children in the collection.
        /// </summary>
        /// <returns>
        /// Returns DrawingContext to populate the DrawingGroup's children.
        /// </returns>
        public DrawingContext Append()
        {
            if (open)
            {
                throw new InvalidOperationException("DrawingGroup_AlreadyOpen");
            }
            open = true;
            openedForAppend = true;

            return new DrawingGroupDrawingContext(this);
        }

        /// <summary>
        /// Convert contained drawings into cached DrawingContents: namely updating cached contents.
        /// </summary>
        /// <param name="context"></param>
        internal override void RenderContent(RenderContext context)
        {
            foreach (var drawing in Children)
            {
                drawing.RenderContent(context);
            }
        }

        /// <summary>
        /// Called by a DrawingContext returned from Open or Append when the content
        /// created by it needs to be committed (because DrawingContext.Close/Dispose
        /// was called)
        /// </summary>
        /// <param name="rootDrawingGroupChildren">
        ///     Collection containing the Drawing elements created by a DrawingContext
        ///     returned from Open or Append.
        /// </param>
        internal void Close(List<Drawing> rootDrawingGroupChildren)
        {

            Debug.Assert(open);
            Debug.Assert(rootDrawingGroupChildren != null);

            if (!openedForAppend)
            {
                // Clear out the previous contents by replacing the current collection with
                // the new collection.
                //
                // When more than one element exists in rootDrawingGroupChildren, the
                // DrawingContext had to create this new collection anyways.  To behave
                // consistently between the one-element and many-element cases,
                // we always set Children to a new List<Drawing> instance during Close().
                //
                // The collection created by the DrawingContext will no longer be
                // used after the DrawingContext is closed, so we can take ownership
                // of the reference here to avoid any more unneccesary copies.
                Children = rootDrawingGroupChildren;
            }
            else
            {
                // Append the collection to the current Children collection
                List<Drawing> children = Children;

                // Ensure that we can Append to the Children collection
                if (children == null)
                {
                    throw new InvalidOperationException("DrawingGroup_CannotAppendToNullCollection");
                }

                // Append the new collection to our current Children.
                children.AddRange(rootDrawingGroupChildren);
            }

            // This DrawingGroup is no longer open
            open = false;
        }

        private bool open;
        private bool openedForAppend;
    }
}