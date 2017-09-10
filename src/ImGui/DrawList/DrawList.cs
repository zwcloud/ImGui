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

        public void AddTextDrawCommand()
        {
            DrawCommand cmd = new DrawCommand();
            if (this.clipRectStack.Count > 0)
            {
                DrawCommand currentCmd = new DrawCommand();
                if (this.TextMesh.Commands.Count > 0)
                {
                    currentCmd = this.TextMesh.Commands[this.TextMesh.Commands.Count - 1];
                    var currentClipRect = this.clipRectStack[this.clipRectStack.Count - 1];
                    if(currentCmd.ClipRect == currentClipRect)//no need to add command
                    {
                        return;
                    }
                }
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

            cmd.TextureData = null;
            this.TextMesh.Commands.Add(cmd);
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

            // update shape mesh command
            var drawCmdBuffer = this.ShapeMesh.CommandBuffer;
            DrawCommand currentCmd;
            if (drawCmdBuffer.Count > 0)
            {
                currentCmd = drawCmdBuffer[drawCmdBuffer.Count - 1];
                if (currentCmd.ElemCount != 0 && currentCmd.ClipRect != currentClipRect)
                {
                    AddDrawCommand();
                    return;
                }
            }
            else
            {
                AddDrawCommand();
                return;
            }

            // try to merge with previous command
            if (drawCmdBuffer.Count > 1)
            {
                DrawCommand previousCmd = drawCmdBuffer[drawCmdBuffer.Count - 2];
                if (previousCmd.TextureData == currentCmd.TextureData && previousCmd.ClipRect == currentClipRect)
                {
                    drawCmdBuffer.RemoveAt(drawCmdBuffer.Count - 1);
                }
            }
        }

        public Rect GetCurrentClipRect()
        {
            Rect currentClipRect = this.clipRectStack[this.clipRectStack.Count - 1];
            return currentClipRect;
        }
    }
}
