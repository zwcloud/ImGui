namespace ImGui
{
    /// <summary>
    /// Wrap mode for textures.
    /// </summary>
    internal enum TextureWrapMode
    {
        /// <summary>
        /// Tiles the texture, creating a repeating pattern.
        /// </summary>
        Repeat,
        /// <summary>
        /// Clamps the texture to the last pixel at the border.
        /// </summary>
        Clamp
    }
}