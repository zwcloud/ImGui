using System.Collections.Generic;

namespace ImGui
{
    /// <summary>
    /// all the data to render
    /// </summary>
    internal partial class DrawList
    {
        private readonly DrawBuffer drawBuffer = new DrawBuffer();
        private readonly DrawBuffer bezierBuffer = new DrawBuffer();
        private readonly DrawBuffer imageBuffer = new DrawBuffer();

        internal readonly List<Rect> _ClipRectStack = new List<Rect>();
        /// <summary>
        /// buffer for colored triangles
        /// </summary>
        public DrawBuffer DrawBuffer
        {
            get { return drawBuffer; }
        }

        /// <summary>
        /// buffer for filled bezier curves
        /// </summary>
        public DrawBuffer BezierBuffer
        {
            get { return bezierBuffer; }
        }

        public DrawBuffer ImageBuffer
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

            // filled bezier curves
            BezierBuffer.Clear();
            _BezierControlPointIndex.Clear();

            // images
            ImageBuffer.Clear();
        }
    }
}
