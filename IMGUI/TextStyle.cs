namespace ImGui
{
    /// <summary>
    /// Styles of texts
    /// </summary>
    internal struct TextStyle
    {
        /// <summary>
        /// Alignment of the text
        /// </summary>
        public TextAlignment TextAlignment { get; set; }

        /// <summary>
        /// Line spacing of the text
        /// </summary>
        public float LineSpacing { get; set; }

        /// <summary>
        /// Tab size (spaces that TabSize characters occupies)
        /// </summary>
        public int TabSize { get; set; }
    }
}