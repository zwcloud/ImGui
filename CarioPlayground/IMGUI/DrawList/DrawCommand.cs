namespace ImGui
{
    class DrawCommand
    {
        int elemCount;
        Rect    clipRect;
        object  textureData;

        DrawCommand()
        {
            elemCount = 0;
            clipRect.X = clipRect.Y = -8192.0f;
            clipRect.Width = clipRect.Height = +8192.0f;
            textureData = null;
        }

        /// <summary>
        /// Number of indices (multiple of 3) to be rendered as triangles. Vertices are stored in the callee DrawList's vtx_buffer[] array, indices in idx_buffer[].
        /// </summary>
        public int ElemCount
        {
            get { return elemCount; }
            set { elemCount = value; }
        }

        /// <summary>
        /// Clipping rectangle 
        /// </summary>
        public Rect ClipRect
        {
            get { return clipRect; }
            set { clipRect = value; }
        }

        /// <summary>
        /// User-provided texture ID
        /// </summary>
        public object TextureData
        {
            get { return textureData; }
            set { textureData = value; }
        }
    }
}
