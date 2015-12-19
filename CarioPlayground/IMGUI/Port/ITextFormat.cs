using System;

namespace ImGui
{
    public interface ITextFormat : IDisposable
    {
        int FontSize { get; set; }
        TextAlignment Alignment { get; set; }
    }

    /// <summary>
    /// Alignment of paragraph text along the reading direction axis relative to 
    /// the leading and trailing edge of the layout box.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        /// The leading edge of the paragraph text is aligned to the layout box's leading edge.
        /// </summary>
        Leading,

        /// <summary>
        /// The trailing edge of the paragraph text is aligned to the layout box's trailing edge.
        /// </summary>
        Trailing,

        /// <summary>
        /// The center of the paragraph text is aligned to the center of the layout box.
        /// </summary>
        Center,

        /// <summary>
        /// Align text to the leading side, and also justify text to fill the lines.
        /// </summary>
        Justified
    };
}