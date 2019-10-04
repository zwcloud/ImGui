using System;

namespace ImGui.OSAbstraction.Text
{
    /// <summary>
    /// Text-related(layout, hit-test, etc.) functions
    /// </summary>
    internal interface ITextContext : IDisposable
    {
        /// <summary>
        /// size of the font in the text
        /// </summary>
        double FontSize { get; }

        /// <summary>
        /// alignment of the text in the rectangle
        /// </summary>
        TextAlignment Alignment { get; set; }

        /// <summary>
        /// position of the layout box
        /// </summary>
        Point Position { get; }

        /// <summary>
        /// size of the layout box
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// the text
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// build text glyphs to retrive text rendering data (line segments and bezier curves)
        /// </summary>
        void Build();

        /// <summary>
        /// get space that the text occupies
        /// </summary>
        /// <returns></returns>
        Size Measure();

        /// <summary>
        /// Get nearest character index from the point.
        /// </summary>
        /// <param name="pointX">x, relative to the top-left location of the layout box.</param>
        /// <param name="pointY">y, relative to the top-left location of the layout box.</param>
        /// <param name="isInside">whether the point is inside the text string</param>
        /// <returns>nearest character index from the point</returns>
        uint XyToIndex(float pointX, float pointY, out bool isInside);

        /// <summary>
        /// Given a character index and whether the caret is on the leading or trailing edge of that position.
        /// </summary>
        /// <param name="textPosition">character index</param>
        /// <param name="isTrailing">whether the caret is on the leading or trailing edge of that position</param>
        /// <param name="pointX">position x</param>
        /// <param name="pointY">position y</param>
        /// <param name="height">the height of the text</param>
        void IndexToXY(uint textPosition, bool isTrailing,
            out float pointX, out float pointY, out float height);
    }
}