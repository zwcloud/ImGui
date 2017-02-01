namespace ImGui
{
    /// <summary>
    /// Filtering mode for textures.
    /// </summary>
    internal enum FilterMode
    {
        /// <summary>
        /// Point filtering - texture pixels become blocky up close.
        /// </summary>
        Point,
        /// <summary>
        /// Bilinear filtering - texture samples are averaged.
        /// </summary>
        Bilinear,
        /// <summary>
        /// Trilinear filtering - texture samples are averaged and also blended between mipmap levels.
        /// </summary>
        Trilinear
    }
}