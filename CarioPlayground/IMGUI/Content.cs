namespace ImGui
{
    public sealed class Content
    {
        private string tooltip = string.Empty;

        /// <summary>
        /// Simple text(Single line and short--three or two word)
        /// Not used! TODO
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// text
        /// </summary>
        public ITextContext TextContext { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        public Texture Image { get; set; }
        
        private Content()
        {
            Text = string.Empty;
            Image = null;
            TextContext = null;
        }

        public Content(string text)
        {
            Text = text;
            Image = null;
            TextContext = null;
        }

        public Content(Texture image)
        {
            Text = null;
            Image = image;
            TextContext = null;
        }

        public Content(ITextContext textContext)
        {
            Text = null;
            Image = null;
            TextContext = textContext;
        }

        /// <summary>
        /// Get rect of the content that it may occupy
        /// </summary>
        /// <returns>rect of the content</returns>
        public Rect GetRect()
        {
            if (TextContext != null)
            {
                return TextContext.Rect;
            }

            //Others' are not implemented
            throw new System.NotImplementedException();
        }
    }
}