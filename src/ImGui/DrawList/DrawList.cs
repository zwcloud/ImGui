using System.Collections.Generic;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    /// <summary>
    /// all the data to render
    /// </summary>
    internal partial class DrawList
    {
        private readonly List<Rect> clipRectStack = new List<Rect>(2);

        /// <summary>
        /// Mesh (colored triangles)
        /// </summary>
        public Mesh ShapeMesh { get; } = new Mesh();

        /// <summary>
        /// Mesh (textured triangles)
        /// </summary>
        public Mesh ImageMesh { get; } = new Mesh();

        /// <summary>
        /// Text mesh
        /// </summary>
        public TextMesh TextMesh { get; } = new TextMesh();

        /// <summary>
        /// Clear the drawlist
        /// </summary>
        public void Clear()
        {
            Path.Clear();

            this.ShapeMesh.Clear();
            this.TextMesh.Clear();
            this.ImageMesh.Clear();

            this.clipRectStack.Clear();
        }

        public void Init()
        {
            AddDrawCommand();

            //No need to add initial command for ImageMesh. It will be added when adding an image.
        }

        public void AddDrawCommand()
        {
            DrawCommand cmd = new DrawCommand();
            if (this.clipRectStack.Count > 0)
            {
                cmd.ClipRect = this.clipRectStack[this.clipRectStack.Count - 1];
            }
            else
            {
                cmd.ClipRect = Rect.Big;
            }
            cmd.TextureData = null;

            if(cmd.ClipRect.IsEmpty)
            {
                return;
            }
            this.ShapeMesh.CommandBuffer.Add(cmd);
        }

        public void AddImageDrawCommand(ITexture texture)
        {
            DrawCommand cmd = new DrawCommand();
            if (this.clipRectStack.Count > 0)
            {
                cmd.ClipRect = this.clipRectStack[this.clipRectStack.Count - 1];
            }
            else
            {
                cmd.ClipRect = Rect.Big;
            }

            if (cmd.ClipRect.IsEmpty)
            {
                return;
            }

            cmd.TextureData = texture;
            this.ImageMesh.CommandBuffer.Add(cmd);
        }

        public void PushClipRect(Rect rect, bool intersectWithCurrentClipRect = false)
        {
            if (intersectWithCurrentClipRect && this.clipRectStack.Count > 0)
            {
                Rect currentClipRect = this.clipRectStack[this.clipRectStack.Count - 1];
                rect.Intersect(currentClipRect);
            }
            this.clipRectStack.Add(rect);
            UpdateClipRect();
        }

        public void PopClipRect()
        {
            if (this.clipRectStack.Count == 0)
            {
                throw new System.InvalidOperationException("Clip stack is empty.");
            }
            this.clipRectStack.RemoveAt(this.clipRectStack.Count - 1);
            UpdateClipRect();
        }

        public void UpdateClipRect()
        {
            // If current command is used with different settings we need to add a new command
            // Get current clip rect
            Rect currentClipRect;
            if (this.clipRectStack.Count > 0)
            {
                currentClipRect = this.clipRectStack[this.clipRectStack.Count - 1];
            }
            else
            {
                currentClipRect = Rect.Big;
            }

            var drawCmdBuffer = this.ShapeMesh.CommandBuffer;
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

            // TODO TextMesh should also have CommandBuffer
            {
                var newDrawCmd = this.TextMesh.Command;
                newDrawCmd.ClipRect = currentClipRect;
            }
        }

    }
}
