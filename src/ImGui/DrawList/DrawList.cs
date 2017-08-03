using System.Collections.Generic;

namespace ImGui
{
    /// <summary>
    /// all the data to render
    /// </summary>
    internal partial class DrawList
    {
        private readonly Mesh drawBuffer = new Mesh();
        private readonly TextMesh textMesh = new TextMesh();
        private readonly Mesh imageBuffer = new Mesh();

        public System.Collections.Generic.List<Rect> clipRectStack = new System.Collections.Generic.List<Rect>(2);

        /// <summary>
        /// Mesh (colored triangles)
        /// </summary>
        public Mesh DrawBuffer
        {
            get { return drawBuffer; }
        }

        /// <summary>
        /// Text mesh
        /// </summary>
        public TextMesh TextMesh
        {
            get { return textMesh; }
        }

        /// <summary>
        /// Mesh (textured triangles)
        /// </summary>
        public Mesh ImageBuffer
        {
            get { return imageBuffer; }
        }

        /// <summary>
        /// Clear the drawlist
        /// </summary>
        public void Clear()
        {
            _Path.Clear();

            // triangles
            DrawBuffer.Clear();

            // text mesh
            TextMesh.Clear();

            // images
            ImageBuffer.Clear();

            this.clipRectStack.Clear();
        }

        public void Init()
        {
            AddDrawCommand();

            //No need to add initial command for ImageBuffer. It will be added when adding an image.
        }

        public void AddDrawCommand()
        {
            DrawCommand draw_cmd = new DrawCommand();
            if (this.clipRectStack.Count > 0)
            {
                draw_cmd.ClipRect = clipRectStack[this.clipRectStack.Count - 1];
            }
            else
            {
                draw_cmd.ClipRect = Rect.Big;
            }
            draw_cmd.TextureData = null;

            if(draw_cmd.ClipRect.IsEmpty)
            {
                return;
            }
            draw_cmd.PrimitiveType = PrimitiveType.TriangleList;
            DrawBuffer.CommandBuffer.Add(draw_cmd);
        }

        public void AddImageDrawCommand(ITexture texture)
        {
            DrawCommand draw_cmd = new DrawCommand();
            if (this.clipRectStack.Count > 0)
            {
                draw_cmd.ClipRect = clipRectStack[this.clipRectStack.Count - 1];
            }
            else
            {
                draw_cmd.ClipRect = Rect.Big;
            }

            if (draw_cmd.ClipRect.IsEmpty)
            {
                return;
            }

            draw_cmd.PrimitiveType = PrimitiveType.TriangleList;

            draw_cmd.TextureData = texture;
            ImageBuffer.CommandBuffer.Add(draw_cmd);
        }

        public void PushClipRect(Rect rect, bool intersectWithCurrentClipRect = false)
        {
            if (intersectWithCurrentClipRect && this.clipRectStack.Count > 0)
            {
                Rect currentClipRect = clipRectStack[this.clipRectStack.Count - 1];
                rect.Intersect(currentClipRect);
            }
            clipRectStack.Add(rect);
            UpdateClipRect();
        }

        public void PopClipRect()
        {
            if (clipRectStack.Count == 0)
            {
                throw new System.InvalidOperationException("Clip stack is empty.");
            }
            clipRectStack.RemoveAt(clipRectStack.Count - 1);
            UpdateClipRect();
        }

        public void UpdateClipRect()
        {
            // If current command is used with different settings we need to add a new command
            // Get current clip rect
            Rect currentClipRect;
            if (this.clipRectStack.Count > 0)
            {
                currentClipRect = clipRectStack[this.clipRectStack.Count - 1];
            }
            else
            {
                currentClipRect = Rect.Big;
            }

            var drawCmdBuffer = DrawBuffer.CommandBuffer;
            {
                if (drawCmdBuffer.Count == 0)
                {
                    AddDrawCommand();
                    return;
                }

                var newDrawCmd = drawCmdBuffer[drawCmdBuffer.Count - 1];
                {
                    if (newDrawCmd.ElemCount != 0 && newDrawCmd.ClipRect != currentClipRect)
                    {
                        AddDrawCommand();
                        return;
                    }

                    // Try to merge with previous command if it matches, else use current command
                    if (drawCmdBuffer.Count > 1)
                    {
                        var previousCmd = drawCmdBuffer[drawCmdBuffer.Count - 2];
                        if (previousCmd.ClipRect == currentClipRect)
                        {
                            drawCmdBuffer.RemoveAt(drawCmdBuffer.Count - 1);
                        }
                        else
                        {
                            newDrawCmd.ClipRect = currentClipRect;
                        }
                    }
                    else
                    {
                        newDrawCmd.ClipRect = currentClipRect;
                    }
                }
                drawCmdBuffer[drawCmdBuffer.Count - 1] = newDrawCmd;
            }

            // TODO Update textmesh clip rect?
        }

    }
}
